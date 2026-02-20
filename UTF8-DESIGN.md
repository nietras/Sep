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

## Proposed Approach: Making Core Generic Over `char`/`byte`

### Key Insight
The SIMD parsers already pack `char` pairs into `byte` vectors for comparison.
For UTF-8 input, the data is *already* bytes — no packing step needed. The
separator, line feed, carriage return, and quote characters are all ASCII (single
byte), so the SIMD comparison logic is fundamentally the same. Only the
"load & pack" step differs.

### Strategy: Generic `TChar` with Static Abstract Interfaces

Use .NET's own `IUtf8SpanParsable<T>` but consider if custom needed etc.

```csharp
// Conceptual - actual design to be refined
interface ISepCharInfo<TChar> where TChar : unmanaged
{
    static abstract TChar LineFeed { get; }
    static abstract TChar CarriageReturn { get; }
    static abstract TChar Quote { get; }
    static abstract TChar Space { get; }
    static abstract int SizeOf { get; }
}

struct SepCharInfo : ISepCharInfo<char>  { /* char constants */ }
struct SepByteInfo : ISepCharInfo<byte>  { /* byte constants */ }
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

6. **`SepReaderHeader`** — Can stay string-based. For UTF-8 reader, header bytes
   would be decoded to strings once during initialization.

### What Must Be Separate / New

1. **Buffer type**: `char[]` vs `byte[]` (via `ArrayPool<char>` vs `ArrayPool<byte>`)

2. **Data source**:
   - `char` reader: `TextReader` (existing)
   - `byte` reader: `Stream`, `ReadOnlyMemory<byte>`, `PipeReader`, or similar

3. **Column access API**:
   - `char` reader: `ReadOnlySpan<char> Span` (existing)
   - `byte` reader: `ReadOnlySpan<byte> Span` (new), consider `.ToString()` → `string`

4. **Parsing (ISpanParsable vs IUtf8SpanParsable)**:
   - `char`: `T.Parse(ReadOnlySpan<char>, IFormatProvider?)`
   - `byte`: `T.Parse(ReadOnlySpan<byte>, IFormatProvider?)` (.NET 8+ `IUtf8SpanParsable<T>`)

5. **ToString / String pooling**: UTF-8 col → `Encoding.UTF8.GetString(span)`

6. **Writer output**:
   - `char`: `TextWriter.Write(ReadOnlySpan<char>)` (existing)
   - `byte`: `Stream.Write(ReadOnlySpan<byte>)` or `IBufferWriter<byte>`

7. **SIMD load step** (thin layer):
   - `char`: `PackUnsignedSaturate(v0, v1)` then permute
   - `byte`: Direct load (simpler, potentially faster)

### Proposed Type Hierarchy

```
// Existing (unchanged public API)
SepReader            : SepReaderState     → reads TextReader, char[] buffer
SepReader.Row        (ref struct)
SepReader.Col        (ref struct) → ReadOnlySpan<char>

// New UTF-8 types
SepUtf8Reader        : SepUtf8ReaderState → reads Stream/bytes, byte[] buffer
SepUtf8Reader.Row    (ref struct)
SepUtf8Reader.Col    (ref struct) → ReadOnlySpan<byte>

// Shared internals (generic)
SepReaderStateCore<TChar>  — generic base with buffer mgmt, col tracking
ISepParser<TChar>          — generic parser interface
SepParseMask               — already generic, extend for TChar
```

### Alternative: Single Generic `SepReader<TChar>`
A fully generic `SepReader<TChar> where TChar : unmanaged` could unify both, but
this may complicate the public API and require users to always specify the type
parameter. A separate `SepUtf8Reader` type with shared internal generic
implementation is likely cleaner.

## Implementation Todos

### Phase 1: Internal Generic Infrastructure
- [ ] `generic-state` — Create generic `SepReaderStateCore<TChar>` extracting
  type-agnostic logic from `SepReaderState` (col tracking, row info, buffer
  management parameterized on `TChar`)
- [ ] `generic-parser-interface` — Define `ISepParser<TChar>` or extend
  `ISepParser` with byte overloads; create byte-oriented load paths in SIMD parsers
- [ ] `generic-parse-mask` — Extend `SepParseMask` methods to work with both
  `ref char` and `ref byte` (the constants are ASCII-identical)

### Phase 2: UTF-8 Reader
- [ ] `utf8-reader-options` — `SepUtf8ReaderOptions` record (mirrors
  `SepReaderOptions` but source is `Stream`/`byte[]`/`ReadOnlyMemory<byte>`)
- [ ] `utf8-reader-state` — `SepUtf8ReaderState` implementing byte-buffer
  management, reading from `Stream`
- [ ] `utf8-reader` — `SepUtf8Reader` class with `Row`, `Col`, `Cols` ref structs
  exposing `ReadOnlySpan<byte>`, `IUtf8SpanParsable<T>` based `Parse<T>()`
- [ ] `utf8-reader-io` — `FromFile`, `FromBytes`, `FromStream` extension methods
- [ ] `utf8-reader-header` — Header parsing: decode UTF-8 bytes to strings for
  column name lookup

### Phase 3: UTF-8 Writer
- [ ] `utf8-writer-options` — `SepUtf8WriterOptions`
- [ ] `utf8-writer` — `SepUtf8Writer` writing `byte[]` to `Stream`/`IBufferWriter<byte>`
- [ ] `utf8-writer-col` — Col with `Set(ReadOnlySpan<byte>)`,
  `Format<T>()` using `IUtf8SpanFormattable`

### Phase 4: Testing & Benchmarks
- [ ] `utf8-tests` — Mirror existing test coverage for UTF-8 paths
- [ ] `utf8-benchmarks` — Benchmark UTF-8 vs char reader for same data

## Key Design Decisions to Consider

1. **Public API naming**: `SepUtf8Reader` vs `Sep.Utf8.Reader()` vs
   `Sep.Reader().FromUtf8File()`?

2. **Should `SepReaderState` become generic?** Making the existing base class
   `SepReaderState<TChar>` would be a breaking change for internal consumers.
   Better to extract shared logic into a new generic internal base.

3. **Parser reuse strategy**: The SIMD parsers could either:
   - (a) Have a generic `Parse<TChar, TColInfo, TColInfoMethods>()` that
     specializes the load step via another static interface
   - (b) Have separate byte/char parse methods that share the mask-processing code
   Option (a) is cleaner but may have JIT implications; option (b) duplicates
   thin shells but is safer for codegen quality.

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
