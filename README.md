# Sep - *one of* the World's Fastest .NET CSV Parser

![.NET](https://img.shields.io/badge/net8.0%20net9.0%20net10.0%20net11.0-5C2D91?logo=.NET&labelColor=gray)
![C#](https://img.shields.io/badge/C%23-15.0-239120?labelColor=gray)
[![Build Status](https://github.com/nietras/Sep/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/nietras/Sep/actions/workflows/dotnet.yml)
[![Super-Linter](https://github.com/nietras/Sep/actions/workflows/super-linter.yml/badge.svg)](https://github.com/marketplace/actions/super-linter)
[![codecov](https://codecov.io/gh/nietras/Sep/branch/main/graph/badge.svg?token=WN56CR3X0D)](https://codecov.io/gh/nietras/Sep)
[![CodeQL](https://github.com/nietras/Sep/workflows/CodeQL/badge.svg)](https://github.com/nietras/Sep/actions?query=workflow%3ACodeQL)
[![Nuget](https://img.shields.io/nuget/v/Sep?color=purple)](https://www.nuget.org/packages/Sep/)
[![Release](https://img.shields.io/github/v/release/nietras/Sep)](https://github.com/nietras/Sep/releases/)
[![downloads](https://img.shields.io/nuget/dt/Sep)](https://www.nuget.org/packages/Sep)
![Size](https://img.shields.io/github/repo-size/nietras/Sep.svg)
[![License](https://img.shields.io/github/license/nietras/Sep)](https://github.com/nietras/Sep/blob/main/LICENSE)
[![Blog](https://img.shields.io/badge/blog-nietras.com-4993DD)](https://nietras.com)
![GitHub Repo stars](https://img.shields.io/github/stars/nietras/Sep?style=flat)

<img src="https://raw.githubusercontent.com/nietras/Sep/icon-v2/Icon.png"
     alt="Icon" align="right" width="128" hspace="2" vspace="2" />

Modern, minimal, fast, zero allocation, reading and writing of separated values
(`csv`, `tsv` etc.). Cross-platform, trimmable and AOT/NativeAOT compatible.
Featuring an opinionated API design and pragmatic implementation targeted at
machine learning use cases.

⭐ Please star this project if you like it. ⭐

**🌃  Modern** - utilizes features such as
[`Span<T>`](https://learn.microsoft.com/en-us/archive/msdn-magazine/2018/january/csharp-all-about-span-exploring-a-new-net-mainstay),
[Generic Math](https://devblogs.microsoft.com/dotnet/dotnet-7-generic-math/)
([`ISpanParsable<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.ispanparsable-1)/
[`ISpanFormattable`](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable)),
[`ref struct`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/ref-struct),
[`ArrayPool<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.arraypool-1)
and similar from [.NET 7+ and C#
11+](https://nietras.com/2022/11/26/dotnet-and-csharp-versions/) for a modern
and highly efficient implementation.

**🔎 Minimal** - a succinct yet expressive API with few options and no hidden
changes to input or output. What you read/write is what you get. E.g. by default
there is no "automatic" escaping/unescaping of quotes or trimming of spaces. To
enable this see [SepReaderOptions](#sepreaderoptions) and
[Unescaping](#unescaping) and [Trimming](#trimming). See
[SepWriterOptions](#sepwriteroptions) for [Escaping](#escaping).

**🚀 Fast** - blazing fast with both architecture specific and cross-platform
SIMD vectorized parsing incl. 64/128/256/512-bit paths e.g. AVX2, AVX-512 (.NET
8.0+), NEON. Uses [csFastFloat](https://github.com/CarlVerret/csFastFloat) for
fast parsing of floating points. See [detailed
benchmarks](#comparison-benchmarks) for cross-platform results.

**🌪️ Multi-threaded** - highly efficient parallel CSV
parsing, see [ParallelEnumerate](#parallelenumerate-and-enumerate) and
[benchmarks](#comparison-benchmarks).

**🌀 Async support** - efficient `ValueTask` based `async/await` support.
Requires C# 13.0+ and for .NET 9.0+ includes `SepReader` implementing
`IAsyncEnumerable<>`. From .NET 11 and C# 15.0 built with
[runtime-async=on](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/runtime#runtime-async).
See [Async Support](#async-support) for details.

**🗑️ Zero allocation** - intelligent and efficient memory management allowing
for zero allocations after warmup incl. supporting use cases of reading or
writing arrays of values (e.g. features) easily without repeated allocations.

**✅ Thorough tests** - great code coverage and focus on edge case testing incl.
randomized [fuzz testing](https://en.wikipedia.org/wiki/Fuzzing).

**🌐 Cross-platform** - works on any platform, any architecture supported by
NET. 100% managed and written in beautiful modern C#.

**✂️ Trimmable and AOT/NativeAOT compatible** - no problematic reflection or
dynamic code generation. Hence, fully
[trimmable](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/prepare-libraries-for-trimming)
and
[Ahead-of-Time](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
compatible. With a simple console tester program executable possible in just a
few MBs. 💾

**🗣️ Opinionated and pragmatic** - conforms to the essentials of
[RFC-4180](https://www.ietf.org/rfc/rfc4180.txt), but takes an opinionated and
pragmatic approach towards this especially with regards to quoting and line
ends. See section [RFC-4180](#rfc-4180).

[Example](#example) | [Naming and Terminology](#naming-and-terminology) | [API](#application-programming-interface-api) | [Limitations and Constraints](#limitations-and-constraints) | [Comparison Benchmarks](#comparison-benchmarks) | [Example Catalogue](#example-catalogue) | [RFC-4180](#rfc-4180) | [FAQ](#frequently-asked-questions-faq)  | [Public API Reference](#public-api-reference)

## Example
```csharp
var text = """
           A;B;C;D;E;F
           Sep;🚀;1;1.2;0.1;0.5
           CSV;✅;2;2.2;0.2;1.5
           """;

using var reader = Sep.Reader().FromText(text);   // Infers separator 'Sep' (`;`) from header
using var writer = reader.Spec.Writer().ToText(); // Writer defined from reader 'Spec'
                                                  // Use .FromFile(...)/ToFile(...) for files
var idx = reader.Header.IndexOf("B");
var nms = new[] { "E", "F" };

foreach (var readRow in reader)           // Read one row at a time
{
    var a = readRow["A"].Span;            // Column as ReadOnlySpan<char>
    var b = readRow[idx].ToString();      // Column to string (might be pooled)
    var c = readRow["C"].Parse<int>();    // Parse any T : ISpanParsable<T>
    var d = readRow["D"].Parse<float>();  // Parse float/double fast via csFastFloat
    var s = readRow[nms].Parse<double>(); // Parse multiple columns as Span<T>
                                          // - Sep handles array allocation and reuse
    foreach (ref var v in s) { v *= 10; }

    using var writeRow = writer.NewRow(); // Start new row. Row written on Dispose.
    writeRow["A"].Set(a);                 // Set by ReadOnlySpan<char>
    writeRow["B"].Set(b);                 // Set by string
    writeRow["C"].Set($"{c * 2}");        // Set via InterpolatedStringHandler, no allocs
    writeRow["D"].Format(d / 2);          // Format any T : ISpanFormattable
    writeRow[nms].Format(s);              // Format multiple columns directly
    // Columns are added on first access as ordered, header written when first row written
}

var expected = """
               A;B;C;D;E;F
               Sep;🚀;2;0.6;1;5
               CSV;✅;4;1.1;2;15
               
               """;                       // Empty line at end is for line ending,
                                          // which is always written.
Assert.AreEqual(expected, writer.ToString());

// Above example code is for demonstration purposes only.
// Short names and repeated constants are only for demonstration.
```

For more examples, incl. how to write and read objects (e.g. `record`s) with
escape/unescape support, see [Example Catalogue](#example-catalogue).

## Naming and Terminology
Sep uses naming and terminology that is not based on [RFC-4180](#rfc-4180), but
is more tailored to usage in machine learning or similar. Additionally, Sep
takes a pragmatic approach towards names by using short names and abbreviations
where it makes sense and there should be no ambiguity given the context. That
is, using `Sep` for `Separator` and `Col` for `Column` to keep code succinct.

|Term | Description |
|-----|-------------|
|`Sep` | Short for separator, also called *delimiter*. E.g. comma (`,`) is the separator for the separated values in a `csv`-file. |
|`Header` | Optional first row defining names of columns. |
|`Row` | A row is a collection of col(umn)s, which may span multiple lines. Also called *record*. |
|`Col` | Short for column, also called *field*. |
|`Line` | Horizontal set of characters until a line ending; `\r\n`, `\r`, `\n`. |
|`Index` | 0-based that is `RowIndex` will be 0 for first row (or the header if present). |
|`Number` | 1-based that is `LineNumber` will be 1 for the first line (as in `notepad`). Given a row may span multiple lines a row can have a *From* line number and a *ToExcl* line number matching the C# range indexing syntax `[LineNumberFrom..LineNumberToExcl]`. |

## Application Programming Interface (API)
Besides being the succinct name of the library, `Sep` is both the main entry
point to using the library and the container for a validated separator. That is,
`Sep` is basically defined as:
```csharp
public readonly record struct Sep(char Separator);
```
The separator `char` is validated upon construction and is guaranteed to be
within a limited range and not being a `char` like `"` (quote) or similar. This
can be seen in [src/Sep/Sep.cs](src/Sep/Sep.cs). The separator is constrained
also for internal optimizations, so you cannot use any `char` as a separator.

⚠ Note that all types are within the namespace `nietras.SeparatedValues` and not
`Sep` since it is problematic to have a type and a namespace with the same name.

To get started you can use `Sep` as the static entry point to building either a
reader or writer. That is, for `SepReader`:
```csharp
using var reader = Sep.Reader().FromFile("titanic.csv");
```
where `.Reader()` is a convenience method corresponding to:
```csharp
using var reader = Sep.Auto.Reader().FromFile("titanic.csv");
```
where `Sep? Auto => null;` is a static property that returns `null` for a
nullable `Sep` to signify that the separator should be inferred from the first
row, which might be a header. If the first row does not contain any of the by
default supported separators or there are no rows, the default separator will be
used.

⚠ Note Sep uses `;` as the default separator, since this is what was used in an
internal proprietary library which Sep was built to replace. This is also to
avoid issues with comma `,` being used as a decimal separator in some locales.
Without having to resort to quoting.

If you want to specify the separator you can write:
```csharp
using var reader = Sep.New(',').Reader().FromFile("titanic.csv");
```
or
```csharp
var sep = new Sep(',');
using var reader = sep.Reader().FromFile("titanic.csv");
```
Similarly, for `SepWriter`:
```csharp
using var writer = Sep.Writer().ToFile("titanic.csv");
```
or
```csharp
using var writer = Sep.New(',').Writer().ToFile("titanic.csv");
```
where you have to specify a valid separator, since it cannot be inferred. To
facilitate easy flow of the separator and `CultureInfo` both `SepReader` and
`SepWriter` expose a `Spec` property of type [`SepSpec`](src/Sep/SepSpec.cs) that simply defines those
two. This means you can write:
```csharp
using var reader = Sep.Reader().FromFile("titanic.csv");
using var writer = reader.Spec.Writer().ToFile("titanic-survivors.csv");
```
where the `writer` then will use the separator inferred by the reader, for
example.

### API Pattern
In general, both reading and writing follow a similar pattern:
```text
Sep/Spec => SepReaderOptions => SepReader => Row => Col(s) => Span/ToString/Parse
Sep/Spec => SepWriterOptions => SepWriter => Row => Col(s) => Set/Format
```
where each continuation flows fluently from the preceding type. For example,
`Reader()` is an extension method to `Sep` or `SepSpec` that returns a
`SepReaderOptions`. Similarly, `Writer()` is an extension method to `Sep` or
`SepSpec` that returns a `SepWriterOptions`.

[`SepReaderOptions`](src/Sep/SepReaderOptions.cs)  and
[`SepWriterOptions`](src/Sep/SepWriterOptions.cs) are optionally configurable.
That and the APIs for reader and writer is covered in the following sections.

For a complete example, see the [example](#example) above or the
[ReadMeTest.cs](src/Sep.XyzTest/ReadMeTest.cs).

⚠ Note that it is important to understand that Sep `Row`/`Col`/`Cols` are [`ref
struct`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/ref-struct)s
(please follow the `ref struct` link and understand how this limits the usage of
those). This is due to these types being simple *facades* or indirections to the
underlying reader or writer. That means you cannot use LINQ or create an array
of all rows like `reader.ToArray()`. While for .NET9+ the reader is now
`IEnumerable<>` since `ref struct`s can now be used in interfaces that have
[`where T: allows ref
struct`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-13.0/ref-struct-interfaces)
this still does not mean it is LINQ compatible. Hence, if you need store per row
state or similar you need to parse or copy to different types instead. The same
applies to `Col`/`Cols` which point to internal state that is also reused. This
is to avoid repeated allocations for each row and get the best possible
performance, while still defining a well structured and straightforward API that
guides users to relevant functionality. See [Why SepReader Was Not IEnumerable
Until .NET 9 and Is Not LINQ
Compatible](#why-sepreader-was-not-ienumerable-until-net-9-and-is-not-linq-compatible)
for more.

⚠ For a full overview of public types and methods see [Public API
Reference](#public-api-reference).

### SepReader API
`SepReader` API has the following structure (in pseudo-C# code):
```csharp
using var reader = Sep.Reader(o => o).FromFile/FromText/From...;
var header = reader.Header;
var _ = header.IndexOf/IndicesOf/NamesStartingWith...;
foreach (var row in reader)
{
    var _ = row[colName/colNames].Span/ToString/Parse<T>...;
    var _ = row[colIndex/colIndices].Span/ToString/Parse<T>...;
}
```
That is, to use `SepReader` follow the points below:

 1. Optionally define `Sep` or use default automatically inferred separator.
 1. Specify reader with optional configuration of `SepReaderOptions`. For
    example, if a csv-file does not have a header this can be configured via:
    ```csharp
    Sep.Reader(o => o with { HasHeader = false })
    ```
    For all options see [SepReaderOptions](#sepreaderoptions).
 1. Specify source e.g. file, text (`string`), `TextWriter`, etc. via `From`
    extension methods.
 1. Optionally access the header. For example, to get all columns starting with
    `GT_` use:
    ```csharp
    var colNames = header.NamesStartingWith("GT_");
    var colIndices = header.IndicesOf(colNames);
    ```
 1. Enumerate rows. One row at a time.
 1. Access a column by name or index. Or access multiple columns with names and
    indices. `Sep` internally handles pooled allocation and reuse of arrays for
    multiple columns.
 1. Use `Span` to access the column directly as a `ReadOnlySpan<char>`. Or use
    `ToString` to convert to a `string`. Or use `Parse<T>` where `T :
    ISpanParsable<T>` to parse the column `char`s to a specific type.

#### SepReaderOptions
The following options are available:
```csharp
/// <summary>
/// Specifies the separator used, if `null` then automatic detection 
/// is used based on first row in source.
/// </summary>
public Sep? Sep { get; init; } = null;
/// <summary>
/// Specifies initial internal `char` buffer length.
/// </summary>
/// <remarks>
/// The length will likely be rounded up to the nearest power of 2. A
/// smaller buffer may end up being used if the underlying source for <see
/// cref="System.IO.TextReader"/> is known to be smaller. Prefer to keep the
/// default length as that has been tuned for performance and cache sizes.
/// Avoid making this unnecessarily large as that will likely not improve
/// performance and may waste memory.
/// </remarks>
public int InitialBufferLength { get; init; } = SepDefaults.InitialBufferLength;
/// <summary>
/// Specifies the culture used for parsing. 
/// May be `null` for default culture.
/// </summary>
public CultureInfo? CultureInfo { get; init; } = SepDefaults.CultureInfo;
/// <summary>
/// Indicates whether the first row is a header row.
/// </summary>
public bool HasHeader { get; init; } = true;
/// <summary>
/// Specifies <see cref="IEqualityComparer{T}" /> to use 
/// for comparing header column names and looking up index.
/// </summary>
public IEqualityComparer<string> ColNameComparer { get; init; } = SepDefaults.ColNameComparer;
/// <summary>
/// Specifies the method factory used to convert a column span 
/// of `char`s to a `string`.
/// </summary>
public SepCreateToString CreateToString { get; init; } = SepToString.Direct;
/// <summary>
/// Disables using [csFastFloat](https://github.com/CarlVerret/csFastFloat)
/// for parsing `float` and `double`.
/// </summary>
public bool DisableFastFloat { get; init; } = false;
/// <summary>
/// Disables checking if column count is the same for all rows.
/// </summary>
public bool DisableColCountCheck { get; init; } = false;
/// <summary>
/// Disables detecting and parsing quotes.
/// </summary>
public bool DisableQuotesParsing { get; init; } = false;
/// <summary>
/// Unescape quotes on column access.
/// </summary>
/// <remarks>
/// When true, if a column starts with a quote then the two outermost quotes
/// are removed and every second inner quote is removed. Note that
/// unquote/unescape happens in-place, which means the <see
/// cref="SepReader.Row.Span" /> (<b>row</b> span) will be modified and
/// contain "garbage" state after unescaped cols before next col. This is
/// for efficiency to avoid allocating secondary memory for unescaped
/// columns. That is, the column span <see cref="SepReader.Col.Span" /> will
/// be correct with unescaped text, but the row span <see
/// cref="SepReader.Row.Span" /> may have "garbage" state between columns if
/// any column had quotes removed. Header columns/names will also be
/// unescaped. Requires <see cref="DisableQuotesParsing"/> to be false.
/// </remarks>
public bool Unescape { get; init; } = false;
/// <summary>
/// Option for trimming spaces (` ` - ASCII 32) on column access.
/// </summary>
/// <remarks>
/// By default no trimming is done. See <see cref="SepTrim"/> for options.
/// Note that trimming may happen in-place e.g. if also unescaping, which
/// means the <see cref="SepReader.Row.Span" /> will be modified and contain
/// "garbage" state for trimmed/unescaped cols. This is for efficiency to
/// avoid allocating secondary memory for trimmed/unescaped columns. That
/// is, the column span <see cref="SepReader.Col.Span" /> will be correct
/// with trimmed text, but the row span <see cref="SepReader.Row.Span" />
/// may have "garbage" state between columns if any column had spaces
/// removed. Header columns/names will also be trimmed. Note that only the
/// space ` ` (ASCII 32) character is trimmed, not any whitespace character.
/// </remarks>
public SepTrim Trim { get; init; } = SepTrim.None;
/// <summary>
/// Forwarded to <see
/// cref="System.Threading.Tasks.ValueTask.ConfigureAwait(bool)"/> or
/// similar when async methods are called.
/// </summary>
public bool AsyncContinueOnCapturedContext { get; init; } = false;
```

#### Unescaping
While great care has been taken to ensure Sep unescaping of quotes is both
correct and fast, there is always the question of how does one respond to
invalid input.

The below table tries to summarize the behavior of Sep vs CsvHelper and Sylvan.
Note that all do the same for valid input. There are differences for how invalid
input is handled. For Sep the design choice has been based on not wanting to
throw exceptions and to use a principle that is both reasonably fast and simple.

| Input | Valid | CsvHelper | CsvHelper¹ | Sylvan | Sep² |
|-|-|-|-|-|-|
| `a` | True | `a` | `a` | `a` | `a` |
| `""` | True | | | | |
| `""""` | True | `"` | `"` | `"` | `"` |
| `""""""` | True | `""` | `""` | `""` | `""` |
| `"a"` | True | `a` | `a` | `a` | `a` |
| `"a""a"` | True | `a"a` | `a"a` | `a"a` | `a"a` |
| `"a""a""a"` | True | `a"a"a` | `a"a"a` | `a"a"a` | `a"a"a` |
| `a""a` | False | EXCEPTION | `a""a` | `a""a` | `a""a` |
| `a"a"a` | False | EXCEPTION | `a"a"a` | `a"a"a` | `a"a"a` |
| `·""·` | False | EXCEPTION | `·""·` | `·""·` | `·""·` |
| `·"a"·` | False | EXCEPTION | `·"a"·` | `·"a"·` | `·"a"·` |
| `·""` | False | EXCEPTION | `·""` | `·""` | `·""` |
| `·"a"` | False | EXCEPTION | `·"a"` | `·"a"` | `·"a"` |
| `a"""a` | False | EXCEPTION | `a"""a` | `a"""a` | `a"""a` |
| `"a"a"a"` | False | EXCEPTION | `aa"a"` | EXCEPTION | `aa"a` |
| `""·` | False | EXCEPTION | `·` | EXCEPTION | `·` |
| `"a"·` | False | EXCEPTION | `a·` | EXCEPTION | `a·` |
| `"a"""a` | False | EXCEPTION | `a"a` | EXCEPTION | `a"a` |
| `"a"""a"` | False | EXCEPTION | `a"a"` | EXCEPTION | `a"a"` |
| `""a"` | False | EXCEPTION | `a"` | EXCEPTION | `a"` |
| `"a"a"` | False | EXCEPTION | `aa"` | EXCEPTION | `aa"` |
| `""a"a""` | False | EXCEPTION | `a"a""` | EXCEPTION | `a"a"` |
| `"""` | False | EXCEPTION | `"` | EXCEPTION | `"` |
| `"""""` | False | EXCEPTION | `""` | EXCEPTION | `""` |

`·` (middle dot) is whitespace to make this visible

¹ CsvHelper with `MalformedData = CsvMalformedDataHandling.Recover`

² Sep with `Unescape = true` in `SepReaderOptions`

#### Trimming
Sep supports trimming by the [`SepTrim`](src/Sep/SepTrim.cs) flags enum, which
has two options as documented there. Below the result of both trimming and
unescaping is shown in comparison to CsvHelper. Note unescaping is enabled for
all results shown. It is possible to trim without unescaping too, of course.

As can be seen Sep supports a simple principle of trimming *before* and *after*
unescaping with trimming before unescaping being important for unescaping if
there is a starting quote after spaces.

| Input | CsvHelper Trim | CsvHelper InsideQuotes | CsvHelper All¹ | Sep Outer | Sep AfterUnescape | Sep All² |
|-|-|-|-|-|-|-|
| `a` | `a` | `a` | `a` | `a` | `a` | `a` |
| `·a` | `a` | `·a` | `a` | `a` | `a` | `a` |
| `a·` | `a` | `a·` | `a` | `a` | `a` | `a` |
| `·a·` | `a` | `·a·` | `a` | `a` | `a` | `a` |
| `·a·a·` | `a·a` | `·a·a·` | `a·a` | `a·a` | `a·a` | `a·a` |
| `"a"` | `a` | `a` | `a` | `a` | `a` | `a` |
| `"·a"` | `·a` | `a` | `a` | `·a` | `a` | `a` |
| `"a·"` | `a·` | `a` | `a` | `a·` | `a` | `a` |
| `"·a·"` | `·a·` | `a` | `a` | `·a·` | `a` | `a` |
| `"·a·a·"` | `·a·a·` | `a·a` | `a·a` | `·a·a·` | `a·a` | `a·a` |
| `·"a"·` | `a` | `·"a"·` | `a` | `a` | `"a"` | `a` |
| `·"·a"·` | `·a` | `·"·a"·` | `a` | `·a` | `"·a"` | `a` |
| `·"a·"·` | `a·` | `·"a·"·` | `a` | `a·` | `"a·"` | `a` |
| `·"·a·"·` | `·a·` | `·"·a·"·` | `a` | `·a·` | `"·a·"` | `a` |
| `·"·a·a·"·` | `·a·a·` | `·"·a·a·"·` | `a·a` | `·a·a·` | `"·a·a·"` | `a·a` |

`·` (middle dot) is whitespace to make this visible

¹ CsvHelper with `TrimWhitespace = true, TrimInsideQuotes = true`

² Sep with `SepTrim.All = SepTrim.Outer | SepTrim.AfterUnescape` in
`SepReaderOptions`

#### SepReader Debuggability
Debuggability is an important part of any library and in Sep. `SepReader` has a
unique feature when looking at it and its rows or cols in a debug context. Given
the following example code:
```csharp
var text = """
           Key;Value
           A;"1
           2
           3"
           B;"Apple
           Banana
           Orange
           Pear"
           """;
using var reader = Sep.Reader().FromText(text);
foreach (var row in reader)
{
    // Hover over reader, row or col when breaking here
    var col = row[1];
    if (Debugger.IsAttached && row.RowIndex == 2) { Debugger.Break(); }
    Debug.WriteLine(col.ToString());
}
```
and you are hovering over `reader` when the break is triggered then this will
show something like:
```text
String Length=55
```
That is, it will show information of the source for the reader, in this case a
string of length 55.

##### SepReader.Row Debuggability
If you are hovering over `row` then this will show something like:
```text
  2:[5..9] = "B;\"Apple\r\nBanana\r\nOrange\r\nPear\""
```
This has the format shown below.
```text
<ROWINDEX>:[<LINENUMBERRANGE>] = "<ROW>"
```
Note how this shows line number range `[FromIncl..ToExcl]`, as in C# [range
expression](https://learn.microsoft.com/en-us/dotnet/csharp/tutorials/ranges-indexes),
so that one can easily find the row in question in `notepad` or similar. This
means Sep has to track line endings inside quotes and is an example of a feature
that makes Sep a bit slower but which is a price considered worth paying.

> GitHub doesn't show line numbers in code blocks so consider copying the
> example text to notepad or similar to see the effect.

Additionally, if you expand the `row` in the debugger (e.g. via the small
triangle) you will see each column of the row similar to below.
```text
00:'Key'   = "B"
01:'Value' = "\"Apple\r\nBanana\r\nOrange\r\nPear\""
```

##### SepReader.Col Debuggability
If you hover over `col` you should see:
```text
"\"Apple\r\nBanana\r\nOrange\r\nPear\""
```

#### Why SepReader Was Not IEnumerable Until .NET 9 and Is Not LINQ Compatible
As mentioned earlier Sep only allows enumeration and access to one row at a time
and `SepReader.Row` is just a simple *facade* or indirection to the underlying
reader. This is why it is defined as a `ref struct`. In fact, the following
code:
```csharp
using var reader = Sep.Reader().FromText(text);
foreach (var row in reader)
{ }
```
can also be rewritten as:
```csharp
using var reader = Sep.Reader().FromText(text);
while (reader.MoveNext())
{
    var row = reader.Current;
}
```
where `row` is just a *facade* for exposing row specific functionality. That is,
`row` is still basically the `reader` underneath. Hence, let's look at using
LINQ with `SepReader` implementing `IEnumerable<SepReader.Row>` and the `Row`
*not* being a `ref struct`. Then, you would be able to write something like below:
```csharp
using var reader = Sep.Reader().FromText(text);
SepReader.Row[] rows = reader.ToArray();
```
Given `Row` is just a facade for the reader, this would be equivalent to
writing:
```csharp
using var reader = Sep.Reader().FromText(text);
SepReader[] rows = reader.ToArray();
```
which hopefully makes it clear why this is not a good thing. The array would
effectively be the reader repeated several times. If this would have to be
supported one would have to allocate memory for each row always, which would
basically be no different than a `ReadLine` approach as benchmarked in
[Comparison Benchmarks](#comparison-benchmarks).

This is perhaps also the reason why no other efficient .NET CSV parser (known to
author) implements an API pattern like Sep, but instead let the reader define
all functionality directly and hence only lets you access the current row and
cols on that. This API, however, is in this author's opinion not ideal and can be
a bit confusing, which is why Sep is designed like it is. The downside is the
above caveat.

The main culprit above is that for example `ToArray()` would store a `ref
struct` in a heap allocated array, the actual enumeration is not a problem and
hence implementing `IEnumerable<SepReader.Row>` is not the problem as such. The
problem was that prior to .NET 9 it was not possible to implement this interface
with `T` being a `ref struct`, but with C# 13 `allows ref struct` and .NET 9
having annotated such interfaces it is now possible and you can assign
`SepReader` to `IEnumerable`, but most if not all of LINQ will still not work as
shown below.
```csharp
var text = """
           Key;Value
           A;1.1
           B;2.2
           """;
using var reader = Sep.Reader().FromText(text);
IEnumerable<SepReader.Row> enumerable = reader;
// Currently, most LINQ methods do not work for ref types. See below.
//
// The type 'SepReader.Row' may not be a ref struct or a type parameter
// allowing ref structs in order to use it as parameter 'TSource' in the
// generic type or method 'Enumerable.Select<TSource,
// TResult>(IEnumerable<TSource>, Func<TSource, TResult>)'
//
// enumerable.Select(row => row["Key"].ToString()).ToArray();
```
Calling `Select` should in principle be possible if this was annotated with `allows ref struct`, but it isn't currently.

If you want to use LINQ or similar you have to first parse or transform the rows
into some other type and enumerate it. This is easy to do and instead of
counting lines you should focus on how such enumeration can be easily expressed
using C# iterators (aka `yield return`). With local functions this can be done
inside a method like:
```csharp
var text = """
           Key;Value
           A;1.1
           B;2.2
           """;
var expected = new (string Key, double Value)[] {
    ("A", 1.1),
    ("B", 2.2),
};

using var reader = Sep.Reader().FromText(text);
var actual = Enumerate(reader).ToArray();

CollectionAssert.AreEqual(expected, actual);

static IEnumerable<(string Key, double Value)> Enumerate(SepReader reader)
{
    foreach (var row in reader)
    {
        yield return (row["Key"].ToString(), row["Value"].Parse<double>());
    }
}
```
Now if instead refactoring this to something LINQ-compatible by defining a
common `Enumerate` or similar method it could be:
```csharp
var text = """
           Key;Value
           A;1.1
           B;2.2
           """;
var expected = new (string Key, double Value)[] {
    ("A", 1.1),
    ("B", 2.2),
};

using var reader = Sep.Reader().FromText(text);
var actual = Enumerate(reader,
    row => (row["Key"].ToString(), row["Value"].Parse<double>()))
    .ToArray();

CollectionAssert.AreEqual(expected, actual);

static IEnumerable<T> Enumerate<T>(SepReader reader, SepReader.RowFunc<T> select)
{
    foreach (var row in reader)
    {
        yield return select(row);
    }
}
```

In fact, Sep provides such a convenience extension method. And, discounting the
`Enumerate` method, this does have less boilerplate, but not really more
effective lines of code. The issue here is that this tends to favor factoring
code in a way that can become very inefficient quickly. Consider if one wanted
to only enumerate rows matching a predicate on `Key` which meant only 1% of rows
were to be enumerated e.g.:
```csharp
var text = """
           Key;Value
           A;1.1
           B;2.2
           """;
var expected = new (string Key, double Value)[] {
    ("B", 2.2),
};

using var reader = Sep.Reader().FromText(text);
var actual = reader.Enumerate(
    row => (row["Key"].ToString(), row["Value"].Parse<double>()))
    .Where(kv => kv.Item1.StartsWith('B'))
    .ToArray();

CollectionAssert.AreEqual(expected, actual);
```
This means you are still parsing the double (which is magnitudes slower than
getting just the key) for all rows. Imagine if this was an array of floating
points or similar. Not only would you then be parsing a lot of values you would
also be allocated 99x arrays that aren't used after filtering with `Where`.

Instead, you should focus on how to express the enumeration in a way that is
both efficient and easy to read. For example, the above could be rewritten as:
```csharp
var text = """
           Key;Value
           A;1.1
           B;2.2
           """;
var expected = new (string Key, double Value)[] {
    ("B", 2.2),
};

using var reader = Sep.Reader().FromText(text);
var actual = Enumerate(reader).ToArray();

CollectionAssert.AreEqual(expected, actual);

static IEnumerable<(string Key, double Value)> Enumerate(SepReader reader)
{
    foreach (var row in reader)
    {
        var keyCol = row["Key"];
        if (keyCol.Span.StartsWith("B"))
        {
            yield return (keyCol.ToString(), row["Value"].Parse<double>());
        }
    }
}
```

To accommodate this Sep provides an overload for `Enumerate` that is similar to:
```csharp
static IEnumerable<T> Enumerate<T>(this SepReader reader, SepReader.RowTryFunc<T> trySelect)
{
    foreach (var row in reader)
    {
        if (trySelect(row, out var value))
        {
            yield return value;
        }
    }
}
```
With this the above custom `Enumerate` can be replaced with:
```csharp
var text = """
           Key;Value
           A;1.1
           B;2.2
           """;
var expected = new (string Key, double Value)[] {
    ("B", 2.2),
};

using var reader = Sep.Reader().FromText(text);
var actual = reader.Enumerate((SepReader.Row row, out (string Key, double Value) kv) =>
{
    var keyCol = row["Key"];
    if (keyCol.Span.StartsWith("B"))
    {
        kv = (keyCol.ToString(), row["Value"].Parse<double>());
        return true;
    }
    kv = default;
    return false;
}).ToArray();

CollectionAssert.AreEqual(expected, actual);
```

Note how this is pretty much the same length as the previous custom `Enumerate`.
Also worse due to how C# requires specifying types for `out` parameters which
then requires all parameter types for the lambda to be specified. Hence, in this
case the custom `Enumerate` does not take significantly longer to write and is a
lot more efficient than using LINQ `.Where` (also avoids allocating a string for
key for each row) and is easier to debug and perhaps even read. All examples
above can be seen in [ReadMeTest.cs](src/Sep.XyzTest/ReadMeTest.cs).

There is a strong case for having an enumerate API though and that is for
parallelized enumeration, which will be discussed next.

#### ParallelEnumerate and Enumerate
As discussed in the previous section Sep provides `Enumerate` convenience
extension methods, that should be used carefully. Alongside these there are
`ParallelEnumerate` extension methods that provide very efficient multi-threaded
enumeration. See [benchmarks](#comparison-benchmarks) for numbers and [Public
API Reference](#public-api-reference).

`ParallelEnumerate` is built on top of LINQ `AsParallel().AsOrdered()` and will
return exactly the same as `Enumerate` but with enumeration parallelized. This
will use more memory during execution and as many threads as possible via the
.NET thread pool. When using `ParallelEnumerate` one should, therefore (as
always), be certain the provided delegate does not refer to or change any
mutable state.

`ParallelEnumerate` comes with a lot of overhead compared to single-threaded
`foreach` or `Enumerate` and should be used carefully based on measuring any
potential benefit. Sep goes a long way to make this very efficient by using
pooled arrays and parsing multiple rows in batches, but if the source only has a
few rows then any benefit is unlikely.

Due to `ParallelEnumerate` being based on batches of rows it is also important
not to "abuse" it in-place of LINQ `AsParallel`. The idea is to use it for
*parsing* rows, not for doing expensive per row operations like loading an image
or similar. In that case, you are better off using `AsParallel()` after
`ParallelEnumerate` or `Enumerate` similarly to:

```csharp
using var reader = Sep.Reader().FromFile("very-long.csv");
var results = reader.ParallelEnumerate(ParseRow)
                    .AsParallel().AsOrdered()
                    .Select(LoadData) // Expensive load
                    .ToList();
```

As a rule of thumb if the time per row exceeds 1 millisecond consider moving the
expensive work to after `ParallelEnumerate`/`Enumerate`,

### SepWriter API
`SepWriter` API has the following structure (in pseudo-C# code):
```csharp
using var writer = Sep.Writer(o => o).ToFile/ToText/To...;
foreach (var data in EnumerateData())
{
    using var row = writer.NewRow();
    var _ = row[colName/colNames].Set/Format<T>...;
    var _ = row[colIndex/colIndices].Set/Format<T>...;
}
```
That is, to use `SepWriter` follow the points below:

 1. Optionally define `Sep` or use default automatically inferred separator.
 1. Specify writer with optional configuration of `SepWriterOptions`.
    For all options see [SepWriterOptions](#sepwriteroptions).
 1. Specify destination e.g. file, text (`string` via `StringWriter`),
    `TextWriter`, etc. via `To` extension methods.
 1. `SepWriter` does allow you to define the header up front via
    `SepWriter.Header.Add(...)`. However, header is usually defined by the order
    in which column names are accessed/created when defining the row.
 1. Define new rows with `NewRow`. ⚠ Be sure to dispose any new rows before
    starting the next! For convenience Sep provides an overload for `NewRow` that
    takes a `SepReader.Row` and copies the columns from that row to the new row:
    ```csharp
    using var reader = Sep.Reader().FromText(text);
    using var writer = reader.Spec.Writer().ToText();
    foreach (var readRow in reader)
    {   using var writeRow = writer.NewRow(readRow); }
    ```
 1. Create a column by selecting by name or index. Or multiple columns via
    indices and names. `Sep` internally handles pooled allocation and reuse of
    arrays for multiple columns.
 1. Use `Set` to set the column value either as a `ReadOnlySpan<char>`, `string`
    or via an interpolated string. Or use `Format<T>` where `T : IFormattable`
    to format `T` to the column value.
 1. Row is written when `Dispose` is called on the row.
    > Note this is to allow a row to be defined flexibly with both column
    > removal, moves and renames in the future. This is not yet supported.

#### SepWriterOptions
The following options are available:
```csharp
/// <summary>
/// Specifies the separator used.
/// </summary>
public Sep Sep { get; init; }
/// <summary>
/// Specifies the culture used for formatting. 
/// May be `null` for default culture.
/// </summary>
public CultureInfo? CultureInfo { get; init; }
/// <summary>
/// Specifies whether to write a header row 
/// before data rows. Requires all columns 
/// to have a name. Otherwise, columns can be
/// added by indexing alone.
/// </summary>
public bool WriteHeader { get; init; } = true;
/// <summary>
/// Disables checking if column count is the 
/// same for all rows.
/// </summary>
/// <remarks>
/// When true, the <see cref="ColNotSetOption"/>
/// will define how columns that are not set
/// are handled. For example, whether to skip
/// or write an empty column if a column has
/// not been set for a given row.
/// <para>
/// If any columns are skipped, then columns of
/// a row may, therefore, be out of sync with
/// column names if <see cref="WriteHeader"/>
/// is true.
/// </para>
/// As such, any number of columns can be
/// written as long as done sequentially.
/// </remarks>
public bool DisableColCountCheck { get; init; } = false;
/// <summary>
/// Specifies how to handle columns that are 
/// not set.
/// </summary>
public SepColNotSetOption ColNotSetOption { get; init; } = SepColNotSetOption.Throw;
/// <summary>
/// Specifies whether to escape column names 
/// and values when writing.
/// </summary>
/// <remarks>
/// When true, if a column contains a separator 
/// (e.g. `;`), carriage return (`\r`), line 
/// feed (`\n`) or quote (`"`) then the column 
/// is prefixed and suffixed with quotes `"` 
/// and any quote in the column is escaped by
/// adding an extra quote so it becomes `""`.
/// Note that escape applies to column names 
/// too, but only the written name.
/// </remarks>
public bool Escape { get; init; } = false;
/// <summary>
/// Forwarded to <see
/// cref="System.Threading.Tasks.ValueTask.ConfigureAwait(bool)"/> or
/// similar when async methods are called.
/// </summary>
public bool AsyncContinueOnCapturedContext { get; init; } = false;
```

#### Escaping
Escaping is not enabled by default in Sep, but when it is it gives the same
results as other popular CSV libraries as shown below. CsvHelper 34 quotes empty
fields but does not quote spaces by default.

| Input | CsvHelper | Sylvan | Sep¹ |
|-|-|-|-|
| `` | `""` | | |
| `·` | `·` | `·` | `·` |
| `a` | `a` | `a` | `a` |
| `;` | `";"` | `";"` | `";"` |
| `,` | `,` | `,` | `,` |
| `"` | `""""` | `""""` | `""""` |
| `\r` | `"\r"` | `"\r"` | `"\r"` |
| `\n` | `"\n"` | `"\n"` | `"\n"` |
| `a"aa"aaa` | `"a""aa""aaa"` | `"a""aa""aaa"` | `"a""aa""aaa"` |
| `a;aa;aaa` | `"a;aa;aaa"` | `"a;aa;aaa"` | `"a;aa;aaa"` |

Separator/delimiter is set to semi-colon `;` (default for Sep)

`·` (middle dot) is whitespace to make this visible

`\r`, `\n` are carriage return and line feed special characters to make these visible

¹ Sep with `Escape = true` in `SepWriterOptions`

## Async Support
Sep supports efficient `ValueTask` based asynchronous reading and writing.

However, given both `SepReader.Row` and `SepWriter.Row` are `ref struct`s, as
they point to internal state and should only be used one at a time,
`async/await` usage is only supported on C# 13.0+ as this has support for **"ref
and unsafe in iterators and async methods"** as covered in [What's new in C#
13](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13). Please
consult details in that for limitations and constraints due to this.

Similarly, `SepReader` only implements `IAsyncEnumerable<SepReader.Row>` (and
`IEnumerable<SepReader.Row>`) for .NET 9.0+/C# 13.0+ since then the interfaces
have been annotated with `allows ref struct` for `T`.

Async support is provided on the existing `SepReader` and `SepWriter` types
similar to how `TextReader` and `TextWriter` support both sync and async usage.
This means you as a developer are responsible for calling async methods and
using `await` when necessary. See below for a simple example and consult tests
on GitHub for more examples.

```csharp
var text = """
           A;B;C;D;E;F
           Sep;🚀;1;1.2;0.1;0.5
           CSV;✅;2;2.2;0.2;1.5
           
           """; // Empty line at end is for line ending

using var reader = await Sep.Reader().FromTextAsync(text);
await using var writer = reader.Spec.Writer().ToText();
await foreach (var readRow in reader)
{
    await using var writeRow = writer.NewRow(readRow);
}
Assert.AreEqual(text, writer.ToString());
```

Note how for `SepReader` the `FromTextAsync` is suffixed with `Async` to
indicate async creation, this is due to the reader having to read the first row
of the source at creation to determine both separator and, if file has a header,
column names of the header. The `From*Async` call then has to be `await`ed.
After that rows can be enumerated asynchronously simply by putting `await`
before `foreach`. If one forgets to do that the rows will be enumerated
synchronously.

For `SepWriter` the usage is kind of reversed. `To*` methods have no `Async`
variants, since creation is synchronous. That is, `StreamWriter` is created by a
simple constructor call. Nothing is written until a header or row is defined and
`Dispose`/`DisposeAsync` is called on the row.

For reader nothing needs to be asynchronously disposed, so `using` does not
require `await`. However, for `SepWriter` dispose may have to write/flush data
to underlying `TextWriter` and hence it should be using `DisposeAsync`, so you
must use `await using`.

To support cancellation many methods have overloads that accept a
`CancellationToken` like the `From*Async` methods for creating a `SepReader` or
for example `NewRow` for `SepWriter`. Consult [Public API
Reference](#public-api-reference) for full set of available methods.

Additionally, both [SepReaderOptions](#sepreaderoptions) and
[SepWriterOptions](#sepwriteroptions) feature the `bool
AsyncContinueOnCapturedContext` option that is forwarded to internal
`ConfigureAwait` calls, see the [ConfigureAwait
FAQ](https://devblogs.microsoft.com/dotnet/configureawait-faq/) for details on
that.

## Limitations and Constraints
Sep is designed to be minimal and fast. As such, it has some limitations and
constraints:

* Comments `#` are not directly supported. You can skip a row by:
   ```csharp
   foreach (var row in reader)
   {
        // Skip row if starts with #
        if (!row.Span.StartsWith("#"))
        {
             // ...
        }
   }
   ```
   This does not allow skipping lines before a header row starting with `#`
   though. In [Example Catalogue](#example-catalogue) a full example is given
   detailing how to skip lines before header.

## Comparison Benchmarks
To investigate the performance of Sep it is compared to:

* [CsvHelper](https://github.com/JoshClose/csvhelper) - *the* most commonly used
   CSV library with a staggering
   ![downloads](https://img.shields.io/nuget/dt/csvhelper) downloads on NuGet.
   Fully featured, battle tested and since v34 very fast.
* [Sylvan](https://github.com/MarkPflug/Sylvan) - is well-known and has
   previously been shown to be [the fastest CSV libraries for
   parsing](https://www.joelverhagen.com/blog/2020/12/fastest-net-csv-parsers).
* `ReadLine`/`WriteLine` - basic naive implementations that read line by line
   and split on separator. While writing columns, separators and line endings
   directly. Does not handle quotes or similar correctly. Going forward not
   always included in benchmarks as it is slow to run.

All benchmarks are run from/to memory either with:

* `StringReader` or `StreamReader + MemoryStream`
* `StringWriter` or `StreamWriter + MemoryStream`

This to avoid confounding factors from reading from or writing to disk.

When using `StringReader`/`StringWriter` each `char` counts as 2 bytes, when
measuring throughput e.g. `MB/s`. When using `StreamReader`/`StreamWriter`
content is UTF-8 encoded and each `char` typically counts as 1 byte, as content
usually limited to 1 byte per char in UTF-8.  Note that in .NET for `TextReader`
and `TextWriter` data is converted to/from `char`, but for reading such
conversion can often be just as fast as `Memmove`.

By default only `StringReader`/`StringWriter` results are shown, if a result is
based on `StreamReader`/`StreamWriter` it will be called out. Usually, results
for `StreamReader`/`StreamWriter` are in line with `StringReader`/`StringWriter`
but with half the throughput due to 1 byte vs 2 bytes. For brevity they are not
shown here.

For all benchmark results, Sep has been defined as the `Baseline` in
[BenchmarkDotNet](https://benchmarkdotnet.org/). This means `Ratio` will be 1.00
for Sep. For the others `Ratio` will then show how many *times* faster Sep is
than that. Or how many *times* more bytes are allocated in `Alloc Ratio`.

> Disclaimer: Any comparison made is based on a number of preconditions and
> assumptions. Sep is a new library written from the ground up to use the latest
> and greatest features in .NET. CsvHelper has a long history and has to take
> into account backwards compatibility and still supporting older runtimes. Same
> goes for Sylvan. Additionally, Sep has a different feature set compared to the
> two. Performance is a feature, but not the only feature. Keep that in mind
> when evaluating results.

### Runtime and Platforms

NOTE: [Garbage Collection
DATAS](https://maoni0.medium.com/dynamically-adapting-to-application-sizes-2d72fcb6f1ea)
mode is disabled since this severely impacts (e.g. [1.7x
slower](https://github.com/dotnet/runtime/issues/109047)) performance for some
benchmarks due to the bursty accumulated allocations. That is,
`GarbageCollectionAdaptationMode` is set to `0`.

See [./benchmarks/](./benchmarks/) directory for detail markdown tables with all
benchmark results including platform details like runtime, OS, CPU, etc.

### Reader Comparison Benchmarks
The following reader scenarios are benchmarked:

* [NCsvPerf](https://github.com/joelverhagen/NCsvPerf) from [The fastest CSV
   parser in
   .NET](https://www.joelverhagen.com/blog/2020/12/fastest-net-csv-parsers)
* [**Floats**](#floats-reader-comparison-benchmarks) as for example in machine learning.

Details for each can be found in the following. However, for each of these 3
different scopes are benchmarked to better ascertain the low-level performance
of each library and approach and what parts of the parsing consume the most
time:

* **Row** - for this scope only the row is enumerated. That is, for Sep all
   that is done is:
   ```csharp
   foreach (var row in reader) { }
   ```
   this should capture parsing both row and columns but without accessing these.
   Note that some libraries (like Sylvan) will defer work for columns to when
   these are accessed.
* **Cols** - for this scope all rows and all columns are enumerated. If
   possible columns are accessed as spans, if not as strings, which then might
   mean a string has to be allocated. That is, for Sep this is:
   ```csharp
   foreach (var row in reader)
   {
       for (var i = 0; i < row.ColCount; i++)
       {
           var span = row[i].Span;
       }
   }
   ```
* **XYZ** - finally the full scope is performed which is specific to each of
   the scenarios.

Additionally, as Sep supports multi-threaded parsing via `ParallelEnumerate`
benchmarks results with `_MT` in the method name are multi-threaded. These show
Sep provides unparalleled performance compared to any other CSV parser.

The overhead of Sep async support is also benchmarked and can be seen with
`_Async` in the method name. Note that this is the absolute best case for async
given there is no real IO involved and hence no actual asynchronous work or
continuations (thus no `Task` allocations) since benchmarks run from memory
only. This is fine as the main purpose of the benchmark is to gauge the overhead
of the async code path.

#### NCsvPerf PackageAssets Reader Comparison Benchmarks
[NCsvPerf](https://github.com/joelverhagen/NCsvPerf) from [The fastest CSV
   parser in
   .NET](https://www.joelverhagen.com/blog/2020/12/fastest-net-csv-parsers) is a
   benchmark which in [Joel Verhagen](https://twitter.com/joelverhagen)'s own
   words was defined with:

> My goal was to find the fastest low-level CSV parser. Essentially, all I
> wanted was a library that gave me a string[] for each line where each field in
> the line was an element in the array.

What is great about this work is it tests a whole of 35 different libraries and
approaches to this. Providing a great overview of those and their performance on
this specific scenario. Given Sylvan is the fastest of those it is used as the
one to beat here, while CsvHelper is used to compare to the most commonly used
library.

The source used for this benchmark [PackageAssetsBench.cs](src/Sep.ComparisonBenchmarks/PackageAssetsBench.cs) is a
[PackageAssets.csv](https://raw.githubusercontent.com/joelverhagen/NCsvPerf/main/NCsvPerf/TestData/PackageAssets.csv)
with NuGet package information in 25 columns with rows like:
```text
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,RuntimeAssemblies,,,net5.0,,,,,,lib/net5.0/BlazorGrid.dll,BlazorGrid.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,CompileLibAssemblies,,,net5.0,,,,,,lib/net5.0/BlazorGrid.dll,BlazorGrid.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,ResourceAssemblies,,,net5.0,,,,,,lib/net5.0/de/BlazorGrid.resources.dll,BlazorGrid.resources.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,MSBuildFiles,,,any,,,,,,build/Microsoft.AspNetCore.StaticWebAssets.props,Microsoft.AspNetCore.StaticWebAssets.props,.props,build,any,Any,0.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,MSBuildFiles,,,any,,,,,,build/Akinzekeel.BlazorGrid.props,Akinzekeel.BlazorGrid.props,.props,build,any,Any,0.0.0.0,,,0.0.0.0
```
For `Scope = Asset` the columns are parsed into a
[`PackageAsset`](src/Sep.ComparisonBenchmarks/PackageAsset.cs) class, which
consists of 25 properties of which 22 are `string`s. Each asset is accumulated
into a `List<PackageAsset>`. Each column is accessed as a `string` regardless.

This means this benchmark is dominated by turning columns into `string`s for the
decently fast parsers. Hence, the fastest libraries in this test employ string
pooling. That is, basically a custom dictionary from `ReadOnlySpan<char>` to
`string`, which avoids allocating a new `string` for repeated values. And as can
be seen in the csv-file there are a lot of repeated values. Both Sylvan and
CsvHelper do this in the benchmark. So does Sep and as with Sep this is an
optional configuration that has to be explicitly enable. For Sep this means the
reader is created with something like:
```csharp
using var reader = Sep.Reader(o => o with
{
    HasHeader = false,
    CreateToString = SepToString.PoolPerCol(maximumStringLength: 128),
})
.From(CreateReader());
```
What is unique for Sep is that it allows defining a pool per column e.g. via
`SepToString.PoolPerCol(...)`. This is based on the fact
that often each column has its own set of values or strings that may be repeated
without any overlap to other columns. This also allows one to define per column
specific handling of `ToString` behavior. Whether to pool or not. Or even to use
a statically defined pool.

Sep supports unescaping via an option, see [SepReaderOptions](#sepreaderoptions)
and [Unescaping](#unescaping). Therefore, Sep has two methods being tested. The
default `Sep` without unescaping and `Sep_Unescape` where unescaping is enabled.
Note that only if there are quotes will there be any unescaping, but to support
unescape one has to track extra state during parsing which means there is a
slight cost no matter what, most notably for the `Cols` scope.

##### PackageAssets Benchmark Results
For pure parsing Sep minimizes allocations through extensive use of pooling and
the `ArrayPool<T>`.

This is in many aspects due to Sep having extremely optimized string pooling and
optimized hashing of `ReadOnlySpan<char>`, and thus not really due to the
csv-parsing itself, since that is not a big part of the time consumed. At least
not for a decently fast csv-parser.

###### AMD.EPYC.7763 - PackageAssets Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     4.035 ms |  1.00 |  29 | 7232.4 |   80.7 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.099 ms |  1.02 |  29 | 7119.7 |   82.0 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.367 ms |  1.08 |  29 | 6682.0 |   87.3 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |     5.057 ms |  1.26 |  29 | 5770.7 |  101.1 |      8659 B |       30.07 |
| CsvHelper    | Row   | 50000   |     6.221 ms |  1.54 |  29 | 4690.9 |  124.4 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     5.083 ms |  1.00 |  29 | 5741.0 |  101.7 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.907 ms |  1.16 |  29 | 4940.0 |  118.1 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.309 ms |  1.64 |  29 | 3511.9 |  166.2 |      8660 B |       30.07 |
| CsvHelper    | Cols  | 50000   |    24.215 ms |  4.76 |  29 | 1205.1 |  484.3 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    42.552 ms |  1.00 |  29 |  685.8 |  851.0 |  14133917 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.246 ms |  0.66 |  29 | 1033.1 |  564.9 |  14220810 B |        1.01 |
| Sylvan___    | Asset | 50000   |    50.678 ms |  1.19 |  29 |  575.8 | 1013.6 |  14297074 B |        1.01 |
| CsvHelper    | Asset | 50000   |    47.163 ms |  1.11 |  29 |  618.7 |  943.3 |  15112520 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   922.401 ms |  1.00 | 583 |  632.9 |  922.4 | 273067296 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   539.278 ms |  0.58 | 583 | 1082.5 |  539.3 | 283816640 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,087.523 ms |  1.18 | 583 |  536.8 | 1087.5 | 273228840 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,029.218 ms |  1.12 | 583 |  567.2 | 1029.2 | 274048008 B |        1.00 |

###### AMD.EPYC.9V45 - PackageAssets Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method       | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-----------:|------:|----:|--------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |   2.190 ms |  1.00 |  29 | 13282.3 |   43.8 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   2.216 ms |  1.01 |  29 | 13122.8 |   44.3 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   2.456 ms |  1.12 |  29 | 11843.9 |   49.1 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |   3.137 ms |  1.43 |  29 |  9271.7 |   62.7 |      8658 B |       30.06 |
| CsvHelper    | Row   | 50000   |   3.034 ms |  1.39 |  29 |  9586.5 |   60.7 |      1264 B |        4.39 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Cols  | 50000   |   2.979 ms |  1.00 |  29 |  9762.7 |   59.6 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   4.042 ms |  1.36 |  29 |  7195.5 |   80.8 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |   5.006 ms |  1.68 |  29 |  5810.5 |  100.1 |      8659 B |       30.07 |
| CsvHelper    | Cols  | 50000   |  13.224 ms |  4.44 |  29 |  2199.5 |  264.5 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 50000   |  32.328 ms |  1.00 |  29 |   899.7 |  646.6 |  14132904 B |        1.00 |
| Sep_MT___    | Asset | 50000   |  20.326 ms |  0.63 |  29 |  1431.0 |  406.5 |  14215396 B |        1.01 |
| Sylvan___    | Asset | 50000   |  36.167 ms |  1.12 |  29 |   804.2 |  723.3 |  14297144 B |        1.01 |
| CsvHelper    | Asset | 50000   |  33.227 ms |  1.03 |  29 |   875.4 |  664.5 |  15112094 B |        1.07 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 1000000 | 601.113 ms |  1.00 | 581 |   968.0 |  601.1 | 273071528 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 392.623 ms |  0.65 | 581 |  1482.0 |  392.6 | 283671072 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 720.689 ms |  1.20 | 581 |   807.4 |  720.7 | 273232912 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 638.392 ms |  1.06 | 581 |   911.5 |  638.4 | 274045672 B |        1.00 |

###### AMD.Ryzen.9.9950X3D2 - PackageAssets Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method       | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-----------:|------:|----:|--------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |   1.147 ms |  1.00 |  29 | 25444.6 |   22.9 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   1.170 ms |  1.02 |  29 | 24930.9 |   23.4 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   1.339 ms |  1.17 |  29 | 21789.1 |   26.8 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |   1.830 ms |  1.60 |  29 | 15946.1 |   36.6 |      8657 B |       30.06 |
| CsvHelper    | Row   | 50000   |   1.693 ms |  1.48 |  29 | 17232.1 |   33.9 |      1264 B |        4.39 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Cols  | 50000   |   1.683 ms |  1.00 |  29 | 17334.4 |   33.7 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   2.265 ms |  1.35 |  29 | 12885.6 |   45.3 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |   3.152 ms |  1.87 |  29 |  9258.7 |   63.0 |      8658 B |       30.06 |
| CsvHelper    | Cols  | 50000   |  10.025 ms |  5.96 |  29 |  2910.8 |  200.5 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 50000   |  20.367 ms |  1.00 |  29 |  1432.8 |  407.3 |  14132455 B |        1.00 |
| Sep_MT___    | Asset | 50000   |   8.433 ms |  0.41 |  29 |  3460.4 |  168.7 |  14303020 B |        1.01 |
| Sylvan___    | Asset | 50000   |  26.342 ms |  1.29 |  29 |  1107.8 |  526.8 |  14296784 B |        1.01 |
| CsvHelper    | Asset | 50000   |  23.147 ms |  1.14 |  29 |  1260.7 |  462.9 |  15111674 B |        1.07 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 1000000 | 385.651 ms |  1.00 | 583 |  1513.8 |  385.7 | 273063040 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 181.312 ms |  0.47 | 583 |  3219.8 |  181.3 | 274851412 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 499.948 ms |  1.30 | 583 |  1167.7 |  499.9 | 273226752 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 428.180 ms |  1.11 | 583 |  1363.4 |  428.2 | 274041952 B |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     3.925 ms |  1.00 |  29 | 7409.9 |   78.5 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.572 ms |  0.95 |  29 | 8143.0 |   71.4 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.872 ms |  1.03 |  29 | 7512.6 |   77.4 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.242 ms |  5.64 |  29 | 1369.3 |  424.8 |      7477 B |       25.96 |
| CsvHelper    | Row   | 50000   |     7.734 ms |  2.05 |  29 | 3761.0 |  154.7 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     4.867 ms |  1.00 |  29 | 5976.4 |   97.3 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.476 ms |  1.13 |  29 | 5312.0 |  109.5 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.896 ms |  4.92 |  29 | 1217.2 |  477.9 |      7480 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    18.767 ms |  3.87 |  29 | 1549.9 |  375.3 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    44.815 ms |  1.00 |  29 |  649.0 |  896.3 |  14133186 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    32.957 ms |  0.74 |  29 |  882.5 |  659.1 |  14227586 B |        1.01 |
| Sylvan___    | Asset | 50000   |    61.671 ms |  1.39 |  29 |  471.6 | 1233.4 |  14296215 B |        1.01 |
| CsvHelper    | Asset | 50000   |    46.784 ms |  1.06 |  29 |  621.7 |  935.7 |  15113249 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   836.731 ms |  1.00 | 581 |  695.4 |  836.7 | 273069336 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   462.578 ms |  0.56 | 581 | 1257.9 |  462.6 | 283460136 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,216.486 ms |  1.46 | 581 |  478.3 | 1216.5 | 273235408 B |        1.00 |
| CsvHelper    | Asset | 1000000 |   782.140 ms |  0.94 | 581 |  744.0 |  782.1 | 274047240 B |        1.00 |

###### Apple.M2.Pro.(Virtual) - PackageAssets Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     3.011 ms |  1.00 |  29 | 9661.4 |   60.2 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.057 ms |  1.02 |  29 | 9513.5 |   61.1 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.054 ms |  1.01 |  29 | 9523.3 |   61.1 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    18.353 ms |  6.10 |  29 | 1584.8 |  367.1 |      7470 B |       25.94 |
| CsvHelper    | Row   | 50000   |     6.899 ms |  2.29 |  29 | 4215.9 |  138.0 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     4.266 ms |  1.00 |  29 | 6817.4 |   85.3 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     4.885 ms |  1.15 |  29 | 5954.3 |   97.7 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    21.860 ms |  5.13 |  29 | 1330.6 |  437.2 |      7477 B |       25.96 |
| CsvHelper    | Cols  | 50000   |    18.161 ms |  4.26 |  29 | 1601.6 |  363.2 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    40.105 ms |  1.00 |  29 |  725.2 |  802.1 |  14133184 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    33.864 ms |  0.85 |  29 |  858.9 |  677.3 |  14270578 B |        1.01 |
| Sylvan___    | Asset | 50000   |    61.812 ms |  1.55 |  29 |  470.6 | 1236.2 |  14296227 B |        1.01 |
| CsvHelper    | Asset | 50000   |    43.298 ms |  1.09 |  29 |  671.8 |  866.0 |  15112562 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   721.960 ms |  1.00 | 581 |  806.0 |  722.0 | 273069384 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   425.894 ms |  0.59 | 581 | 1366.2 |  425.9 | 284521584 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,148.618 ms |  1.59 | 581 |  506.6 | 1148.6 | 273228680 B |        1.00 |
| CsvHelper    | Asset | 1000000 |   727.604 ms |  1.01 | 581 |  799.7 |  727.6 | 274047120 B |        1.00 |

###### Cobalt.100 - PackageAssets Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     5.175 ms |  1.00 |  29 | 5638.4 |  103.5 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.124 ms |  0.99 |  29 | 5694.5 |  102.5 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.011 ms |  0.97 |  29 | 5823.5 |  100.2 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    18.772 ms |  3.63 |  29 | 1554.5 |  375.4 |      7470 B |       25.94 |
| CsvHelper    | Row   | 50000   |    12.547 ms |  2.43 |  29 | 2325.7 |  250.9 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     6.483 ms |  1.00 |  29 | 4501.3 |  129.7 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.344 ms |  1.13 |  29 | 3973.7 |  146.9 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.467 ms |  3.62 |  29 | 1243.5 |  469.3 |      7478 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    28.135 ms |  4.34 |  29 | 1037.2 |  562.7 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    36.425 ms |  1.00 |  29 |  801.1 |  728.5 |  14132256 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    18.623 ms |  0.51 |  29 | 1566.9 |  372.5 |  14199780 B |        1.00 |
| Sylvan___    | Asset | 50000   |    55.956 ms |  1.54 |  29 |  521.5 | 1119.1 |  14295531 B |        1.01 |
| CsvHelper    | Asset | 50000   |    43.072 ms |  1.18 |  29 |  677.5 |  861.4 |  15111560 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   776.453 ms |  1.00 | 583 |  751.9 |  776.5 | 273063200 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   366.160 ms |  0.47 | 583 | 1594.3 |  366.2 | 284984336 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,208.903 ms |  1.56 | 583 |  482.9 | 1208.9 | 273225560 B |        1.00 |
| CsvHelper    | Asset | 1000000 |   950.466 ms |  1.22 | 583 |  614.2 |  950.5 | 274042112 B |        1.00 |

###### Neoverse-N2 - PackageAssets Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     4.620 ms |  1.00 |  29 | 6295.4 |   92.4 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.684 ms |  1.01 |  29 | 6210.2 |   93.7 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.625 ms |  1.00 |  29 | 6289.4 |   92.5 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    18.395 ms |  3.98 |  29 | 1581.2 |  367.9 |      7470 B |       25.94 |
| CsvHelper    | Row   | 50000   |    11.363 ms |  2.46 |  29 | 2559.6 |  227.3 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     6.155 ms |  1.00 |  29 | 4725.5 |  123.1 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.713 ms |  1.09 |  29 | 4332.7 |  134.3 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.000 ms |  3.74 |  29 | 1264.6 |  460.0 |      7478 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    27.551 ms |  4.48 |  29 | 1055.7 |  551.0 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    34.554 ms |  1.00 |  29 |  841.7 |  691.1 |  14132256 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    18.045 ms |  0.52 |  29 | 1611.9 |  360.9 |  14186090 B |        1.00 |
| Sylvan___    | Asset | 50000   |    56.141 ms |  1.62 |  29 |  518.1 | 1122.8 |  14295531 B |        1.01 |
| CsvHelper    | Asset | 50000   |    43.056 ms |  1.25 |  29 |  675.5 |  861.1 |  15111560 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   782.374 ms |  1.00 | 581 |  743.7 |  782.4 | 273063064 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   365.392 ms |  0.47 | 581 | 1592.5 |  365.4 | 281345720 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,224.224 ms |  1.56 | 581 |  475.3 | 1224.2 | 273225568 B |        1.00 |
| CsvHelper    | Asset | 1000000 |   945.773 ms |  1.21 | 581 |  615.2 |  945.8 | 274041960 B |        1.00 |


##### PackageAssets Benchmark Results (SERVER GC)
The package assets benchmark (Scope `Asset`) has a very high base load in the
form of the accumulated instances of `PackageAsset` and since Sep is so fast the
GC becomes a significant bottleneck for the benchmark, especially for
multi-threaded parsing. Switching to [SERVER
GC](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/workstation-server-gc)
can, therefore, provide significant speedup as can be seen below.

###### AMD.EPYC.7763 - PackageAssets Benchmark Results (SERVER GC) (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope | Rows    | Mean      | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  34.16 ms |  1.00 |  29 |  854.2 |  683.3 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |  16.98 ms |  0.50 |  29 | 1718.6 |  339.6 |  13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |  42.33 ms |  1.24 |  29 |  689.3 |  846.7 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  41.52 ms |  1.22 |  29 |  702.7 |  830.5 |  14.41 MB |        1.07 |
|           |       |         |           |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 672.42 ms |  1.00 | 583 |  868.2 |  672.4 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 344.74 ms |  0.51 | 583 | 1693.4 |  344.7 | 269.58 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 860.44 ms |  1.28 | 583 |  678.5 |  860.4 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 803.56 ms |  1.20 | 583 |  726.5 |  803.6 | 261.35 MB |        1.00 |

###### AMD.EPYC.9V45 - PackageAssets Benchmark Results (SERVER GC) (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope | Rows    | Mean      | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  20.11 ms |  1.00 |  29 | 1446.3 |  402.2 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |  10.37 ms |  0.52 |  29 | 2804.2 |  207.4 |  13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |  27.14 ms |  1.35 |  29 | 1071.8 |  542.8 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  22.60 ms |  1.12 |  29 | 1286.8 |  452.1 |  14.41 MB |        1.07 |
|           |       |         |           |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 396.28 ms |  1.00 | 581 | 1468.3 |  396.3 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 214.04 ms |  0.54 | 581 | 2718.5 |  214.0 | 270.21 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 550.83 ms |  1.39 | 581 | 1056.4 |  550.8 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 447.11 ms |  1.13 | 581 | 1301.4 |  447.1 | 261.35 MB |        1.00 |

###### AMD.Ryzen.9.9950X3D2 - PackageAssets Benchmark Results (SERVER GC) (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|--------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  14.044 ms |  1.00 |  29 |  2077.8 |  280.9 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   2.919 ms |  0.21 |  29 |  9997.3 |   58.4 |  13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  18.849 ms |  1.34 |  29 |  1548.1 |  377.0 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  16.296 ms |  1.16 |  29 |  1790.6 |  325.9 |  14.41 MB |        1.07 |
|           |       |         |            |       |     |         |        |           |             |
| Sep______ | Asset | 1000000 | 281.728 ms |  1.00 | 583 |  2072.2 |  281.7 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |  50.774 ms |  0.18 | 583 | 11497.6 |   50.8 |  261.6 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 376.077 ms |  1.33 | 583 |  1552.3 |  376.1 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 333.274 ms |  1.18 | 583 |  1751.7 |  333.3 | 261.35 MB |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets Benchmark Results (SERVER GC) (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    26.64 ms |  1.00 |  29 | 1091.8 |  532.8 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    13.39 ms |  0.51 |  29 | 2172.9 |  267.7 |  13.57 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    59.11 ms |  2.23 |  29 |  492.1 | 1182.1 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    32.02 ms |  1.21 |  29 |  908.4 |  640.4 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   540.30 ms |  1.00 | 581 | 1076.9 |  540.3 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   320.41 ms |  0.61 | 581 | 1816.1 |  320.4 | 270.58 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 1,952.48 ms |  3.69 | 581 |  298.0 | 1952.5 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   736.86 ms |  1.39 | 581 |  789.7 |  736.9 | 261.35 MB |        1.00 |

###### Apple.M2.Pro.(Virtual) - PackageAssets Benchmark Results (SERVER GC) (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    25.65 ms |  1.00 |  29 | 1134.0 |  513.0 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    10.23 ms |  0.40 |  29 | 2842.2 |  204.7 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    54.09 ms |  2.11 |  29 |  537.7 | 1081.8 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    30.12 ms |  1.17 |  29 |  965.7 |  602.4 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   522.54 ms |  1.00 | 581 | 1113.6 |  522.5 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   129.54 ms |  0.25 | 581 | 4491.9 |  129.5 | 267.01 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,057.15 ms |  2.03 | 581 |  550.4 | 1057.1 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   536.69 ms |  1.03 | 581 | 1084.2 |  536.7 | 261.35 MB |        1.00 |

###### Cobalt.100 - PackageAssets Benchmark Results (SERVER GC) (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    31.73 ms |  1.00 |  29 |  919.7 |  634.6 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    13.36 ms |  0.42 |  29 | 2183.5 |  267.3 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    53.40 ms |  1.68 |  29 |  546.5 | 1068.0 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    40.93 ms |  1.29 |  29 |  713.0 |  818.5 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   637.89 ms |  1.00 | 583 |  915.2 |  637.9 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   208.12 ms |  0.33 | 583 | 2805.0 |  208.1 | 269.06 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,061.57 ms |  1.66 | 583 |  549.9 | 1061.6 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   816.24 ms |  1.28 | 583 |  715.2 |  816.2 | 261.35 MB |        1.00 |

###### Neoverse-N2 - PackageAssets Benchmark Results (SERVER GC) (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    32.62 ms |  1.00 |  29 |  891.5 |  652.5 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.10 ms |  0.37 |  29 | 2404.6 |  241.9 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    54.06 ms |  1.66 |  29 |  538.0 | 1081.2 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    41.76 ms |  1.28 |  29 |  696.4 |  835.3 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   651.02 ms |  1.00 | 581 |  893.8 |  651.0 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   218.10 ms |  0.34 | 581 | 2668.0 |  218.1 | 266.98 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,076.77 ms |  1.65 | 581 |  540.4 | 1076.8 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   821.60 ms |  1.26 | 581 |  708.2 |  821.6 | 261.35 MB |        1.00 |


##### PackageAssets with Quotes Benchmark Results
`NCsvPerf` does not examine performance in the face of quotes in the csv. This
is relevant since some libraries like Sylvan will revert to a slower (not SIMD
vectorized) parsing code path if it encounters quotes. Sep was designed to
always use SIMD vectorization no matter what.

Since there are two extra `char`s to handle per column, it does have a
significant impact on performance, no matter what though. This is expected when
looking at the numbers. For each row of 25 columns, there are 24 separators
(here `,`) and one set of line endings (here `\r\n`). That's 26 characters.
Adding quotes around each of the 25 columns will add 50 characters or almost
triple the total to 76.

###### AMD.EPYC.7763 - PackageAssets with Quotes Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     7.451 ms |  1.00 |  33 | 4479.5 |  149.0 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.622 ms |  1.02 |  33 | 4378.8 |  152.4 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     8.094 ms |  1.09 |  33 | 4123.7 |  161.9 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    24.471 ms |  3.28 |  33 | 1363.9 |  489.4 |      8670 B |       30.10 |
| CsvHelper    | Row   | 50000   |    10.351 ms |  1.39 |  33 | 3224.7 |  207.0 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     8.772 ms |  1.00 |  33 | 3804.8 |  175.4 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    11.313 ms |  1.29 |  33 | 2950.4 |  226.3 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    29.036 ms |  3.31 |  33 | 1149.5 |  580.7 |      8673 B |       30.11 |
| CsvHelper    | Cols  | 50000   |    31.281 ms |  3.57 |  33 | 1067.0 |  625.6 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    50.097 ms |  1.00 |  33 |  666.3 | 1001.9 |  14132847 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    32.717 ms |  0.65 |  33 | 1020.2 |  654.3 |  14195552 B |        1.00 |
| Sylvan___    | Asset | 50000   |    70.420 ms |  1.41 |  33 |  474.0 | 1408.4 |  14297022 B |        1.01 |
| CsvHelper    | Asset | 50000   |    58.020 ms |  1.16 |  33 |  575.3 | 1160.4 |  15112661 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 | 1,016.106 ms |  1.00 | 667 |  657.1 | 1016.1 | 273067216 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   604.291 ms |  0.59 | 667 | 1104.9 |  604.3 | 283225024 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,482.327 ms |  1.46 | 667 |  450.4 | 1482.3 | 273228872 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,173.731 ms |  1.16 | 667 |  568.9 | 1173.7 | 274045528 B |        1.00 |

###### AMD.EPYC.9V45 - PackageAssets with Quotes Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method       | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-----------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |   3.912 ms |  1.00 |  33 | 8506.6 |   78.2 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   3.956 ms |  1.01 |  33 | 8412.7 |   79.1 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   4.408 ms |  1.13 |  33 | 7551.1 |   88.2 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |  15.965 ms |  4.08 |  33 | 2084.7 |  319.3 |      8662 B |       30.08 |
| CsvHelper    | Row   | 50000   |   4.661 ms |  1.19 |  33 | 7141.2 |   93.2 |      1264 B |        4.39 |
|              |       |         |            |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |   4.589 ms |  1.00 |  33 | 7252.2 |   91.8 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   6.144 ms |  1.34 |  33 | 5417.4 |  122.9 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |  16.873 ms |  3.68 |  33 | 1972.6 |  337.5 |      8662 B |       30.08 |
| CsvHelper    | Cols  | 50000   |  16.449 ms |  3.58 |  33 | 2023.3 |  329.0 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |  33.906 ms |  1.00 |  33 |  981.6 |  678.1 |  14132958 B |        1.00 |
| Sep_MT___    | Asset | 50000   |  20.875 ms |  0.62 |  33 | 1594.3 |  417.5 |  14240436 B |        1.01 |
| Sylvan___    | Asset | 50000   |  48.567 ms |  1.43 |  33 |  685.3 |  971.3 |  14297119 B |        1.01 |
| CsvHelper    | Asset | 50000   |  35.958 ms |  1.06 |  33 |  925.6 |  719.2 |  15112103 B |        1.07 |
|              |       |         |            |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 | 643.560 ms |  1.00 | 665 | 1034.6 |  643.6 | 273067512 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 433.440 ms |  0.67 | 665 | 1536.1 |  433.4 | 282026976 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 945.606 ms |  1.47 | 665 |  704.1 |  945.6 | 273228840 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 698.821 ms |  1.09 | 665 |  952.7 |  698.8 | 274045760 B |        1.00 |

###### AMD.Ryzen.9.9950X3D2 - PackageAssets with Quotes Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method       | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-----------:|------:|----:|--------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |   2.225 ms |  1.00 |  33 | 15003.5 |   44.5 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   2.259 ms |  1.02 |  33 | 14778.2 |   45.2 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   2.664 ms |  1.20 |  33 | 12530.4 |   53.3 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |  10.617 ms |  4.77 |  33 |  3143.9 |  212.3 |      8660 B |       30.07 |
| CsvHelper    | Row   | 50000   |   2.799 ms |  1.26 |  33 | 11925.0 |   56.0 |      1264 B |        4.39 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Cols  | 50000   |   2.749 ms |  1.00 |  33 | 12143.5 |   55.0 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   3.896 ms |  1.42 |  33 |  8567.7 |   77.9 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |  12.104 ms |  4.40 |  33 |  2757.6 |  242.1 |      8662 B |       30.08 |
| CsvHelper    | Cols  | 50000   |  12.778 ms |  4.65 |  33 |  2612.0 |  255.6 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 50000   |  21.302 ms |  1.00 |  33 |  1566.8 |  426.0 |  14132407 B |        1.00 |
| Sep_MT___    | Asset | 50000   |  10.640 ms |  0.50 |  33 |  3137.1 |  212.8 |  14304828 B |        1.01 |
| Sylvan___    | Asset | 50000   |  32.367 ms |  1.52 |  33 |  1031.2 |  647.3 |  14296721 B |        1.01 |
| CsvHelper    | Asset | 50000   |  26.121 ms |  1.23 |  33 |  1277.8 |  522.4 |  15111668 B |        1.07 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 1000000 | 432.455 ms |  1.00 | 667 |  1544.0 |  432.5 | 273063040 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 198.416 ms |  0.46 | 667 |  3365.2 |  198.4 | 275809996 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 677.688 ms |  1.57 | 667 |   985.3 |  677.7 | 273226752 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 521.251 ms |  1.21 | 667 |  1281.0 |  521.3 | 274042112 B |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets with Quotes Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     5.406 ms |  1.00 |  33 | 6156.6 |  108.1 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.339 ms |  0.99 |  33 | 6234.1 |  106.8 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.126 ms |  1.32 |  33 | 4670.5 |  142.5 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.727 ms |  3.84 |  33 | 1605.7 |  414.5 |      7470 B |       25.94 |
| CsvHelper    | Row   | 50000   |    12.234 ms |  2.26 |  33 | 2720.6 |  244.7 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     8.815 ms |  1.00 |  33 | 3775.4 |  176.3 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.435 ms |  1.53 |  33 | 2477.2 |  268.7 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.958 ms |  3.30 |  33 | 1149.3 |  579.2 |      7481 B |       25.98 |
| CsvHelper    | Cols  | 50000   |    31.228 ms |  3.56 |  33 | 1065.8 |  624.6 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    48.148 ms |  1.00 |  33 |  691.2 |  963.0 |  14133158 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    43.507 ms |  0.91 |  33 |  765.0 |  870.1 |  14217291 B |        1.01 |
| Sylvan___    | Asset | 50000   |    65.317 ms |  1.36 |  33 |  509.5 | 1306.3 |  14296126 B |        1.01 |
| CsvHelper    | Asset | 50000   |    70.284 ms |  1.46 |  33 |  473.5 | 1405.7 |  15112294 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 | 1,044.496 ms |  1.00 | 665 |  637.4 | 1044.5 | 273069336 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   604.362 ms |  0.59 | 665 | 1101.7 |  604.4 | 283647504 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,373.792 ms |  1.35 | 665 |  484.6 | 1373.8 | 273233280 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,144.441 ms |  1.12 | 665 |  581.8 | 1144.4 | 274048496 B |        1.00 |

###### Apple.M2.Pro.(Virtual) - PackageAssets with Quotes Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     5.408 ms |  1.00 |  33 | 6154.4 |  108.2 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.395 ms |  1.00 |  33 | 6168.8 |  107.9 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.379 ms |  1.36 |  33 | 4510.6 |  147.6 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.198 ms |  3.92 |  33 | 1570.0 |  424.0 |      7470 B |       25.94 |
| CsvHelper    | Row   | 50000   |    11.246 ms |  2.08 |  33 | 2959.4 |  224.9 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     6.727 ms |  1.00 |  33 | 4947.4 |  134.5 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     9.266 ms |  1.38 |  33 | 3592.0 |  185.3 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.624 ms |  3.52 |  33 | 1408.8 |  472.5 |      7478 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    26.937 ms |  4.01 |  33 | 1235.5 |  538.7 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    44.260 ms |  1.00 |  33 |  752.0 |  885.2 |  14133151 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    35.951 ms |  0.81 |  33 |  925.8 |  719.0 |  14248068 B |        1.01 |
| Sylvan___    | Asset | 50000   |    61.259 ms |  1.39 |  33 |  543.3 | 1225.2 |  14296224 B |        1.01 |
| CsvHelper    | Asset | 50000   |    50.410 ms |  1.14 |  33 |  660.2 | 1008.2 |  15112468 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   799.532 ms |  1.00 | 665 |  832.7 |  799.5 | 273069304 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   460.625 ms |  0.58 | 665 | 1445.4 |  460.6 | 282657704 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,204.471 ms |  1.51 | 665 |  552.8 | 1204.5 | 273228544 B |        1.00 |
| CsvHelper    | Asset | 1000000 |   968.121 ms |  1.21 | 665 |  687.7 |  968.1 | 274047248 B |        1.00 |

###### Cobalt.100 - PackageAssets with Quotes Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     7.122 ms |  1.00 |  33 | 4686.8 |  142.4 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.339 ms |  1.03 |  33 | 4548.1 |  146.8 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     9.229 ms |  1.30 |  33 | 3616.4 |  184.6 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.802 ms |  3.06 |  33 | 1530.9 |  436.0 |      7477 B |       25.96 |
| CsvHelper    | Row   | 50000   |    19.703 ms |  2.77 |  33 | 1694.0 |  394.1 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     9.247 ms |  1.00 |  33 | 3609.7 |  184.9 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.607 ms |  1.36 |  33 | 2647.5 |  252.1 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.689 ms |  2.78 |  33 | 1299.3 |  513.8 |      7479 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    43.738 ms |  4.73 |  33 |  763.1 |  874.8 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    39.137 ms |  1.00 |  33 |  852.8 |  782.7 |  14132256 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    22.713 ms |  0.58 |  33 | 1469.5 |  454.3 |  14187634 B |        1.00 |
| Sylvan___    | Asset | 50000   |    57.543 ms |  1.47 |  33 |  580.0 | 1150.9 |  14295507 B |        1.01 |
| CsvHelper    | Asset | 50000   |    58.763 ms |  1.50 |  33 |  568.0 | 1175.3 |  15111560 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   880.973 ms |  1.00 | 667 |  757.9 |  881.0 | 273063200 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   373.785 ms |  0.42 | 667 | 1786.3 |  373.8 | 278984496 B |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,270.955 ms |  1.44 | 667 |  525.4 | 1271.0 | 273225560 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,284.670 ms |  1.46 | 667 |  519.7 | 1284.7 | 274041936 B |        1.00 |

###### Neoverse-N2 - PackageAssets with Quotes Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     6.660 ms |  1.00 |  33 | 4997.7 |  133.2 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     6.752 ms |  1.01 |  33 | 4928.9 |  135.0 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     8.818 ms |  1.32 |  33 | 3774.5 |  176.4 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.074 ms |  3.16 |  33 | 1579.3 |  421.5 |      7477 B |       25.96 |
| CsvHelper    | Row   | 50000   |    18.532 ms |  2.78 |  33 | 1796.0 |  370.6 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     8.089 ms |  1.00 |  33 | 4114.4 |  161.8 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    11.735 ms |  1.45 |  33 | 2836.2 |  234.7 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.274 ms |  3.12 |  33 | 1316.8 |  505.5 |      7479 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    42.700 ms |  5.28 |  33 |  779.4 |  854.0 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    39.145 ms |  1.00 |  33 |  850.2 |  782.9 |  14132256 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    20.497 ms |  0.52 |  33 | 1623.8 |  409.9 |  14192982 B |        1.00 |
| Sylvan___    | Asset | 50000   |    57.904 ms |  1.48 |  33 |  574.8 | 1158.1 |  14295507 B |        1.01 |
| CsvHelper    | Asset | 50000   |    58.051 ms |  1.48 |  33 |  573.3 | 1161.0 |  15111560 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   869.430 ms |  1.00 | 665 |  765.8 |  869.4 | 273063176 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   378.659 ms |  0.44 | 665 | 1758.3 |  378.7 | 281121960 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,265.994 ms |  1.46 | 665 |  525.9 | 1266.0 | 273225488 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,287.738 ms |  1.48 | 665 |  517.0 | 1287.7 | 274041944 B |        1.00 |


##### PackageAssets with Quotes Benchmark Results (SERVER GC)
Here again are benchmark results with server garbage collection, which provides
significant speedup over workstation garbage collection.

###### AMD.EPYC.7763 - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    38.69 ms |  1.00 |  33 |  862.7 |  773.8 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    19.66 ms |  0.51 |  33 | 1697.7 |  393.2 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    61.44 ms |  1.59 |  33 |  543.2 | 1228.8 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    47.35 ms |  1.23 |  33 |  704.9 |  947.0 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   785.66 ms |  1.00 | 667 |  849.9 |  785.7 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   395.36 ms |  0.50 | 667 | 1688.9 |  395.4 | 266.83 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,274.77 ms |  1.62 | 667 |  523.8 | 1274.8 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   992.33 ms |  1.26 | 667 |  672.9 |  992.3 | 261.35 MB |        1.00 |

###### AMD.EPYC.9V45 - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope | Rows    | Mean      | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  22.20 ms |  1.00 |  33 | 1499.3 |  444.0 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |  11.58 ms |  0.52 |  33 | 2874.0 |  231.6 |  13.56 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  39.75 ms |  1.80 |  33 |  837.3 |  795.0 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  26.86 ms |  1.22 |  33 | 1239.2 |  537.2 |  14.41 MB |        1.07 |
|           |       |         |           |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 455.12 ms |  1.00 | 665 | 1462.9 |  455.1 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 240.79 ms |  0.53 | 665 | 2765.1 |  240.8 | 269.25 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 771.07 ms |  1.70 | 665 |  863.5 |  771.1 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 517.63 ms |  1.14 | 665 | 1286.2 |  517.6 | 261.35 MB |        1.00 |

###### AMD.Ryzen.9.9950X3D2 - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  16.014 ms |  1.00 |  33 | 2084.2 |  320.3 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   4.886 ms |  0.31 |  33 | 6831.7 |   97.7 |  13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  26.843 ms |  1.68 |  33 | 1243.4 |  536.9 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  19.488 ms |  1.22 |  33 | 1712.7 |  389.8 |  14.41 MB |        1.07 |
|           |       |         |            |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 329.845 ms |  1.00 | 667 | 2024.3 |  329.8 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |  74.398 ms |  0.23 | 667 | 8974.8 |   74.4 | 261.54 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 564.077 ms |  1.71 | 667 | 1183.7 |  564.1 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 394.538 ms |  1.20 | 667 | 1692.4 |  394.5 | 261.35 MB |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    30.34 ms |  1.00 |  33 | 1097.0 |  606.8 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    18.79 ms |  0.62 |  33 | 1771.4 |  375.8 |  13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    58.28 ms |  1.94 |  33 |  571.1 | 1165.5 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    40.57 ms |  1.35 |  33 |  820.4 |  811.3 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   750.59 ms |  1.00 | 665 |  887.0 |  750.6 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   372.38 ms |  0.50 | 665 | 1787.9 |  372.4 | 265.55 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,261.99 ms |  1.70 | 665 |  527.6 | 1262.0 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   919.25 ms |  1.24 | 665 |  724.3 |  919.2 | 261.35 MB |        1.00 |

###### Apple.M2.Pro.(Virtual) - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    28.05 ms |  1.00 |  33 | 1186.6 |  560.9 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.29 ms |  0.44 |  33 | 2707.7 |  245.8 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    49.64 ms |  1.77 |  33 |  670.5 |  992.8 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    33.95 ms |  1.21 |  33 |  980.3 |  679.0 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   563.44 ms |  1.00 | 665 | 1181.7 |  563.4 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   221.98 ms |  0.39 | 665 | 2999.3 |  222.0 | 261.07 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,131.37 ms |  2.01 | 665 |  588.5 | 1131.4 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   795.53 ms |  1.41 | 665 |  836.9 |  795.5 | 261.35 MB |        1.00 |

###### Cobalt.100 - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    36.50 ms |  1.00 |  33 |  914.4 |  730.1 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.61 ms |  0.43 |  33 | 2138.5 |  312.2 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    55.83 ms |  1.53 |  33 |  597.8 | 1116.6 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    57.10 ms |  1.56 |  33 |  584.5 | 1142.1 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   736.25 ms |  1.00 | 667 |  906.9 |  736.2 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   239.01 ms |  0.32 | 667 | 2793.7 |  239.0 | 262.19 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,111.87 ms |  1.51 | 667 |  600.5 | 1111.9 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 1,131.49 ms |  1.54 | 667 |  590.1 | 1131.5 | 261.35 MB |        1.00 |

###### Neoverse-N2 - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    37.21 ms |  1.00 |  33 |  894.4 |  744.3 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.02 ms |  0.40 |  33 | 2215.7 |  300.4 |  13.52 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.39 ms |  1.52 |  33 |  590.2 | 1127.9 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    57.64 ms |  1.55 |  33 |  577.4 | 1152.8 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   747.45 ms |  1.00 | 665 |  890.8 |  747.5 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   237.41 ms |  0.32 | 665 | 2804.4 |  237.4 | 262.24 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,125.41 ms |  1.51 | 665 |  591.6 | 1125.4 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 1,140.24 ms |  1.53 | 665 |  583.9 | 1140.2 | 261.35 MB |        1.00 |


##### PackageAssets with Spaces and Quotes Benchmark Results

Similar to the benchmark related to quotes here spaces ` ` and quotes `"` are
added to relevant columns to benchmark impact of trimming and unescape on low
level column access. That is, basically ` " ` is prepended and appended to each
column. This will test the assumed most common case and fast path part of
trimming and unescaping in Sep.
Sylvan does not appear to support automatic trimming and is, therefore, not
included.

###### AMD.EPYC.7763 - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 10.33 ms |  1.00 | 41 | 4042.1 |  206.7 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 15.73 ms |  1.52 | 41 | 2655.3 |  314.6 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 17.08 ms |  1.65 | 41 | 2445.3 |  341.6 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 20.43 ms |  1.98 | 41 | 2044.2 |  408.7 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 81.99 ms |  7.93 | 41 |  509.4 | 1639.8 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 88.14 ms |  8.53 | 41 |  473.9 | 1762.8 |    1264 B |        4.39 |

###### AMD.EPYC.9V45 - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  5.251 ms |  1.00 | 41 | 7935.7 |  105.0 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  8.441 ms |  1.61 | 41 | 4937.0 |  168.8 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  9.859 ms |  1.88 | 41 | 4227.0 |  197.2 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 10.643 ms |  2.03 | 41 | 3915.5 |  212.9 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 46.891 ms |  8.93 | 41 |  888.8 |  937.8 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 51.000 ms |  9.71 | 41 |  817.1 | 1020.0 |    1264 B |        4.39 |

###### AMD.Ryzen.9.9950X3D2 - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  3.116 ms |  1.00 | 41 | 13406.1 |   62.3 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  5.923 ms |  1.90 | 41 |  7052.0 |  118.5 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  6.989 ms |  2.24 | 41 |  5976.3 |  139.8 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  7.638 ms |  2.45 | 41 |  5468.5 |  152.8 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 35.344 ms | 11.34 | 41 |  1181.8 |  706.9 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 38.500 ms | 12.36 | 41 |  1084.9 |  770.0 |    1264 B |        4.39 |

###### Apple.M1.(Virtual) - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method                     | Scope | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |-----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |   9.228 ms |  1.00 | 41 | 4516.0 |  184.6 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  12.763 ms |  1.40 | 41 | 3265.3 |  255.3 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  15.231 ms |  1.67 | 41 | 2736.2 |  304.6 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  17.833 ms |  1.96 | 41 | 2336.9 |  356.7 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 105.243 ms | 11.56 | 41 |  396.0 | 2104.9 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 |  78.654 ms |  8.64 | 41 |  529.8 | 1573.1 |    1264 B |        4.39 |

###### Apple.M2.Pro.(Virtual) - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  7.270 ms |  1.00 | 41 | 5732.6 |  145.4 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 10.953 ms |  1.51 | 41 | 3805.0 |  219.1 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 13.378 ms |  1.84 | 41 | 3115.1 |  267.6 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 14.086 ms |  1.94 | 41 | 2958.5 |  281.7 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 63.780 ms |  8.78 | 41 |  653.4 | 1275.6 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 69.826 ms |  9.61 | 41 |  596.8 | 1396.5 |    1264 B |        4.39 |

###### Cobalt.100 - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 11.01 ms |  1.00 | 41 | 3793.6 |  220.2 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 15.49 ms |  1.41 | 41 | 2696.9 |  309.8 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 18.17 ms |  1.65 | 41 | 2299.3 |  363.3 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 19.88 ms |  1.81 | 41 | 2101.4 |  397.5 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 75.05 ms |  6.82 | 41 |  556.6 | 1501.0 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 80.06 ms |  7.27 | 41 |  521.7 | 1601.2 |    1264 B |        4.39 |

###### Neoverse-N2 - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  9.379 ms |  1.00 | 41 | 4443.6 |  187.6 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 13.689 ms |  1.46 | 41 | 3044.3 |  273.8 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 16.367 ms |  1.75 | 41 | 2546.3 |  327.3 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 18.058 ms |  1.93 | 41 | 2307.8 |  361.2 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 74.516 ms |  7.95 | 41 |  559.3 | 1490.3 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 80.380 ms |  8.57 | 41 |  518.5 | 1607.6 |    1264 B |        4.39 |


#### Floats Reader Comparison Benchmarks
The [FloatsReaderBench.cs](src/Sep.ComparisonBenchmarks/FloatsReaderBench.cs)
benchmark demonstrates what Sep is built for. Namely parsing 32-bit floating
points or features as in machine learning. Here a simple CSV-file is randomly
generated with `N` ground truth values, `N` predicted result values and nothing
else (note this was changed from version 0.3.0, prior to that there were some
extra leading columns). `N = 20`
here. For example:
```text
GT_Feature0;GT_Feature1;GT_Feature2;GT_Feature3;GT_Feature4;GT_Feature5;GT_Feature6;GT_Feature7;GT_Feature8;GT_Feature9;GT_Feature10;GT_Feature11;GT_Feature12;GT_Feature13;GT_Feature14;GT_Feature15;GT_Feature16;GT_Feature17;GT_Feature18;GT_Feature19;RE_Feature0;RE_Feature1;RE_Feature2;RE_Feature3;RE_Feature4;RE_Feature5;RE_Feature6;RE_Feature7;RE_Feature8;RE_Feature9;RE_Feature10;RE_Feature11;RE_Feature12;RE_Feature13;RE_Feature14;RE_Feature15;RE_Feature16;RE_Feature17;RE_Feature18;RE_Feature19
0.52276427;0.16843422;0.26259267;0.7244084;0.51292276;0.17365117;0.76125056;0.23458846;0.2573214;0.50560355;0.3202332;0.3809696;0.26024464;0.5174511;0.035318818;0.8141374;0.57719684;0.3974705;0.15219308;0.09011261;0.70515215;0.81618196;0.5399706;0.044147138;0.7111546;0.14776127;0.90621275;0.6925897;0.5164137;0.18637845;0.041509967;0.30819967;0.5831603;0.8210651;0.003954861;0.535722;0.8051845;0.7483589;0.3845737;0.14911908
0.6264564;0.11517637;0.24996082;0.77242833;0.2896067;0.6481459;0.14364648;0.044498358;0.6045593;0.51591337;0.050794687;0.42036617;0.7065823;0.6284636;0.21844554;0.013253775;0.36516154;0.2674384;0.06866083;0.71817476;0.07094294;0.46409357;0.012033525;0.7978093;0.43917948;0.5134962;0.4995968;0.008952909;0.82883793;0.012896823;0.0030740085;0.063773096;0.6541431;0.034539033;0.9135142;0.92897075;0.46119377;0.37533295;0.61660606;0.044443816
0.7922863;0.5323656;0.400699;0.29737252;0.9072584;0.58673894;0.73510516;0.019412167;0.88168067;0.9576787;0.33283427;0.7107;0.1623628;0.10314285;0.4521515;0.33324885;0.7761104;0.14854911;0.13469358;0.21566042;0.59166247;0.5128394;0.98702157;0.766223;0.67204326;0.7149494;0.2894748;0.55206;0.9898286;0.65083236;0.02421702;0.34540752;0.92906284;0.027142895;0.21974725;0.26544374;0.03848049;0.2161237;0.59233844;0.42221397
0.10609442;0.32130885;0.32383907;0.7511514;0.8258279;0.00904226;0.0420841;0.84049565;0.8958947;0.23807365;0.92621964;0.8452882;0.2794469;0.545344;0.63447595;0.62532926;0.19230893;0.29726416;0.18304513;0.029583583;0.23084833;0.93346167;0.98742676;0.78163713;0.13521992;0.8833956;0.18670778;0.29476836;0.5599867;0.5562107;0.7124796;0.121927656;0.5981778;0.39144602;0.88092715;0.4449142;0.34820423;0.96379805;0.46364686;0.54301775
```
For `Scope=Floats` the benchmark will parse the features as two spans of
`float`s; one for ground truth values and one for predicted result values. Then
calculates the mean squared error (MSE) of those as an example. For Sep this
code is succinct and still incredibly efficient:
```csharp
using var reader = Sep.Reader().From(Reader.CreateReader());

var groundTruthColNames = reader.Header.NamesStartingWith("GT_");
var resultColNames = groundTruthColNames.Select(n =>
    n.Replace("GT_", "RE_", StringComparison.Ordinal))
    .ToArray();

var sum = 0.0;
var count = 0;
foreach (var row in reader)
{
    var gts = row[groundTruthColNames].Parse<float>();
    var res = row[resultColNames].Parse<float>();

    sum += MeanSquaredError(gts, res);
    ++count;
}
return sum / count;
````
Note how one can access and parse multiple columns easily while there are no
repeated allocations for the parsed floating points. Sep internally handles a
pool of arrays for handling multiple columns and returns spans for them.

The benchmark is based on an assumption of accessing columns by name per
row. Ideally, one would look up the indices of the columns by name before
enumerating rows, but this is a repeated nuisance to have to handle and Sep was
built to avoid this. Hence, the comparison is based on looking up by name for
each, even if this ends up adding a bit more code in the benchmark for other
approaches.

As can be seen below, the actual low level parsing of the separated values is a
tiny part of the total runtime for Sep for which the runtime is dominated by
parsing the floating points. Sep uses
[csFastFloat](https://github.com/CarlVerret/csFastFloat) for an integrated fast
floating point parser. If using Sylvan one may consider using csFastFloat if
that is an option.

In these benchmarks, CsvHelper accesses columns as strings, which requires an
allocation for each column (ReadLine by definition always allocates a string
per column).

Note that `ParallelEnumerate` provides significant speedup over single-threaded
parsing even though the source is only about 20 MB. This underlines how
efficient `ParallelEnumerate` is, but bear in mind that this is for the case of
repeated micro-benchmark runs.

##### AMD.EPYC.7763 - FloatsReader Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |  2.995 ms |  1.00 | 20 | 6785.6 |  119.8 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  3.723 ms |  1.24 | 20 | 5457.7 |  148.9 |  16.94 KB |        4.44 |
| CsvHelper | Row    | 25000 |  3.796 ms |  1.27 | 20 | 5353.2 |  151.8 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Cols   | 25000 |  4.216 ms |  1.00 | 20 | 4819.5 |  168.6 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  6.327 ms |  1.50 | 20 | 3211.5 |  253.1 |  16.94 KB |        4.44 |
| CsvHelper | Cols   | 25000 |  5.068 ms |  1.20 | 20 | 4009.3 |  202.7 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Floats | 25000 | 29.990 ms |  1.00 | 20 |  677.5 | 1199.6 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 | 14.919 ms |  0.50 | 20 | 1362.0 |  596.8 |  57.13 KB |        9.76 |
| Sylvan___ | Floats | 25000 | 73.283 ms |  2.44 | 20 |  277.3 | 2931.3 |  18.62 KB |        3.18 |
| CsvHelper | Floats | 25000 | 34.922 ms |  1.16 | 20 |  581.9 | 1396.9 |   6.09 KB |        1.04 |

##### AMD.EPYC.9V45 - FloatsReader Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |  1.482 ms |  1.00 | 20 | 13675.4 |   59.3 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  2.571 ms |  1.74 | 20 |  7884.0 |  102.9 |  16.94 KB |        4.44 |
| CsvHelper | Row    | 25000 |  1.894 ms |  1.28 | 20 | 10703.5 |   75.8 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Cols   | 25000 |  1.999 ms |  1.00 | 20 | 10140.8 |   80.0 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  3.886 ms |  1.94 | 20 |  5217.3 |  155.4 |  16.94 KB |        4.44 |
| CsvHelper | Cols   | 25000 |  2.507 ms |  1.25 | 20 |  8086.0 |  100.3 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Floats | 25000 | 20.886 ms |  1.00 | 20 |   970.6 |  835.5 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  8.639 ms |  0.41 | 20 |  2346.7 |  345.5 |  54.08 KB |        9.24 |
| Sylvan___ | Floats | 25000 | 43.062 ms |  2.06 | 20 |   470.8 | 1722.5 |   18.6 KB |        3.18 |
| CsvHelper | Floats | 25000 | 21.695 ms |  1.04 | 20 |   934.4 |  867.8 |   6.09 KB |        1.04 |

##### AMD.Ryzen.9.9950X3D2 - FloatsReader Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope  | Rows  | Mean        | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |------------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |    796.0 μs |  1.00 | 20 | 25528.1 |   31.8 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1,660.5 μs |  2.09 | 20 | 12236.8 |   66.4 |  16.94 KB |        4.44 |
| CsvHelper | Row    | 25000 |  1,086.9 μs |  1.37 | 20 | 18694.3 |   43.5 |    4.3 KB |        1.13 |
|           |        |       |             |       |    |         |        |           |             |
| Sep______ | Cols   | 25000 |  1,227.9 μs |  1.00 | 20 | 16548.1 |   49.1 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2,583.2 μs |  2.10 | 20 |  7866.0 |  103.3 |  16.94 KB |        4.44 |
| CsvHelper | Cols   | 25000 |  1,581.5 μs |  1.29 | 20 | 12848.3 |   63.3 |    4.3 KB |        1.13 |
|           |        |       |             |       |    |         |        |           |             |
| Sep______ | Floats | 25000 | 15,489.5 μs |  1.00 | 20 |  1311.8 |  619.6 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  1,733.5 μs |  0.11 | 20 | 11721.6 |   69.3 | 169.55 KB |       28.97 |
| Sylvan___ | Floats | 25000 | 33,565.8 μs |  2.17 | 20 |   605.4 | 1342.6 |   18.6 KB |        3.18 |
| CsvHelper | Floats | 25000 | 15,621.0 μs |  1.01 | 20 |  1300.8 |  624.8 |   6.09 KB |        1.04 |

##### Apple.M1.(Virtual) - FloatsReader Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |  2.922 ms |  1.00 | 20 | 6937.3 |  116.9 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 | 22.341 ms |  7.70 | 20 |  907.4 |  893.6 |  16.55 KB |        4.34 |
| CsvHelper | Row    | 25000 |  4.940 ms |  1.70 | 20 | 4103.8 |  197.6 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Cols   | 25000 |  4.066 ms |  1.00 | 20 | 4985.8 |  162.6 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 | 24.689 ms |  6.10 | 20 |  821.1 |  987.6 |  16.56 KB |        4.34 |
| CsvHelper | Cols   | 25000 |  5.961 ms |  1.47 | 20 | 3401.0 |  238.4 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Floats | 25000 | 27.362 ms |  1.00 | 20 |  740.9 | 1094.5 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 | 13.097 ms |  0.48 | 20 | 1547.8 |  523.9 |  62.07 KB |       10.61 |
| Sylvan___ | Floats | 25000 | 71.263 ms |  2.63 | 20 |  284.5 | 2850.5 |  18.23 KB |        3.12 |
| CsvHelper | Floats | 25000 | 35.538 ms |  1.31 | 20 |  570.4 | 1421.5 |   6.09 KB |        1.04 |

##### Apple.M2.Pro.(Virtual) - FloatsReader Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |  2.486 ms |  1.00 | 20 | 8155.4 |   99.4 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 | 20.142 ms |  8.11 | 20 | 1006.5 |  805.7 |  16.55 KB |        4.34 |
| CsvHelper | Row    | 25000 |  4.407 ms |  1.77 | 20 | 4600.4 |  176.3 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Cols   | 25000 |  3.812 ms |  1.00 | 20 | 5318.0 |  152.5 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 | 22.395 ms |  5.88 | 20 |  905.2 |  895.8 |  16.56 KB |        4.34 |
| CsvHelper | Cols   | 25000 |  5.469 ms |  1.44 | 20 | 3707.0 |  218.7 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Floats | 25000 | 25.694 ms |  1.00 | 20 |  789.0 | 1027.8 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  6.853 ms |  0.27 | 20 | 2958.1 |  274.1 |  57.91 KB |        9.90 |
| Sylvan___ | Floats | 25000 | 67.617 ms |  2.63 | 20 |  299.8 | 2704.7 |  18.23 KB |        3.12 |
| CsvHelper | Floats | 25000 | 32.111 ms |  1.25 | 20 |  631.3 | 1284.4 |   6.09 KB |        1.04 |

##### Cobalt.100 - FloatsReader Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |  3.371 ms |  1.00 | 20 | 6028.4 |  134.8 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 | 23.235 ms |  6.90 | 20 |  874.5 |  929.4 |  16.56 KB |        4.34 |
| CsvHelper | Row    | 25000 |  8.040 ms |  2.39 | 20 | 2527.2 |  321.6 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Cols   | 25000 |  4.564 ms |  1.00 | 20 | 4452.0 |  182.6 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 | 26.026 ms |  5.70 | 20 |  780.7 | 1041.0 |  16.56 KB |        4.34 |
| CsvHelper | Cols   | 25000 |  9.191 ms |  2.01 | 20 | 2210.9 |  367.6 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Floats | 25000 | 30.027 ms |  1.00 | 20 |  676.7 | 1201.1 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  9.392 ms |  0.31 | 20 | 2163.5 |  375.7 |  52.53 KB |        8.98 |
| Sylvan___ | Floats | 25000 | 87.052 ms |  2.90 | 20 |  233.4 | 3482.1 |  18.24 KB |        3.12 |
| CsvHelper | Floats | 25000 | 38.649 ms |  1.29 | 20 |  525.8 | 1546.0 |   6.09 KB |        1.04 |

##### Neoverse-N2 - FloatsReader Benchmark Results (Sep 0.18.0.0, Sylvan  1.4.4.0, CsvHelper 34.0.0.29)

| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |  3.220 ms |  1.00 | 20 | 6295.9 |  128.8 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 | 22.748 ms |  7.06 | 20 |  891.2 |  909.9 |  16.56 KB |        4.34 |
| CsvHelper | Row    | 25000 |  7.512 ms |  2.33 | 20 | 2698.6 |  300.5 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Cols   | 25000 |  4.252 ms |  1.00 | 20 | 4767.9 |  170.1 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 | 25.762 ms |  6.06 | 20 |  786.9 | 1030.5 |  16.56 KB |        4.34 |
| CsvHelper | Cols   | 25000 |  8.503 ms |  2.00 | 20 | 2384.1 |  340.1 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Floats | 25000 | 31.538 ms |  1.00 | 20 |  642.8 | 1261.5 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  9.304 ms |  0.29 | 20 | 2178.9 |  372.1 |  51.25 KB |        8.76 |
| Sylvan___ | Floats | 25000 | 89.167 ms |  2.83 | 20 |  227.3 | 3566.7 |  18.24 KB |        3.12 |
| CsvHelper | Floats | 25000 | 38.162 ms |  1.21 | 20 |  531.2 | 1526.5 |   6.09 KB |        1.04 |


### Writer Comparison Benchmarks
Writer benchmarks are still pending, but Sep is unlikely to be the fastest here
since it is explicitly designed to make writing more convenient and flexible.
Still efficient, but not necessarily fastest. That is, Sep does not require
writing header up front and hence having to keep header column order and row
values column order the same. This means Sep does not write columns *directly*
upon definition but defers this until a new row has been fully defined and then
is ended.

## Example Catalogue
The following examples are available in [ReadMeTest.cs](src/Sep.XyzTest/ReadMeTest.cs).

### Example - Write and Read Objects with Escape/Unescape
```csharp
Person[] writePersons =
[
    new("Alice", new DateOnly(1990, 1, 1), "123 Main St, 1."),
    new("Bob", new DateOnly(1985, 5, 23), "456 Oak Ave"),
    new("Charlie", new DateOnly(2000, 12, 31), "789 Pine Rd, 3."),
];
// Write
using var writer = Sep.New(',').Writer().Strict().ToText();
foreach (var person in writePersons)
{
    using var row = writer.NewRow();
    row[nameof(person.Name)].Set(person.Name);
    row[nameof(person.BirthDay)].Format(person.BirthDay);
    row[nameof(person.Address)].Set(person.Address);
}
var text = writer.ToString();
// Read
using var reader = Sep.New(',').Reader().Strict().FromText(text);
var readPersons = reader.Enumerate<Person>(row =>
        new(Name: row[nameof(Person.Name)].ToString(),
            BirthDay: row[nameof(Person.BirthDay)].Parse<DateOnly>(),
            Address: row[nameof(Person.Address)].ToString()))
    .ToArray();
// Assert
Assert.AreEqual("""
                Name,BirthDay,Address
                Alice,01/01/1990,"123 Main St, 1."
                Bob,05/23/1985,456 Oak Ave
                Charlie,12/31/2000,"789 Pine Rd, 3."
                
                """, text);
CollectionAssert.AreEqual(writePersons, readPersons);
```
with `Person` defined as:
```csharp
record Person(string Name, DateOnly BirthDay, string Address);
```

### Example - Copy Rows
```csharp
var text = """
           A;B;C;D;E;F
           Sep;🚀;1;1.2;0.1;0.5
           CSV;✅;2;2.2;0.2;1.5
           
           """; // Empty line at end is for line ending

using var reader = Sep.Reader().FromText(text);
using var writer = reader.Spec.Writer().ToText();
foreach (var readRow in reader)
{
    using var writeRow = writer.NewRow(readRow);
}
Assert.AreEqual(text, writer.ToString());
```

### Example - Copy Rows (Async)
```csharp
var text = """
           A;B;C;D;E;F
           Sep;🚀;1;1.2;0.1;0.5
           CSV;✅;2;2.2;0.2;1.5
           
           """; // Empty line at end is for line ending

using var reader = await Sep.Reader().FromTextAsync(text);
await using var writer = reader.Spec.Writer().ToText();
await foreach (var readRow in reader)
{
    await using var writeRow = writer.NewRow(readRow);
}
Assert.AreEqual(text, writer.ToString());
```

### Example - Skip Empty Rows
```csharp
var text = """
           A
           1
           2

           3


           4
           
           """; // Empty line at end is for line ending
var expected = new[] { 1, 2, 3, 4 };

// Disable col count check to allow empty rows
using var reader = Sep.Reader(o => o with { DisableColCountCheck = true }).FromText(text);
var actual = new List<int>();
foreach (var row in reader)
{
    // Skip empty row
    if (row.Span.Length == 0) { continue; }

    actual.Add(row["A"].Parse<int>());
}
CollectionAssert.AreEqual(expected, actual);
```

### Example - Use Extension Method Enumerate within async/await Context (prior to C# 13.0)
Since `SepReader.Row` is a `ref struct` as covered above, one has to avoid
referencing it directly in async context for C# prior to 13.0. This can be done
in a number of ways, but one way is to use `Enumerate` extension method to
parse/extract data from row like shown below.

```csharp
var text = """
           C
           1
           2
           """;

using var reader = Sep.Reader().FromText(text);
var squaredSum = 0;
// Use Enumerate to avoid referencing SepReader.Row in async context
foreach (var value in reader.Enumerate(row => row["C"].Parse<int>()))
{
    squaredSum += await Task.Run(() => value * value);
}
Assert.AreEqual(5, squaredSum);
```

### Example - Use Local Function within async/await Context
Another way to avoid referencing `SepReader.Row` directly in async context is to
use custom iterator via `yield return` to parse/extract data from row like shown
below.

```csharp
var text = """
           C
           1
           2
           """;

using var reader = Sep.Reader().FromText(text);
var squaredSum = 0;
// Use custom local function Enumerate to avoid referencing
// SepReader.Row in async context
foreach (var value in Enumerate(reader))
{
    squaredSum += await Task.Run(() => value * value);
}
Assert.AreEqual(5, squaredSum);

static IEnumerable<int> Enumerate(SepReader reader)
{
    foreach (var r in reader) { yield return r["C"].Parse<int>(); }
}
```

### Example - Skip Lines/Rows Starting with Comment `#`

Below shows how one can skip lines starting with comment `#` since Sep does not
have built-in support for this. Note that this presumes lines to be skipped
before header do not contain quotes or rather line endings within quotes as that
is not handled by the `Peek()` skipping. The rows starting with comment `#`
after header are skipped if handling quoting is enabled in Sep options.

```csharp
var text = """
           # Comment 1
           # Comment 2
           A
           # Comment 3
           1
           2
           # Comment 4
           """;

const char Comment = '#';

using var textReader = new StringReader(text);
// Skip initial lines (not rows) before header
while (textReader.Peek() == Comment &&
       textReader.ReadLine() is string line) { }

using var reader = Sep.Reader().From(textReader);
var values = new List<int>();
foreach (var row in reader)
{
    // Skip rows starting with comment
    if (row.Span.StartsWith([Comment])) { continue; }

    var value = row["A"].Parse<int>();
    values.Add(value);
}
CollectionAssert.AreEqual(new int[] { 1, 2 }, values);
```

## RFC-4180
While the [RFC-4180](https://www.ietf.org/rfc/rfc4180.txt) requires `\r\n`
(CR,LF) as line ending, the well-known line endings (`\r\n`, `\n` and `\r`) are
supported similar to .NET. `Environment.NewLine` is used when writing. Quoting
is supported by simply matching pairs of quotes, no matter what.

Note that some libraries will claim conformance but the RFC is, perhaps
naturally, quite strict e.g. only comma is supported as separator/delimiter. Sep
defaults to using `;` as separator if writing, while auto-detecting supported
separators when reading. This is decidedly non-conforming.

The RFC defines the following condensed [ABNF
grammar](https://en.wikipedia.org/wiki/Augmented_Backus%E2%80%93Naur_form):
```text
file = [header CRLF] record *(CRLF record) [CRLF]
header = name *(COMMA name)
record = field *(COMMA field)
name = field
field = (escaped / non-escaped)
escaped = DQUOTE *(TEXTDATA / COMMA / CR / LF / 2DQUOTE) DQUOTE
non-escaped = *TEXTDATA
COMMA = %x2C
CR = %x0D ;as per section 6.1 of RFC 2234 [2]
DQUOTE =  %x22 ;as per section 6.1 of RFC 2234 [2]
LF = %x0A ;as per section 6.1 of RFC 2234 [2]
CRLF = CR LF ;as per section 6.1 of RFC 2234 [2]
TEXTDATA =  %x20-21 / %x23-2B / %x2D-7E
```
Note how `TEXTDATA` is restricted too, yet many will allow any character incl.
emojis or similar (which Sep supports), but is not in conformance with the RFC.

Quotes inside an escaped field e.g. `"fie""ld"` are only allowed to be double
quotes. Sep currently allows any pairs of quotes and quoting doesn't need to be
at start of or end of field (col or column in Sep terminology).

All in all Sep takes a pretty pragmatic approach here as the primary use case is
**not** exchanging data on the internet, but for use in machine learning
pipelines or similar.

## Frequently Asked Questions (FAQ)
Ask questions on GitHub and this section will be expanded. :)

* *Does Sep support [object mapping like
  CsvHelper](https://joshclose.github.io/CsvHelper/examples/reading/get-class-records/)?*
  No, Sep is a minimal library and does not support object mapping. First, this
  is usually supported via reflection, which Sep avoids. Second, object mapping
  often only works well in a few cases without actually writing custom mapping
  for each property, which then basically amounts to writing the parsing code
  yourself. If object mapping is a must have, consider writing your own [source
  generator](https://devblogs.microsoft.com/dotnet/new-c-source-generator-samples/)
  for it if you want to use Sep. Maybe some day Sep will have a built-in source
  generator, but not in the foreseeable future.

### SepReader FAQ

### SepWriter FAQ

## Links

### Sep

* [.NET Matrix - Sep](https://matrix.dev-team.org/?category=csv-processing&library=Sep)
* [Sep on NuGet](https://www.nuget.org/packages/Sep/)
* [Sep on GitHub](https://github.com/nietras/Sep)

### Blog posts

* [Introducing Sep - Possibly the World's Fastest .NET CSV
  Parser](https://nietras.com/2023/06/05/introducing-sep/)
* [Sep 0.2.0 - Even Faster Parsing (~10 GB/s on Zen 3) and
  More](https://nietras.com/2023/08/07/sep-0-2-0/)
* [Sep 0.3.0 - Unescape Support (still the Most Efficient .NET CSV
  Parser)](https://nietras.com/2023/11/27/sep-0-3-0/)
* [Sep 0.4.0-0.5.2 - Insanely Fast Single- & Multi-threaded .NET CSV
  Parsing](https://nietras.com/2024/05/05/sep-0-4-0-0-5-2/)
* [Sep 0.10.0 - 21 GB/s CSV Parsing Using SIMD on AMD
  9950X](https://nietras.com/2025/05/09/sep-0-10-0/)
* [Sep 0.11.0 - 9.5 GB/s CSV Parsing Using ARM NEON SIMD on Apple
  M1](https://nietras.com/2025/06/17/sep-0-11-0/)

### External

* [The fastest CSV parsers in .NET](https://www.joelverhagen.com/blog/2020/12/fastest-net-csv-parsers)
  and its [NCsvPerf benchmark](https://github.com/joelverhagen/NCsvPerf)
* [Using Sep to Export a CSV in Razor
  Pages](https://thomasryan.dev/2025/11/sep-export-csv-razor-pages.html)
* [From Atari to Sep: Unleashing the Power of Performance in
  Programming](https://dotnetcore.show/season-6/from-atari-to-sep-unleashing-the-power-of-performance-in-programming-with-niels-rassmussen/)
* [Publishing a NuGet package using GitHub and GitHub
  Actions](https://www.meziantou.net/publishing-a-nuget-package-following-best-practices-using-github.htm)

## Public API Reference
```csharp
[assembly: System.CLSCompliant(false)]
[assembly: System.Reflection.AssemblyMetadata("IsAotCompatible", "True")]
[assembly: System.Reflection.AssemblyMetadata("IsTrimmable", "True")]
[assembly: System.Reflection.AssemblyMetadata("RepositoryUrl", "https://github.com/nietras/Sep/")]
[assembly: System.Resources.NeutralResourcesLanguage("en")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Sep.Benchmarks")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Sep.ComparisonBenchmarks")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Sep.Test")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Sep.XyzTest")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v11.0", FrameworkDisplayName=".NET 11.0")]
namespace nietras.SeparatedValues
{
    public readonly struct Sep : System.IEquatable<nietras.SeparatedValues.Sep>
    {
        public Sep() { }
        public Sep(char separator) { }
        public char Separator { get; init; }
        public static nietras.SeparatedValues.Sep? Auto { get; }
        public static nietras.SeparatedValues.Sep Default { get; }
        public static nietras.SeparatedValues.Sep New(char separator) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader() { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(System.Func<nietras.SeparatedValues.SepReaderOptions, nietras.SeparatedValues.SepReaderOptions> configure) { }
        public static nietras.SeparatedValues.SepWriterOptions Writer() { }
        public static nietras.SeparatedValues.SepWriterOptions Writer(System.Func<nietras.SeparatedValues.SepWriterOptions, nietras.SeparatedValues.SepWriterOptions> configure) { }
    }
    public enum SepColNotSetOption : byte
    {
        Throw = 0,
        Empty = 1,
        Skip = 2,
    }
    public delegate nietras.SeparatedValues.SepToString SepCreateToString(nietras.SeparatedValues.SepReaderHeader? maybeHeader, int colCount);
    public static class SepDefaults
    {
        public static System.StringComparer ColNameComparer { get; }
        public static System.Globalization.CultureInfo CultureInfo { get; }
        public static char Separator { get; }
    }
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class SepReader : nietras.SeparatedValues.SepReaderState, System.Collections.Generic.IAsyncEnumerable<nietras.SeparatedValues.SepReader.Row>, System.Collections.Generic.IEnumerable<nietras.SeparatedValues.SepReader.Row>, System.Collections.Generic.IEnumerator<nietras.SeparatedValues.SepReader.Row>, System.Collections.IEnumerable, System.Collections.IEnumerator, System.IDisposable
    {
        public nietras.SeparatedValues.SepReader.Row Current { get; }
        public bool HasHeader { get; }
        public bool HasRows { get; }
        public nietras.SeparatedValues.SepReaderHeader Header { get; }
        public bool IsEmpty { get; }
        public nietras.SeparatedValues.SepSpec Spec { get; }
        public nietras.SeparatedValues.SepReader.AsyncEnumerator GetAsyncEnumerator(System.Threading.CancellationToken cancellationToken = default) { }
        public nietras.SeparatedValues.SepReader GetEnumerator() { }
        public bool MoveNext() { }
        public System.Threading.Tasks.ValueTask<bool> MoveNextAsync(System.Threading.CancellationToken cancellationToken = default) { }
        public string ToString(int index) { }
        public readonly struct AsyncEnumerator : System.Collections.Generic.IAsyncEnumerator<nietras.SeparatedValues.SepReader.Row>, System.IAsyncDisposable
        {
            public nietras.SeparatedValues.SepReader.Row Current { get; }
            public System.Threading.Tasks.ValueTask DisposeAsync() { }
            public System.Threading.Tasks.ValueTask<bool> MoveNextAsync() { }
        }
        [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay}")]
        public readonly ref struct Col
        {
            public System.ReadOnlySpan<char> Span { get; }
            public T Parse<T>()
                where T : System.ISpanParsable<T> { }
            public T ParseEnum<T>()
                where T :  struct, System.Enum { }
            public override string ToString() { }
            public T? TryParse<T>()
                where T :  struct, System.ISpanParsable<T> { }
            public bool TryParse<T>(out T value)
                where T : System.ISpanParsable<T> { }
            public bool TryParseEnum<T>(out T value)
                where T :  struct, System.Enum { }
            public static System.ReadOnlySpan<char> op_Implicit(nietras.SeparatedValues.SepReader.Col col) { }
        }
        public readonly ref struct Cols
        {
            public int Count { get; }
            public nietras.SeparatedValues.SepReader.Col this[int index] { get; }
            public string CombinePathsToString() { }
            public System.ReadOnlySpan<char> Join(System.ReadOnlySpan<char> separator) { }
            public string JoinPathsToString() { }
            public string JoinToString(System.ReadOnlySpan<char> separator) { }
            public System.Span<T> Parse<T>()
                where T : System.ISpanParsable<T> { }
            public void Parse<T>(System.Span<T> span)
                where T : System.ISpanParsable<T> { }
            public T[] ParseToArray<T>()
                where T : System.ISpanParsable<T> { }
            public System.Span<T> Select<T>(method selector) { }
            public System.Span<T> Select<T>(nietras.SeparatedValues.SepReader.ColFunc<T> selector) { }
            public System.Span<string> ToStrings() { }
            public string[] ToStringsArray() { }
            public System.Span<T?> TryParse<T>()
                where T :  struct, System.ISpanParsable<T> { }
            public void TryParse<T>(System.Span<T?> span)
                where T :  struct, System.ISpanParsable<T> { }
        }
        [System.Diagnostics.DebuggerDisplay("{DebuggerDisplayPrefix,nq}{Span}")]
        [System.Diagnostics.DebuggerTypeProxy(typeof(nietras.SeparatedValues.SepReader.Row.DebugView))]
        public readonly ref struct Row
        {
            public int ColCount { get; }
            public nietras.SeparatedValues.SepReader.Col this[int index] { get; }
            public nietras.SeparatedValues.SepReader.Col this[System.Index index] { get; }
            public nietras.SeparatedValues.SepReader.Col this[string colName] { get; }
            public nietras.SeparatedValues.SepReader.Cols this[System.Range range] { get; }
            public nietras.SeparatedValues.SepReader.Cols this[System.ReadOnlySpan<int> indices] { get; }
            public nietras.SeparatedValues.SepReader.Cols this[System.Collections.Generic.IReadOnlyList<int> indices] { get; }
            public nietras.SeparatedValues.SepReader.Cols this[int[] indices] { get; }
            public nietras.SeparatedValues.SepReader.Cols this[System.ReadOnlySpan<string> colNames] { get; }
            public nietras.SeparatedValues.SepReader.Cols this[System.Collections.Generic.IReadOnlyList<string> colNames] { get; }
            public nietras.SeparatedValues.SepReader.Cols this[string[] colNames] { get; }
            public int LineNumberFrom { get; }
            public int LineNumberToExcl { get; }
            public int RowIndex { get; }
            public System.ReadOnlySpan<char> Span { get; }
            public System.Func<int, string> UnsafeToStringDelegate { get; }
            public override string ToString() { }
            public bool TryGet(string colName, out nietras.SeparatedValues.SepReader.Col col) { }
        }
        public delegate void ColAction(nietras.SeparatedValues.SepReader.Col col);
        public delegate T ColFunc<T>(nietras.SeparatedValues.SepReader.Col col);
        public delegate void ColsAction(nietras.SeparatedValues.SepReader.Cols cols);
        public delegate void RowAction(nietras.SeparatedValues.SepReader.Row row);
        public delegate T RowFunc<T>(nietras.SeparatedValues.SepReader.Row row);
        public delegate bool RowTryFunc<T>(nietras.SeparatedValues.SepReader.Row row, out T value);
    }
    public static class SepReaderExtensions
    {
        public static System.Collections.Generic.IEnumerable<T> Enumerate<T>(this nietras.SeparatedValues.SepReader reader, nietras.SeparatedValues.SepReader.RowFunc<T> select) { }
        public static System.Collections.Generic.IEnumerable<T> Enumerate<T>(this nietras.SeparatedValues.SepReader reader, nietras.SeparatedValues.SepReader.RowTryFunc<T> trySelect) { }
        public static System.Collections.Generic.IAsyncEnumerable<T> EnumerateAsync<T>(this nietras.SeparatedValues.SepReader reader, nietras.SeparatedValues.SepReader.RowFunc<T> select) { }
        public static System.Collections.Generic.IAsyncEnumerable<T> EnumerateAsync<T>(this nietras.SeparatedValues.SepReader reader, nietras.SeparatedValues.SepReader.RowTryFunc<T> trySelect) { }
        public static nietras.SeparatedValues.SepReader From(this in nietras.SeparatedValues.SepReaderOptions options, System.IO.Stream stream) { }
        public static nietras.SeparatedValues.SepReader From(this in nietras.SeparatedValues.SepReaderOptions options, System.IO.TextReader reader) { }
        public static nietras.SeparatedValues.SepReader From(this in nietras.SeparatedValues.SepReaderOptions options, byte[] buffer) { }
        public static nietras.SeparatedValues.SepReader From(this in nietras.SeparatedValues.SepReaderOptions options, System.IO.Stream stream, bool leaveOpen) { }
        public static nietras.SeparatedValues.SepReader From(this in nietras.SeparatedValues.SepReaderOptions options, System.IO.TextReader reader, bool leaveOpen) { }
        public static nietras.SeparatedValues.SepReader From(this in nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.Stream> nameToStream) { }
        public static nietras.SeparatedValues.SepReader From(this in nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.TextReader> nameToReader) { }
        public static nietras.SeparatedValues.SepReader From(this in nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.Stream> nameToStream, bool leaveOpen) { }
        public static nietras.SeparatedValues.SepReader From(this in nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.TextReader> nameToReader, bool leaveOpen) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, System.IO.Stream stream, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, System.IO.TextReader reader, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, byte[] buffer, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, System.IO.Stream stream, bool leaveOpen, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, System.IO.TextReader reader, bool leaveOpen, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.Stream> nameToStream, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.TextReader> nameToReader, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.Stream> nameToStream, bool leaveOpen, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.TextReader> nameToReader, bool leaveOpen, System.Threading.CancellationToken cancellationToken = default) { }
        public static nietras.SeparatedValues.SepReader FromFile(this in nietras.SeparatedValues.SepReaderOptions options, string filePath) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromFileAsync(this nietras.SeparatedValues.SepReaderOptions options, string filePath, System.Threading.CancellationToken cancellationToken = default) { }
        public static nietras.SeparatedValues.SepReader FromText(this in nietras.SeparatedValues.SepReaderOptions options, string text) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromTextAsync(this nietras.SeparatedValues.SepReaderOptions options, string text, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Collections.Generic.IEnumerable<T> ParallelEnumerate<T>(this nietras.SeparatedValues.SepReader reader, nietras.SeparatedValues.SepReader.RowFunc<T> select) { }
        public static System.Collections.Generic.IEnumerable<T> ParallelEnumerate<T>(this nietras.SeparatedValues.SepReader reader, nietras.SeparatedValues.SepReader.RowTryFunc<T> trySelect) { }
        public static System.Collections.Generic.IEnumerable<T> ParallelEnumerate<T>(this nietras.SeparatedValues.SepReader reader, nietras.SeparatedValues.SepReader.RowFunc<T> select, int degreeOfParallelism) { }
        public static System.Collections.Generic.IEnumerable<T> ParallelEnumerate<T>(this nietras.SeparatedValues.SepReader reader, nietras.SeparatedValues.SepReader.RowTryFunc<T> trySelect, int degreeOfParallelism) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.Sep sep) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.Sep? sep) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.SepSpec spec) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.Sep sep, System.Func<nietras.SeparatedValues.SepReaderOptions, nietras.SeparatedValues.SepReaderOptions> configure) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.Sep? sep, System.Func<nietras.SeparatedValues.SepReaderOptions, nietras.SeparatedValues.SepReaderOptions> configure) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.SepSpec spec, System.Func<nietras.SeparatedValues.SepReaderOptions, nietras.SeparatedValues.SepReaderOptions> configure) { }
        public static nietras.SeparatedValues.SepReaderOptions Strict(this in nietras.SeparatedValues.SepReaderOptions options) { }
    }
    public sealed class SepReaderHeader
    {
        public System.Collections.Generic.IReadOnlyList<string> ColNames { get; }
        public bool IsEmpty { get; }
        public static nietras.SeparatedValues.SepReaderHeader Empty { get; }
        public int IndexOf(System.ReadOnlySpan<char> colName) { }
        public int IndexOf(string colName) { }
        public int[] IndicesOf(System.Collections.Generic.IReadOnlyList<string> colNames) { }
        public int[] IndicesOf([System.Runtime.CompilerServices.ParamCollection] [System.Runtime.CompilerServices.ScopedRef] System.ReadOnlySpan<string> colNames) { }
        public int[] IndicesOf(params string[] colNames) { }
        public void IndicesOf(System.ReadOnlySpan<string> colNames, System.Span<int> colIndices) { }
        public System.Collections.Generic.IReadOnlyList<string> NamesStartingWith(string prefix, System.StringComparison comparison = 4) { }
        public override string ToString() { }
        public bool TryIndexOf(System.ReadOnlySpan<char> colName, out int colIndex) { }
        public bool TryIndexOf(string colName, out int colIndex) { }
    }
    public readonly struct SepReaderOptions : System.IEquatable<nietras.SeparatedValues.SepReaderOptions>
    {
        public SepReaderOptions() { }
        public SepReaderOptions(nietras.SeparatedValues.Sep? sep) { }
        public bool AsyncContinueOnCapturedContext { get; init; }
        public System.Collections.Generic.IEqualityComparer<string> ColNameComparer { get; init; }
        public nietras.SeparatedValues.SepCreateToString CreateToString { get; init; }
        public System.Globalization.CultureInfo? CultureInfo { get; init; }
        public bool DisableColCountCheck { get; init; }
        public bool DisableFastFloat { get; init; }
        public bool DisableQuotesParsing { get; init; }
        public bool HasHeader { get; init; }
        public int InitialBufferLength { get; init; }
        public nietras.SeparatedValues.Sep? Sep { get; init; }
        public nietras.SeparatedValues.SepTrim Trim { get; init; }
        public bool Unescape { get; init; }
    }
    public class SepReaderState : System.IDisposable
    {
        public void Dispose() { }
    }
    public static class SepReaderWriterExtensions
    {
        public static void CopyTo(this nietras.SeparatedValues.SepReader.Row readerRow, nietras.SeparatedValues.SepWriter.Row writerRow) { }
        public static nietras.SeparatedValues.SepWriter.Row NewRow(this nietras.SeparatedValues.SepWriter writer, nietras.SeparatedValues.SepReader.Row rowToCopy) { }
        public static nietras.SeparatedValues.SepWriter.Row NewRow(this nietras.SeparatedValues.SepWriter writer, nietras.SeparatedValues.SepReader.Row rowToCopy, System.Threading.CancellationToken cancellationToken) { }
    }
    public readonly struct SepSpec : System.IEquatable<nietras.SeparatedValues.SepSpec>
    {
        public SepSpec() { }
        public SepSpec(nietras.SeparatedValues.Sep sep, System.Globalization.CultureInfo? cultureInfo) { }
        public SepSpec(nietras.SeparatedValues.Sep sep, System.Globalization.CultureInfo? cultureInfo, bool asyncContinueOnCapturedContext) { }
        public bool AsyncContinueOnCapturedContext { get; init; }
        public System.Globalization.CultureInfo? CultureInfo { get; init; }
        public nietras.SeparatedValues.Sep Sep { get; init; }
    }
    public abstract class SepToString : System.IDisposable
    {
        protected SepToString() { }
        public virtual bool IsThreadSafe { get; }
        public static nietras.SeparatedValues.SepCreateToString Direct { get; }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        public abstract string ToString(System.ReadOnlySpan<char> colSpan, int colIndex);
        public static nietras.SeparatedValues.SepCreateToString OnePool(int maximumStringLength = 32, int initialCapacity = 64, int maximumCapacity = 4096) { }
        public static nietras.SeparatedValues.SepCreateToString PoolPerCol(int maximumStringLength = 32, int initialCapacity = 64, int maximumCapacity = 4096) { }
        public static nietras.SeparatedValues.SepCreateToString PoolPerColThreadSafe(int maximumStringLength = 32, int initialCapacity = 64, int maximumCapacity = 4096) { }
        public static nietras.SeparatedValues.SepCreateToString PoolPerColThreadSafeFixedCapacity(int maximumStringLength = 32, int capacity = 2048) { }
    }
    [System.Flags]
    public enum SepTrim : byte
    {
        None = 0,
        Outer = 1,
        AfterUnescape = 2,
        All = 3,
    }
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class SepWriter : System.IAsyncDisposable, System.IDisposable
    {
        public nietras.SeparatedValues.SepWriterHeader Header { get; }
        public nietras.SeparatedValues.SepSpec Spec { get; }
        public void Dispose() { }
        public System.Threading.Tasks.ValueTask DisposeAsync() { }
        public void Flush() { }
        public System.Threading.Tasks.Task FlushAsync(System.Threading.CancellationToken cancellationToken = default) { }
        public nietras.SeparatedValues.SepWriter.Row NewRow() { }
        public nietras.SeparatedValues.SepWriter.Row NewRow(System.Threading.CancellationToken cancellationToken) { }
        public override string ToString() { }
        public readonly ref struct Col
        {
            public void Format<T>(T value)
                where T : System.ISpanFormattable { }
            public void Format<T>(T value, System.ReadOnlySpan<char> format)
                where T : System.ISpanFormattable { }
            public void FormatEnum<T>(T value)
                where T :  struct, System.Enum { }
            public void FormatEnum<T>(T value, System.ReadOnlySpan<char> format)
                where T :  struct, System.Enum { }
            public void Set(System.ReadOnlySpan<byte> utf8Span) { }
            public void Set(System.ReadOnlySpan<char> span) { }
            public void Set([System.Runtime.CompilerServices.InterpolatedStringHandlerArgument("")] ref nietras.SeparatedValues.SepWriter.Col.FormatInterpolatedStringHandler handler) { }
            public void Set(System.IFormatProvider? provider, [System.Runtime.CompilerServices.InterpolatedStringHandlerArgument(new string?[]?[] {
                    "",
                    "provider"})] ref nietras.SeparatedValues.SepWriter.Col.FormatInterpolatedStringHandler handler) { }
            [System.Runtime.CompilerServices.InterpolatedStringHandler]
            public ref struct FormatInterpolatedStringHandler
            {
                public FormatInterpolatedStringHandler(int literalLength, int formattedCount, nietras.SeparatedValues.SepWriter.Col col) { }
                public FormatInterpolatedStringHandler(int literalLength, int formattedCount, nietras.SeparatedValues.SepWriter.Col col, System.IFormatProvider? provider) { }
                public void AppendFormatted(System.ReadOnlySpan<char> value) { }
                public void AppendFormatted(string? value) { }
                public void AppendFormatted(System.ReadOnlySpan<char> value, int alignment = 0, string? format = null) { }
                public void AppendFormatted(object? value, int alignment = 0, string? format = null) { }
                public void AppendFormatted(string? value, int alignment = 0, string? format = null) { }
                public void AppendFormatted<T>(T value) { }
                public void AppendFormatted<T>(T value, int alignment) { }
                public void AppendFormatted<T>(T value, string? format) { }
                public void AppendFormatted<T>(T value, int alignment, string? format) { }
                public void AppendLiteral(string value) { }
            }
        }
        public readonly ref struct Cols
        {
            public int Count { get; }
            public nietras.SeparatedValues.SepWriter.Col this[int colIndex] { get; }
            public void Format<T>(System.Collections.Generic.IReadOnlyList<T> values)
                where T : System.ISpanFormattable { }
            public void Format<T>([System.Runtime.CompilerServices.ParamCollection] [System.Runtime.CompilerServices.ScopedRef] System.ReadOnlySpan<T> values)
                where T : System.ISpanFormattable { }
            public void Format<T>(System.Span<T> values)
                where T : System.ISpanFormattable { }
            public void Format<T>(T[] values)
                where T : System.ISpanFormattable { }
            public void Format<T>(System.ReadOnlySpan<T> values, nietras.SeparatedValues.SepWriter.ColAction<T> format) { }
            public void Set(System.Collections.Generic.IReadOnlyList<string> values) { }
            public void Set([System.Runtime.CompilerServices.ParamCollection] [System.Runtime.CompilerServices.ScopedRef] System.ReadOnlySpan<string> values) { }
            public void Set(nietras.SeparatedValues.SepReader.Cols cols) { }
            public void Set(string[] values) { }
        }
        public ref struct Row : System.IAsyncDisposable, System.IDisposable
        {
            public nietras.SeparatedValues.SepWriter.Col this[int colIndex] { get; }
            public nietras.SeparatedValues.SepWriter.Col this[string colName] { get; }
            public nietras.SeparatedValues.SepWriter.Cols this[System.ReadOnlySpan<int> indices] { get; }
            public nietras.SeparatedValues.SepWriter.Cols this[System.ReadOnlySpan<string> colNames] { get; }
            public nietras.SeparatedValues.SepWriter.Cols this[System.Collections.Generic.IReadOnlyList<string> colNames] { get; }
            public nietras.SeparatedValues.SepWriter.Cols this[string[] colNames] { get; }
            public void Dispose() { }
            public System.Threading.Tasks.ValueTask DisposeAsync() { }
        }
        public delegate void ColAction(nietras.SeparatedValues.SepWriter.Col col);
        public delegate void ColAction<T>(nietras.SeparatedValues.SepWriter.Col col, T value);
        public delegate void RowAction(nietras.SeparatedValues.SepWriter.Row row);
    }
    public static class SepWriterExtensions
    {
        public static nietras.SeparatedValues.SepWriterOptions Strict(this in nietras.SeparatedValues.SepWriterOptions options) { }
        public static nietras.SeparatedValues.SepWriter To(this in nietras.SeparatedValues.SepWriterOptions options, System.IO.Stream stream) { }
        public static nietras.SeparatedValues.SepWriter To(this in nietras.SeparatedValues.SepWriterOptions options, System.IO.TextWriter writer) { }
        public static nietras.SeparatedValues.SepWriter To(this in nietras.SeparatedValues.SepWriterOptions options, System.Text.StringBuilder stringBuilder) { }
        public static nietras.SeparatedValues.SepWriter To(this in nietras.SeparatedValues.SepWriterOptions options, System.IO.Stream stream, bool leaveOpen) { }
        public static nietras.SeparatedValues.SepWriter To(this in nietras.SeparatedValues.SepWriterOptions options, System.IO.TextWriter writer, bool leaveOpen) { }
        public static nietras.SeparatedValues.SepWriter To(this in nietras.SeparatedValues.SepWriterOptions options, string name, System.Func<string, System.IO.Stream> nameToStream, bool leaveOpen = false) { }
        public static nietras.SeparatedValues.SepWriter To(this in nietras.SeparatedValues.SepWriterOptions options, string name, System.Func<string, System.IO.TextWriter> nameToWriter, bool leaveOpen = false) { }
        public static nietras.SeparatedValues.SepWriter ToFile(this in nietras.SeparatedValues.SepWriterOptions options, string filePath) { }
        public static nietras.SeparatedValues.SepWriter ToText(this in nietras.SeparatedValues.SepWriterOptions options) { }
        public static nietras.SeparatedValues.SepWriter ToText(this in nietras.SeparatedValues.SepWriterOptions options, int capacity) { }
        public static nietras.SeparatedValues.SepWriterOptions Writer(this nietras.SeparatedValues.Sep sep) { }
        public static nietras.SeparatedValues.SepWriterOptions Writer(this nietras.SeparatedValues.SepSpec spec) { }
        public static nietras.SeparatedValues.SepWriterOptions Writer(this nietras.SeparatedValues.Sep sep, System.Func<nietras.SeparatedValues.SepWriterOptions, nietras.SeparatedValues.SepWriterOptions> configure) { }
        public static nietras.SeparatedValues.SepWriterOptions Writer(this nietras.SeparatedValues.SepSpec spec, System.Func<nietras.SeparatedValues.SepWriterOptions, nietras.SeparatedValues.SepWriterOptions> configure) { }
    }
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    [System.Diagnostics.DebuggerTypeProxy(typeof(nietras.SeparatedValues.SepWriterHeader.DebugView))]
    public sealed class SepWriterHeader
    {
        public void Add(System.Collections.Generic.IReadOnlyList<string> colNames) { }
        public void Add([System.Runtime.CompilerServices.ParamCollection] [System.Runtime.CompilerServices.ScopedRef] System.ReadOnlySpan<string> colNames) { }
        public void Add(string colName) { }
        public void Add(string[] colNames) { }
        public bool Contains(string colName) { }
        public bool TryAdd(string colName) { }
        public void Write() { }
        public System.Threading.Tasks.ValueTask WriteAsync(System.Threading.CancellationToken cancellationToken = default) { }
    }
    public readonly struct SepWriterOptions : System.IEquatable<nietras.SeparatedValues.SepWriterOptions>
    {
        public SepWriterOptions() { }
        public SepWriterOptions(nietras.SeparatedValues.Sep sep) { }
        public bool AsyncContinueOnCapturedContext { get; init; }
        public nietras.SeparatedValues.SepColNotSetOption ColNotSetOption { get; init; }
        public System.Globalization.CultureInfo? CultureInfo { get; init; }
        public bool DisableColCountCheck { get; init; }
        public bool Escape { get; init; }
        public nietras.SeparatedValues.Sep Sep { get; init; }
        public bool WriteHeader { get; init; }
    }
}
```
