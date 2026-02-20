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
9. **`SepReaderHeader`** — stores col names as `string`, uses `Dictionary<string, int>`
10. **`SepToString`** / string pooling — converts `ReadOnlySpan<char>` to `string`

## No Breaking Changes Constraint
The existing public API **must not break**. Specifically:
- `SepReader`, `SepReader.Row`, `SepReader.Col`, `SepReader.Cols` — unchanged
- `SepWriter`, `SepWriter.Row`, `SepWriter.Col`, `SepWriter.Cols` — unchanged
- `SepReaderOptions`, `SepWriterOptions` — unchanged
- `SepReaderHeader` — extended (not changed) with UTF-8 lookup
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

### Strategy: Generic `TChar` with Static Abstract Interfaces

Use .NET's own `IUtf8SpanParsable<T>` for parsing, and define a thin internal
static abstract interface for char/byte constants and operations:

```csharp
// Internal static abstract interface for char vs byte specialization
interface ISepCharInfo<TChar> where TChar : unmanaged
{
    static abstract TChar LineFeed { get; }
    static abstract TChar CarriageReturn { get; }
    static abstract TChar Quote { get; }
    static abstract TChar Space { get; }
    static abstract int SizeOf { get; }
}

struct SepChar : ISepCharInfo<char>  { /* char constants */ }
struct SepByte : ISepCharInfo<byte>  { /* byte constants, same ASCII values */ }
```

### What Can Be Shared (Reused with generics)

1. **`SepParseMask`** — Already generic over `TColInfo`. The char-specific logic
   (`ParseAnyChar`, etc.) compares against `LineFeed`, `CarriageReturn`, `Quote`,
   `separator`. These can be parameterized:
   - For `char`: `ref char charsRef` + compare to `char` constants
   - For `byte`: `ref byte bytesRef` + compare to `byte` constants (same values!)
   - The mask-walking logic (TrailingZeroCount, mask iteration) is identical.

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
   The difference is buffer type (`char[]` vs `byte[]`) and read source.

6. **`SepReaderHeader`** — Stays string-based internally. Extended with UTF-8
   lookup support (see Header section below).

### What Must Be Separate / New

1. **Buffer type**: `char[]` vs `byte[]` (via `ArrayPool<char>` vs `ArrayPool<byte>`)

2. **Data source** (see IO Abstraction Analysis below):
   - `char` reader: `TextReader` (existing)
   - `byte` reader: `Stream` (primary), `byte[]`/`ReadOnlyMemory<byte>` (in-memory),
     `PipeReader` (future native optimization)

3. **Column access API**:
   - `char` reader: `ReadOnlySpan<char> Span` (existing)
   - `byte` reader: `ReadOnlySpan<byte> Span` (new), `.ToString()` decodes UTF-8

4. **Parsing (ISpanParsable vs IUtf8SpanParsable)**:
   - `char`: `T.Parse(ReadOnlySpan<char>, IFormatProvider?)`
   - `byte`: `T.Parse(ReadOnlySpan<byte>, IFormatProvider?)` (.NET 8+ `IUtf8SpanParsable<T>`)

5. **ToString / String pooling**: UTF-8 col → `Encoding.UTF8.GetString(span)`

6. **Writer output** (see IO Abstraction Analysis below):
   - `char`: `TextWriter.Write(ReadOnlySpan<char>)` (existing)
   - `byte`: `Stream.Write(ReadOnlySpan<byte>)` (primary) or
     `IBufferWriter<byte>` (for Kestrel/PipeWriter zero-copy output)

7. **SIMD load step** (thin layer):
   - `char`: `PackUnsignedSaturate(v0, v1)` then permute
   - `byte`: Direct load (simpler, potentially faster)

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

### Header Lookup: Both `char` and `byte` Based

`SepReaderHeader` is reused by both `SepReader` and `SepUtf8Reader`. It is
extended to support UTF-8 byte-based lookup in addition to existing `string` and
`ReadOnlySpan<char>` lookup. The header is always stored as strings internally
(column names decoded from UTF-8 once during init for the UTF-8 reader).

