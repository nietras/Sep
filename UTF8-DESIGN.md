# Sep UTF8 Support - Design Analysis & Plan

## Problem Statement
Sep currently only supports `TextReader`/`TextWriter` based reading and writing,
which means all CSV data flows through `char` (UTF-16) buffers internally. For
many use cases — especially pipelines working with raw UTF-8 bytes (e.g. network
streams, memory-mapped files, `Span<byte>` APIs) — this introduces an
unnecessary transcoding step. Adding native UTF-8 support based on
`Span<byte>`/`ReadOnlySpan<byte>` would enable zero-copy parsing of UTF-8 CSV
data and potentially significant performance improvements.

## Current Architecture Summary

### Core Data Flow
```
Sep/Spec → SepReaderOptions → SepReader → Row → Col(s) → Span<char>/ToString/Parse
Sep/Spec → SepWriterOptions → SepWriter → Row → Col(s) → Set/Format
```

### Key Types & Their Roles

| Type | Role |
|------|------|
| `Sep` | Validated separator (`char`), entry point |
| `SepReaderOptions` / `SepWriterOptions` | Configuration records |
| `SepReader` | Sealed class, owns `TextReader`, orchestrates parsing |
| `SepReaderState` | Base class of `SepReader`, holds all mutable parsing state: `char[] _chars`, `int[] _colEndsOrColInfos`, `SepRowInfo[] _parsedRows` |
| `SepReader.Row` | `ref struct` facade over `SepReaderState` |
| `SepReader.Col` | `ref struct`, exposes `ReadOnlySpan<char> Span`, `Parse<T>()` |
| `SepReader.Cols` | `ref struct`, multi-column access |
| `ISepParser` | Interface for SIMD-vectorized parsers: `ParseColEnds(SepReaderState)` / `ParseColInfos(SepReaderState)` |
| `SepParseMask` | Static generic helper methods parameterized on `<TColInfo, TColInfoMethods>` |
| `SepColInfo` / `ISepColInfoMethods<T>` | Static abstract interface for col end/info tracking (already generic!) |
| `SepWriter` | Sealed class, owns `TextWriter`, char-based output |
| `SepWriter.ColImpl` | Internal, owns `char[] _buffer` per column |

### Parser Architecture (Already Partially Generic)
The parsing layer already uses **static abstract interfaces** for genericity:
- `ISepColInfoMethods<TColInfo>` abstracts over `int` (col ends only) vs
  `SepColInfo` (col end + quote count)
- `SepParseMask` methods are all generic: `ParseSeparatorsMask<TColInfo,
  TColInfoMethods>`, etc.
- Each SIMD parser (AVX2, SSE2, AVX-512, NEON, Vector128/256/512, IndexOfAny)
  has a `Parse<TColInfo, TColInfoMethods>(SepReaderState s)` method
- The SIMD parsers **pack chars to bytes** (`ISA.PackUnsignedSaturate`) before
  comparing — this means the core comparison logic already works on byte vectors!

### Character-Specific Coupling Points
1. **`SepReaderState._chars`** — `char[]` buffer, all col spans index into this
2. **`SepReader` constructor** — reads from `TextReader` into `char[]`
3. **`FillAndMaybeDoubleCharsBuffer`** — calls `TextReader.Read(Span<char>)`
4. **`GetColSpan(int index)`** — returns `ReadOnlySpan<char>` from `_chars`
5. **`Parse<T>()`** — calls `ISpanParsable<T>.Parse(ReadOnlySpan<char>, ...)`
6. **`SepParseMask.ParseAnyChar`** — reads `char` via `Unsafe.Add(ref charsRef, ...)`
   and compares to `CarriageReturn`, `LineFeed`, `Quote`, `separator`
7. **`SepWriter._writer`** — `TextWriter`
8. **`SepWriter.ColImpl._buffer`** — `char[]`, writes via `TextWriter.Write(ReadOnlySpan<char>)`
9. **`SepReaderHeader`** — stores col names as `string`, uses `Dictionary<string, int>` (char reader only; UTF-8 reader gets `SepUtf8ReaderHeader`)
10. **`SepToString`** / string pooling — converts `ReadOnlySpan<char>` to `string`

## No Breaking Changes Constraint
The existing public API **must not break**. Specifically:
- `SepReader`, `SepReader.Row`, `SepReader.Col`, `SepReader.Cols` — unchanged
- `SepWriter`, `SepWriter.Row`, `SepWriter.Col`, `SepWriter.Cols` — unchanged
- `SepReaderOptions`, `SepWriterOptions` — unchanged
- `SepReaderHeader` — unchanged
- `Sep.Reader()`, `Sep.Writer()` and all `FromXXX`/`ToXXX` — unchanged
- Extension methods on `Sep`, `SepSpec` — unchanged

Internal changes (e.g. changing `SepReaderState` to inherit from a new generic
base) are acceptable as `SepReaderState` is `public` but its members are mostly
`internal` and it is not designed for external subclassing. If the base class
change causes a binary-level breaking change for rare cases, that is acceptable
as long as source-level compatibility is preserved for all normal usage.

## Proposed Approach: Making Core Generic Over `char`/`byte`

### Key Insight
The SIMD parsers already pack `char` pairs into `byte` vectors for comparison.
For UTF-8 input, the data is *already* bytes — no packing step needed. The
separator, line feed, carriage return, and quote characters are all ASCII (single
byte), so the SIMD comparison logic is fundamentally the same. Only the
"load & pack" step differs.

### Strategy: Generic `TChar` Base Class with Static Abstract Interfaces

Use .NET's own `IUtf8SpanParsable<T>` for parsing, and define a thin internal
static abstract interface for char/byte constants and operations. The generic
base class `SepReaderStateBase<TChar, TCharInfo>` captures the ~700 lines of
buffer management, col-tracking, and unescape logic that is **identical** between
`char` and `byte` readers. Derived classes (`SepReaderState`, `SepUtf8ReaderState`)
add only the type-specific operations (~100-150 lines each).

```csharp
// Internal static abstract interface for char vs byte specialization
internal interface ISepCharInfo<TChar> where TChar : unmanaged, IEquatable<TChar>
{
    static abstract TChar LineFeed { get; }
    static abstract TChar CarriageReturn { get; }
    static abstract TChar Quote { get; }
    static abstract TChar Space { get; }
}

internal readonly struct SepCharInfoUtf16 : ISepCharInfo<char>
{
    public static char LineFeed => SepDefaults.LineFeed;
    public static char CarriageReturn => SepDefaults.CarriageReturn;
    public static char Quote => SepDefaults.Quote;
    public static char Space => SepDefaults.Space;
}

internal readonly struct SepCharInfoUtf8 : ISepCharInfo<byte>
{
    public static byte LineFeed => SepDefaults.LineFeedByte;
    public static byte CarriageReturn => SepDefaults.CarriageReturnByte;
    public static byte Quote => SepDefaults.QuoteByte;
    public static byte Space => SepDefaults.SpaceByte;
}
```

### Generic Base Class Design: `SepReaderStateBase<TChar, TCharInfo>`

The base class is parameterized on `TChar` (the buffer element type) and
`TCharInfo` (the struct implementing `ISepCharInfo<TChar>` for constant
resolution). Using a **struct type parameter for `TCharInfo`** ensures the JIT
specializes all comparisons — the `TCharInfo.Quote`, `TCharInfo.LineFeed` etc.
calls become inlined constants, identical to the current hardcoded values. No
virtual dispatch, no dictionary lookups, zero runtime overhead.

