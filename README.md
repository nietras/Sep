<img src="https://raw.githubusercontent.com/nietras/Sep/icon-v2/Icon.png"
     alt="Icon" align="right" width="128" hspace="2" vspace="2" />

# Sep - the World's Fastest .NET CSV Parser
![.NET](https://img.shields.io/badge/net8.0%20net9.0%20net10.0-5C2D91?logo=.NET&labelColor=gray)
![C#](https://img.shields.io/badge/C%23-14.0-239120?labelColor=gray)
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

Modern, minimal, fast, zero allocation, reading and writing of separated values
(`csv`, `tsv` etc.). Cross-platform, trimmable and AOT/NativeAOT compatible.
Featuring an opinionated API design and pragmatic implementation targetted at
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

**🌪️ Multi-threaded** - unparalleled speed with highly efficient parallel CSV
parsing that is [up to 35x faster than
CsvHelper](#floats-reader-comparison-benchmarks), see
[ParallelEnumerate](#parallelenumerate-and-enumerate) and
[benchmarks](#comparison-benchmarks).

**🌀 Async support** - efficient `ValueTask` based `async/await` support.
Requires C# 13.0+ and for .NET 9.0+ includes `SepReader` implementing
`IAsyncEnumerable<>`. See [Async Support](#async-support) for details.

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

using var reader = Sep.Reader().FromText(text);   // Infers separator 'Sep' from header
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
fascillitate easy flow of the separator and `CultureInfo` both `SepReader` and
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
    var colNames = header.NamesStarting("GT_");
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
/// cref="SepReader.Row.Span" /> will be modified and contain "garbage"
/// state after unescaped cols before next col. This is for efficiency to
/// avoid allocating secondary memory for unescaped columns. Header
/// columns/names will also be unescaped.
/// Requires <see cref="DisableQuotesParsing"/> to be false.
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
/// avoid allocating secondary memory for trimmed/unescaped columns. Header
/// columns/names will also be trimmed. Note that only the space ` ` (ASCII
/// 32) character is trimmed, not any whitespace character.
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
| `"a"a"a"` | False | EXCEPTION | `aa"a"` | `a"a"a` | `aa"a` |
| `""·` | False | EXCEPTION | `·` | `"` | `·` |
| `"a"·` | False | EXCEPTION | `a·` | `a"` | `a·` |
| `"a"""a` | False | EXCEPTION | `aa` | EXCEPTION | `a"a` |
| `"a"""a"` | False | EXCEPTION | `aa"` | `a"a<NULL>` | `a"a"` |
| `""a"` | False | EXCEPTION | `a"` | `"a` | `a"` |
| `"a"a"` | False | EXCEPTION | `aa"` | `a"a` | `aa"` |
| `""a"a""` | False | EXCEPTION | `a"a""` | `"a"a"` | `a"a"` |
| `"""` | False | | | EXCEPTION | `"` |
| `"""""` | False | `"` | `"` | EXCEPTION | `""` |

`·` (middle dot) is whitespace to make this visible

¹ CsvHelper with `BadDataFound = null`

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

¹ CsvHelper with `TrimOptions.Trim | TrimOptions.InsideQuotes`

² Sep with `SepTrim.All = SepTrim.Outer | SepTrim.AfterUnescape` in
`SepReaderOptions`

#### SepReader Debuggability
Debuggability is an important part of any library and while this is still a work
in progress for Sep, `SepReader` does have a unique feature when looking at it
and it's row or cols in a debug context. Given the below example code:
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
all functionality directly and hence only let's you access the current row and
cols on that. This API, however, is in this authors opinion not ideal and can be
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

To accomodate this Sep provides an overload for `Enumerate` that is similar to:
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

`ParallelEnumerate` is build on top of LINQ `AsParallel().AsOrdered()` and will
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
 1. MISSING: `SepWriter` currently does not allow you to define the header up
    front. Instead, header is defined by the order in which column names are
    accessed/created when defining the row.
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
/// Specifies the culture used for parsing. 
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
/// feed (`\n` or quote (`"`) then the column 
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
results as other popular CSV librares as shown below. Although, CsvHelper
appears to be escaping spaces as well, which is not necessary.

| Input | CsvHelper | Sylvan | Sep¹ |
|-|-|-|-|
| `` | | | |
| `·` | `"·"` | `·` | `·` |
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

* [CsvHelper](https://github.com/JoshClose/csvhelper) - *the* most commonly
   used CSV library with a staggering
   ![downloads](https://img.shields.io/nuget/dt/csvhelper) downloads on NuGet. Fully
   featured and battle tested.
* [Sylvan](https://github.com/MarkPflug/Sylvan) - is well-known and has
   previously been shown to be [the fastest CSV libraries for
   parsing](https://www.joelverhagen.com/blog/2020/12/fastest-net-csv-parsers)
   (Sep changes that 😉).
* `ReadLine`/`WriteLine` - basic naive implementations that read line by line
   and split on separator. While writing columns, separators and line endings
   directly. Does not handle quotes or similar correctly.

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
> into account backwards compatibility and still supporting older runtimes, so
> may not be able to easily utilize more recent features. Same goes for Sylvan.
> Additionally, Sep has a different feature set compared to the two. Performance
> is a feature, but not the only feature. Keep that in mind when evaluating
> results.

### Runtime and Platforms
The following runtime is used for benchmarking:

* `NET 9.0.X`

NOTE: [Garbage Collection
DATAS](https://maoni0.medium.com/dynamically-adapting-to-application-sizes-2d72fcb6f1ea)
mode is disabled since this severely impacts (e.g. [1.7x
slower](https://github.com/dotnet/runtime/issues/109047)) performance for some
benchmarks due to the bursty accumulated allocations. That is,
`GarbageCollectionAdaptationMode` is set to `0`.

The following platforms are used for benchmarking:

* `AMD EPYC 7763` (Virtual) X64 Platform Information
  ```ini
  OS=Ubuntu 22.04.5 LTS (Jammy Jellyfish)
  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
  ```
* `AMD Ryzen 7 PRO 7840U` (Laptop on battery) X64 Platform Information
  ```ini
  OS=Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
  AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
  ```
* `AMD 5950X` (Desktop) X64 Platform Information (no longer available)
  ```ini
  OS=Windows 10 (10.0.19044.2846/21H2/November2021Update)
  AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
  ```
* `AMD 9950X` (Desktop) X64 Platform Information
  ```ini
  OS=Windows 10 (10.0.19044.3086/21H2/November2021Update)
  AMD Ryzen 9 9950X, 1 CPU, 32 logical and 16 physical cores
  ```
* `Apple M1` (Virtual) ARM64 Platform Information
  ```ini
  OS=macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
  ```

### Reader Comparison Benchmarks
The following reader scenarios are benchmarked:

* [NCsvPerf](https://github.com/joelverhagen/NCsvPerf) from [The fastest CSV
   parser in
   .NET](https://www.joelverhagen.com/blog/2020/12/fastest-net-csv-parsers)
* [**Floats**](#floats-reader-comparison-benchmarks) as for example in machine learning.

Details for each can be found in the following. However, for each of these 3
different scopes are benchmarked to better assertain the low-level performance
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
   benchmark which in [Joel Verhagen](https://twitter.com/joelverhagen) own
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
slight cost no matter what, most notably for the `Cols` scope. Sep is still the
fastest of all (by far in many cases).

##### PackageAssets Benchmark Results
The results below show Sep is **the fastest .NET CSV Parser** (for this
benchmark on these platforms and machines 😀). While for pure parsing allocating
only a fraction of the memory due to extensive use of pooling and the
`ArrayPool<T>`.

This is in many aspects due to Sep having extremely optimized string pooling and
optimized hashing of `ReadOnlySpan<char>`, and thus not really due the the
csv-parsing itself, since that is not a big part of the time consumed. At least
not for a decently fast csv-parser.

With `ParallelEnumerate` (MT) Sep is **>2x faster than Sylvan and up to 9x
faster than CsvHelper**.

At the lowest level of enumerating rows only, that is csv parsing only, **Sep
hits a staggering 21 GB/s on 9950X**. Single-threaded.

###### AMD.EPYC.7763 - PackageAssets Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.531 ms |  1.00 |  29 | 8236.4 |   70.6 |       1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.778 ms |  1.07 |  29 | 7698.2 |   75.6 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.520 ms |  1.00 |  29 | 8263.0 |   70.4 |       1.13 KB |        1.12 |
| Sylvan___    | Row   | 50000   |     4.298 ms |  1.22 |  29 | 6766.8 |   86.0 |       7.65 KB |        7.59 |
| ReadLine_    | Row   | 50000   |    21.890 ms |  6.20 |  29 | 1328.7 |  437.8 |   88608.23 KB |   87,921.34 |
| CsvHelper    | Row   | 50000   |    64.318 ms | 18.22 |  29 |  452.2 | 1286.4 |      19.95 KB |       19.79 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.921 ms |  1.00 |  29 | 5911.1 |   98.4 |       1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.793 ms |  1.18 |  29 | 5020.8 |  115.9 |       1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.024 ms |  1.63 |  29 | 3625.1 |  160.5 |       7.65 KB |        7.59 |
| ReadLine_    | Cols  | 50000   |    23.332 ms |  4.74 |  29 | 1246.6 |  466.6 |   88608.23 KB |   87,921.34 |
| CsvHelper    | Cols  | 50000   |   101.802 ms | 20.69 |  29 |  285.7 | 2036.0 |      445.6 KB |      442.15 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    39.836 ms |  1.00 |  29 |  730.1 |  796.7 |   13803.05 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    29.563 ms |  0.74 |  29 |  983.9 |  591.3 |   13875.06 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    47.912 ms |  1.20 |  29 |  607.1 |  958.2 |   13962.03 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   131.577 ms |  3.31 |  29 |  221.1 | 2631.5 |  102133.63 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   122.181 ms |  3.07 |  29 |  238.1 | 2443.6 |   13971.42 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   864.069 ms |  1.00 | 581 |  673.4 |  864.1 |  266667.16 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   516.021 ms |  0.60 | 581 | 1127.6 |  516.0 |  276325.84 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,011.286 ms |  1.17 | 581 |  575.4 | 1011.3 |  266824.48 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,850.125 ms |  3.30 | 581 |  204.2 | 2850.1 | 2038834.76 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,579.496 ms |  2.99 | 581 |  225.6 | 2579.5 |  266840.54 KB |        1.00 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.501 ms |  1.00 |  29 | 8334.3 |   70.0 |       1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.748 ms |  1.07 |  29 | 7785.1 |   75.0 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.489 ms |  1.00 |  29 | 8364.7 |   69.8 |       1.01 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.436 ms |  1.27 |  29 | 6578.6 |   88.7 |       7.65 KB |        7.59 |
| ReadLine_    | Row   | 50000   |    18.126 ms |  5.18 |  29 | 1609.9 |  362.5 |   88608.23 KB |   87,921.34 |
| CsvHelper    | Row   | 50000   |    65.367 ms | 18.69 |  29 |  446.4 | 1307.3 |      19.95 KB |       19.79 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.800 ms |  1.00 |  29 | 6079.7 |   96.0 |       1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.952 ms |  1.24 |  29 | 4902.5 |  119.0 |       1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.572 ms |  1.79 |  29 | 3404.3 |  171.4 |       7.65 KB |        7.59 |
| ReadLine_    | Cols  | 50000   |    19.182 ms |  4.00 |  29 | 1521.3 |  383.6 |   88608.23 KB |   87,921.34 |
| CsvHelper    | Cols  | 50000   |   108.414 ms | 22.59 |  29 |  269.2 | 2168.3 |      445.6 KB |      442.15 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    52.283 ms |  1.00 |  29 |  558.1 | 1045.7 |   13802.65 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    34.441 ms |  0.66 |  29 |  847.3 |  688.8 |   13913.24 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    53.960 ms |  1.04 |  29 |  540.8 | 1079.2 |   13962.29 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   210.735 ms |  4.04 |  29 |  138.5 | 4214.7 |  102133.96 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   131.644 ms |  2.53 |  29 |  221.7 | 2632.9 |   13970.83 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   978.280 ms |  1.00 | 583 |  596.7 |  978.3 |  266668.88 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   515.844 ms |  0.53 | 583 | 1131.7 |  515.8 |  267776.55 KB |        1.00 |
| Sylvan___    | Asset | 1000000 | 1,130.594 ms |  1.16 | 583 |  516.4 | 1130.6 |  266825.13 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,155.567 ms |  3.23 | 583 |  185.0 | 3155.6 | 2038835.23 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,669.767 ms |  2.73 | 583 |  218.7 | 2669.8 |  266844.99 KB |        1.00 |

###### AMD.Ryzen.9.5950X - PackageAssets Benchmark Results (Sep 0.9.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     2.230 ms |  1.00 |  29 | 13088.4 |   44.6 |       1.09 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     2.379 ms |  1.07 |  29 | 12264.0 |   47.6 |       1.02 KB |        0.93 |
| Sep_Unescape | Row   | 50000   |     2.305 ms |  1.03 |  29 | 12657.6 |   46.1 |       1.02 KB |        0.93 |
| Sylvan___    | Row   | 50000   |     2.993 ms |  1.33 |  29 |  9750.2 |   59.9 |       7.65 KB |        7.52 |
| ReadLine_    | Row   | 50000   |    12.106 ms |  5.36 |  29 |  2410.5 |  242.1 |   88608.25 KB |   87,077.59 |
| CsvHelper    | Row   | 50000   |    43.313 ms | 19.19 |  29 |   673.7 |  866.3 |      20.04 KB |       19.69 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     3.211 ms |  1.00 |  29 |  9089.3 |   64.2 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     3.845 ms |  1.20 |  29 |  7589.1 |   76.9 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     5.065 ms |  1.58 |  29 |  5760.9 |  101.3 |       7.66 KB |        7.52 |
| ReadLine_    | Cols  | 50000   |    12.850 ms |  4.00 |  29 |  2270.9 |  257.0 |   88608.25 KB |   86,910.78 |
| CsvHelper    | Cols  | 50000   |    68.999 ms | 21.49 |  29 |   422.9 | 1380.0 |     445.85 KB |      437.31 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    33.615 ms |  1.00 |  29 |   868.1 |  672.3 |   13802.47 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    20.231 ms |  0.60 |  29 |  1442.4 |  404.6 |    13992.1 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    34.762 ms |  1.03 |  29 |   839.5 |  695.2 |    13962.2 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    97.204 ms |  2.89 |  29 |   300.2 | 1944.1 |   102133.9 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    83.550 ms |  2.49 |  29 |   349.3 | 1671.0 |   13970.66 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   629.552 ms |  1.00 | 583 |   927.3 |  629.6 |  266669.13 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   261.089 ms |  0.41 | 583 |  2236.0 |  261.1 |  267793.45 KB |        1.00 |
| Sylvan___    | Asset | 1000000 |   761.171 ms |  1.21 | 583 |   767.0 |  761.2 |  266825.09 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,636.526 ms |  2.60 | 583 |   356.7 | 1636.5 | 2038835.59 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,754.461 ms |  2.79 | 583 |   332.7 | 1754.5 |  266833.16 KB |        1.00 |

###### AMD.Ryzen.9.9950X - PackageAssets Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     1.385 ms |  1.00 |  29 | 21072.5 |   27.7 |       1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     1.518 ms |  1.10 |  29 | 19225.7 |   30.4 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     1.393 ms |  1.01 |  29 | 20942.6 |   27.9 |       1.01 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     1.885 ms |  1.36 |  29 | 15484.5 |   37.7 |       7.65 KB |        7.59 |
| ReadLine_    | Row   | 50000   |     7.885 ms |  5.69 |  29 |  3701.1 |  157.7 |   88608.23 KB |   87,921.34 |
| CsvHelper    | Row   | 50000   |    23.848 ms | 17.22 |  29 |  1223.6 |  477.0 |      19.95 KB |       19.79 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     2.010 ms |  1.00 |  29 | 14520.0 |   40.2 |       1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     2.366 ms |  1.18 |  29 | 12332.1 |   47.3 |       1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     3.188 ms |  1.59 |  29 |  9152.3 |   63.8 |       7.65 KB |        7.59 |
| ReadLine_    | Cols  | 50000   |     8.349 ms |  4.15 |  29 |  3495.3 |  167.0 |   88608.23 KB |   87,921.34 |
| CsvHelper    | Cols  | 50000   |    43.794 ms | 21.79 |  29 |   666.3 |  875.9 |     445.61 KB |      442.15 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    24.029 ms |  1.00 |  29 |  1214.4 |  480.6 |   13802.35 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    14.474 ms |  0.60 |  29 |  2016.1 |  289.5 |    13994.5 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    25.166 ms |  1.05 |  29 |  1159.6 |  503.3 |   13962.14 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    78.997 ms |  3.29 |  29 |   369.4 | 1579.9 |  102133.85 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    54.192 ms |  2.26 |  29 |   538.5 | 1083.8 |   13973.22 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   441.913 ms |  1.00 | 583 |  1321.0 |  441.9 |  266667.27 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   186.231 ms |  0.42 | 583 |  3134.7 |  186.2 |  268775.45 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   520.745 ms |  1.18 | 583 |  1121.1 |  520.7 |  266824.38 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,377.806 ms |  3.12 | 583 |   423.7 | 1377.8 | 2038834.84 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,173.256 ms |  2.65 | 583 |   497.6 | 1173.3 |  266840.63 KB |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     3.972 ms |  1.01 |  29 | 7323.2 |   79.4 |        952 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.992 ms |  1.01 |  29 | 7286.9 |   79.8 |        952 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.902 ms |  1.24 |  29 | 5933.7 |   98.0 |        952 B |        1.00 |
| Sylvan___    | Row   | 50000   |    27.633 ms |  7.01 |  29 | 1052.6 |  552.7 |       6692 B |        7.03 |
| ReadLine_    | Row   | 50000   |    21.068 ms |  5.34 |  29 | 1380.6 |  421.4 |   90734824 B |   95,309.69 |
| CsvHelper    | Row   | 50000   |    58.651 ms | 14.88 |  29 |  495.9 | 1173.0 |      20424 B |       21.45 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     4.505 ms |  1.00 |  29 | 6456.1 |   90.1 |        952 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.077 ms |  1.13 |  29 | 5729.1 |  101.5 |        952 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    29.637 ms |  6.58 |  29 |  981.4 |  592.7 |       6692 B |        7.03 |
| ReadLine_    | Cols  | 50000   |    25.961 ms |  5.77 |  29 | 1120.4 |  519.2 |   90734824 B |   95,309.69 |
| CsvHelper    | Cols  | 50000   |    70.982 ms | 15.76 |  29 |  409.8 | 1419.6 |     456296 B |      479.30 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    39.331 ms |  1.01 |  29 |  739.5 |  786.6 |   14133358 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    32.270 ms |  0.83 |  29 |  901.3 |  645.4 |   14228814 B |        1.01 |
| Sylvan___    | Asset | 50000   |    58.127 ms |  1.49 |  29 |  500.4 | 1162.5 |   14295768 B |        1.01 |
| ReadLine_    | Asset | 50000   |   115.274 ms |  2.95 |  29 |  252.3 | 2305.5 |  104585064 B |        7.40 |
| CsvHelper    | Asset | 50000   |    92.185 ms |  2.36 |  29 |  315.5 | 1843.7 |   14305450 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   752.281 ms |  1.00 | 581 |  773.5 |  752.3 |  273067192 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   804.179 ms |  1.07 | 581 |  723.6 |  804.2 |  287567112 B |        1.05 |
| Sylvan___    | Asset | 1000000 | 1,410.084 ms |  1.88 | 581 |  412.7 | 1410.1 |  273226864 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,592.746 ms |  4.79 | 581 |  162.0 | 3592.7 | 2087766752 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,322.007 ms |  3.10 | 581 |  250.6 | 2322.0 |  273241184 B |        1.00 |

###### Cobalt.100 - PackageAssets Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     5.786 ms |  1.00 |  29 | 5043.2 |  115.7 |        952 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.919 ms |  1.02 |  29 | 4929.9 |  118.4 |        952 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.527 ms |  0.96 |  29 | 5280.2 |  110.5 |        952 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.097 ms |  3.65 |  29 | 1383.2 |  421.9 |       6653 B |        6.99 |
| ReadLine_    | Row   | 50000   |    20.407 ms |  3.53 |  29 | 1430.0 |  408.1 |   90734824 B |   95,309.69 |
| CsvHelper    | Row   | 50000   |    54.870 ms |  9.49 |  29 |  531.8 | 1097.4 |      20424 B |       21.45 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     7.355 ms |  1.00 |  29 | 3967.3 |  147.1 |        952 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.875 ms |  1.07 |  29 | 3705.7 |  157.5 |        952 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    24.954 ms |  3.39 |  29 | 1169.4 |  499.1 |       6656 B |        6.99 |
| ReadLine_    | Cols  | 50000   |    20.826 ms |  2.83 |  29 | 1401.2 |  416.5 |   90734827 B |   95,309.69 |
| CsvHelper    | Cols  | 50000   |    88.817 ms | 12.08 |  29 |  328.6 | 1776.3 |     456368 B |      479.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    31.305 ms |  1.00 |  29 |  932.2 |  626.1 |   14132992 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    13.853 ms |  0.44 |  29 | 2106.5 |  277.1 |   14184144 B |        1.00 |
| Sylvan___    | Asset | 50000   |    56.747 ms |  1.81 |  29 |  514.2 | 1134.9 |   14295595 B |        1.01 |
| ReadLine_    | Asset | 50000   |   127.959 ms |  4.09 |  29 |  228.1 | 2559.2 |  104584112 B |        7.40 |
| CsvHelper    | Asset | 50000   |    98.423 ms |  3.14 |  29 |  296.5 | 1968.5 |   14305310 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   790.081 ms |  1.00 | 583 |  738.9 |  790.1 |  273063584 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   376.771 ms |  0.48 | 583 | 1549.4 |  376.8 |  282250016 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,234.977 ms |  1.56 | 583 |  472.7 | 1235.0 |  273225232 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,851.876 ms |  3.61 | 583 |  204.7 | 2851.9 | 2087764592 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,133.327 ms |  2.70 | 583 |  273.6 | 2133.3 |  273234944 B |        1.00 |


##### PackageAssets Benchmark Results (SERVER GC)
The package assets benchmark (Scope `Asset`) has a very high base load in the
form of the accumulated instances of `PackageAsset` and since Sep is so fast the
GC becomes a significant bottleneck for the benchmark, especially for
multi-threaded parsing. Switching to [SERVER
GC](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/workstation-server-gc)
can, therefore, provide significant speedup as can be seen below.

With `ParallelEnumerate` and server GC Sep is **>4x faster than Sylvan and up to
18x faster than CsvHelper**. Breaking 8 GB/s parsing speed on package assets on
9950X.

###### AMD.EPYC.7763 - PackageAssets Benchmark Results (SERVER GC) (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    33.24 ms |  1.00 |  29 |  874.9 |  664.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    16.93 ms |  0.51 |  29 | 1718.1 |  338.6 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    41.02 ms |  1.23 |  29 |  709.1 |  820.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    59.34 ms |  1.79 |  29 |  490.1 | 1186.9 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   114.45 ms |  3.44 |  29 |  254.1 | 2289.0 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   656.08 ms |  1.00 | 581 |  886.9 |  656.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   333.94 ms |  0.51 | 581 | 1742.5 |  333.9 |   268.8 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   824.37 ms |  1.26 | 581 |  705.8 |  824.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,231.05 ms |  1.88 | 581 |  472.7 | 1231.1 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,342.51 ms |  3.57 | 581 |  248.4 | 2342.5 |  260.58 MB |        1.00 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets Benchmark Results (SERVER GC) (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    35.630 ms |  1.00 |  29 |  819.0 |  712.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     8.892 ms |  0.25 |  29 | 3281.6 |  177.8 |   13.57 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    38.901 ms |  1.09 |  29 |  750.1 |  778.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    43.945 ms |  1.23 |  29 |  664.0 |  878.9 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   116.596 ms |  3.27 |  29 |  250.3 | 2331.9 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   654.638 ms |  1.00 | 583 |  891.8 |  654.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   244.949 ms |  0.37 | 583 | 2383.3 |  244.9 |  262.32 MB |        1.01 |
| Sylvan___ | Asset | 1000000 |   825.727 ms |  1.26 | 583 |  707.0 |  825.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   968.707 ms |  1.48 | 583 |  602.6 |  968.7 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,364.922 ms |  3.62 | 583 |  246.9 | 2364.9 |  260.58 MB |        1.00 |

###### AMD.Ryzen.9.5950X - PackageAssets Benchmark Results (SERVER GC) (Sep 0.9.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    20.951 ms |  1.00 |  29 | 1392.9 |  419.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     6.614 ms |  0.32 |  29 | 4411.8 |  132.3 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    27.761 ms |  1.33 |  29 | 1051.2 |  555.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    33.516 ms |  1.60 |  29 |  870.7 |  670.3 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    77.007 ms |  3.68 |  29 |  378.9 | 1540.1 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   432.887 ms |  1.00 | 583 | 1348.6 |  432.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   119.430 ms |  0.28 | 583 | 4888.1 |  119.4 |  261.39 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   559.550 ms |  1.29 | 583 | 1043.3 |  559.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   573.637 ms |  1.33 | 583 | 1017.7 |  573.6 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,537.602 ms |  3.55 | 583 |  379.7 | 1537.6 |  260.58 MB |        1.00 |

###### AMD.Ryzen.9.9950X - PackageAssets Benchmark Results (SERVER GC) (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |  14.152 ms |  1.00 |  29 | 2062.0 |  283.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   3.460 ms |  0.24 |  29 | 8434.1 |   69.2 |   13.65 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  18.113 ms |  1.28 |  29 | 1611.1 |  362.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |  18.960 ms |  1.34 |  29 | 1539.1 |  379.2 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |  48.971 ms |  3.46 |  29 |  595.9 |  979.4 |   13.64 MB |        1.01 |
|           |       |         |            |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 289.503 ms |  1.00 | 583 | 2016.5 |  289.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |  58.678 ms |  0.20 | 583 | 9949.0 |   58.7 |  261.63 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 375.349 ms |  1.30 | 583 | 1555.3 |  375.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 364.651 ms |  1.26 | 583 | 1600.9 |  364.7 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 999.550 ms |  3.45 | 583 |  584.0 |  999.5 |  260.58 MB |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets Benchmark Results (SERVER GC) (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    31.64 ms |  1.01 |  29 |  919.3 |  632.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    23.46 ms |  0.75 |  29 | 1239.8 |  469.2 |   13.65 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    61.59 ms |  1.96 |  29 |  472.2 | 1231.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    53.87 ms |  1.71 |  29 |  539.9 | 1077.4 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   109.28 ms |  3.48 |  29 |  266.2 | 2185.6 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   689.69 ms |  1.00 | 581 |  843.7 |  689.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   311.19 ms |  0.45 | 581 | 1869.8 |  311.2 |  270.88 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 1,380.63 ms |  2.01 | 581 |  421.5 | 1380.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 2,073.15 ms |  3.02 | 581 |  280.7 | 2073.1 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,448.67 ms |  3.57 | 581 |  237.6 | 2448.7 |  260.58 MB |        1.00 |

###### Cobalt.100 - PackageAssets Benchmark Results (SERVER GC) (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.59 ms |  1.00 |  29 |  895.3 |  651.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.62 ms |  0.36 |  29 | 2510.8 |  232.4 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    54.88 ms |  1.68 |  29 |  531.8 | 1097.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    49.51 ms |  1.52 |  29 |  589.4 |  990.2 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    99.62 ms |  3.06 |  29 |  292.9 | 1992.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   650.92 ms |  1.00 | 583 |  896.9 |  650.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   200.74 ms |  0.31 | 583 | 2908.2 |  200.7 |  266.53 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,104.81 ms |  1.70 | 583 |  528.4 | 1104.8 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,025.26 ms |  1.58 | 583 |  569.4 | 1025.3 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,089.05 ms |  3.21 | 583 |  279.4 | 2089.1 |  260.58 MB |        1.00 |


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

###### AMD.EPYC.7763 - PackageAssets with Quotes Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.81 ms |  1.00 |  33 | 3079.3 |  216.2 |       1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    11.24 ms |  1.04 |  33 | 2962.1 |  224.7 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    11.45 ms |  1.06 |  33 | 2906.6 |  229.0 |       1.01 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    25.19 ms |  2.33 |  33 | 1321.3 |  503.8 |       7.66 KB |        7.60 |
| ReadLine_    | Row   | 50000   |    26.33 ms |  2.44 |  33 | 1263.9 |  526.6 |  108778.73 KB |  107,935.48 |
| CsvHelper    | Row   | 50000   |    78.33 ms |  7.25 |  33 |  424.9 | 1566.6 |      20.02 KB |       19.86 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    13.13 ms |  1.00 |  33 | 2535.3 |  262.5 |       1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.97 ms |  1.06 |  33 | 2382.3 |  279.4 |       1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.21 ms |  2.15 |  33 | 1179.7 |  564.2 |       7.67 KB |        7.61 |
| ReadLine_    | Cols  | 50000   |    27.63 ms |  2.10 |  33 | 1204.5 |  552.6 |  108778.73 KB |  107,935.48 |
| CsvHelper    | Cols  | 50000   |   106.12 ms |  8.08 |  33 |  313.6 | 2122.4 |     445.68 KB |      442.22 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    46.78 ms |  1.00 |  33 |  711.5 |  935.6 |   13802.41 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    30.32 ms |  0.65 |  33 | 1097.6 |  606.5 |   13869.65 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    69.34 ms |  1.48 |  33 |  480.0 | 1386.7 |   13962.04 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   150.28 ms |  3.21 |  33 |  221.5 | 3005.6 |  122304.69 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   126.16 ms |  2.70 |  33 |  263.8 | 2523.2 |   13971.27 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   973.20 ms |  1.00 | 665 |  684.1 |  973.2 |  266667.22 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   589.97 ms |  0.61 | 665 | 1128.5 |  590.0 |  271570.43 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,404.92 ms |  1.44 | 665 |  473.9 | 1404.9 |  266824.36 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,386.92 ms |  3.48 | 665 |  196.6 | 3386.9 | 2442318.06 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,553.57 ms |  2.62 | 665 |  260.7 | 2553.6 |  266834.87 KB |        1.00 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets with Quotes Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    11.228 ms |  1.00 |  33 | 2972.8 |  224.6 |      1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     9.916 ms |  0.88 |  33 | 3365.9 |  198.3 |      1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     9.953 ms |  0.89 |  33 | 3353.4 |  199.1 |      1.01 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    23.829 ms |  2.12 |  33 | 1400.7 |  476.6 |      7.66 KB |        7.60 |
| ReadLine_    | Row   | 50000   |    21.909 ms |  1.95 |  33 | 1523.4 |  438.2 | 108778.73 KB |  107,935.48 |
| CsvHelper    | Row   | 50000   |    71.571 ms |  6.38 |  33 |  466.4 | 1431.4 |     19.95 KB |       19.79 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.798 ms |  1.00 |  33 | 2608.0 |  256.0 |      1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.689 ms |  1.07 |  33 | 2438.2 |  273.8 |      1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.287 ms |  2.13 |  33 | 1223.2 |  545.7 |      7.67 KB |        7.61 |
| ReadLine_    | Cols  | 50000   |    23.201 ms |  1.81 |  33 | 1438.6 |  464.0 | 108778.73 KB |  107,935.48 |
| CsvHelper    | Cols  | 50000   |   104.298 ms |  8.15 |  33 |  320.0 | 2086.0 |     445.6 KB |      442.15 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    55.957 ms |  1.00 |  33 |  596.5 | 1119.1 |  13802.68 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    44.668 ms |  0.80 |  33 |  747.2 |  893.4 |  13934.89 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    68.353 ms |  1.22 |  33 |  488.3 | 1367.1 |  13961.88 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   260.030 ms |  4.65 |  33 |  128.4 | 5200.6 | 122304.52 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   127.992 ms |  2.29 |  33 |  260.8 | 2559.8 |  13971.32 KB |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 | 1,145.307 ms |  1.00 | 667 |  583.0 | 1145.3 | 266673.49 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   696.277 ms |  0.61 | 667 |  959.0 |  696.3 | 267979.84 KB |        1.00 |
| Sylvan___    | Asset | 1000000 | 1,499.974 ms |  1.31 | 667 |  445.1 | 1500.0 | 266827.47 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,060.701 ms |  3.55 | 667 |  164.4 | 4060.7 |   2442318 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,601.519 ms |  2.27 | 667 |  256.7 | 2601.5 | 266839.95 KB |        1.00 |

###### AMD.Ryzen.9.5950X - PackageAssets with Quotes Benchmark Results (Sep 0.9.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     7.046 ms |  1.00 |  33 | 4737.2 |  140.9 |       1.04 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     8.137 ms |  1.15 |  33 | 4101.8 |  162.7 |       1.04 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.473 ms |  1.06 |  33 | 4466.7 |  149.5 |       1.04 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    17.571 ms |  2.38 |  33 | 1899.5 |  351.4 |       7.69 KB |        7.41 |
| ReadLine_    | Row   | 50000   |    14.336 ms |  1.94 |  33 | 2328.2 |  286.7 |  108778.75 KB |  104,689.33 |
| CsvHelper    | Row   | 50000   |    52.672 ms |  7.12 |  33 |  633.7 | 1053.4 |      20.05 KB |       19.29 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     8.126 ms |  1.00 |  33 | 4107.5 |  162.5 |       1.04 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     9.748 ms |  1.20 |  33 | 3424.0 |  195.0 |       1.05 KB |        1.01 |
| Sylvan___    | Cols  | 50000   |    20.503 ms |  2.52 |  33 | 1628.0 |  410.1 |        7.7 KB |        7.39 |
| ReadLine_    | Cols  | 50000   |    16.513 ms |  2.03 |  33 | 2021.3 |  330.3 |  108778.76 KB |  104,394.99 |
| CsvHelper    | Cols  | 50000   |    74.224 ms |  9.13 |  33 |  449.7 | 1484.5 |     445.85 KB |      427.88 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    39.523 ms |  1.00 |  33 |  844.5 |  790.5 |   13802.63 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    23.386 ms |  0.59 |  33 | 1427.2 |  467.7 |   13981.76 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    50.803 ms |  1.29 |  33 |  657.0 | 1016.1 |   13962.08 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   114.306 ms |  2.89 |  33 |  292.0 | 2286.1 |  122304.45 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    88.786 ms |  2.25 |  33 |  375.9 | 1775.7 |   13970.43 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   752.681 ms |  1.00 | 667 |  887.1 |  752.7 |     266669 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   377.733 ms |  0.50 | 667 | 1767.7 |  377.7 |   267992.5 KB |        1.00 |
| Sylvan___    | Asset | 1000000 | 1,091.345 ms |  1.45 | 667 |  611.8 | 1091.3 |  266825.09 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,615.390 ms |  3.47 | 667 |  255.3 | 2615.4 | 2442319.06 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,756.409 ms |  2.33 | 667 |  380.2 | 1756.4 |  266839.53 KB |        1.00 |

###### AMD.Ryzen.9.9950X - PackageAssets with Quotes Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     4.288 ms |  1.00 |  33 | 7783.5 |   85.8 |       1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     5.226 ms |  1.22 |  33 | 6386.4 |  104.5 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.170 ms |  0.97 |  33 | 8004.3 |   83.4 |       1.15 KB |        1.14 |
| Sylvan___    | Row   | 50000   |    10.286 ms |  2.40 |  33 | 3245.1 |  205.7 |       7.65 KB |        7.59 |
| ReadLine_    | Row   | 50000   |     9.464 ms |  2.21 |  33 | 3526.9 |  189.3 |  108778.73 KB |  107,935.48 |
| CsvHelper    | Row   | 50000   |    27.086 ms |  6.32 |  33 | 1232.3 |  541.7 |      19.95 KB |       19.79 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.100 ms |  1.00 |  33 | 6544.9 |  102.0 |       1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.496 ms |  1.08 |  33 | 6073.5 |  109.9 |       1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    11.899 ms |  2.33 |  33 | 2805.1 |  238.0 |       7.66 KB |        7.60 |
| ReadLine_    | Cols  | 50000   |    10.023 ms |  1.97 |  33 | 3330.1 |  200.5 |  108778.73 KB |  107,935.48 |
| CsvHelper    | Cols  | 50000   |    40.244 ms |  7.89 |  33 |  829.4 |  804.9 |     445.61 KB |      442.15 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    28.037 ms |  1.00 |  33 | 1190.5 |  560.7 |    13802.3 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    17.037 ms |  0.61 |  33 | 1959.1 |  340.7 |   13989.51 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    31.020 ms |  1.11 |  33 | 1076.0 |  620.4 |   13962.02 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    95.065 ms |  3.39 |  33 |  351.1 | 1901.3 |  122304.75 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    51.339 ms |  1.83 |  33 |  650.1 | 1026.8 |   13970.75 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   519.607 ms |  1.00 | 667 | 1285.0 |  519.6 |  266667.37 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   221.654 ms |  0.43 | 667 | 3012.4 |  221.7 |   270630.1 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   703.908 ms |  1.35 | 667 |  948.6 |  703.9 |  266824.38 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,870.691 ms |  3.60 | 667 |  356.9 | 1870.7 | 2442318.59 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,097.942 ms |  2.11 | 667 |  608.1 | 1097.9 |  266832.53 KB |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets with Quotes Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    10.004 ms |  1.01 |  33 | 3326.8 |  200.1 |        952 B |        1.00 |
| Sep_Async    | Row   | 50000   |     8.591 ms |  0.86 |  33 | 3874.3 |  171.8 |        952 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.763 ms |  0.78 |  33 | 4287.5 |  155.3 |        952 B |        1.00 |
| Sylvan___    | Row   | 50000   |    23.401 ms |  2.36 |  33 | 1422.2 |  468.0 |       6692 B |        7.03 |
| ReadLine_    | Row   | 50000   |    27.778 ms |  2.80 |  33 | 1198.1 |  555.6 |  111389416 B |  117,005.69 |
| CsvHelper    | Row   | 50000   |    53.094 ms |  5.34 |  33 |  626.9 | 1061.9 |      20424 B |       21.45 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.885 ms |  1.01 |  33 | 2800.2 |  237.7 |        952 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.821 ms |  1.09 |  33 | 2596.0 |  256.4 |        952 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.213 ms |  2.31 |  33 | 1223.0 |  544.3 |       6692 B |        7.03 |
| ReadLine_    | Cols  | 50000   |    23.611 ms |  2.00 |  33 | 1409.6 |  472.2 |  111389416 B |  117,005.69 |
| CsvHelper    | Cols  | 50000   |    92.689 ms |  7.86 |  33 |  359.1 | 1853.8 |     456296 B |      479.30 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    40.423 ms |  1.00 |  33 |  823.3 |  808.5 |   14134213 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    36.395 ms |  0.90 |  33 |  914.5 |  727.9 |   14211028 B |        1.01 |
| Sylvan___    | Asset | 50000   |    87.081 ms |  2.16 |  33 |  382.2 | 1741.6 |   14295952 B |        1.01 |
| ReadLine_    | Asset | 50000   |   135.098 ms |  3.35 |  33 |  246.4 | 2702.0 |  125239776 B |        8.86 |
| CsvHelper    | Asset | 50000   |    89.705 ms |  2.23 |  33 |  371.0 | 1794.1 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 | 1,023.915 ms |  1.01 | 665 |  650.2 | 1023.9 |  273066968 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   901.079 ms |  0.89 | 665 |  738.9 |  901.1 |  285049448 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,455.283 ms |  1.44 | 665 |  457.5 | 1455.3 |  273232840 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,628.378 ms |  4.58 | 665 |  143.9 | 4628.4 | 2500933560 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,299.950 ms |  2.28 | 665 |  289.5 | 2299.9 |  273242120 B |        1.00 |

###### Cobalt.100 - PackageAssets with Quotes Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    11.06 ms |  1.00 |  33 | 3017.0 |  221.3 |        953 B |        1.00 |
| Sep_Async    | Row   | 50000   |    11.38 ms |  1.03 |  33 | 2934.3 |  227.5 |        954 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    11.28 ms |  1.02 |  33 | 2958.1 |  225.7 |        954 B |        1.00 |
| Sylvan___    | Row   | 50000   |    23.17 ms |  2.09 |  33 | 1440.4 |  463.4 |       6654 B |        6.98 |
| ReadLine_    | Row   | 50000   |    24.39 ms |  2.20 |  33 | 1368.6 |  487.8 |  111389416 B |  116,882.91 |
| CsvHelper    | Row   | 50000   |    63.45 ms |  5.74 |  33 |  526.0 | 1269.0 |      20424 B |       21.43 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.67 ms |  1.00 |  33 | 2634.1 |  253.4 |        952 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.86 ms |  1.09 |  33 | 2408.9 |  277.1 |        954 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.54 ms |  2.17 |  33 | 1211.8 |  550.9 |       6657 B |        6.99 |
| ReadLine_    | Cols  | 50000   |    24.94 ms |  1.97 |  33 | 1338.1 |  498.9 |  111389419 B |  117,005.69 |
| CsvHelper    | Cols  | 50000   |    97.05 ms |  7.66 |  33 |  343.9 | 1941.0 |     456368 B |      479.38 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    38.12 ms |  1.00 |  33 |  875.5 |  762.4 |   14132992 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    15.19 ms |  0.40 |  33 | 2198.0 |  303.7 |   14190126 B |        1.00 |
| Sylvan___    | Asset | 50000   |    58.84 ms |  1.54 |  33 |  567.3 | 1176.8 |   14295595 B |        1.01 |
| ReadLine_    | Asset | 50000   |   135.98 ms |  3.57 |  33 |  245.5 | 2719.6 |  125238788 B |        8.86 |
| CsvHelper    | Asset | 50000   |   106.50 ms |  2.79 |  33 |  313.4 | 2130.1 |   14305310 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   919.60 ms |  1.00 | 667 |  726.1 |  919.6 |  273063640 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   392.61 ms |  0.43 | 667 | 1700.7 |  392.6 |  275587208 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,303.94 ms |  1.42 | 667 |  512.1 | 1303.9 |  273225360 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,503.86 ms |  3.81 | 667 |  190.6 | 3503.9 | 2500932192 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,309.99 ms |  2.51 | 667 |  289.1 | 2310.0 |  273236680 B |        1.00 |


##### PackageAssets with Quotes Benchmark Results (SERVER GC)
Here again are benchmark results with server garbage collection, which provides
significant speedup over workstation garbage collection.

###### AMD.EPYC.7763 - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    37.82 ms |  1.00 |  33 |  880.1 |  756.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    19.95 ms |  0.53 |  33 | 1668.2 |  399.0 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    60.86 ms |  1.61 |  33 |  546.8 | 1217.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    65.12 ms |  1.72 |  33 |  511.1 | 1302.3 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   117.65 ms |  3.11 |  33 |  282.9 | 2353.0 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   814.18 ms |  1.00 | 665 |  817.8 |  814.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   420.01 ms |  0.52 | 665 | 1585.2 |  420.0 |  262.55 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,275.65 ms |  1.57 | 665 |  521.9 | 1275.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,362.68 ms |  1.67 | 665 |  488.6 | 1362.7 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,362.46 ms |  2.90 | 665 |  281.8 | 2362.5 |  260.58 MB |        1.00 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.09 ms |  1.00 |  33 |  854.0 |  781.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    25.90 ms |  0.66 |  33 | 1288.9 |  517.9 |   13.58 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    59.92 ms |  1.53 |  33 |  557.0 | 1198.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    60.27 ms |  1.54 |  33 |  553.8 | 1205.4 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   111.54 ms |  2.85 |  33 |  299.2 | 2230.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   810.33 ms |  1.00 | 667 |  824.0 |  810.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   433.80 ms |  0.54 | 667 | 1539.2 |  433.8 |  261.43 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,216.78 ms |  1.50 | 667 |  548.8 | 1216.8 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,079.99 ms |  1.33 | 667 |  618.3 | 1080.0 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,275.20 ms |  2.81 | 667 |  293.5 | 2275.2 |  260.58 MB |        1.00 |

###### AMD.Ryzen.9.5950X - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.9.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    26.42 ms |  1.00 |  33 | 1263.1 |  528.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.53 ms |  0.44 |  33 | 2894.1 |  230.7 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    43.05 ms |  1.63 |  33 |  775.3 |  861.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    37.30 ms |  1.41 |  33 |  894.8 |  746.0 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    78.91 ms |  2.99 |  33 |  423.0 | 1578.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   538.48 ms |  1.00 | 667 | 1240.0 |  538.5 |  260.43 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   213.29 ms |  0.40 | 667 | 3130.5 |  213.3 |  261.37 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   879.04 ms |  1.63 | 667 |  759.6 |  879.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   642.57 ms |  1.19 | 667 | 1039.1 |  642.6 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,598.79 ms |  2.97 | 667 |  417.6 | 1598.8 |  260.58 MB |        1.00 |

###### AMD.Ryzen.9.9950X - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |  17.792 ms |  1.00 |  33 | 1876.0 |  355.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   5.863 ms |  0.33 |  33 | 5692.8 |  117.3 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  27.020 ms |  1.52 |  33 | 1235.3 |  540.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |  22.191 ms |  1.25 |  33 | 1504.1 |  443.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |  46.946 ms |  2.64 |  33 |  711.0 |  938.9 |   13.65 MB |        1.01 |
|           |       |         |            |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 363.921 ms |  1.00 | 667 | 1834.8 |  363.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 104.427 ms |  0.29 | 667 | 6394.0 |  104.4 |  261.51 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 541.503 ms |  1.49 | 667 | 1233.1 |  541.5 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 422.719 ms |  1.16 | 667 | 1579.6 |  422.7 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 938.489 ms |  2.58 | 667 |  711.5 |  938.5 |  260.58 MB |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    38.27 ms |  1.02 |  33 |  869.6 |  765.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    24.23 ms |  0.65 |  33 | 1373.3 |  484.7 |   13.75 MB |        1.02 |
| Sylvan___ | Asset | 50000   |    78.94 ms |  2.11 |  33 |  421.6 | 1578.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    73.58 ms |  1.97 |  33 |  452.3 | 1471.6 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   127.06 ms |  3.40 |  33 |  261.9 | 2541.3 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   896.49 ms |  1.00 | 665 |  742.7 |  896.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   388.68 ms |  0.43 | 665 | 1713.0 |  388.7 |  271.08 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 1,703.56 ms |  1.90 | 665 |  390.8 | 1703.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 3,773.61 ms |  4.22 | 665 |  176.4 | 3773.6 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,813.28 ms |  3.14 | 665 |  236.7 | 2813.3 |  260.58 MB |        1.00 |

###### Cobalt.100 - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    38.23 ms |  1.00 |  33 |  873.2 |  764.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.94 ms |  0.39 |  33 | 2234.7 |  298.7 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.75 ms |  1.48 |  33 |  588.1 | 1135.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    56.10 ms |  1.47 |  33 |  595.0 | 1122.0 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   105.98 ms |  2.77 |  33 |  314.9 | 2119.7 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   761.89 ms |  1.00 | 667 |  876.4 |  761.9 |  260.42 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   244.95 ms |  0.32 | 667 | 2725.9 |  244.9 |  262.42 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,139.57 ms |  1.50 | 667 |  585.9 | 1139.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,236.30 ms |  1.62 | 667 |  540.1 | 1236.3 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,151.47 ms |  2.82 | 667 |  310.3 | 2151.5 |  260.58 MB |        1.00 |


##### PackageAssets with Spaces and Quotes Benchmark Results

Similar to the benchmark related to quotes here spaces ` ` and quotes `"` are
added to relevant columns to benchmark impact of trimming and unescape on low
level column access. That is, basically ` " ` is prepended and appended to each
column. This will test the assumed most common case and fast path part of
trimming and unescaping in Sep. Sep is about 10x faster than CsvHelper for this.
Sylvan does not appear to support automatic trimming and is, therefore, not
included.

###### AMD.EPYC.7763 - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.98 ms |  1.00 | 41 | 3211.7 |  259.5 |   1.01 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  19.39 ms |  1.49 | 41 | 2148.8 |  387.9 |   1.01 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.45 ms |  1.50 | 41 | 2142.7 |  389.0 |   1.01 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.53 ms |  1.66 | 41 | 1935.4 |  430.7 |   1.01 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 142.47 ms | 10.98 | 41 |  292.5 | 2849.5 | 451.34 KB |      447.84 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 141.73 ms | 10.92 | 41 |  294.1 | 2834.5 | 445.68 KB |      442.23 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  13.76 ms |  1.00 | 41 | 3036.5 |  275.1 |   1.01 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.25 ms |  1.33 | 41 | 2289.2 |  364.9 |   1.01 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.43 ms |  1.34 | 41 | 2266.6 |  368.6 |   1.01 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.16 ms |  1.54 | 41 | 1974.2 |  423.2 |   1.01 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 127.73 ms |  9.29 | 41 |  327.0 | 2554.6 | 451.34 KB |      447.84 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 126.60 ms |  9.21 | 41 |  329.9 | 2532.1 | 445.68 KB |      442.22 |

###### AMD.Ryzen.9.5950X - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.9.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  9.467 ms |  1.00 | 41 | 4412.2 |  189.3 |   1.05 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 12.972 ms |  1.37 | 41 | 3219.9 |  259.4 |   1.06 KB |        1.01 |
| Sep_TrimUnescape           | Cols  | 50000 | 13.630 ms |  1.44 | 41 | 3064.5 |  272.6 |   1.06 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 15.502 ms |  1.64 | 41 | 2694.4 |  310.0 |   1.07 KB |        1.03 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 98.444 ms | 10.40 | 41 |  424.3 | 1968.9 | 451.52 KB |      431.70 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 97.110 ms | 10.26 | 41 |  430.1 | 1942.2 | 445.86 KB |      426.29 |

###### AMD.Ryzen.9.9950X - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  5.434 ms |  1.00 | 41 | 7687.0 |  108.7 |   1.01 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  7.906 ms |  1.46 | 41 | 5283.2 |  158.1 |   1.01 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  8.541 ms |  1.57 | 41 | 4890.5 |  170.8 |   1.01 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  9.059 ms |  1.67 | 41 | 4610.7 |  181.2 |   1.01 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 61.560 ms | 11.33 | 41 |  678.5 | 1231.2 | 451.27 KB |      447.77 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 60.812 ms | 11.19 | 41 |  686.9 | 1216.2 |  445.6 KB |      442.15 |

###### Apple.M1.(Virtual) - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.36 ms |  1.01 | 41 | 3373.0 |  247.1 |     952 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  19.17 ms |  1.56 | 41 | 2174.4 |  383.3 |     952 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  20.35 ms |  1.66 | 41 | 2047.8 |  407.0 |     952 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  17.07 ms |  1.39 | 41 | 2441.6 |  341.4 |     952 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 110.49 ms |  8.99 | 41 |  377.2 | 2209.7 |  462096 B |      485.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 130.51 ms | 10.62 | 41 |  319.3 | 2610.1 |  456368 B |      479.38 |

###### Cobalt.100 - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  14.19 ms |  1.00 | 41 | 2942.7 |  283.9 |     954 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.47 ms |  1.30 | 41 | 2261.5 |  369.4 |     955 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.41 ms |  1.37 | 41 | 2152.0 |  388.2 |     955 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.91 ms |  1.47 | 41 | 1997.4 |  418.3 |     952 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 120.00 ms |  8.45 | 41 |  348.1 | 2400.0 |  462174 B |      484.46 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 115.80 ms |  8.16 | 41 |  360.7 | 2316.1 |  459458 B |      481.61 |


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
parsing the floating points. Since Sep uses
[csFastFloat](https://github.com/CarlVerret/csFastFloat) for an integrated fast
floating point parser, it is **>2x faster than Sylvan** for example. If using
Sylvan one may consider using csFastFloat if that is an option. With the
multi-threaded (MT) `ParallelEnumerate` implementation Sep is **up to 23x faster
than Sylvan**.

CsvHelper suffers from the fact that one can only access the column as a string
so this has to be allocated for each column (ReadLine by definition always
allocates a string per column). Still CsvHelper is significantly slower than the
naive `ReadLine` approach. With Sep being **>4x faster than CsvHelper** and **up to
35x times faster when using `ParallelEnumerate`**.

Note that `ParallelEnumerate` provides significant speedup over single-threaded
parsing even though the source is only about 20 MB. This underlines how
efficient `ParallelEnumerate` is, but bear in mind that this is for the case of
repeated micro-benchmark runs.

It is a testament to how good the .NET and the .NET GC is that the ReadLine is
pretty good compared to CsvHelper regardless of allocating a lot of strings.

##### AMD.EPYC.7763 - FloatsReader Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.002 ms |  1.00 | 20 | 6753.9 |  120.1 |     1.24 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.305 ms |  1.10 | 20 | 6133.3 |  132.2 |     10.7 KB |        8.61 |
| ReadLine_ | Row    | 25000 |  18.502 ms |  6.16 | 20 | 1095.7 |  740.1 | 73489.62 KB |   59,161.45 |
| CsvHelper | Row    | 25000 |  37.670 ms | 12.55 | 20 |  538.1 | 1506.8 |    19.95 KB |       16.06 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.076 ms |  1.00 | 20 | 4973.3 |  163.0 |     1.24 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   5.792 ms |  1.42 | 20 | 3500.2 |  231.7 |    10.71 KB |        8.62 |
| ReadLine_ | Cols   | 25000 |  18.372 ms |  4.51 | 20 | 1103.4 |  734.9 | 73489.62 KB |   59,161.45 |
| CsvHelper | Cols   | 25000 |  40.450 ms |  9.92 | 20 |  501.2 | 1618.0 | 21340.16 KB |   17,179.50 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.470 ms |  1.00 | 20 |  644.2 | 1258.8 |     7.89 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  12.902 ms |  0.41 | 20 | 1571.2 |  516.1 |    68.47 KB |        8.68 |
| Sylvan___ | Floats | 25000 |  82.451 ms |  2.62 | 20 |  245.9 | 3298.0 |    21.71 KB |        2.75 |
| ReadLine_ | Floats | 25000 | 110.689 ms |  3.52 | 20 |  183.1 | 4427.6 | 73492.96 KB |    9,313.96 |
| CsvHelper | Floats | 25000 | 157.042 ms |  4.99 | 20 |  129.1 | 6281.7 | 22061.51 KB |    2,795.91 |

##### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - FloatsReader Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.090 ms |  1.00 | 20 | 6576.8 |  123.6 |     1.24 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.662 ms |  1.19 | 20 | 5548.9 |  146.5 |     10.7 KB |        8.61 |
| ReadLine_ | Row    | 25000 |  14.759 ms |  4.78 | 20 | 1376.7 |  590.4 | 73489.62 KB |   59,161.45 |
| CsvHelper | Row    | 25000 |  40.346 ms | 13.06 | 20 |  503.6 | 1613.9 |    19.95 KB |       16.06 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.244 ms |  1.00 | 20 | 4787.9 |  169.8 |     1.24 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.501 ms |  1.53 | 20 | 3125.5 |  260.1 |     10.7 KB |        8.61 |
| ReadLine_ | Cols   | 25000 |  15.460 ms |  3.64 | 20 | 1314.3 |  618.4 | 73489.62 KB |   59,161.45 |
| CsvHelper | Cols   | 25000 |  42.710 ms | 10.06 | 20 |  475.8 | 1708.4 | 21340.16 KB |   17,179.50 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  30.251 ms |  1.00 | 20 |  671.7 | 1210.1 |     7.89 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   6.924 ms |  0.23 | 20 | 2934.7 |  277.0 |   111.53 KB |       14.13 |
| Sylvan___ | Floats | 25000 |  78.705 ms |  2.60 | 20 |  258.2 | 3148.2 |     18.7 KB |        2.37 |
| ReadLine_ | Floats | 25000 | 102.918 ms |  3.40 | 20 |  197.4 | 4116.7 | 73492.94 KB |    9,313.96 |
| CsvHelper | Floats | 25000 | 156.097 ms |  5.16 | 20 |  130.2 | 6243.9 | 22061.22 KB |    2,795.88 |

##### AMD.Ryzen.9.5950X - FloatsReader Benchmark Results (Sep 0.9.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.013 ms |  1.00 | 20 | 10093.4 |   80.5 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   2.355 ms |  1.17 | 20 |  8627.4 |   94.2 |     10.7 KB |        8.56 |
| ReadLine_ | Row    | 25000 |   9.787 ms |  4.86 | 20 |  2076.1 |  391.5 | 73489.63 KB |   58,791.71 |
| CsvHelper | Row    | 25000 |  25.143 ms | 12.49 | 20 |   808.2 | 1005.7 |       20 KB |       16.00 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |   2.666 ms |  1.00 | 20 |  7622.2 |  106.6 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   3.702 ms |  1.39 | 20 |  5488.4 |  148.1 |    10.71 KB |        8.54 |
| ReadLine_ | Cols   | 25000 |  10.544 ms |  3.96 | 20 |  1927.1 |  421.8 | 73489.63 KB |   58,654.23 |
| CsvHelper | Cols   | 25000 |  27.442 ms | 10.29 | 20 |   740.5 | 1097.7 | 21340.34 KB |   17,032.36 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Floats | 25000 |  20.297 ms |  1.00 | 20 |  1001.1 |  811.9 |     7.97 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   3.780 ms |  0.19 | 20 |  5375.6 |  151.2 |   179.49 KB |       22.51 |
| Sylvan___ | Floats | 25000 |  52.343 ms |  2.58 | 20 |   388.2 | 2093.7 |    18.88 KB |        2.37 |
| ReadLine_ | Floats | 25000 |  68.698 ms |  3.38 | 20 |   295.8 | 2747.9 | 73493.12 KB |    9,215.89 |
| CsvHelper | Floats | 25000 | 100.913 ms |  4.97 | 20 |   201.4 | 4036.5 | 22061.69 KB |    2,766.49 |

##### AMD.Ryzen.9.9950X - FloatsReader Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |  1.265 ms |  1.00 | 20 | 16063.2 |   50.6 |     1.24 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.624 ms |  1.28 | 20 | 12511.4 |   65.0 |    10.75 KB |        8.66 |
| ReadLine_ | Row    | 25000 |  6.545 ms |  5.17 | 20 |  3104.8 |  261.8 | 73489.62 KB |   59,161.45 |
| CsvHelper | Row    | 25000 | 14.820 ms | 11.72 | 20 |  1371.1 |  592.8 |    19.95 KB |       16.06 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |  1.929 ms |  1.00 | 20 | 10533.1 |   77.2 |     1.24 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.589 ms |  1.34 | 20 |  7850.0 |  103.5 |     10.7 KB |        8.61 |
| ReadLine_ | Cols   | 25000 |  6.814 ms |  3.53 | 20 |  2982.2 |  272.5 | 73489.62 KB |   59,161.45 |
| CsvHelper | Cols   | 25000 | 15.995 ms |  8.29 | 20 |  1270.4 |  639.8 | 21340.17 KB |   17,179.50 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Floats | 25000 | 13.965 ms |  1.00 | 20 |  1455.0 |  558.6 |     7.89 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.049 ms |  0.15 | 20 |  9918.5 |   81.9 |   178.98 KB |       22.68 |
| Sylvan___ | Floats | 25000 | 35.057 ms |  2.51 | 20 |   579.6 | 1402.3 |    18.67 KB |        2.37 |
| ReadLine_ | Floats | 25000 | 47.821 ms |  3.42 | 20 |   424.9 | 1912.8 | 73492.95 KB |    9,311.65 |
| CsvHelper | Floats | 25000 | 66.495 ms |  4.76 | 20 |   305.6 | 2659.8 | 22061.22 KB |    2,795.19 |

##### Apple.M1.(Virtual) - FloatsReader Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.546 ms |  1.00 | 20 | 5716.9 |  141.8 |     1.16 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  21.472 ms |  6.08 | 20 |  944.1 |  858.9 |    10.36 KB |        8.90 |
| ReadLine_ | Row    | 25000 |  19.918 ms |  5.64 | 20 | 1017.8 |  796.7 | 73489.62 KB |   63,132.02 |
| CsvHelper | Row    | 25000 |  37.422 ms | 10.59 | 20 |  541.7 | 1496.9 |    19.95 KB |       17.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.986 ms |  1.01 | 20 | 5085.7 |  159.4 |     1.16 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  24.931 ms |  6.29 | 20 |  813.1 |  997.2 |    10.52 KB |        9.04 |
| ReadLine_ | Cols   | 25000 |  17.889 ms |  4.51 | 20 | 1133.2 |  715.6 | 73489.62 KB |   63,132.02 |
| CsvHelper | Cols   | 25000 |  47.480 ms | 11.98 | 20 |  427.0 | 1899.2 | 21340.16 KB |   18,332.49 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  34.253 ms |  1.02 | 20 |  591.8 | 1370.1 |     7.81 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  17.345 ms |  0.52 | 20 | 1168.7 |  693.8 |   101.17 KB |       12.95 |
| Sylvan___ | Floats | 25000 |  87.013 ms |  2.60 | 20 |  233.0 | 3480.5 |    18.31 KB |        2.34 |
| ReadLine_ | Floats | 25000 | 101.080 ms |  3.01 | 20 |  200.6 | 4043.2 | 73492.94 KB |    9,407.10 |
| CsvHelper | Floats | 25000 | 116.872 ms |  3.49 | 20 |  173.5 | 4674.9 | 22061.55 KB |    2,823.88 |

##### Cobalt.100 - FloatsReader Benchmark Results (Sep 0.12.0.0, Sylvan  1.4.3.0, CsvHelper 33.1.0.26)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.295 ms |  1.00 | 20 | 6166.2 |  131.8 |     1.16 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.661 ms |  7.18 | 20 |  858.8 |  946.4 |    10.32 KB |        8.86 |
| ReadLine_ | Row    | 25000 |  16.952 ms |  5.14 | 20 | 1198.6 |  678.1 | 73489.62 KB |   63,132.02 |
| CsvHelper | Row    | 25000 |  33.251 ms | 10.09 | 20 |  611.1 | 1330.0 |    20.02 KB |       17.19 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.695 ms |  1.00 | 20 | 4327.6 |  187.8 |     1.16 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.819 ms |  5.71 | 20 |  757.6 | 1072.8 |    10.32 KB |        8.87 |
| ReadLine_ | Cols   | 25000 |  17.588 ms |  3.75 | 20 | 1155.3 |  703.5 | 73489.62 KB |   63,132.02 |
| CsvHelper | Cols   | 25000 |  36.000 ms |  7.67 | 20 |  564.4 | 1440.0 | 21340.16 KB |   18,332.49 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  29.130 ms |  1.00 | 20 |  697.6 | 1165.2 |     7.81 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   8.689 ms |  0.30 | 20 | 2338.6 |  347.6 |    86.84 KB |       11.12 |
| Sylvan___ | Floats | 25000 |  86.914 ms |  2.98 | 20 |  233.8 | 3476.6 |    18.31 KB |        2.34 |
| ReadLine_ | Floats | 25000 |  94.119 ms |  3.23 | 20 |  215.9 | 3764.7 | 73492.94 KB |    9,407.10 |
| CsvHelper | Floats | 25000 | 129.945 ms |  4.46 | 20 |  156.4 | 5197.8 | 22060.98 KB |    2,823.81 |


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
using var writer = Sep.New(',')
    .Writer(o => o with { Escape = true })
    .ToText();
foreach (var person in writePersons)
{
    using var row = writer.NewRow();
    row[nameof(person.Name)].Set(person.Name);
    row[nameof(person.BirthDay)].Format(person.BirthDay);
    row[nameof(person.Address)].Set(person.Address);
}
var text = writer.ToString();
// Read
using var reader = Sep.New(',')
    .Reader(o => o with { Unescape = true })
    .FromText(text);
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

* [Publishing a NuGet package using GitHub and GitHub Actions](https://www.meziantou.net/publishing-a-nuget-package-following-best-practices-using-github.htm)

## Public API Reference
```csharp
[assembly: System.CLSCompliant(false)]
[assembly: System.Reflection.AssemblyMetadata("IsTrimmable", "True")]
[assembly: System.Reflection.AssemblyMetadata("RepositoryUrl", "https://github.com/nietras/Sep/")]
[assembly: System.Resources.NeutralResourcesLanguage("en")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Sep.Benchmarks")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Sep.ComparisonBenchmarks")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Sep.Test")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Sep.XyzTest")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v9.0", FrameworkDisplayName=".NET 9.0")]
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
            public override string ToString() { }
            public T? TryParse<T>()
                where T :  struct, System.ISpanParsable<T> { }
            public bool TryParse<T>(out T value)
                where T : System.ISpanParsable<T> { }
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
        public delegate void ColsAction(nietras.SeparatedValues.SepReader.Cols col);
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
        public static nietras.SeparatedValues.SepReader From(this in nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.Stream> nameToStream) { }
        public static nietras.SeparatedValues.SepReader From(this in nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.TextReader> nameToReader) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, System.IO.Stream stream, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, System.IO.TextReader reader, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, byte[] buffer, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.Stream> nameToStream, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.TextReader> nameToReader, System.Threading.CancellationToken cancellationToken = default) { }
        public static nietras.SeparatedValues.SepReader FromFile(this in nietras.SeparatedValues.SepReaderOptions options, string filePath) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromFileAsync(this nietras.SeparatedValues.SepReaderOptions options, string filePath, System.Threading.CancellationToken cancellationToken = default) { }
        public static nietras.SeparatedValues.SepReader FromText(this in nietras.SeparatedValues.SepReaderOptions options, string text) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromTextAsync(this nietras.SeparatedValues.SepReaderOptions options, string text, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Collections.Generic.IEnumerable<T> ParallelEnumerate<T>(this nietras.SeparatedValues.SepReader reader, nietras.SeparatedValues.SepReader.RowFunc<T> select) { }
        public static System.Collections.Generic.IEnumerable<T> ParallelEnumerate<T>(this nietras.SeparatedValues.SepReader reader, nietras.SeparatedValues.SepReader.RowTryFunc<T> trySelect) { }
        public static System.Collections.Generic.IEnumerable<T> ParallelEnumerate<T>(this nietras.SeparatedValues.SepReader reader, nietras.SeparatedValues.SepReader.RowFunc<T> select, int degreeOfParallism) { }
        public static System.Collections.Generic.IEnumerable<T> ParallelEnumerate<T>(this nietras.SeparatedValues.SepReader reader, nietras.SeparatedValues.SepReader.RowTryFunc<T> trySelect, int degreeOfParallism) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.Sep sep) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.Sep? sep) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.SepSpec spec) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.Sep sep, System.Func<nietras.SeparatedValues.SepReaderOptions, nietras.SeparatedValues.SepReaderOptions> configure) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.Sep? sep, System.Func<nietras.SeparatedValues.SepReaderOptions, nietras.SeparatedValues.SepReaderOptions> configure) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.SepSpec spec, System.Func<nietras.SeparatedValues.SepReaderOptions, nietras.SeparatedValues.SepReaderOptions> configure) { }
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