```csharp
public sealed class SepReaderHeader
{
    // Existing (unchanged)
    public int IndexOf(string colName) => ...;
    public bool TryIndexOf(string colName, out int colIndex) => ...;
#if NET9_0_OR_GREATER
    public int IndexOf(ReadOnlySpan<char> colName) => ...;
    public bool TryIndexOf(ReadOnlySpan<char> colName, out int colIndex) => ...;
#endif
    public IReadOnlyList<string> ColNames => ...;
    public IReadOnlyList<string> NamesStartingWith(string prefix, ...) => ...;
    public int[] IndicesOf(...) => ...;

    // New: UTF-8 byte-based lookup
    public int IndexOf(ReadOnlySpan<byte> utf8ColName) => ...;
    public bool TryIndexOf(ReadOnlySpan<byte> utf8ColName, out int colIndex) => ...;
    public int[] IndicesOf(ReadOnlySpan<ReadOnlyMemory<byte>> utf8ColNames) => ...;
}
```

**Implementation strategy for UTF-8 lookup:**
- Lazy-init a `Dictionary<string, int>` alternate lookup via
  `Encoding.UTF8.GetString()` on the input span, then look up in the existing
  `_colNameToIndex` dictionary. This reuses the existing storage.
- Alternatively, for high-performance hot-path lookups, maintain a parallel
  `byte[][]` array of UTF-8-encoded column names and do span-based comparison.
  This avoids allocating a `string` on every `IndexOf(ReadOnlySpan<byte>)` call.
- The preferred approach: store UTF-8 encoded names lazily alongside string names
  and use `SequenceEqual` for byte lookup. This is allocation-free on the lookup
  hot path:

```csharp
// Internal: lazy-initialized UTF-8 encoded column names
byte[][]? _utf8ColNames;

public int IndexOf(ReadOnlySpan<byte> utf8ColName)
{
    var utf8Names = _utf8ColNames ??= CreateUtf8ColNames();
    for (var i = 0; i < utf8Names.Length; i++)
    {
        if (utf8ColName.SequenceEqual(utf8Names[i]))
            return i;
    }
    SepThrow.KeyNotFoundException_ColNameNotFound(
        Encoding.UTF8.GetString(utf8ColName));
    return -1;
}
```

For the `SepUtf8Reader.Row` indexer, both `string` and `ReadOnlySpan<byte>` 
column access is supported:
```csharp
public readonly ref struct Row  // SepUtf8Reader.Row
{
    public Col this[string colName] => ...;      // string-based (reuses header cache)
    public Col this[ReadOnlySpan<byte> utf8ColName] => ...; // UTF-8 byte-based
    public Col this[int index] => ...;           // index-based
}
```

### Proposed Type Hierarchy

```
// Existing (unchanged public API)
SepReaderState           → base class (internal members), may now inherit from
                           SepReaderStateBase<char> but this is invisible externally
SepReader : SepReaderState → reads TextReader, char[] buffer
SepReader.Row            (ref struct)
SepReader.Col            (ref struct) → ReadOnlySpan<char>

SepWriter                → writes to TextWriter, char-based
SepWriter.Row            (ref struct)
SepWriter.Col            (ref struct)

// New UTF-8 types (parallel to existing)
SepUtf8ReaderState       → inherits from SepReaderStateBase<byte> (internal)
SepUtf8Reader : SepUtf8ReaderState → reads Stream, byte[] buffer
SepUtf8Reader.Row        (ref struct)
SepUtf8Reader.Col        (ref struct) → ReadOnlySpan<byte>

SepUtf8Writer            → writes to Stream or IBufferWriter<byte>
SepUtf8Writer.Row        (ref struct)
SepUtf8Writer.Col        (ref struct)

// Shared internals (generic, internal only)
SepReaderStateBase<TChar> where TChar : unmanaged
    — TChar[] _buffer, int[] _colEndsOrColInfos, SepRowInfo[] _parsedRows
    — MoveNextAlreadyParsed(), GetColRange(), col tracking
    — SepReaderHeader (shared, extended with UTF-8 lookup)

ISepParser               — unchanged for char parsers (existing)
ISepUtf8Parser           — new, for byte parsers (ParseColEnds/ParseColInfos
                           taking SepUtf8ReaderState)
```

### Alternative: Single Generic `SepReader<TChar>`
A fully generic `SepReader<TChar> where TChar : unmanaged` could unify both, but
this would complicate the public API, require users to always specify the type
parameter, and be a breaking change. A separate `SepUtf8Reader` type with shared
internal generic implementation is cleaner and preserves backward compatibility.

## Implementation Todos