```csharp
public class SepReaderStateBase<TChar, TCharInfo> : IDisposable
    where TChar : unmanaged, IEquatable<TChar>
    where TCharInfo : struct, ISepCharInfo<TChar>
{
    // ─── Buffer & data ───
    internal TChar[] _buffer;           // was _chars (char[]) or _bytes (byte[])
    internal int _charsDataStart;       // keep field names for grep-ability
    internal int _charsDataEnd;
    internal bool _trailingCarriageReturn;
    internal int _charsParseStart;

    // ─── Col tracking (completely TChar-agnostic) ───
    internal int[] _colEndsOrColInfos;
    internal SepRowInfo[] _parsedRows;
    internal int _parsedRowsCount, _parsedRowIndex;
    internal int _currentRowColCount, _currentRowColEndsOrInfosOffset;
    internal int _currentRowLineNumberFrom, _currentRowLineNumberTo;
    internal int _rowIndex;

    // ─── Parsing state ───
    internal int _parsingLineNumber;
    internal int _parsingRowCharsStartIndex;
    internal int _parsingRowColEndsOrInfosStartIndex;
    internal int _parsingRowColCount;
    internal uint _colSpanFlags;

    // ─── Header & caching (shared) ───
    internal SepReaderHeader _header;  // both char and byte readers have this
    internal bool _hasHeader;
    internal CultureInfo? _cultureInfo;
    internal SepArrayPoolAccessIndexed _arrayPool;
    internal (string colName, int colIndex)[] _colNameCache;
    internal int _cacheIndex;
    internal int _colCountExpected;

    // ─── Methods that are 100% shared ───
    internal bool MoveNextAlreadyParsed() { ... }     // ~25 lines, pure index math
    internal (int start, int end) RowStartEnd() { ... } // ~20 lines, col-end indexing
    internal int GetCachedColIndex(string) { ... }     // ~10 lines
    internal bool TryGetCachedColIndex(string, out int) { ... } // ~25 lines

    // ─── Col span (generic over TChar) ───
    internal ReadOnlySpan<TChar> GetColSpan(int index) { ... }
    // Uses TCharInfo.Quote for unescape comparison
    // Uses Unsafe.Add(ref TChar, ...) + MemoryMarshal.CreateReadOnlySpan
    // SepUnescape becomes generic: UnescapeInPlace<TChar, TCharInfo>(ref TChar, int)

    internal SepRange GetColRange(int index) { ... }   // ~30 lines

    // ─── Row span ───
    internal ReadOnlySpan<TChar> RowSpan() { ... }

    // ─── Trim (generic) ───
    static ref TChar TrimSpace(ref TChar col, ref int length) { ... }
    // Compares to TCharInfo.Space

    // ─── Dispose ───
    internal virtual void DisposeManaged()
    {
        ArrayPool<TChar>.Shared.Return(_buffer);
        ArrayPool<int>.Shared.Return(_colEndsOrColInfos);
        ArrayPool<SepRowInfo>.Shared.Return(_parsedRows);
        _arrayPool.Dispose();
    }
}
```

### What Lives in the Generic Base (~700 lines shared)

1. **Buffer management**: `TChar[] _buffer` with `ArrayPool<TChar>`, all
   data-start/data-end tracking, `ClearPaddingAfterData` (already generic in
   `SepArrayExtensions`)

2. **Col tracking**: `_colEndsOrColInfos` (int[]), `_parsedRows` (SepRowInfo[]),
   `_parsingRowColEndsOrInfosStartIndex`, `_parsingRowColCount`, etc. — all
   pure integer index math, completely TChar-agnostic

3. **`MoveNextAlreadyParsed()`** — ~25 lines of index bookkeeping, uses only
   `_parsedRowIndex`, `_parsedRowsCount`, `_currentRowColCount`, etc.

4. **`RowStartEnd()`** — pure `_colEndsOrColInfos` index math

5. **`GetColRange(int)`** / **`GetColSpan(int)`** — these are the critical ones.
   Currently ~70 lines each. They read from `_colEndsOrColInfos` to find
   start/end indices, then create a `ReadOnlySpan<TChar>` from `_buffer`. The
   unescape path compares against `Quote` — this becomes `TCharInfo.Quote`. The
   `Unsafe.Add(ref TChar, ...)` and `MemoryMarshal.CreateReadOnlySpan<TChar>()`
   calls are naturally generic.

6. **`GetColSpanTrimmed(int)`** — same structure, `Space` comparison becomes
   `TCharInfo.Space`

7. **`TrimSpace(ref TChar, ref int)`** — trivially generic

8. **`SepUnescape`** — `UnescapeInPlace` and `TrimUnescapeInPlace` compare
   against `SepDefaults.Quote` and `SepDefaults.Space`. These become generic:
   `UnescapeInPlace<TChar, TCharInfo>(ref TChar charRef, int length)`. The
   algorithms are byte-wise iteration with `Unsafe.Add` — identical for both
   `char` and `byte`.

9. **`GetCachedColIndex` / `TryGetCachedColIndex`** — header lookup cache, pure
   string-based, TChar-agnostic

10. **`ThrowInvalidDataExceptionColCountMismatch`** — needs a small adaption to
    create a string from `ReadOnlySpan<TChar>` (via `new string(chars)` for char,
    `Encoding.UTF8.GetString(bytes)` for byte). This can be a virtual/abstract
    method or use a static interface method for the conversion.

11. **Dispose pattern** — `ArrayPool<TChar>.Shared.Return(_buffer)` is naturally
    generic

12. **`CheckColInfosCapacityMaybeDouble`, `DoubleColInfosCapacityCopyState`** —
    pure int[] operations

13. **`SwapParsedRowsTo`** — already references `_chars` as `TChar[]`, the swap
    logic is identical

### What Lives in Derived Classes (char-specific / byte-specific)

Each derived class adds only the operations that **differ** between char and byte:

**`SepReaderState : SepReaderStateBase<char, SepCharInfoUtf16>`** (~150 lines)
```csharp
public class SepReaderState : SepReaderStateBase<char, SepCharInfoUtf16>
{
    readonly string[] _singleCharToString;  // char-specific cache
    internal SepCreateToString _createToString;
    internal SepToString _toString;
    internal char _fastFloatDecimalSeparatorOrZero;

    // char-specific methods:
    internal string ToStringDefault(int index);  // uses SepToString pooling
    internal string ToStringDirect(int index);
    internal string? TryGetStaticallyCachedString(ReadOnlySpan<char> span);
    internal T Parse<T>(int index) where T : ISpanParsable<T>;
    internal bool TryParse<T>(int index, out T value) where T : ISpanParsable<T>;

    // Cols string/parse methods (delegate to above):
    internal string[] ToStringsArray(...);
    internal Span<string> ToStrings(...);
    internal Span<T> Parse<T>(...); // multi-col

    // Cols Join/Path (char-specific: work on char spans):
    internal ReadOnlySpan<char> Join(...);
    internal string JoinToString(...);
    // etc.

    internal Func<int, string> UnsafeToStringDelegate { get; }

    internal override void DisposeManaged()
    {
        _toString?.Dispose();
        base.DisposeManaged();
    }
}
```

**`SepUtf8ReaderState : SepReaderStateBase<byte, SepCharInfoUtf8>`** (~150 lines)
```csharp
public class SepUtf8ReaderState : SepReaderStateBase<byte, SepCharInfoUtf8>
{
    internal SepUtf8ReaderHeader _utf8Header;  // byte-based header (additional)
    internal SepCreateToBytes _createToBytes;
    internal SepToBytes _toBytes;
    internal byte _fastFloatDecimalSeparatorOrZero;

    // byte-specific methods:
    internal ReadOnlyMemory<byte> ToBytesDefault(int index); // uses SepToBytes pooling
    internal string ToStringDefault(int index);              // UTF-8 decode convenience
    internal T Parse<T>(int index) where T : IUtf8SpanParsable<T>;
    internal bool TryParse<T>(int index, out T value) where T : IUtf8SpanParsable<T>;

    // Cols byte pooling methods:
    internal ReadOnlyMemory<byte>[] ToBytesArray(...);
    internal Span<T> Parse<T>(...); // multi-col, IUtf8SpanParsable

    // Cols Join (byte-specific):
    internal ReadOnlySpan<byte> Join(...);

    internal override void DisposeManaged()
    {
        _toBytes?.Dispose();
        base.DisposeManaged();
    }
}
```

### Performance Guarantee

The generic approach preserves performance because:

1. **Struct type parameters eliminate virtual dispatch**: `TCharInfo` is a struct
   (`SepCharInfoUtf16` or `SepCharInfoUtf8`). The JIT fully specializes each generic
   instantiation — `TCharInfo.Quote` becomes a constant load, not a virtual call.
   This is the same pattern as `ISepColInfoMethods<TColInfo>` already used in
   `SepParseMask`.

2. **`ArrayPool<TChar>` is a known pattern**: The .NET runtime already provides
   type-specialized `ArrayPool<char>` and `ArrayPool<byte>`. No overhead vs
   direct usage.

