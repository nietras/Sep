﻿# Sep - the World's Fastest .NET CSV Parser
![.NET](https://img.shields.io/badge/net8.0%20net9.0-5C2D91?logo=.NET&labelColor=gray)
![C#](https://img.shields.io/badge/C%23-13.0-239120?labelColor=gray)
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
expression](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/ranges#systemrange),
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
* `AMD 5950X` (Desktop) X64 Platform Information
  ```ini
  OS=Windows 10 (10.0.19044.2846/21H2/November2021Update)
  AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
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

###### AMD.EPYC.7763 - PackageAssets Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.465 ms |  1.00 |  29 | 8393.2 |   69.3 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.631 ms |  1.05 |  29 | 8011.5 |   72.6 |       1.03 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.551 ms |  1.02 |  29 | 8191.8 |   71.0 |       1.03 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.356 ms |  1.26 |  29 | 6676.9 |   87.1 |       7.66 KB |        7.48 |
| ReadLine_    | Row   | 50000   |    21.994 ms |  6.35 |  29 | 1322.4 |  439.9 |    88608.3 KB |   86,496.56 |
| CsvHelper    | Row   | 50000   |    63.463 ms | 18.31 |  29 |  458.3 | 1269.3 |      20.12 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.859 ms |  1.00 |  29 | 5985.5 |   97.2 |       1.04 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.693 ms |  1.17 |  29 | 5109.5 |  113.9 |       1.23 KB |        1.19 |
| Sylvan___    | Cols  | 50000   |     8.515 ms |  1.75 |  29 | 3415.7 |  170.3 |       7.68 KB |        7.41 |
| ReadLine_    | Cols  | 50000   |    26.891 ms |  5.53 |  29 | 1081.6 |  537.8 |   88608.31 KB |   85,437.77 |
| CsvHelper    | Cols  | 50000   |   107.289 ms | 22.08 |  29 |  271.1 | 2145.8 |     448.95 KB |      432.89 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    40.048 ms |  1.00 |  29 |  726.3 |  801.0 |   13802.76 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.007 ms |  0.70 |  29 | 1038.5 |  560.1 |   13868.88 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    50.537 ms |  1.26 |  29 |  575.5 | 1010.7 |   13962.28 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   124.559 ms |  3.11 |  29 |  233.5 | 2491.2 |  102134.83 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   126.945 ms |  3.17 |  29 |  229.1 | 2538.9 |      13971 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   844.001 ms |  1.00 | 581 |  689.4 |  844.0 |  266670.18 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   506.846 ms |  0.60 | 581 | 1148.0 |  506.8 |  276119.66 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,027.096 ms |  1.22 | 581 |  566.5 | 1027.1 |  266831.32 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,565.816 ms |  3.04 | 581 |  226.8 | 2565.8 | 2038838.04 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,628.958 ms |  3.12 | 581 |  221.3 | 2629.0 |  266847.05 KB |        1.00 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.449 ms |  1.00 |  29 | 8461.8 |   69.0 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.650 ms |  1.06 |  29 | 7995.5 |   73.0 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.565 ms |  1.03 |  29 | 8184.9 |   71.3 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.536 ms |  1.32 |  29 | 6432.6 |   90.7 |       7.66 KB |        7.50 |
| ReadLine_    | Row   | 50000   |    20.179 ms |  5.85 |  29 | 1446.1 |  403.6 |   88608.27 KB |   86,744.62 |
| CsvHelper    | Row   | 50000   |    62.865 ms | 18.23 |  29 |  464.2 | 1257.3 |      20.07 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.030 ms |  1.00 |  29 | 5801.5 |  100.6 |       1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.806 ms |  1.15 |  29 | 5025.7 |  116.1 |       1.03 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     7.502 ms |  1.49 |  29 | 3889.7 |  150.0 |       7.67 KB |        7.46 |
| ReadLine_    | Cols  | 50000   |    21.399 ms |  4.25 |  29 | 1363.7 |  428.0 |   88608.28 KB |   86,167.97 |
| CsvHelper    | Cols  | 50000   |   109.575 ms | 21.78 |  29 |  266.3 | 2191.5 |     448.88 KB |      436.51 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    52.163 ms |  1.00 |  29 |  559.4 | 1043.3 |   13803.17 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    32.842 ms |  0.63 |  29 |  888.5 |  656.8 |   13920.79 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    58.391 ms |  1.12 |  29 |  499.8 | 1167.8 |   13962.64 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   171.508 ms |  3.30 |  29 |  170.1 | 3430.2 |  102134.46 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   132.390 ms |  2.55 |  29 |  220.4 | 2647.8 |   13971.94 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   992.044 ms |  1.00 | 583 |  588.5 |  992.0 |  266670.83 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   487.203 ms |  0.49 | 583 | 1198.2 |  487.2 |  269058.01 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,195.871 ms |  1.21 | 583 |  488.2 | 1195.9 |  266826.02 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,254.174 ms |  3.28 | 583 |  179.4 | 3254.2 | 2038843.73 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,644.197 ms |  2.67 | 583 |  220.8 | 2644.2 |  266841.13 KB |        1.00 |

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

###### AMD.Ryzen.9.9950X - PackageAssets Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     1.365 ms |  1.00 |  29 | 21384.9 |   27.3 |       1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     1.455 ms |  1.07 |  29 | 20059.6 |   29.1 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     1.399 ms |  1.02 |  29 | 20865.5 |   28.0 |       1.01 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     1.934 ms |  1.42 |  29 | 15085.4 |   38.7 |       7.72 KB |        7.63 |
| ReadLine_    | Row   | 50000   |     8.833 ms |  6.47 |  29 |  3303.7 |  176.7 |   88608.25 KB |   87,581.89 |
| CsvHelper    | Row   | 50000   |    24.072 ms | 17.64 |  29 |  1212.3 |  481.4 |         20 KB |       19.76 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     1.895 ms |  1.00 |  29 | 15399.8 |   37.9 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     2.321 ms |  1.22 |  29 | 12573.8 |   46.4 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     3.141 ms |  1.66 |  29 |  9290.5 |   62.8 |       7.66 KB |        7.54 |
| ReadLine_    | Cols  | 50000   |     9.248 ms |  4.88 |  29 |  3155.2 |  185.0 |   88608.25 KB |   87,245.04 |
| CsvHelper    | Cols  | 50000   |    44.453 ms | 23.46 |  29 |   656.4 |  889.1 |      445.7 KB |      438.84 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    26.486 ms |  1.00 |  29 |  1101.7 |  529.7 |   13802.57 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    16.693 ms |  0.63 |  29 |  1748.1 |  333.9 |   13992.75 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    29.711 ms |  1.12 |  29 |   982.2 |  594.2 |   13963.15 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    82.744 ms |  3.13 |  29 |   352.7 | 1654.9 |  102134.11 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    54.704 ms |  2.07 |  29 |   533.4 | 1094.1 |   13970.84 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   473.474 ms |  1.00 | 583 |  1233.0 |  473.5 |   266668.7 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   207.790 ms |  0.44 | 583 |  2809.5 |  207.8 |  268542.84 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   573.016 ms |  1.21 | 583 |  1018.8 |  573.0 |  266825.02 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,303.550 ms |  2.75 | 583 |   447.8 | 1303.5 | 2038835.55 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,149.642 ms |  2.43 | 583 |   507.8 | 1149.6 |  266841.26 KB |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     4.172 ms |  1.00 |  29 | 6971.8 |   83.4 |        989 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.875 ms |  0.93 |  29 | 7505.5 |   77.5 |       1030 B |        1.04 |
| Sep_Unescape | Row   | 50000   |     3.776 ms |  0.91 |  29 | 7702.2 |   75.5 |        987 B |        1.00 |
| Sylvan___    | Row   | 50000   |    19.368 ms |  4.64 |  29 | 1501.8 |  387.4 |       6958 B |        7.04 |
| ReadLine_    | Row   | 50000   |    18.163 ms |  4.35 |  29 | 1601.4 |  363.3 |   90734887 B |   91,744.07 |
| CsvHelper    | Row   | 50000   |    42.467 ms | 10.18 |  29 |  684.9 |  849.3 |      20764 B |       20.99 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     5.296 ms |  1.00 |  29 | 5491.7 |  105.9 |        994 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.612 ms |  1.06 |  29 | 5182.3 |  112.2 |        995 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    22.012 ms |  4.16 |  29 | 1321.3 |  440.2 |       6725 B |        6.77 |
| ReadLine_    | Cols  | 50000   |    20.804 ms |  3.93 |  29 | 1398.1 |  416.1 |   90734891 B |   91,282.59 |
| CsvHelper    | Cols  | 50000   |    66.289 ms | 12.52 |  29 |  438.8 | 1325.8 |     457440 B |      460.20 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    34.140 ms |  1.01 |  29 |  852.0 |  682.8 |   14134048 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.616 ms |  0.64 |  29 | 1345.6 |  432.3 |   14225818 B |        1.01 |
| Sylvan___    | Asset | 50000   |    53.141 ms |  1.56 |  29 |  547.3 | 1062.8 |   14296896 B |        1.01 |
| ReadLine_    | Asset | 50000   |    86.283 ms |  2.54 |  29 |  337.1 | 1725.7 |  104585846 B |        7.40 |
| CsvHelper    | Asset | 50000   |    75.267 ms |  2.22 |  29 |  386.4 | 1505.3 |   14306376 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   593.732 ms |  1.00 | 581 |  980.0 |  593.7 |  273070232 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   440.534 ms |  0.74 | 581 | 1320.8 |  440.5 |  282222496 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,122.696 ms |  1.89 | 581 |  518.3 | 1122.7 |  273232584 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,988.487 ms |  3.35 | 581 |  292.6 | 1988.5 | 2087769648 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,589.624 ms |  2.68 | 581 |  366.0 | 1589.6 |  273239384 B |        1.00 |


##### PackageAssets Benchmark Results (SERVER GC)
The package assets benchmark (Scope `Asset`) has a very high base load in the
form of the accumulated instances of `PackageAsset` and since Sep is so fast the
GC becomes a significant bottleneck for the benchmark, especially for
multi-threaded parsing. Switching to [SERVER
GC](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/workstation-server-gc)
can, therefore, provide significant speedup as can be seen below.

With `ParallelEnumerate` and server GC Sep is **>4x faster than Sylvan and up to
18x faster than CsvHelper**. Breaking 4 GB/s parsing speed on package assets on
5950X.

###### AMD.EPYC.7763 - PackageAssets Benchmark Results (SERVER GC) (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    33.22 ms |  1.00 |  29 |  875.4 |  664.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    16.78 ms |  0.51 |  29 | 1733.3 |  335.6 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    43.96 ms |  1.33 |  29 |  661.7 |  879.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    59.14 ms |  1.78 |  29 |  491.8 | 1182.7 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   119.47 ms |  3.60 |  29 |  243.5 | 2389.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   677.09 ms |  1.00 | 581 |  859.4 |  677.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   346.13 ms |  0.51 | 581 | 1681.1 |  346.1 |  271.12 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   877.32 ms |  1.30 | 581 |  663.2 |  877.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,223.19 ms |  1.81 | 581 |  475.7 | 1223.2 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,459.94 ms |  3.63 | 581 |  236.5 | 2459.9 |  260.58 MB |        1.00 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets Benchmark Results (SERVER GC) (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.427 ms |  1.00 |  29 |  899.9 |  648.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     9.377 ms |  0.29 |  29 | 3112.1 |  187.5 |   13.57 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    43.724 ms |  1.35 |  29 |  667.4 |  874.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    48.915 ms |  1.51 |  29 |  596.6 |  978.3 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   118.160 ms |  3.64 |  29 |  247.0 | 2363.2 |   13.65 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   680.684 ms |  1.00 | 583 |  857.6 |  680.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   264.669 ms |  0.39 | 583 | 2205.7 |  264.7 |  262.35 MB |        1.01 |
| Sylvan___ | Asset | 1000000 |   910.222 ms |  1.34 | 583 |  641.4 |  910.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,043.395 ms |  1.54 | 583 |  559.5 | 1043.4 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,433.801 ms |  3.58 | 583 |  239.9 | 2433.8 |  260.58 MB |        1.00 |

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

###### AMD.Ryzen.9.9950X - PackageAssets Benchmark Results (SERVER GC) (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    14.536 ms |  1.00 |  29 | 2007.5 |  290.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     3.606 ms |  0.25 |  29 | 8091.8 |   72.1 |   13.65 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    20.457 ms |  1.41 |  29 | 1426.5 |  409.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    20.307 ms |  1.40 |  29 | 1437.0 |  406.1 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    54.022 ms |  3.72 |  29 |  540.2 | 1080.4 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   291.979 ms |  1.00 | 583 | 1999.4 |  292.0 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |    72.213 ms |  0.25 | 583 | 8084.1 |   72.2 |  261.63 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   413.265 ms |  1.42 | 583 | 1412.6 |  413.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   377.033 ms |  1.29 | 583 | 1548.4 |  377.0 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,005.323 ms |  3.44 | 583 |  580.7 | 1005.3 |  260.58 MB |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets Benchmark Results (SERVER GC) (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    29.44 ms |  1.01 |  29 |  988.1 |  588.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    19.53 ms |  0.67 |  29 | 1489.2 |  390.6 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    53.48 ms |  1.83 |  29 |  543.9 | 1069.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    45.04 ms |  1.54 |  29 |  645.7 |  900.9 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    74.14 ms |  2.53 |  29 |  392.3 | 1482.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   495.08 ms |  1.00 | 581 | 1175.3 |  495.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   200.23 ms |  0.40 | 581 | 2906.0 |  200.2 |  268.19 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   965.53 ms |  1.95 | 581 |  602.7 |  965.5 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,068.55 ms |  2.16 | 581 |  544.5 | 1068.5 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,518.90 ms |  3.07 | 581 |  383.1 | 1518.9 |  260.58 MB |        1.00 |


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

###### AMD.EPYC.7763 - PackageAssets with Quotes Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.37 ms |  1.00 |  33 | 3209.0 |  207.4 |       1.06 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    11.98 ms |  1.16 |  33 | 2777.9 |  239.6 |       1.07 KB |        1.01 |
| Sep_Unescape | Row   | 50000   |    10.80 ms |  1.04 |  33 | 3082.6 |  215.9 |       1.06 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    26.61 ms |  2.57 |  33 | 1250.5 |  532.3 |       8.67 KB |        8.18 |
| ReadLine_    | Row   | 50000   |    26.19 ms |  2.53 |  33 | 1270.7 |  523.8 |  108778.81 KB |  102,663.13 |
| CsvHelper    | Row   | 50000   |    78.40 ms |  7.56 |  33 |  424.5 | 1568.0 |      20.28 KB |       19.14 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.72 ms |  1.00 |  33 | 2839.2 |  234.4 |       1.07 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.11 ms |  1.12 |  33 | 2539.4 |  262.1 |       1.07 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    30.91 ms |  2.64 |  33 | 1076.7 |  618.2 |       7.76 KB |        7.27 |
| ReadLine_    | Cols  | 50000   |    27.63 ms |  2.36 |  33 | 1204.5 |  552.6 |  108778.81 KB |  101,818.55 |
| CsvHelper    | Cols  | 50000   |   105.61 ms |  9.01 |  33 |  315.1 | 2112.3 |     445.94 KB |      417.41 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    47.56 ms |  1.00 |  33 |  699.8 |  951.1 |   13802.86 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    31.99 ms |  0.67 |  33 | 1040.4 |  639.8 |   13863.12 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    73.27 ms |  1.54 |  33 |  454.2 | 1465.5 |   13962.34 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   157.87 ms |  3.32 |  33 |  210.8 | 3157.4 |  122305.39 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   125.36 ms |  2.64 |  33 |  265.5 | 2507.2 |   13971.01 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   978.16 ms |  1.00 | 665 |  680.7 |  978.2 |  266670.47 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   570.02 ms |  0.58 | 665 | 1168.0 |  570.0 |  273627.52 KB |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,484.26 ms |  1.52 | 665 |  448.6 | 1484.3 |  266826.25 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,104.63 ms |  3.17 | 665 |  214.5 | 3104.6 | 2442321.76 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,556.98 ms |  2.61 | 665 |  260.4 | 2557.0 |  266839.98 KB |        1.00 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets with Quotes Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    11.24 ms |  1.00 |  33 | 2970.2 |  224.7 |       1.06 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    13.00 ms |  1.16 |  33 | 2567.1 |  260.0 |       1.06 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.34 ms |  0.92 |  33 | 3229.0 |  206.7 |       1.04 KB |        0.99 |
| Sylvan___    | Row   | 50000   |    26.59 ms |  2.37 |  33 | 1255.1 |  531.9 |       7.72 KB |        7.31 |
| ReadLine_    | Row   | 50000   |    24.04 ms |  2.14 |  33 | 1388.3 |  480.8 |  108778.78 KB |  103,042.99 |
| CsvHelper    | Row   | 50000   |    71.28 ms |  6.34 |  33 |  468.3 | 1425.6 |      20.21 KB |       19.14 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    12.42 ms |  1.00 |  33 | 2686.7 |  248.5 |       1.05 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.22 ms |  1.06 |  33 | 2524.0 |  264.5 |       1.07 KB |        1.01 |
| Sylvan___    | Cols  | 50000   |    30.14 ms |  2.43 |  33 | 1107.3 |  602.8 |       7.73 KB |        7.35 |
| ReadLine_    | Cols  | 50000   |    26.19 ms |  2.11 |  33 | 1274.3 |  523.8 |  108778.79 KB |  103,425.70 |
| CsvHelper    | Cols  | 50000   |   101.12 ms |  8.14 |  33 |  330.1 | 2022.4 |     445.86 KB |      423.92 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    59.60 ms |  1.00 |  33 |  560.0 | 1192.0 |   13803.13 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    42.87 ms |  0.72 |  33 |  778.6 |  857.4 |      13947 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    79.36 ms |  1.33 |  33 |  420.6 | 1587.2 |    13962.4 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   197.46 ms |  3.32 |  33 |  169.0 | 3949.2 |  122304.88 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   126.66 ms |  2.13 |  33 |  263.5 | 2533.2 |   13971.72 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,114.98 ms |  1.00 | 667 |  598.9 | 1115.0 |  266678.59 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   666.45 ms |  0.60 | 667 | 1001.9 |  666.4 |   268990.3 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,613.38 ms |  1.45 | 667 |  413.9 | 1613.4 |  266825.61 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,194.02 ms |  3.76 | 667 |  159.2 | 4194.0 | 2442318.66 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,549.93 ms |  2.29 | 667 |  261.9 | 2549.9 |  266834.65 KB |        1.00 |

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

###### AMD.Ryzen.9.9950X - PackageAssets with Quotes Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     4.233 ms |  1.00 |  33 | 7884.4 |   84.7 |       1.03 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     4.577 ms |  1.08 |  33 | 7291.9 |   91.5 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.435 ms |  1.05 |  33 | 7526.1 |   88.7 |       1.03 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    11.399 ms |  2.69 |  33 | 2928.1 |  228.0 |       7.67 KB |        7.48 |
| ReadLine_    | Row   | 50000   |     9.827 ms |  2.32 |  33 | 3396.6 |  196.5 |  108778.75 KB |  106,085.18 |
| CsvHelper    | Row   | 50000   |    27.453 ms |  6.48 |  33 | 1215.8 |  549.1 |         20 KB |       19.51 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.995 ms |  1.00 |  33 | 6681.9 |   99.9 |       1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.515 ms |  1.10 |  33 | 6052.4 |  110.3 |       1.03 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    12.760 ms |  2.55 |  33 | 2615.9 |  255.2 |       7.67 KB |        7.46 |
| ReadLine_    | Cols  | 50000   |    10.853 ms |  2.17 |  33 | 3075.3 |  217.1 |  108778.74 KB |  105,782.94 |
| CsvHelper    | Cols  | 50000   |    41.586 ms |  8.33 |  33 |  802.6 |  831.7 |      445.7 KB |      433.42 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    28.426 ms |  1.00 |  33 | 1174.2 |  568.5 |   13802.71 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    20.467 ms |  0.72 |  33 | 1630.8 |  409.3 |   13993.59 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    38.589 ms |  1.36 |  33 |  864.9 |  771.8 |   13962.21 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   100.029 ms |  3.52 |  33 |  333.7 | 2000.6 |  122305.28 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    52.035 ms |  1.83 |  33 |  641.4 | 1040.7 |   13971.41 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   521.217 ms |  1.00 | 667 | 1281.1 |  521.2 |  266668.73 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   270.971 ms |  0.52 | 667 | 2464.1 |  271.0 |   268699.4 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   776.453 ms |  1.49 | 667 |  859.9 |  776.5 |  266824.44 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,850.722 ms |  3.55 | 667 |  360.8 | 1850.7 | 2442318.88 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,117.789 ms |  2.14 | 667 |  597.3 | 1117.8 |  266833.25 KB |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets with Quotes Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    10.38 ms |  1.00 |  33 | 3206.3 |  207.6 |       1022 B |        1.00 |
| Sep_Async    | Row   | 50000   |    10.45 ms |  1.01 |  33 | 3186.0 |  208.9 |       1021 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.14 ms |  0.98 |  33 | 3282.8 |  202.8 |       1021 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.48 ms |  2.07 |  33 | 1549.7 |  429.5 |       6958 B |        6.81 |
| ReadLine_    | Row   | 50000   |    21.00 ms |  2.02 |  33 | 1584.9 |  420.0 |  111389487 B |  108,991.67 |
| CsvHelper    | Row   | 50000   |    46.86 ms |  4.51 |  33 |  710.2 |  937.3 |      20764 B |       20.32 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.11 ms |  1.00 |  33 | 2994.6 |  222.3 |       1102 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.11 ms |  1.09 |  33 | 2747.4 |  242.3 |       1102 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    24.78 ms |  2.23 |  33 | 1343.1 |  495.6 |       6731 B |        6.11 |
| ReadLine_    | Cols  | 50000   |    22.19 ms |  2.00 |  33 | 1500.2 |  443.7 |  111389493 B |  101,079.39 |
| CsvHelper    | Cols  | 50000   |    71.44 ms |  6.43 |  33 |  465.9 | 1428.8 |     459732 B |      417.18 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    34.79 ms |  1.00 |  33 |  956.6 |  695.8 |   14135314 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    26.01 ms |  0.75 |  33 | 1279.6 |  520.2 |   14209301 B |        1.01 |
| Sylvan___    | Asset | 50000   |    55.38 ms |  1.59 |  33 |  601.0 | 1107.5 |   14297549 B |        1.01 |
| ReadLine_    | Asset | 50000   |   124.17 ms |  3.57 |  33 |  268.0 | 2483.5 |  125240894 B |        8.86 |
| CsvHelper    | Asset | 50000   |    80.01 ms |  2.30 |  33 |  416.0 | 1600.2 |   14306376 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   790.14 ms |  1.00 | 665 |  842.6 |  790.1 |  273070232 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   509.62 ms |  0.65 | 665 | 1306.5 |  509.6 |  280622856 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,180.82 ms |  1.49 | 665 |  563.8 | 1180.8 |  273228792 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,406.08 ms |  3.05 | 665 |  276.7 | 2406.1 | 2500937896 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,711.27 ms |  2.17 | 665 |  389.1 | 1711.3 |  273242512 B |        1.00 |


##### PackageAssets with Quotes Benchmark Results (SERVER GC)
Here again are benchmark results with server garbage collection, which provides
significant speedup over workstation garbage collection.

###### AMD.EPYC.7763 - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.52 ms |  1.00 |  33 |  842.1 |  790.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    21.87 ms |  0.55 |  33 | 1521.7 |  437.4 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    66.35 ms |  1.68 |  33 |  501.6 | 1327.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    64.69 ms |  1.64 |  33 |  514.5 | 1293.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   118.97 ms |  3.01 |  33 |  279.8 | 2379.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   845.73 ms |  1.00 | 665 |  787.3 |  845.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   432.46 ms |  0.51 | 665 | 1539.6 |  432.5 |  262.71 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,343.61 ms |  1.59 | 665 |  495.5 | 1343.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,369.35 ms |  1.62 | 665 |  486.2 | 1369.3 | 2385.08 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,406.94 ms |  2.85 | 665 |  276.6 | 2406.9 |  260.58 MB |        1.00 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.91 ms |  1.00 |  33 |  836.4 |  798.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    18.80 ms |  0.47 |  33 | 1775.4 |  376.0 |   13.58 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    67.17 ms |  1.68 |  33 |  496.9 | 1343.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    67.19 ms |  1.68 |  33 |  496.8 | 1343.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   113.44 ms |  2.84 |  33 |  294.2 | 2268.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   871.42 ms |  1.00 | 667 |  766.2 |  871.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   343.11 ms |  0.39 | 667 | 1946.0 |  343.1 |   261.3 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,358.88 ms |  1.56 | 667 |  491.4 | 1358.9 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,157.77 ms |  1.33 | 667 |  576.7 | 1157.8 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,299.56 ms |  2.64 | 667 |  290.4 | 2299.6 |  260.58 MB |        1.00 |

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

###### AMD.Ryzen.9.9950X - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |  17.916 ms |  1.00 |  33 | 1862.9 |  358.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   6.753 ms |  0.38 |  33 | 4942.7 |  135.1 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  30.678 ms |  1.71 |  33 | 1088.0 |  613.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |  22.998 ms |  1.28 |  33 | 1451.3 |  460.0 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |  47.582 ms |  2.66 |  33 |  701.5 |  951.6 |   13.64 MB |        1.01 |
|           |       |         |            |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 360.848 ms |  1.00 | 667 | 1850.4 |  360.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 143.101 ms |  0.40 | 667 | 4666.0 |  143.1 |  261.69 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 624.089 ms |  1.73 | 667 | 1069.9 |  624.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 419.179 ms |  1.16 | 667 | 1592.9 |  419.2 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 953.606 ms |  2.64 | 667 |  700.2 |  953.6 |  260.58 MB |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    30.08 ms |  1.00 |  33 | 1106.5 |  601.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.52 ms |  0.52 |  33 | 2144.4 |  310.4 |   13.52 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    54.35 ms |  1.81 |  33 |  612.4 | 1086.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    36.99 ms |  1.23 |  33 |  899.8 |  739.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    77.85 ms |  2.59 |  33 |  427.5 | 1556.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   630.57 ms |  1.00 | 665 | 1055.9 |  630.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   287.28 ms |  0.46 | 665 | 2317.6 |  287.3 |  260.89 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,020.17 ms |  1.62 | 665 |  652.6 | 1020.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,232.68 ms |  1.95 | 665 |  540.1 | 1232.7 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,631.39 ms |  2.59 | 665 |  408.1 | 1631.4 |  260.58 MB |        1.00 |


##### PackageAssets with Spaces and Quotes Benchmark Results
Similar to the benchmark related to quotes here spaces ` ` and quotes `"` are
added to relevant columns to benchmark impact of trimming and unescape on low
level column access. That is, basically ` " ` is prepended and appended to each
column. This will test the assumed most common case and fast path part of
trimming and unescaping in Sep. Sep is about 10x faster than CsvHelper for this.
Sylvan does not appear to have support automatic trimming and is, therefore, not
included.

###### AMD.EPYC.7763 - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.76 ms |  1.00 | 41 | 3267.2 |  255.1 |   1.07 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.28 ms |  1.43 | 41 | 2280.0 |  365.6 |    1.1 KB |        1.03 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.16 ms |  1.50 | 41 | 2174.6 |  383.3 |   1.77 KB |        1.65 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.42 ms |  1.60 | 41 | 2040.7 |  408.4 |   1.11 KB |        1.03 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 145.29 ms | 11.39 | 41 |  286.8 | 2905.7 | 451.87 KB |      421.03 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 143.57 ms | 11.26 | 41 |  290.3 | 2871.3 |  446.2 KB |      415.75 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  13.81 ms |  1.00 | 41 | 3023.7 |  276.3 |   1.07 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  19.30 ms |  1.40 | 41 | 2163.7 |  386.1 |   1.09 KB |        1.02 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.84 ms |  1.44 | 41 | 2105.6 |  396.7 |   1.09 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  22.54 ms |  1.63 | 41 | 1853.3 |  450.8 |    1.1 KB |        1.03 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 129.64 ms |  9.39 | 41 |  322.2 | 2592.7 | 454.54 KB |      426.63 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 127.70 ms |  9.25 | 41 |  327.1 | 2554.0 | 445.86 KB |      418.48 |

###### AMD.Ryzen.9.5950X - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.9.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  9.467 ms |  1.00 | 41 | 4412.2 |  189.3 |   1.05 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 12.972 ms |  1.37 | 41 | 3219.9 |  259.4 |   1.06 KB |        1.01 |
| Sep_TrimUnescape           | Cols  | 50000 | 13.630 ms |  1.44 | 41 | 3064.5 |  272.6 |   1.06 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 15.502 ms |  1.64 | 41 | 2694.4 |  310.0 |   1.07 KB |        1.03 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 98.444 ms | 10.40 | 41 |  424.3 | 1968.9 | 451.52 KB |      431.70 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 97.110 ms | 10.26 | 41 |  430.1 | 1942.2 | 445.86 KB |      426.29 |

###### AMD.Ryzen.9.9950X - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  5.235 ms |  1.00 | 41 | 7978.5 |  104.7 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  7.953 ms |  1.52 | 41 | 5252.0 |  159.1 |   1.04 KB |        1.02 |
| Sep_TrimUnescape           | Cols  | 50000 |  8.544 ms |  1.63 | 41 | 4888.7 |  170.9 |   1.04 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  9.254 ms |  1.77 | 41 | 4513.7 |  185.1 |   1.05 KB |        1.02 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 62.406 ms | 11.92 | 41 |  669.3 | 1248.1 | 451.34 KB |      440.58 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 61.412 ms | 11.73 | 41 |  680.2 | 1228.2 | 445.72 KB |      435.10 |

###### Apple.M1.(Virtual) - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 12.21 ms |  1.00 | 41 | 3413.5 |  244.2 |   1.09 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 16.12 ms |  1.32 | 41 | 2584.8 |  322.5 |   1.19 KB |        1.09 |
| Sep_TrimUnescape           | Cols  | 50000 | 16.54 ms |  1.35 | 41 | 2519.4 |  330.8 |   1.23 KB |        1.13 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 20.24 ms |  1.66 | 41 | 2059.5 |  404.7 |   1.04 KB |        0.96 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 88.79 ms |  7.27 | 41 |  469.3 | 1775.8 |  451.6 KB |      415.49 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 87.00 ms |  7.13 | 41 |  479.0 | 1740.0 | 445.93 KB |      410.27 |


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

##### AMD.EPYC.7763 - FloatsReader Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.909 ms |  1.00 | 20 | 6968.1 |  116.4 |     1.26 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.565 ms |  1.23 | 20 | 5687.1 |  142.6 |    10.71 KB |        8.53 |
| ReadLine_ | Row    | 25000 |  18.022 ms |  6.19 | 20 | 1124.9 |  720.9 | 73489.67 KB |   58,517.44 |
| CsvHelper | Row    | 25000 |  37.309 ms | 12.82 | 20 |  543.4 | 1492.3 |    20.06 KB |       15.97 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.065 ms |  1.00 | 20 | 4986.9 |  162.6 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.151 ms |  1.51 | 20 | 3295.9 |  246.0 |     10.8 KB |        8.55 |
| ReadLine_ | Cols   | 25000 |  19.595 ms |  4.82 | 20 | 1034.6 |  783.8 | 73489.68 KB |   58,155.67 |
| CsvHelper | Cols   | 25000 |  40.218 ms |  9.89 | 20 |  504.1 | 1608.7 | 21340.28 KB |   16,887.52 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  30.939 ms |  1.00 | 20 |  655.2 | 1237.6 |     8.08 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  13.164 ms |  0.43 | 20 | 1540.0 |  526.5 |    68.56 KB |        8.49 |
| Sylvan___ | Floats | 25000 |  87.688 ms |  2.83 | 20 |  231.2 | 3507.5 |    18.96 KB |        2.35 |
| ReadLine_ | Floats | 25000 | 113.903 ms |  3.68 | 20 |  178.0 | 4556.1 |  73493.2 KB |    9,101.10 |
| CsvHelper | Floats | 25000 | 160.055 ms |  5.17 | 20 |  126.7 | 6402.2 | 22062.48 KB |    2,732.13 |

##### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - FloatsReader Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.119 ms |  1.00 | 20 | 6514.3 |  124.8 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.690 ms |  1.18 | 20 | 5506.9 |  147.6 |    10.71 KB |        8.53 |
| ReadLine_ | Row    | 25000 |  16.859 ms |  5.40 | 20 | 1205.3 |  674.4 | 73489.66 KB |   58,562.97 |
| CsvHelper | Row    | 25000 |  39.719 ms | 12.73 | 20 |  511.6 | 1588.8 |    20.03 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.215 ms |  1.00 | 20 | 4821.0 |  168.6 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.074 ms |  1.44 | 20 | 3345.4 |  243.0 |    10.71 KB |        8.52 |
| ReadLine_ | Cols   | 25000 |  17.072 ms |  4.05 | 20 | 1190.3 |  682.9 | 73489.65 KB |   58,471.95 |
| CsvHelper | Cols   | 25000 |  42.474 ms | 10.08 | 20 |  478.4 | 1699.0 | 21340.26 KB |   16,979.35 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  32.362 ms |  1.00 | 20 |  627.9 | 1294.5 |     8.32 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   5.940 ms |  0.18 | 20 | 3420.6 |  237.6 |   114.06 KB |       13.71 |
| Sylvan___ | Floats | 25000 |  81.574 ms |  2.52 | 20 |  249.1 | 3263.0 |    18.88 KB |        2.27 |
| ReadLine_ | Floats | 25000 | 106.297 ms |  3.28 | 20 |  191.2 | 4251.9 | 73493.12 KB |    8,837.12 |
| CsvHelper | Floats | 25000 | 156.658 ms |  4.84 | 20 |  129.7 | 6266.3 | 22062.72 KB |    2,652.91 |

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

##### AMD.Ryzen.9.9950X - FloatsReader Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |  1.371 ms |  1.00 | 20 | 14824.5 |   54.8 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.715 ms |  1.25 | 20 | 11845.1 |   68.6 |     10.7 KB |        8.59 |
| ReadLine_ | Row    | 25000 |  7.060 ms |  5.15 | 20 |  2878.2 |  282.4 | 73489.63 KB |   58,976.01 |
| CsvHelper | Row    | 25000 | 15.281 ms | 11.15 | 20 |  1329.7 |  611.2 |    19.96 KB |       16.02 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |  1.764 ms |  1.00 | 20 | 11520.0 |   70.6 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.675 ms |  1.52 | 20 |  7596.7 |  107.0 |     10.7 KB |        8.59 |
| ReadLine_ | Cols   | 25000 |  8.170 ms |  4.63 | 20 |  2487.0 |  326.8 | 73489.63 KB |   58,976.01 |
| CsvHelper | Cols   | 25000 | 16.694 ms |  9.47 | 20 |  1217.2 |  667.8 |  21340.2 KB |   17,125.68 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Floats | 25000 | 16.182 ms |  1.00 | 20 |  1255.7 |  647.3 |     7.94 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.497 ms |  0.15 | 20 |  8136.8 |   99.9 |   179.81 KB |       22.64 |
| Sylvan___ | Floats | 25000 | 38.800 ms |  2.40 | 20 |   523.7 | 1552.0 |    18.72 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 54.117 ms |  3.34 | 20 |   375.5 | 2164.7 | 73493.05 KB |    9,253.27 |
| CsvHelper | Floats | 25000 | 71.601 ms |  4.42 | 20 |   283.8 | 2864.1 | 22061.55 KB |    2,777.70 |

##### Apple.M1.(Virtual) - FloatsReader Benchmark Results (Sep 0.10.0.0, Sylvan  1.4.1.0, CsvHelper 33.0.1.24)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.885 ms |  1.00 | 20 | 5218.3 |  155.4 |     1.21 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  17.967 ms |  4.62 | 20 | 1128.3 |  718.7 |    10.37 KB |        8.54 |
| ReadLine_ | Row    | 25000 |  14.275 ms |  3.67 | 20 | 1420.1 |  571.0 | 73489.65 KB |   60,493.09 |
| CsvHelper | Row    | 25000 |  27.800 ms |  7.16 | 20 |  729.2 | 1112.0 |    20.28 KB |       16.69 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.763 ms |  1.00 | 20 | 4256.3 |  190.5 |     1.21 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  20.663 ms |  4.34 | 20 |  981.1 |  826.5 |    10.62 KB |        8.74 |
| ReadLine_ | Cols   | 25000 |  15.147 ms |  3.18 | 20 | 1338.4 |  605.9 | 73489.65 KB |   60,493.09 |
| CsvHelper | Cols   | 25000 |  29.583 ms |  6.21 | 20 |  685.3 | 1183.3 |  21340.5 KB |   17,566.45 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  24.192 ms |  1.00 | 20 |  838.0 |  967.7 |     8.34 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.410 ms |  0.39 | 20 | 2154.4 |  376.4 |    77.88 KB |        9.34 |
| Sylvan___ | Floats | 25000 |  69.948 ms |  2.89 | 20 |  289.8 | 2797.9 |    18.57 KB |        2.23 |
| ReadLine_ | Floats | 25000 |  78.990 ms |  3.27 | 20 |  256.6 | 3159.6 |  73493.2 KB |    8,816.43 |
| CsvHelper | Floats | 25000 | 103.248 ms |  4.27 | 20 |  196.3 | 4129.9 | 22063.34 KB |    2,646.77 |


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