### Phase 1: Internal Generic Infrastructure
- [ ] `generic-state` — Create internal generic `SepReaderStateBase<TChar>`
  extracting type-agnostic logic from `SepReaderState` (col tracking, row info,
  buffer management parameterized on `TChar`). Make existing `SepReaderState`
  inherit from `SepReaderStateBase<char>` preserving all existing public/internal
  members.
- [ ] `generic-parser-interface` — Define `ISepUtf8Parser` (parallel to
  `ISepParser`) for byte-based parsers. Create byte-oriented load paths in SIMD
  parsers — for byte input, skip the char-to-byte packing step and load bytes
  directly.
- [ ] `generic-parse-mask` — Extend `SepParseMask` methods to work with both
  `ref char` and `ref byte` (the constants are ASCII-identical). Add a
  `TChar`/`TSepChar` type parameter to `ParseAnyChar` and related methods.

### Phase 2: Fluent API & Options
- [ ] `utf8-reader-options` — `SepUtf8ReaderOptions` record (mirrors
  `SepReaderOptions` but without `TextReader`-specific options like
  `CreateToString`). Sources are `Stream`/`byte[]`/`ReadOnlyMemory<byte>`.
- [ ] `utf8-writer-options` — `SepUtf8WriterOptions` record (mirrors
  `SepWriterOptions`).
- [ ] `fluent-api` — Add `Sep.Utf8Reader()`, `Sep.Utf8Writer()` static methods.
  Add `Utf8Reader()`/`Utf8Writer()` extension methods on `Sep` (instance),
  `Sep?`, and `SepSpec`. This follows the exact same pattern as
  `Sep.Reader()`/`Sep.Writer()`.

### Phase 3: UTF-8 Reader
- [ ] `utf8-reader-state` — `SepUtf8ReaderState` inheriting from
  `SepReaderStateBase<byte>`, implementing byte-buffer management, reading from
  `Stream`.
- [ ] `utf8-reader` — `SepUtf8Reader` class with `Row`, `Col`, `Cols` ref
  structs. `Col.Span` returns `ReadOnlySpan<byte>`. `Col.Parse<T>()` uses
  `IUtf8SpanParsable<T>`. `Col.ToString()` decodes via
  `Encoding.UTF8.GetString()`.
- [ ] `utf8-reader-io` — `FromFile`, `FromBytes`, `From(Stream)` extension
  methods on `SepUtf8ReaderOptions`.
- [ ] `utf8-reader-header` — Extend `SepReaderHeader` with
  `IndexOf(ReadOnlySpan<byte>)`, `TryIndexOf(ReadOnlySpan<byte>, out int)`, and
  `IndicesOf` overloads for UTF-8 byte spans. Lazy-init UTF-8 encoded name cache
  for allocation-free lookup. Header is decoded from UTF-8 bytes to strings once
  during `SepUtf8Reader` initialization.

### Phase 4: UTF-8 Writer
- [ ] `utf8-writer` — `SepUtf8Writer` writing `byte[]` to
  `Stream`/`IBufferWriter<byte>`. ColImpl uses `byte[]` buffer via
  `ArrayPool<byte>`.
- [ ] `utf8-writer-col` — Col with `Set(ReadOnlySpan<byte>)`,
  `Set(ReadOnlySpan<char>)` (transcoding), `Format<T>()` using
  `IUtf8SpanFormattable`.
- [ ] `utf8-writer-io` — `ToFile`, `To(Stream)`, `To(IBufferWriter<byte>)`,
  `ToBytes()` extension methods on `SepUtf8WriterOptions`.

### Phase 5: Testing & Benchmarks
- [ ] `utf8-tests` — Mirror existing test coverage for UTF-8 paths, including
  header lookup via both `string` and `ReadOnlySpan<byte>`.
- [ ] `utf8-benchmarks` — Benchmark UTF-8 vs char reader for same data.

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

7. **String pooling for UTF-8**: `SepToString` would need a UTF-8 variant that
   decodes `ReadOnlySpan<byte>` → `string` with optional pooling.

8. **Header lookup via UTF-8 bytes**: `SepReaderHeader` is extended (not replaced)
   with `IndexOf(ReadOnlySpan<byte>)` overloads. A lazy `byte[][]` cache of
   UTF-8-encoded column names enables allocation-free `SequenceEqual` lookup on
   the hot path. This benefits both `SepUtf8Reader` (primary) and any advanced
   `SepReader` users who have column names as UTF-8 bytes.

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