3. **`Unsafe.Add(ref TChar, ...)` generates optimal code**: For both `char` and
   `byte`, the JIT emits a simple address calculation. For `char` it's
   `base + index * 2`, for `byte` it's `base + index * 1`.

4. **`MemoryMarshal.CreateReadOnlySpan<TChar>`**: Already works for any unmanaged
   type.

5. **No boxing**: Struct constraints (`where TCharInfo : struct`) prevent boxing.
   The static abstract interface pattern ensures compile-time dispatch.

6. **Binary-identical hot paths**: `MoveNextAlreadyParsed()`, `GetColRange()`,
   col-tracking state machine — these are the hot paths and they contain zero
   TChar-dependent operations. They will JIT to identical machine code for both
   instantiations.

### What Can Be Shared (Reused with generics)

1. **`SepParseMask`** — Already generic over `TColInfo`. The char-specific logic
   (`ParseAnyChar`, etc.) compares against `LineFeed`, `CarriageReturn`, `Quote`,
   `separator`. These are parameterized via `ISepCharInfo<TChar>`:
   - For `char`: `ref char charsRef` + compare to `char` constants
   - For `byte`: `ref byte bytesRef` + compare to `byte` constants (same values!)
   - The mask-walking logic (TrailingZeroCount, mask iteration) is identical.
   - Methods become `ParseAny<TChar, TCharInfo, TColInfo, TColInfoMethods>`

2. **SIMD Parsers** — The inner loop structure is the same. Key difference:
   - `char` path: Load 2×`Vector<short>`, pack to `Vector<byte>`, then compare
   - `byte` path: Load 1×`Vector<byte>` directly, then compare
   - The compare & mask extraction is identical
   - Could parameterize with a `Load` abstraction or duplicate the thin shell

3. **`_colEndsOrColInfos`** tracking — Entirely element-type-agnostic (stores
   indices into the buffer, not the chars/bytes themselves)

4. **`SepRowInfo`, `SepColInfo`, `ISepColInfoMethods`** — Completely type-agnostic

5. **Row parsing state machine** (`ParseNewRows`, `MoveNext`,
   `MoveNextAlreadyParsed`) — Logic is the same: fill buffer, parse, track rows.
   The difference is buffer type (`TChar[]`) and read source.

6. **Header**: `SepReaderHeader` lives in the generic base class — both char
   and byte readers use it for `GetCachedColIndex(string)`. The byte reader
   additionally holds `SepUtf8ReaderHeader` for `ReadOnlySpan<byte>` lookups.

7. **`SepUnescape`** — Becomes `SepUnescape<TChar, TCharInfo>`. The algorithm
   iterates bytes/chars with `Unsafe.Add`, comparing to `Quote` and `Space`.
   These constants come from `TCharInfo`.

### What Must Be Separate / New

1. **Data source**:
   - `char` reader: `TextReader` (existing)
   - `byte` reader: `Stream` (primary), `byte[]`/`ReadOnlyMemory<byte>` (in-memory)

2. **Column access API**:
   - `char` reader: `ReadOnlySpan<char> Span` (existing)
   - `byte` reader: `ReadOnlySpan<byte> Span` (new), `.ToString()` decodes UTF-8

3. **Parsing (ISpanParsable vs IUtf8SpanParsable)**:
   - `char`: `T.Parse(ReadOnlySpan<char>, IFormatProvider?)`
   - `byte`: `T.Parse(ReadOnlySpan<byte>, IFormatProvider?)` via `IUtf8SpanParsable<T>`

4. **Pooling**: `SepToString` (char, returns `string`) vs `SepToBytes` (byte,
   returns `ReadOnlyMemory<byte>`)

5. **Writer output**:
   - `char`: `TextWriter.Write(ReadOnlySpan<char>)` (existing)
   - `byte`: `Stream.Write(ReadOnlySpan<byte>)` (primary) or
     `IBufferWriter<byte>` (for Kestrel/PipeWriter zero-copy output)

6. **SIMD load step** (thin layer):
   - `char`: `PackUnsignedSaturate(v0, v1)` then permute
   - `byte`: Direct load (simpler, potentially faster)

7. **csFastFloat**: Different overloads for `ReadOnlySpan<char>` vs
   `ReadOnlySpan<byte>` (both already supported by csFastFloat)

### IO Abstraction Analysis: Stream vs PipeReader vs IBufferWriter

#### Sep's Pull-Based Buffer Model & SIMD Padding Requirement
Sep currently owns its buffer and fills it on demand (pull model):
1. Rents `char[]` from `ArrayPool<char>`
2. Calls `TextReader.Read(Span<char>)` to fill free space
3. Parses in-place using SIMD on the contiguous buffer
4. Compacts (copies trailing unparsed data to front) when needed
5. Doubles buffer if a single row exceeds current capacity

**Critical constraint — SIMD padding**: Every SIMD parser requires **zeroed
padding bytes** after the data end (`ClearPaddingAfterData`). The padding size
equals the vector width (16–64 bytes depending on ISA). SIMD loads read a full
vector past the last data byte; zeros prevent false-positive matches. This means
**Sep must own or control the buffer** — external buffers (e.g. from PipeReader)
don't guarantee zeroed padding at the right offset.

For UTF-8, the equivalent is `Stream.Read(Span<byte>)` → fills `byte[]` buffer.
All internal buffer management, compaction, doubling, and padding logic stays
identical.

#### No `IBufferReader<T>` in .NET BCL

There is no `IBufferReader<T>` interface in the .NET BCL. The reading-side
abstractions that exist are:

| Abstraction | What it is | Model |
|-------------|-----------|-------|
| `Stream` | Base class with `Read(Span<byte>)` | Pull: "here's my span, fill it" |
| `PipeReader` | Concrete class returning `ReadOnlySequence<byte>` | Push/pull hybrid: "here's data, tell me what you consumed" |
| `ReadOnlySequence<T>` | Data structure (possibly multi-segment) | N/A — not IO, just data |
| `SequenceReader<T>` | Ref struct parser over `ReadOnlySequence<T>` | N/A — not IO, just parsing |

**`IBufferWriter<T>` is for output only** — it provides a "give me a span to
write into, then I advance" model. It doesn't fit the reading side. A data source
can't "push" into Sep via `IBufferWriter<byte>` because Sep's pull model needs
to control *when* and *how much* data is read.

#### Stream Already Means "Read Directly Into Sep's Buffer"

`Stream.Read(Span<byte>)` is exactly the pattern where external data goes
**directly into Sep's owned buffer** with zero intermediate copies:

```csharp
// Existing char reader pattern:
_reader.Read(_chars.AsSpan(_charsDataEnd, freeLength))  // TextReader → char[]

// UTF-8 equivalent:
_stream.Read(_bytes.AsSpan(_bytesDataEnd, freeLength))  // Stream → byte[]
```

For a `FileStream`, this means: `kernel buffer → Sep's byte[]` (one copy — the
minimum possible for file I/O). There's no intermediate buffer. Sep provides its
own `ArrayPool<byte>`-rented span, complete with room for SIMD padding, and the
Stream fills it directly.

This is fundamentally more efficient than PipeReader for Sep's use case, because
PipeReader interposes its own buffer pool between the source and the consumer.

#### Why PipeReader Cannot Replace Stream for Sep

1. **Buffer ownership conflict**: Sep needs a **writable, owned, contiguous
   buffer** with zeroed padding after data. PipeReader returns **read-only,
   possibly multi-segment** buffers that it owns and manages. Sep can't clear
   padding, compact, or double PipeReader's buffers.

2. **Multi-segment ReadOnlySequence**: `ReadOnlySequence<byte>` from PipeReader
   can span multiple non-contiguous memory segments. Sep's SIMD parsers use
   `Vector256.LoadUnsafe` and require contiguous `Span<byte>`. Handling segment
   boundaries in SIMD loops is impractical.

3. **No way to inject Sep's buffer into PipeReader**: You cannot tell a
   `PipeReader` "use my ArrayPool buffer". The Pipe manages its own buffer pool.
   So PipeReader always means: `source → pipe buffer → (copy to Sep buffer or
   parse from pipe buffer)`.

4. **Parse from pipe buffer isn't safe**: Even for single-segment sequences, the
   pipe buffer doesn't have Sep's required zeroed padding at the end. Parsing
   directly from `ReadOnlySequence.FirstSpan` would read past the data boundary
   into unknown memory (or another segment's data).

#### Can Sep's Buffer Live Inside an External Abstraction?

The question is: could Sep's `byte[]` be the backing buffer of some external
abstraction so that data arrives already in Sep's buffer?

- **PipeReader**: No. PipeReader allocates from its own `MemoryPool<byte>`. You
  can provide a custom `MemoryPool` to `PipeReader.Create(Stream,
  StreamPipeReaderOptions)`, but the pool returns `IMemoryOwner<byte>` segments
  that PipeReader manages — Sep can't control segment sizes, alignment, or
  padding. This is a dead end.

- **IBufferWriter<byte> inverted**: Sep could theoretically implement
  `IBufferWriter<byte>` and ask data sources to write into it. But this inverts
  the control flow — there's no standard "data source writes to IBufferWriter"
  pattern in .NET for files or streams. You'd still need `Stream.Read()` or
  `RandomAccess.Read()` to pull from files.

- **Memory-mapped files**: `MemoryMappedFile` gives a pointer to file data, which
  could be wrapped in `ReadOnlyMemory<byte>`. However: (a) Sep needs writable
  padding beyond data end — mmap'd pages may be read-only, (b) page faults cause
  unpredictable latency, (c) not suitable for streaming/network data. Not
  practical as a general solution.

**Conclusion**: `Stream.Read(Span<byte>)` is already the "read directly into
Sep's buffer" pattern. No other abstraction improves on this for Sep's needs.

#### Internal Abstraction: ISepByteSource (optional refinement)

Rather than hardcoding `Stream` inside the UTF-8 reader, Sep could define a thin
internal interface to abstract over different byte sources:

```csharp
internal interface ISepByteSource : IDisposable
{
    int Read(Span<byte> buffer);
    ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken ct);
}

// Implementations:
// SepStreamByteSource    — wraps Stream (default, universal)
// SepMemoryByteSource    — wraps byte[]/ReadOnlyMemory<byte> (avoids MemoryStream)
// SepPipeReaderByteSource — wraps PipeReader (future, copies segments to span)
```

The `SepMemoryByteSource` would avoid `MemoryStream` overhead for in-memory data
by copying directly from the source memory into Sep's span. For small data that
fits in one buffer, it could even set Sep's buffer to the source array directly
(with extra allocation for padding). This is a minor optimization — start with
`MemoryStream` wrapper for simplicity, refine later if profiling shows benefit.

#### Files and BOM Handling

For file reading with the UTF-8 byte reader, two key considerations:

1. **File IO path**: `FileStream.Read(Span<byte>)` reads directly into Sep's
   buffer with one kernel→user copy. This is optimal. Using `FileOptions.
   SequentialScan` (as the existing char reader already does) enables OS
   read-ahead. No improvement possible here — this is the minimum copy path.

2. **UTF-8 BOM (Byte Order Mark)**: The 3-byte sequence `0xEF 0xBB 0xBF` may
   appear at the start of UTF-8 files. Currently, `StreamReader` handles BOM
   detection/skipping automatically for the char reader. For the UTF-8 byte
   reader, Sep must handle this itself:
   - After the first buffer fill, check if the first 3 bytes are `0xEF 0xBB 0xBF`
   - If so, advance `_bytesDataStart` past the BOM (skip 3 bytes)
   - This is trivial and only runs once during initialization
   - For `byte[]` input, same check on first 3 bytes
   - An option `SkipBom` (default `true`) could control this behavior

   Note: BOM detection is orthogonal to the IO abstraction choice — any source
   (Stream, byte[], PipeReader) needs the same BOM check on first bytes read.

#### Kestrel / ASP.NET Core Integration

In ASP.NET Core (including .NET 10), Kestrel exposes request bodies two ways:
- `HttpRequest.Body` → `Stream` (wraps the internal PipeReader)
- `HttpRequest.BodyReader` → `PipeReader` (direct access to pipe)

Using `HttpRequest.Body` (Stream) with Sep UTF-8:
```
Kestrel socket → Pipe buffer → Stream adapter → Sep's byte[] buffer
```
One copy (pipe buffer → Sep buffer). This is the practical, recommended path.

Using `HttpRequest.BodyReader` (PipeReader) directly would require copying
from possibly multi-segment `ReadOnlySequence<byte>` into Sep's contiguous padded
buffer — the same one copy, but with more complex code. The theoretical
advantage of avoiding this copy is blocked by Sep's padding requirement.

**Verdict**: `Stream` is the correct primary abstraction. For Kestrel, the copy
from pipe buffer to Sep's buffer is negligible — Sep parses many rows of CSV
data per buffer fill, so the parsing work dominates the fill cost.

#### Writer Output Abstractions

For writing, the situation is simpler since Sep writes complete rows:
- **`Stream`** (primary): `Stream.Write(ReadOnlySpan<byte>)` — universal, works
  everywhere
- **`IBufferWriter<byte>`** (secondary): Optimal for `PipeWriter` /
  `HttpResponse.BodyWriter` in Kestrel. Sep can request a span from the
  `IBufferWriter`, write the row directly into the pipe's memory, and advance —
  avoiding an intermediate copy. `PipeWriter` implements `IBufferWriter<byte>`,
  so this covers the Kestrel output path naturally. Worth including from V1.

```csharp
// Reader input (V1)
Sep.Utf8Reader().FromFile("data.csv")          // FileStream internally
Sep.Utf8Reader().FromBytes(bytes)              // byte[] / ReadOnlyMemory<byte>
Sep.Utf8Reader().From(stream)                  // any Stream

// Writer output (V1)
Sep.Utf8Writer().ToFile("data.csv")            // FileStream internally
Sep.Utf8Writer().To(stream)                    // any Stream
Sep.Utf8Writer().To(bufferWriter)              // IBufferWriter<byte>
```

#### Summary of IO Abstraction Decision

| Abstraction | Role | Rationale | When |
|-------------|------|-----------|------|
| `Stream` (read) | Primary input | Pull model fits Sep; reads directly into Sep's padded buffer | V1 |
| `byte[]` / `ReadOnlyMemory<byte>` (read) | In-memory input | Via `MemoryStream` initially; `ISepByteSource` later | V1 |
| `Stream` (write) | Primary output | Universal | V1 |
| `IBufferWriter<byte>` (write) | High-perf output | Enables zero-copy writes to Kestrel PipeWriter | V1 |
| `PipeReader` (read) | Advanced input | Copies segments to Sep's padded buffer; no zero-copy benefit due to padding requirement | Future |
| `ISepByteSource` (internal) | Abstraction over sources | Allows optimized byte[]/PipeReader paths without Stream overhead | Future |

### Fluent API Design

The UTF-8 API mirrors the existing pattern exactly, using `Sep.Utf8Reader()` /
`Sep.Utf8Writer()` as entry points that return options records flowing to
`FromXXX`/`ToXXX`:

```csharp
// Existing char API (unchanged)
Sep.Reader().FromFile("data.csv")           // → SepReader
Sep.Reader().FromText(text)                 // → SepReader
Sep.New(',').Reader().FromFile("data.csv")  // → SepReader

// New UTF-8 API (parallel structure)
Sep.Utf8Reader().FromFile("data.csv")               // → SepUtf8Reader
Sep.Utf8Reader().FromBytes(bytes)                    // → SepUtf8Reader
Sep.Utf8Reader().From(stream)                        // → SepUtf8Reader
Sep.New(',').Utf8Reader().FromFile("data.csv")       // → SepUtf8Reader

// Writer
Sep.Utf8Writer().ToFile("data.csv")                  // → SepUtf8Writer
Sep.Utf8Writer().To(stream)                          // → SepUtf8Writer
Sep.New(',').Utf8Writer().ToFile("data.csv")         // → SepUtf8Writer
```

Implementation on `Sep`:
```csharp
public readonly record struct Sep
{
    // Existing (unchanged)
    public static SepReaderOptions Reader() => new(null);
    public static SepWriterOptions Writer() => new(Default);

    // New
    public static SepUtf8ReaderOptions Utf8Reader() => new(null);
    public static SepUtf8ReaderOptions Utf8Reader(
        Func<SepUtf8ReaderOptions, SepUtf8ReaderOptions> configure) =>
        configure(Utf8Reader());

    public static SepUtf8WriterOptions Utf8Writer() => new(Default);
    public static SepUtf8WriterOptions Utf8Writer(
        Func<SepUtf8WriterOptions, SepUtf8WriterOptions> configure) =>
        configure(Utf8Writer());
}
```

Extension methods on `Sep` (instance), `Sep?`, and `SepSpec`:
```csharp
public static partial class SepUtf8ReaderExtensions
{
    public static SepUtf8ReaderOptions Utf8Reader(this Sep sep) => new(sep);
    public static SepUtf8ReaderOptions Utf8Reader(this Sep? sep) => new(sep);
    public static SepUtf8ReaderOptions Utf8Reader(this SepSpec spec) => ...;
    // + configure overloads
}

public static partial class SepUtf8WriterExtensions
{
    public static SepUtf8WriterOptions Utf8Writer(this Sep sep) => new(sep);
    public static SepUtf8WriterOptions Utf8Writer(this SepSpec spec) => ...;
    // + configure overloads
}
```

### Header Lookup: Separate Types for `char` and `byte`

`SepReaderHeader` (existing, char/string-based) is kept in the generic base class
so that `GetCachedColIndex(string)` works identically for both readers — no
interface or virtual dispatch needed. `SepUtf8ReaderHeader` (new, byte-based) is
an **additional** header held only by `SepUtf8ReaderState` for byte-span lookups.

**`SepUtf8Reader` holds both headers:**
- `SepReaderHeader _header` (inherited from base) — built during init by
  decoding UTF-8 column names to strings. Used for `Row[string colName]` and
  the `_colNameCache` fast path. This is the same header the char reader uses.
- `SepUtf8ReaderHeader _utf8Header` (in `SepUtf8ReaderState`) — built during
  init from the raw UTF-8 bytes. Used for `Row[ReadOnlySpan<byte> utf8ColName]`.

This means:
- The base class stays simple — `SepReaderHeader _header` field, no interfaces.
- `GetCachedColIndex(string)` is unchanged, calls `_header.TryIndexOf(string)`.
- The char reader (`SepReader`) uses only `_header` — no change at all.
- The byte reader (`SepUtf8Reader`) has both, paying the small cost of storing
  column names twice (strings + bytes). Headers are tiny relative to data.

```csharp
// Existing (unchanged)
public sealed class SepReaderHeader
{
    public int IndexOf(string colName) => ...;
    public bool TryIndexOf(string colName, out int colIndex) => ...;
#if NET9_0_OR_GREATER
    public int IndexOf(ReadOnlySpan<char> colName) => ...;
    public bool TryIndexOf(ReadOnlySpan<char> colName, out int colIndex) => ...;
#endif
    public IReadOnlyList<string> ColNames => ...;
    public IReadOnlyList<string> NamesStartingWith(string prefix, ...) => ...;
    public int[] IndicesOf(...) => ...;
}

// New: UTF-8 byte-based header (held by SepUtf8ReaderState alongside SepReaderHeader)
public sealed class SepUtf8ReaderHeader
{
    readonly byte[] _row;                          // raw UTF-8 header row
    readonly byte[][] _utf8ColNames;               // each column name as UTF-8 bytes

    // Primary: byte-based lookup (allocation-free hot path)
    public int IndexOf(ReadOnlySpan<byte> utf8ColName) => ...;
    public bool TryIndexOf(ReadOnlySpan<byte> utf8ColName, out int colIndex) => ...;
    public int[] IndicesOf(params ReadOnlySpan<ReadOnlyMemory<byte>> utf8ColNames) => ...;

    // Column names as bytes
    public IReadOnlyList<ReadOnlyMemory<byte>> Utf8ColNames => ...;

    // String access delegates to the companion SepReaderHeader
    // (no need to duplicate — SepUtf8Reader exposes both headers)
}
```

For the `SepUtf8Reader.Row` indexer, both `string` and `ReadOnlySpan<byte>`
column access is supported:
```csharp
public readonly ref struct Row  // SepUtf8Reader.Row
{
    public Col this[string colName] => ...;      // via base _header (SepReaderHeader)
    public Col this[ReadOnlySpan<byte> utf8ColName] => ...; // via _utf8Header
    public Col this[int index] => ...;           // index-based
}
```

### Proposed Type Hierarchy

```
// Generic base (internal, shared by both readers)
SepReaderStateBase<TChar, TCharInfo>
    where TChar : unmanaged, IEquatable<TChar>
    where TCharInfo : struct, ISepCharInfo<TChar>
    — TChar[] _buffer, int[] _colEndsOrColInfos, SepRowInfo[] _parsedRows
    — MoveNextAlreadyParsed(), GetColRange(), GetColSpan(), RowSpan()
    — GetCachedColIndex() via base _header (SepReaderHeader), TrimSpace()
    — SepUnescape (generic)
    — ~700 lines of shared code

// Existing (unchanged public API, now inherits from generic base)
SepReaderState : SepReaderStateBase<char, SepCharInfoUtf16>
    — SepToString, Parse<T> via ISpanParsable, csFastFloat (char overloads)
    — Join/JoinPaths (char-specific), UnsafeToStringDelegate
    — ~150 lines char-specific
SepReader : SepReaderState → reads TextReader, char[] buffer
SepReader.Row            (ref struct)
SepReader.Col            (ref struct) → ReadOnlySpan<char>

SepWriter                → writes to TextWriter, char-based
SepWriter.Row            (ref struct)
SepWriter.Col            (ref struct)

// New UTF-8 types (parallel to existing, same generic base)
SepUtf8ReaderState : SepReaderStateBase<byte, SepCharInfoUtf8>
    — SepUtf8ReaderHeader (byte-based, additional to base _header)
    — SepToBytes, Parse<T> via IUtf8SpanParsable, csFastFloat (byte overloads)
    — ~150 lines byte-specific
SepUtf8Reader : SepUtf8ReaderState → reads Stream, byte[] buffer
SepUtf8Reader.Row        (ref struct)
SepUtf8Reader.Col        (ref struct) → ReadOnlySpan<byte>

SepUtf8Writer            → writes to Stream or IBufferWriter<byte>
SepUtf8Writer.Row        (ref struct)
SepUtf8Writer.Col        (ref struct)

// Parser interfaces
ISepParser               — unchanged for char parsers (takes SepReaderState)
ISepUtf8Parser           — new, for byte parsers (takes SepUtf8ReaderState)
// Both could later unify via ISepParser<TState> but not necessary for V1
```

### Alternative: Single Generic `SepReader<TChar>`
A fully generic `SepReader<TChar> where TChar : unmanaged` could unify both, but
this would complicate the public API, require users to always specify the type
parameter, and be a breaking change. A separate `SepUtf8Reader` type with shared
internal generic implementation is cleaner and preserves backward compatibility.

## Implementation Plan — Step by Step

Focus on **UTF-8 Reader first** (Stream-based). Writer is a separate follow-up.
Each step must leave the build green. Existing tests must never break.

### Step 0: Verify Baseline
- `dotnet build` the solution
- `dotnet test` all test projects
- Confirm clean starting point on `utf8` branch

### Step 1: `ISepCharInfo<TChar>` — Char/Byte Constants Abstraction
**Create** `src/Sep/Internals/ISepCharInfo.cs`

Also add `SpaceByte` to `SepDefaults.cs` (alongside existing `LineFeedByte`,
`CarriageReturnByte`, `QuoteByte`):
```csharp
internal const byte SpaceByte = (byte)Space;
```

```csharp
internal interface ISepCharInfo<TChar> where TChar : unmanaged, IEquatable<TChar>
{
    static abstract TChar LineFeed { get; }
    static abstract TChar CarriageReturn { get; }
    static abstract TChar Quote { get; }
    static abstract TChar Space { get; }
}

internal readonly struct SepCharInfoUtf16 : ISepCharInfo<char>
{
    public static char LineFeed => SepDefaults.LineFeed;
    public static char CarriageReturn => SepDefaults.CarriageReturn;
    public static char Quote => SepDefaults.Quote;
    public static char Space => SepDefaults.Space;
}

internal readonly struct SepCharInfoUtf8 : ISepCharInfo<byte>
{
    public static byte LineFeed => SepDefaults.LineFeedByte;
    public static byte CarriageReturn => SepDefaults.CarriageReturnByte;
    public static byte Quote => SepDefaults.QuoteByte;
    public static byte Space => SepDefaults.SpaceByte;
}
```
- Build — no behavioral change, just new types.

### Step 2: `SepUnescape<TChar, TCharInfo>` — Generic Unescape
**Modify** `src/Sep/Internals/SepUnescape.cs`
- Make `UnescapeInPlace` and `TrimUnescapeInPlace` generic:
  `UnescapeInPlace<TChar, TCharInfo>(ref TChar charRef, int length)`
  where `TCharInfo : struct, ISepCharInfo<TChar>`
- Replace `SepDefaults.Quote` → `TCharInfo.Quote`
- Replace `SepDefaults.Space` → `TCharInfo.Space`
- Keep the existing non-generic methods as thin wrappers calling the generic
  version (avoids breaking internal callers):
  ```csharp
  internal static int UnescapeInPlace(ref char charRef, int length)
      => UnescapeInPlace<char, SepCharInfoUtf16>(ref charRef, length);
  ```
- Build + test — existing behavior unchanged.

### Step 3: `SepReaderStateBase<TChar, TCharInfo>` — Extract Generic Base
**Create** `src/Sep/SepReaderStateBase.cs`
- Extract from `SepReaderState.cs` all TChar-agnostic fields and methods:
  - Fields: `_buffer` (renamed from `_chars`), `_charsDataStart`, `_charsDataEnd`,
    `_trailingCarriageReturn`, `_charsParseStart`, `_parsingLineNumber`,
    `_parsingRowCharsStartIndex`, `_parsingRowColEndsOrInfosStartIndex`,
    `_parsingRowColCount`, `_colSpanFlags`, `_colCountExpected`,
    `_colEndsOrColInfos`, `_parsedRows`, `_parsedRowsCount`, `_parsedRowIndex`,
    `_currentRowColCount`, `_currentRowColEndsOrInfosOffset`,
    `_currentRowLineNumberFrom`, `_currentRowLineNumberTo`, `_rowIndex`,
    `_header`, `_hasHeader`, `_cultureInfo`, `_arrayPool`, `_colNameCache`,
    `_cacheIndex`
  - Methods: `MoveNextAlreadyParsed()`, `RowStartEnd()`, `RowSpan()`,
    `GetCachedColIndex()`, `TryGetCachedColIndex()`, `GetColRange()`,
    `GetColSpan()`, `GetColSpanTrimmed()`, `GetColSpanTrimmedRange()`,
    `TrimSpace()` (now uses `TCharInfo.Space`),
    `CheckColInfosCapacityMaybeDouble()`, `DoubleColInfosCapacityCopyState()`,
    `SwapParsedRowsTo()`, `GetColsEntireSpanAs<T>()`, `GetColInfosLength()`,
    `GetIntegersPerColInfo()`, `GetColsRefAs<T>()`
  - Dispose: `ArrayPool<TChar>.Shared.Return(_buffer)`, `_colEndsOrColInfos`,
    `_parsedRows`, `_arrayPool`
  - Unescape calls in `GetColSpan()` become `SepUnescape.UnescapeInPlace<TChar, TCharInfo>(...)`
  - `ThrowInvalidDataExceptionColCountMismatch`: make virtual/abstract `CreateRowString()`
    method — char version uses `new string(...)`, byte version uses `Encoding.UTF8.GetString(...)`

**Modify** `src/Sep/SepReaderState.cs`
- Change: `public class SepReaderState : SepReaderStateBase<char, SepCharInfoUtf16>`
- Keep **only** char-specific members:
  - `_singleCharToString`, `_createToString`, `_toString`,
    `_fastFloatDecimalSeparatorOrZero`
  - `ToStringDefault()`, `ToStringDirect()`, `TryGetStaticallyCachedString()`
  - `Parse<T>()`, `TryParse<T>()` (ISpanParsable)
  - All `#region Cols` methods that produce strings: `ToStringsArray`, `ToStrings`,
    `ParseToArray`, `Parse`, `TryParse`, `ColsSelect`, `Join`, `JoinToString`,
    `JoinPathsToString`, `CombinePathsToString`
  - `UnsafeToStringDelegate`
  - `DisposeManaged()` override: dispose `_toString`, call `base.DisposeManaged()`
- The `SepReaderState(SepReader other)` copy-constructor remains in
  `SepReaderState`, calling `base(other)` for shared fields

- **Critical**: `SepReader` already inherits from `SepReaderState`. Since
  `SepReaderState` now inherits from `SepReaderStateBase<char, SepCharInfoUtf16>`,
  `SepReader` transitively gets all the base methods. No change needed in
  `SepReader.cs` itself.

- Build + run all existing tests — must pass identically. This is a **refactor
  with no functional change**. The JIT produces identical code because all
  `SepCharInfoUtf16` static abstract calls are inlined as constants.

### Step 4: `SepParseMask` Generic Overloads
**Modify** `src/Sep/Internals/SepParseMask.cs`
- Add generic overloads that take `ref TChar` instead of `ref char`:
  ```csharp
  internal static ref TColInfo ParseAny<TChar, TCharInfo, TColInfo, TColInfoMethods>(
      scoped ref TChar charsRef, int charsIndex, int relativeIndex, TChar separator,
      scoped ref int rowLineEndingOffset, scoped ref nuint quoteCount,
      ref TColInfo colInfosRef, scoped ref int lineNumber)
      where TChar : unmanaged, IEquatable<TChar>
      where TCharInfo : struct, ISepCharInfo<TChar>
      where TColInfo : unmanaged
      where TColInfoMethods : ISepColInfoMethods<TColInfo>
  ```
- Replace hardcoded `CarriageReturn`/`LineFeed`/`Quote` → `TCharInfo.XXX`
- Replace `==` comparisons → `.Equals()` or `EqualityComparer<TChar>.Default`
  (the JIT devirtualizes for `char.Equals(char)` and `byte.Equals(byte)`)
- Keep existing non-generic `ParseAnyChar` methods as thin wrappers:
  ```csharp
  internal static ref TColInfo ParseAnyChar<TColInfo, TColInfoMethods>(...)
      => ref ParseAny<char, SepCharInfoUtf16, TColInfo, TColInfoMethods>(...);
  ```
- Similarly for `ParseSeparatorsLineEndingsMasks`, `ParseLineEndingMask`,
  `ParseSeparatorLineEndingChar`
- Build + test — all existing tests pass (wrappers ensure binary compatibility).

### Step 5: `ISepUtf8Parser` — Byte Parser Interface
**Create** `src/Sep/Internals/ISepUtf8Parser.cs`
```csharp
internal interface ISepUtf8Parser
{
    int PaddingLength { get; }
    int QuoteCount { get; }
    void ParseColEnds(SepUtf8ReaderState s);
    void ParseColInfos(SepUtf8ReaderState s);
}
```
- Mirrors `ISepParser` exactly but takes `SepUtf8ReaderState`.
- Build (requires forward-declared `SepUtf8ReaderState` — create minimal stub).

### Step 6: `SepUtf8ParserIndexOfAny` — First Byte Parser
**Create** `src/Sep/Internals/SepUtf8ParserIndexOfAny.cs`
- Port `SepParserIndexOfAny` for bytes
- Replace `string.IndexOfAny` → `Span<byte>.IndexOfAny(SearchValues<byte>)`
  (or `byte[]` special chars array)
- Uses `SepParseMask` generic overloads (`ParseAny<byte, SepCharInfoUtf8, ...>`)
- Implement `ISepUtf8Parser`
- Build.

### Step 7: `SepUtf8ParserFactory`
**Create** `src/Sep/Internals/SepUtf8ParserFactory.cs`
- For now returns `SepUtf8ParserIndexOfAny` only (simple, correct)
- Same `SepParserOptions` input (separator + disableQuotesParsing)
- SIMD byte parsers added later as optimization
- Build.

### Step 8: `SepToBytes` — ReadOnlyMemory\<byte\> Pooling Abstraction
**Create** `src/Sep/SepToBytes.cs`
```csharp
public delegate SepToBytes SepCreateToBytes(
    SepUtf8ReaderHeader? maybeHeader, int colCount);

public abstract class SepToBytes : IDisposable
{
    public abstract ReadOnlyMemory<byte> ToMemory(
        ReadOnlySpan<byte> colSpan, int colIndex);
    public virtual bool IsThreadSafe => false;

    public static SepCreateToBytes Direct { get; }
    public static SepCreateToBytes PoolPerCol(
        int maximumByteLength = 32, int maximumCapacityPerCol = 4096);
    public static SepCreateToBytes PoolPerColThreadSafe(
        int maximumByteLength = 32, int initialCapacityPerCol = 64);
}
```

**Create** `src/Sep/Internals/SepToBytesDirect.cs`
- `ToMemory` → `new byte[span.Length]` + copy, returns as `ReadOnlyMemory<byte>`

**Create** `src/Sep/Internals/SepBytesHashPool.cs`
- Port of `SepStringHashPool` for `byte[]` instead of `string`
- Hash function over `ReadOnlySpan<byte>` (same `SepHash` approach)
- Returns `ReadOnlyMemory<byte>` backed by cached `byte[]`
- Single-byte cache: `byte[][]` for bytes 0–255 (replaces `_singleCharToString`)
- Same per-column strategy, collision limits, max-length thresholds

**Create** `src/Sep/Internals/SepToBytesHashPoolPerCol.cs`
- Per-column pool array, mirrors `SepToStringHashPoolPerCol`

- Build.

### Step 9: `SepUtf8ReaderState` — Byte-Specific Derived Class
**Create** `src/Sep/SepUtf8ReaderState.cs`
```csharp
public class SepUtf8ReaderState : SepReaderStateBase<byte, SepCharInfoUtf8>
{
    internal SepUtf8ReaderHeader _utf8Header;  // byte-based header (alongside base _header)
    internal SepCreateToBytes _createToBytes;
    internal SepToBytes _toBytes;
    internal byte _fastFloatDecimalSeparatorOrZero;

    // Parse via IUtf8SpanParsable<T>
    internal T Parse<T>(int index) where T : IUtf8SpanParsable<T>;
    internal bool TryParse<T>(int index, out T value) where T : IUtf8SpanParsable<T>;

    // Byte pooling
    internal ReadOnlyMemory<byte> ToBytesDefault(int index);
    // String convenience (decodes UTF-8)
    internal string ToStringDefault(int index);

    // Cols multi-column methods (parallel to SepReaderState)
    internal Span<T> Parse<T>(ReadOnlySpan<int> colIndices) where T : IUtf8SpanParsable<T>;
    // ... etc.

    internal override void DisposeManaged()
    {
        _toBytes?.Dispose();
        base.DisposeManaged();
    }
}
```
- Uses csFastFloat byte overloads for float/double fast paths
- Build.

### Step 10: `SepUtf8ReaderOptions` — Options Record
**Create** `src/Sep/SepUtf8ReaderOptions.cs`
```csharp
public readonly record struct SepUtf8ReaderOptions
{
    public Sep? Sep { get; init; }
    public int InitialBufferLength { get; init; }
    public CultureInfo? CultureInfo { get; init; }
    public bool HasHeader { get; init; }
    public IEqualityComparer<string> ColNameComparer { get; init; }
    public SepCreateToBytes CreateToBytes { get; init; }  // ReadOnlyMemory<byte> pooling
    public bool DisableFastFloat { get; init; }
    public bool DisableColCountCheck { get; init; }
    public bool DisableQuotesParsing { get; init; }
    public bool Unescape { get; init; }
    public SepTrim Trim { get; init; }
    public bool SkipBom { get; init; }  // new, default true
}
```
- Mirror `SepReaderOptions` (omit `AsyncContinueOnCapturedContext` initially).
- Build.

### Step 11: `SepUtf8Reader` + Row/Col/Cols — Main Reader
**Create** `src/Sep/SepUtf8Reader.cs`
- Sealed class inheriting `SepUtf8ReaderState`
- Owns `Stream _stream` (not TextReader)
- Constructor: takes `Info`, `SepUtf8ReaderOptions`, `Stream`
- `Initialize()`: first buffer fill, BOM detection (skip 3 bytes if `0xEF BB BF`),
  separator auto-detection, header parsing. Builds **both** `SepReaderHeader`
  (decodes col names to strings, stored in base `_header`) and
  `SepUtf8ReaderHeader` (stores raw UTF-8 byte col names in `_utf8Header`).
- `MoveNext()` / `ParseNewRows()` / `FillAndMaybeDoubleBytesBuffer()`:
  copy from `SepReader.IO.Sync.cs`, replace `_reader.Read(Span<char>)` →
  `_stream.Read(Span<byte>)`. Note: the carriage return lookahead logic is
  simpler for bytes since `Stream.Read` reads bytes directly (no character
  encoding complications). The `_trailingCarriageReturn` pattern works identically.
- `Dispose()`: dispose stream + base

**Create** `src/Sep/SepUtf8Reader.Row.cs`
```csharp
public readonly ref struct Row
{
    public Col this[int index] => ...;
    public Col this[string colName] => ...;      // string-based (via header)
    public Col this[ReadOnlySpan<byte> utf8ColName] => ...; // UTF-8 byte-based
    public Cols this[Range range] => ...;
    // Mirror SepReader.Row API
}
```

**Create** `src/Sep/SepUtf8Reader.Col.cs`
```csharp
public readonly ref struct Col
{
    public ReadOnlySpan<byte> Span { get; }        // raw UTF-8 bytes (ephemeral, tied to row)
    public ReadOnlyMemory<byte> ToMemory();        // pooled ReadOnlyMemory<byte> (persists beyond row)
    public T Parse<T>() where T : IUtf8SpanParsable<T>;
    public override string ToString();             // Encoding.UTF8.GetString (convenience)
}
```

**Create** `src/Sep/SepUtf8Reader.Cols.cs`
- Multi-column access, mirrors SepReader.Cols for byte spans

- Build.

### Step 12: IO Extensions — FromFile / FromBytes / From(Stream)
**Create** `src/Sep/SepUtf8ReaderExtensions.cs`
```csharp
public static partial class SepUtf8ReaderExtensions
{
    public static SepUtf8ReaderOptions Utf8Reader(this Sep sep) => ...;
    public static SepUtf8ReaderOptions Utf8Reader(this Sep? sep) => ...;
    public static SepUtf8ReaderOptions Utf8Reader(this SepSpec spec) => ...;
}
```

**Create** `src/Sep/SepUtf8ReaderExtensions.IO.Sync.cs`
```csharp
public static partial class SepUtf8ReaderExtensions
{
    public static SepUtf8Reader FromFile(this in SepUtf8ReaderOptions options, string filePath);
    public static SepUtf8Reader FromBytes(this in SepUtf8ReaderOptions options, byte[] buffer);
    public static SepUtf8Reader FromBytes(this in SepUtf8ReaderOptions options,
        ReadOnlyMemory<byte> buffer);
    public static SepUtf8Reader From(this in SepUtf8ReaderOptions options, Stream stream);
}
```
- `FromFile` opens `FileStream` with `SequentialScan`
- `FromBytes` wraps in `MemoryStream`
- `From(Stream)` passes stream directly

**Modify** `src/Sep/Sep.cs`
- Add `public static SepUtf8ReaderOptions Utf8Reader() => new(null);`
- Add configure overload

- Build. **Reader is now usable end-to-end!**

### Step 13: `SepUtf8ReaderHeader` — Byte Header
**Create** `src/Sep/SepUtf8ReaderHeader.cs`
- Stores `byte[] _row` (raw UTF-8 header row) and `byte[][] _utf8ColNames`
- `IndexOf(ReadOnlySpan<byte>)` — linear scan with `SequenceEqual` (allocation-free)
- `TryIndexOf(ReadOnlySpan<byte>, out int)` — same
- `IndicesOf(params ReadOnlySpan<ReadOnlyMemory<byte>>)` — batch lookup
- `Utf8ColNames` property returns `IReadOnlyList<ReadOnlyMemory<byte>>`
- Build.

### Step 14: Tests
**Create** `src/Sep.Test/SepUtf8ReaderTest.cs` (or add to existing test project)
- Basic CSV parsing from byte array
- Separator auto-detection
- Column access by index, name (string), name (UTF-8 bytes)
- `Parse<int>()`, `Parse<float>()`, `Parse<double>()` via IUtf8SpanParsable
- `ToString()` on Col (UTF-8 → string decode)
- BOM handling (with BOM, without BOM, SkipBom=false)
- Multi-row iteration
- Empty input / single row / no header
- Quoted fields / unescaping
- Header column count mismatch
- Build + test all. Verify existing tests still pass.

### Step 15: SIMD Byte Parsers (Optimization)
**Create** byte variants of key SIMD parsers (skip char→byte pack step):
- `SepUtf8ParserAvx2CmpOrMoveMaskTzcnt.cs` — direct `Vector256<byte>` load
- `SepUtf8ParserSse2CmpOrMoveMaskTzcnt.cs` — direct `Vector128<byte>` load
- Add others based on platform support
**Modify** `SepUtf8ParserFactory.cs` — select best parser based on ISA
- Build + test. Benchmark to verify SIMD advantage.

### Step 16: Writer (Future Phase)
Separate PR/phase. Uses `Stream.Write(ReadOnlySpan<byte>)` for output.
`IBufferWriter<byte>` support for Kestrel scenarios.
Not planned for this execution pass.

### File Summary

| Step | Action | File |
|------|--------|------|
| 1 | Create | `src/Sep/Internals/ISepCharInfo.cs` |
| 2 | Modify | `src/Sep/Internals/SepUnescape.cs` |
| 3 | Create | `src/Sep/SepReaderStateBase.cs` |
| 3 | Modify | `src/Sep/SepReaderState.cs` (extract base) |
| 4 | Modify | `src/Sep/Internals/SepParseMask.cs` |
| 5 | Create | `src/Sep/Internals/ISepUtf8Parser.cs` |
| 6 | Create | `src/Sep/Internals/SepUtf8ParserIndexOfAny.cs` |
| 7 | Create | `src/Sep/Internals/SepUtf8ParserFactory.cs` |
| 8 | Create | `src/Sep/SepToBytes.cs`, `SepToBytesDirect.cs`, `SepBytesHashPool.cs`, `SepToBytesHashPoolPerCol.cs` |
| 9 | Create | `src/Sep/SepUtf8ReaderState.cs` |
| 10 | Create | `src/Sep/SepUtf8ReaderOptions.cs` |
| 11 | Create | `src/Sep/SepUtf8Reader.cs`, `SepUtf8Reader.Row.cs`, `.Col.cs`, `.Cols.cs` |
| 12 | Create | `src/Sep/SepUtf8ReaderExtensions.cs`, `.IO.Sync.cs` |
| 12 | Modify | `src/Sep/Sep.cs` |
| 13 | Create | `src/Sep/SepUtf8ReaderHeader.cs` |
| 14 | Create | `src/Sep.Test/SepUtf8ReaderTest.cs` + related |
| 15 | Create | SIMD byte parsers (optimization) |

## Key Design Decisions

1. **Public API naming**: `Sep.Utf8Reader()` / `Sep.Utf8Writer()` — consistent
   with existing `Sep.Reader()` / `Sep.Writer()`, no ambiguity.

2. **`SepReaderState` base class change**: `SepReaderState` will inherit from
   a new internal `SepReaderStateBase<char>`. Since `SepReaderState`'s
   constructor is `internal` and it's not designed for external subclassing, this
   is not a source-level breaking change. It may be a binary-level breaking
   change for anyone directly referencing `SepReaderState` as a base type, but
   this is an extremely unlikely scenario. Package validation baseline should be
   checked.

3. **Parser reuse strategy**: The SIMD parsers could either:
   - (a) Have a generic `Parse<TChar, TColInfo, TColInfoMethods>()` that
     specializes the load step via another static interface
   - (b) Have separate byte/char parse methods that share the mask-processing code
   Option (a) is cleaner but may have JIT implications; option (b) duplicates
   thin shells but is safer for codegen quality. Start with (b) for the first
   implementation.

4. **csFastFloat for UTF-8**: csFastFloat already supports `ReadOnlySpan<byte>`
   parsing, so the fast float path works directly.

5. **Carriage return handling**: The `\r\n` lookahead logic works identically for
   bytes since CR/LF are single-byte ASCII.

6. **Multi-byte UTF-8 characters**: Sep's separator, quote, CR, LF are all
   ASCII. Multi-byte UTF-8 sequences can never produce these bytes as
   continuation bytes (they are always 0x80+). So byte-level scanning is
   **correct** for UTF-8 — no special handling needed.

7. **Byte pooling via `SepToBytes`**: The UTF-8 reader's primary pooling
   abstraction is `ReadOnlyMemory<byte>`-based, not `string`-based. This parallels
   `SepToString` for the char reader but avoids UTF-8→UTF-16 transcoding:
   ```csharp
   public delegate SepToBytes SepCreateToBytes(
       SepUtf8ReaderHeader? maybeHeader, int colCount);

   public abstract class SepToBytes : IDisposable
   {
       public abstract ReadOnlyMemory<byte> ToMemory(
           ReadOnlySpan<byte> colSpan, int colIndex);
       public virtual bool IsThreadSafe => false;

       // Factory methods parallel to SepToString:
       public static SepCreateToBytes Direct { get; }
       public static SepCreateToBytes PoolPerCol(...);
       public static SepCreateToBytes PoolPerColThreadSafe(...);
   }
   ```
   Implementation uses `SepBytesHashPool` (parallel to `SepStringHashPool`):
   hashes byte spans, stores `byte[]` in hash table, returns
   `ReadOnlyMemory<byte>` backed by cached `byte[]`. Same per-column strategy,
   collision limits, and max-length thresholds. Single-byte ASCII cache (0–255)
   replaces `_singleCharToString`. `Col.ToMemory()` calls through this.
   `Col.ToString()` is still available via `Encoding.UTF8.GetString(span)` but
   is a separate, non-pooled convenience path.

8. **Header: dual headers for UTF-8 reader**: `SepReaderHeader` (string-based)
   lives in the generic base class, unchanged. `SepUtf8ReaderHeader` (byte-based)
   is an additional header held by `SepUtf8ReaderState`. During UTF-8 reader
   initialization, both are built: strings decoded from UTF-8 for `SepReaderHeader`,
   raw bytes stored in `SepUtf8ReaderHeader`. `Row[string]` uses the base header's
   cache. `Row[ReadOnlySpan<byte>]` uses the byte header's `SequenceEqual` lookup.
   This avoids interfaces or virtual dispatch in the base class and keeps the char
   reader completely unchanged.

9. **IO abstraction: Stream over PipeReader**: `Stream` is the primary input
   abstraction because (a) Sep's pull model needs to fill its own contiguous,
   padded buffer — `Stream.Read(Span<byte>)` does this directly with zero
   intermediate copies for `FileStream`, (b) PipeReader's
   `ReadOnlySequence<byte>` can be multi-segment and doesn't provide the zeroed
   SIMD padding Sep requires, (c) there is no `IBufferReader<T>` in .NET BCL.
   For Kestrel, `HttpRequest.Body` (Stream) incurs one copy (pipe → Sep buffer),
   which is negligible vs. parsing cost. Direct PipeReader support is a future
   refinement but still requires copying to Sep's padded buffer — no zero-copy
   benefit is achievable due to the SIMD padding requirement.

10. **Writer output: Stream + IBufferWriter\<byte\>**: `Stream` is the primary
    output abstraction for universality. `IBufferWriter<byte>` is included from V1
    because it's the optimal abstraction for Kestrel's `HttpResponse.BodyWriter`
    (`PipeWriter` implements `IBufferWriter<byte>`), enabling writes directly into
    the pipe's memory without an intermediate copy.

11. **BOM handling**: For the UTF-8 byte reader, Sep must detect and skip the
    3-byte UTF-8 BOM (`0xEF 0xBB 0xBF`) itself, since there's no `StreamReader`
    to do it automatically. Check first 3 bytes after initial buffer fill; if BOM,
    advance `_bytesDataStart` past it. Run once during initialization. Controllable
    via an option (`SkipBom`, default `true`).
