# Sep - ~~Possibly~~ the World's Fastest .NET CSV Parser
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

Modern, minimal, fast, zero allocation, reading and writing of separated values
(`csv`, `tsv` etc.). Cross-platform, trimmable and AOT/NativeAOT compatible.
Featuring an opinionated API design and pragmatic implementation targetted at
machine learning use cases.

⭐ Please star this project if you like it. ⭐

* **🌃  Modern** - utilizes features such as
[`Span<T>`](https://learn.microsoft.com/en-us/archive/msdn-magazine/2018/january/csharp-all-about-span-exploring-a-new-net-mainstay),
[Generic Math](https://devblogs.microsoft.com/dotnet/dotnet-7-generic-math/)
([`ISpanParsable<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.ispanparsable-1)/
[`ISpanFormattable`](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable)), [`ref
struct`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/ref-struct),
[`ArrayPool<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.arraypool-1)
and similar from [.NET 7+ and C#
11+](https://nietras.com/2022/11/26/dotnet-and-csharp-versions/) for a modern
and highly efficient implementation.
* **🔎 Minimal** - a succinct yet expressive API with few options and no hidden
changes to input or output. What you read/write is what you get. E.g. by default
there is no "automatic" escaping/unescaping of quotes or trimming of spaces. To
enable this see [SepReaderOptions](#sepreaderoptions) and
[Unescaping](#unescaping) and [Trimming](#trimming). See
[SepWriterOptions](#sepwriteroptions) for [Escaping](#escaping).
* **🚀 Fast** - blazing fast with both architecture specific and cross-platform
SIMD vectorized parsing incl. 64/128/256/512-bit paths e.g. AVX2, AVX-512 (.NET
8.0+), NEON. Uses [csFastFloat](https://github.com/CarlVerret/csFastFloat) for
fast parsing of floating points. Reads or writes one row at a time efficiently
with [detailed benchmarks](#comparison-benchmarks) to prove it.
* **🌪️ Multi-threaded** - unparalleled speed with highly efficient parallel CSV
  parsing that is [up to 35x faster than
  CsvHelper](#floats-reader-comparison-benchmarks), see
  [ParallelEnumerate](#parallelenumerate-and-enumerate) and
  [benchmarks](#comparison-benchmarks) .
* **🗑️ Zero allocation** - intelligent and efficient memory management allowing
for zero allocations after warmup incl. supporting use cases of reading or
writing arrays of values (e.g. features) easily without repeated allocations.
* **✅ Thorough tests** - great code coverage and focus on edge case testing
  incl. randomized [fuzz testing](https://en.wikipedia.org/wiki/Fuzzing).
* **🌐 Cross-platform** - works on any platform, any architecture supported by
 .NET. 100% managed and written in beautiful modern C#.
* **✂️ Trimmable and AOT/NativeAOT compatible** - no problematic reflection or
dynamic code generation. Hence, fully
[trimmable](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/prepare-libraries-for-trimming)
and
[Ahead-of-Time](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
compatible. With a simple console tester program executable possible in just a
few MBs. 💾
* **🗣️ Opinionated and pragmatic** - conforms to the essentials of
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
   This does not allow skipping a header row starting with `#` though.

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

###### AMD.EPYC.7763 - PackageAssets Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.582 ms |  1.00 |  29 | 8120.5 |   71.6 |       1.03 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.568 ms |  1.00 |  29 | 8151.2 |   71.4 |       1.03 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.468 ms |  1.25 |  29 | 6510.0 |   89.4 |       7.66 KB |        7.47 |
| ReadLine_    | Row   | 50000   |    49.799 ms | 13.91 |  29 |  584.1 |  996.0 |   88608.38 KB |   86,414.26 |
| CsvHelper    | Row   | 50000   |    66.445 ms | 18.55 |  29 |  437.7 | 1328.9 |       23.3 KB |       22.72 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.848 ms |  1.00 |  29 | 5999.4 |   97.0 |        1.2 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.710 ms |  1.18 |  29 | 5093.7 |  114.2 |       1.04 KB |        0.86 |
| Sylvan___    | Cols  | 50000   |     8.223 ms |  1.70 |  29 | 3537.2 |  164.5 |       7.68 KB |        6.41 |
| ReadLine_    | Cols  | 50000   |    25.293 ms |  5.22 |  29 | 1150.0 |  505.9 |   88608.32 KB |   73,888.37 |
| CsvHelper    | Cols  | 50000   |   109.877 ms | 22.66 |  29 |  264.7 | 2197.5 |     445.93 KB |      371.85 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    30.823 ms |  1.00 |  29 |  943.6 |  616.5 |   13802.34 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    23.560 ms |  0.76 |  29 | 1234.6 |  471.2 |   13852.34 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    47.591 ms |  1.54 |  29 |  611.2 |  951.8 |   13961.85 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   116.425 ms |  3.78 |  29 |  249.8 | 2328.5 |  102133.09 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   120.019 ms |  3.89 |  29 |  242.3 | 2400.4 |   13970.29 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   797.462 ms |  1.00 | 581 |  729.7 |  797.5 |  266669.05 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   507.024 ms |  0.64 | 581 | 1147.6 |  507.0 |  275457.14 KB |        1.03 |
| Sylvan___    | Asset | 1000000 |   991.891 ms |  1.25 | 581 |  586.6 |  991.9 |  266826.74 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,259.026 ms |  4.09 | 581 |  178.5 | 3259.0 | 2038846.35 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,527.438 ms |  3.17 | 581 |  230.2 | 2527.4 |  266850.81 KB |        1.00 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     4.200 ms |  1.00 |  29 | 6948.2 |   84.0 |      1.18 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.236 ms |  1.01 |  29 | 6888.6 |   84.7 |      1.33 KB |        1.13 |
| Sylvan___    | Row   | 50000   |     4.491 ms |  1.07 |  29 | 6497.9 |   89.8 |      7.66 KB |        6.49 |
| ReadLine_    | Row   | 50000   |    21.321 ms |  5.08 |  29 | 1368.7 |  426.4 |  88608.28 KB |   75,049.53 |
| CsvHelper    | Row   | 50000   |    62.781 ms | 14.95 |  29 |  464.8 | 1255.6 |      20.2 KB |       17.11 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     5.852 ms |  1.00 |  29 | 4986.3 |  117.0 |      1.19 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.573 ms |  1.12 |  29 | 4439.6 |  131.5 |      1.19 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.553 ms |  1.46 |  29 | 3411.8 |  171.1 |      7.67 KB |        6.44 |
| ReadLine_    | Cols  | 50000   |    22.541 ms |  3.85 |  29 | 1294.6 |  450.8 |  88608.29 KB |   74,372.86 |
| CsvHelper    | Cols  | 50000   |   108.328 ms | 18.51 |  29 |  269.4 | 2166.6 |    445.78 KB |      374.16 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    53.003 ms |  1.00 |  29 |  550.6 | 1060.1 |  13803.34 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    38.829 ms |  0.74 |  29 |  751.5 |  776.6 |  13913.62 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    70.050 ms |  1.33 |  29 |  416.6 | 1401.0 |   13962.4 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   233.476 ms |  4.42 |  29 |  125.0 | 4669.5 | 102134.79 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   133.896 ms |  2.54 |  29 |  217.9 | 2677.9 |  13972.19 KB |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   993.034 ms |  1.00 | 583 |  587.9 |  993.0 | 266670.95 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   504.125 ms |  0.51 | 583 | 1158.0 |  504.1 | 268509.69 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,206.663 ms |  1.22 | 583 |  483.8 | 1206.7 | 266825.98 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,256.930 ms |  3.28 | 583 |  179.2 | 3256.9 |   2038848 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,657.874 ms |  2.68 | 583 |  219.6 | 2657.9 | 266843.54 KB |        1.00 |

###### AMD.Ryzen.9.5950X - PackageAssets Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     2.188 ms |  1.00 |  29 | 13339.9 |   43.8 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     2.214 ms |  1.01 |  29 | 13181.4 |   44.3 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     2.825 ms |  1.29 |  29 | 10328.7 |   56.5 |       7.66 KB |        7.53 |
| ReadLine_    | Row   | 50000   |    11.707 ms |  5.35 |  29 |  2492.7 |  234.1 |   88608.25 KB |   87,161.24 |
| CsvHelper    | Row   | 50000   |    43.779 ms | 20.01 |  29 |   666.6 |  875.6 |      20.04 KB |       19.71 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     3.210 ms |  1.00 |  29 |  9091.8 |   64.2 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     3.856 ms |  1.20 |  29 |  7567.4 |   77.1 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     5.072 ms |  1.58 |  29 |  5753.7 |  101.4 |       7.66 KB |        7.51 |
| ReadLine_    | Cols  | 50000   |    13.080 ms |  4.08 |  29 |  2230.9 |  261.6 |   88608.25 KB |   86,827.61 |
| CsvHelper    | Cols  | 50000   |    71.032 ms | 22.13 |  29 |   410.8 | 1420.6 |     445.86 KB |      436.90 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    32.075 ms |  1.00 |  29 |   909.8 |  641.5 |   13802.45 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    19.243 ms |  0.60 |  29 |  1516.5 |  384.9 |   13993.52 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    34.429 ms |  1.07 |  29 |   847.6 |  688.6 |   13963.52 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    95.729 ms |  2.99 |  29 |   304.8 | 1914.6 |   102133.9 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    82.784 ms |  2.58 |  29 |   352.5 | 1655.7 |   13970.93 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   627.827 ms |  1.00 | 583 |   929.8 |  627.8 |  266680.78 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   254.075 ms |  0.40 | 583 |  2297.7 |  254.1 |   267692.5 KB |        1.00 |
| Sylvan___    | Asset | 1000000 |   736.667 ms |  1.17 | 583 |   792.5 |  736.7 |  266825.22 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,585.514 ms |  2.53 | 583 |   368.2 | 1585.5 | 2038834.79 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,661.220 ms |  2.65 | 583 |   351.4 | 1661.2 |   266833.3 KB |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     3.761 ms |  1.00 |  29 | 7732.9 |   75.2 |        988 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.202 ms |  1.12 |  29 | 6921.5 |   84.0 |       1033 B |        1.05 |
| Sylvan___    | Row   | 50000   |    19.616 ms |  5.22 |  29 | 1482.7 |  392.3 |       6958 B |        7.04 |
| ReadLine_    | Row   | 50000   |    17.984 ms |  4.78 |  29 | 1617.3 |  359.7 |   90734887 B |   91,836.93 |
| CsvHelper    | Row   | 50000   |    42.806 ms | 11.38 |  29 |  679.5 |  856.1 |      20764 B |       21.02 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     5.242 ms |  1.00 |  29 | 5548.3 |  104.8 |        995 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.306 ms |  1.01 |  29 | 5481.3 |  106.1 |        994 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    22.184 ms |  4.23 |  29 | 1311.1 |  443.7 |       6958 B |        6.99 |
| ReadLine_    | Cols  | 50000   |    19.444 ms |  3.71 |  29 | 1495.9 |  388.9 |   90734891 B |   91,190.85 |
| CsvHelper    | Cols  | 50000   |    66.750 ms | 12.73 |  29 |  435.7 | 1335.0 |     456636 B |      458.93 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    32.905 ms |  1.00 |  29 |  883.9 |  658.1 |   14134044 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.284 ms |  0.65 |  29 | 1366.5 |  425.7 |   14236153 B |        1.01 |
| Sylvan___    | Asset | 50000   |    52.459 ms |  1.60 |  29 |  554.4 | 1049.2 |   14296308 B |        1.01 |
| ReadLine_    | Asset | 50000   |    90.785 ms |  2.77 |  29 |  320.4 | 1815.7 |  104586002 B |        7.40 |
| CsvHelper    | Asset | 50000   |    79.577 ms |  2.43 |  29 |  365.5 | 1591.5 |   14305850 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   586.057 ms |  1.00 | 581 |  992.9 |  586.1 |  273070336 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   450.524 ms |  0.77 | 581 | 1291.6 |  450.5 |  283959880 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,054.994 ms |  1.80 | 581 |  551.5 | 1055.0 |  273242248 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,834.688 ms |  3.13 | 581 |  317.2 | 1834.7 | 2087769376 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,561.048 ms |  2.66 | 581 |  372.7 | 1561.0 |  273242536 B |        1.00 |


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

###### AMD.EPYC.7763 - PackageAssets Benchmark Results (SERVER GC) (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.39 ms |  1.00 |  29 |  898.1 |  647.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.89 ms |  0.46 |  29 | 1953.6 |  297.8 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    41.94 ms |  1.30 |  29 |  693.5 |  838.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    37.66 ms |  1.16 |  29 |  772.3 |  753.3 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   121.64 ms |  3.76 |  29 |  239.1 | 2432.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   620.61 ms |  1.00 | 581 |  937.6 |  620.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   287.75 ms |  0.46 | 581 | 2022.2 |  287.7 |  269.46 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   828.44 ms |  1.33 | 581 |  702.4 |  828.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,307.41 ms |  2.11 | 581 |  445.1 | 1307.4 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,355.53 ms |  3.80 | 581 |  247.0 | 2355.5 |  260.58 MB |        1.00 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets Benchmark Results (SERVER GC) (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.87 ms |  1.00 |  29 |  887.7 |  657.4 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.08 ms |  0.37 |  29 | 2415.5 |  241.6 |   13.57 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    44.10 ms |  1.34 |  29 |  661.7 |  881.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    46.91 ms |  1.43 |  29 |  622.1 |  938.2 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   117.89 ms |  3.59 |  29 |  247.5 | 2357.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   712.78 ms |  1.00 | 583 |  819.0 |  712.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   280.21 ms |  0.39 | 583 | 2083.3 |  280.2 |   261.5 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   911.34 ms |  1.28 | 583 |  640.6 |  911.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,080.48 ms |  1.52 | 583 |  540.3 | 1080.5 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,393.86 ms |  3.37 | 583 |  243.9 | 2393.9 |  260.58 MB |        1.00 |

###### AMD.Ryzen.9.5950X - PackageAssets Benchmark Results (SERVER GC) (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    20.739 ms |  1.00 |  29 | 1407.1 |  414.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     6.042 ms |  0.29 |  29 | 4829.6 |  120.8 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    27.858 ms |  1.34 |  29 | 1047.5 |  557.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    32.844 ms |  1.58 |  29 |  888.5 |  656.9 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    76.303 ms |  3.68 |  29 |  382.4 | 1526.1 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   435.624 ms |  1.00 | 583 | 1340.1 |  435.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   116.180 ms |  0.27 | 583 | 5024.8 |  116.2 |   261.3 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   558.898 ms |  1.28 | 583 | 1044.5 |  558.9 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   573.815 ms |  1.32 | 583 | 1017.4 |  573.8 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,504.212 ms |  3.45 | 583 |  388.1 | 1504.2 |  260.58 MB |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets Benchmark Results (SERVER GC) (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    26.67 ms |  1.00 |  29 | 1090.7 |  533.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    20.88 ms |  0.78 |  29 | 1393.1 |  417.6 |   13.66 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    51.56 ms |  1.93 |  29 |  564.2 | 1031.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    44.37 ms |  1.67 |  29 |  655.5 |  887.5 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    80.94 ms |  3.04 |  29 |  359.4 | 1618.7 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   547.04 ms |  1.00 | 581 | 1063.7 |  547.0 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   227.53 ms |  0.42 | 581 | 2557.4 |  227.5 |  266.03 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,058.09 ms |  1.93 | 581 |  549.9 | 1058.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,105.75 ms |  2.02 | 581 |  526.2 | 1105.8 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,588.28 ms |  2.90 | 581 |  366.4 | 1588.3 |  260.58 MB |        1.00 |


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

###### AMD.EPYC.7763 - PackageAssets with Quotes Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.54 ms |  1.00 |  33 | 3159.0 |  210.7 |       1.06 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.80 ms |  1.02 |  33 | 3083.0 |  215.9 |       1.06 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    26.19 ms |  2.49 |  33 | 1270.9 |  523.7 |       7.74 KB |        7.30 |
| ReadLine_    | Row   | 50000   |    28.51 ms |  2.71 |  33 | 1167.2 |  570.3 |  108778.83 KB |  102,568.62 |
| CsvHelper    | Row   | 50000   |    76.92 ms |  7.30 |  33 |  432.7 | 1538.3 |      20.28 KB |       19.12 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.63 ms |  1.00 |  33 | 2860.6 |  232.7 |       1.07 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.58 ms |  1.17 |  33 | 2450.1 |  271.7 |       1.08 KB |        1.01 |
| Sylvan___    | Cols  | 50000   |    30.23 ms |  2.60 |  33 | 1100.8 |  604.7 |       7.76 KB |        7.25 |
| ReadLine_    | Cols  | 50000   |    29.64 ms |  2.55 |  33 | 1122.7 |  592.9 |  108778.84 KB |  101,540.13 |
| CsvHelper    | Cols  | 50000   |   105.52 ms |  9.07 |  33 |  315.4 | 2110.3 |     445.86 KB |      416.19 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    38.31 ms |  1.00 |  33 |  868.8 |  766.1 |   13802.34 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    26.27 ms |  0.69 |  33 | 1266.9 |  525.4 |    13864.9 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    62.31 ms |  1.63 |  33 |  534.1 | 1246.3 |   13964.99 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   138.98 ms |  3.63 |  33 |  239.5 | 2779.6 |  122303.89 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   117.01 ms |  3.05 |  33 |  284.4 | 2340.3 |   13970.29 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   958.41 ms |  1.00 | 665 |  694.7 |  958.4 |  266669.05 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   609.74 ms |  0.64 | 665 | 1091.9 |  609.7 |  269433.56 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,452.68 ms |  1.52 | 665 |  458.3 | 1452.7 |  266827.02 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,888.60 ms |  4.06 | 665 |  171.2 | 3888.6 | 2442326.16 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,539.78 ms |  2.65 | 665 |  262.1 | 2539.8 |  266834.94 KB |        1.00 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets with Quotes Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.531 ms |  1.00 |  33 | 3169.5 |  210.6 |       1.21 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     9.910 ms |  0.94 |  33 | 3368.2 |  198.2 |       1.21 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    26.180 ms |  2.49 |  33 | 1274.9 |  523.6 |       7.72 KB |        6.38 |
| ReadLine_    | Row   | 50000   |    24.019 ms |  2.28 |  33 | 1389.6 |  480.4 |  108778.78 KB |   89,902.72 |
| CsvHelper    | Row   | 50000   |    71.228 ms |  6.76 |  33 |  468.6 | 1424.6 |       20.2 KB |       16.70 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.933 ms |  1.00 |  33 | 2797.1 |  238.7 |       1.22 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.486 ms |  1.13 |  33 | 2474.9 |  269.7 |       1.22 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    29.535 ms |  2.48 |  33 | 1130.1 |  590.7 |       7.73 KB |        6.35 |
| ReadLine_    | Cols  | 50000   |    26.438 ms |  2.22 |  33 | 1262.5 |  528.8 |  108778.78 KB |   89,397.65 |
| CsvHelper    | Cols  | 50000   |   104.426 ms |  8.75 |  33 |  319.6 | 2088.5 |     445.78 KB |      366.36 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    57.695 ms |  1.00 |  33 |  578.5 | 1153.9 |    13803.3 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    47.415 ms |  0.82 |  33 |  703.9 |  948.3 |   13941.84 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    92.123 ms |  1.60 |  33 |  362.3 | 1842.5 |   13962.42 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   196.126 ms |  3.40 |  33 |  170.2 | 3922.5 |  122305.15 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   127.767 ms |  2.22 |  33 |  261.2 | 2555.3 |   13972.13 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,129.007 ms |  1.00 | 667 |  591.4 | 1129.0 |  266670.93 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   674.504 ms |  0.60 | 667 |  989.9 |  674.5 |  268208.37 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,656.453 ms |  1.47 | 667 |  403.1 | 1656.5 |  266825.34 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,269.727 ms |  3.78 | 667 |  156.4 | 4269.7 | 2442331.02 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,581.438 ms |  2.29 | 667 |  258.7 | 2581.4 |  266834.02 KB |        1.00 |

###### AMD.Ryzen.9.5950X - PackageAssets with Quotes Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     7.077 ms |  1.00 |  33 | 4716.4 |  141.5 |       1.04 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     6.916 ms |  0.98 |  33 | 4826.4 |  138.3 |       1.04 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    16.794 ms |  2.37 |  33 | 1987.5 |  335.9 |       7.69 KB |        7.42 |
| ReadLine_    | Row   | 50000   |    14.068 ms |  1.99 |  33 | 2372.5 |  281.4 |  108778.74 KB |  104,886.47 |
| CsvHelper    | Row   | 50000   |    52.336 ms |  7.40 |  33 |  637.7 | 1046.7 |      20.01 KB |       19.29 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     7.709 ms |  1.00 |  33 | 4329.4 |  154.2 |       1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     8.991 ms |  1.17 |  33 | 3712.1 |  179.8 |       1.04 KB |        1.01 |
| Sylvan___    | Cols  | 50000   |    19.529 ms |  2.53 |  33 | 1709.2 |  390.6 |        7.7 KB |        7.47 |
| ReadLine_    | Cols  | 50000   |    15.205 ms |  1.97 |  33 | 2195.1 |  304.1 |  108778.76 KB |  105,482.43 |
| CsvHelper    | Cols  | 50000   |    70.755 ms |  9.18 |  33 |  471.7 | 1415.1 |     448.87 KB |      435.27 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    37.129 ms |  1.00 |  33 |  899.0 |  742.6 |   13803.03 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.253 ms |  0.57 |  33 | 1570.5 |  425.1 |   13981.42 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    48.990 ms |  1.32 |  33 |  681.3 |  979.8 |   13962.13 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   118.684 ms |  3.20 |  33 |  281.2 | 2373.7 |  122305.02 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    83.758 ms |  2.26 |  33 |  398.5 | 1675.2 |   13970.38 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   707.538 ms |  1.00 | 667 |  943.7 |  707.5 |  266668.99 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   349.710 ms |  0.49 | 667 | 1909.3 |  349.7 |  268243.91 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,022.358 ms |  1.44 | 667 |  653.1 | 1022.4 |  266825.07 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,210.209 ms |  3.12 | 667 |  302.1 | 2210.2 | 2442320.25 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,709.222 ms |  2.42 | 667 |  390.6 | 1709.2 |   266833.3 KB |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets with Quotes Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    10.03 ms |  1.00 |  33 | 3318.2 |  200.6 |       1018 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.06 ms |  1.00 |  33 | 3309.9 |  201.1 |       1093 B |        1.07 |
| Sylvan___    | Row   | 50000   |    21.10 ms |  2.10 |  33 | 1577.5 |  422.0 |       6958 B |        6.83 |
| ReadLine_    | Row   | 50000   |    20.90 ms |  2.08 |  33 | 1592.2 |  418.1 |  111389487 B |  109,419.93 |
| CsvHelper    | Row   | 50000   |    46.54 ms |  4.64 |  33 |  715.1 |  930.8 |      20764 B |       20.40 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.19 ms |  1.00 |  33 | 2973.6 |  223.8 |       1026 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.09 ms |  1.08 |  33 | 2752.0 |  241.9 |       1127 B |        1.10 |
| Sylvan___    | Cols  | 50000   |    24.91 ms |  2.23 |  33 | 1336.1 |  498.2 |       6958 B |        6.78 |
| ReadLine_    | Cols  | 50000   |    21.52 ms |  1.92 |  33 | 1546.3 |  430.5 |  111389493 B |  108,566.76 |
| CsvHelper    | Cols  | 50000   |    71.19 ms |  6.36 |  33 |  467.5 | 1423.8 |     459732 B |      448.08 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    34.77 ms |  1.00 |  33 |  957.1 |  695.4 |   14134032 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    26.38 ms |  0.76 |  33 | 1261.6 |  527.6 |   14200790 B |        1.00 |
| Sylvan___    | Asset | 50000   |    55.33 ms |  1.59 |  33 |  601.5 | 1106.6 |   14297352 B |        1.01 |
| ReadLine_    | Asset | 50000   |   129.30 ms |  3.72 |  33 |  257.4 | 2586.0 |  125240862 B |        8.86 |
| CsvHelper    | Asset | 50000   |    84.84 ms |  2.44 |  33 |  392.3 | 1696.8 |   14305848 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   812.54 ms |  1.00 | 665 |  819.4 |  812.5 |  273070232 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   517.67 ms |  0.64 | 665 | 1286.2 |  517.7 |  280315208 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,208.02 ms |  1.49 | 665 |  551.2 | 1208.0 |  273235192 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,483.07 ms |  3.06 | 665 |  268.1 | 2483.1 | 2500937776 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,769.26 ms |  2.18 | 665 |  376.3 | 1769.3 |  273246256 B |        1.00 |


##### PackageAssets with Quotes Benchmark Results (SERVER GC)
Here again are benchmark results with server garbage collection, which provides
significant speedup over workstation garbage collection.

###### AMD.EPYC.7763 - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    40.61 ms |  1.00 |  33 |  819.6 |  812.1 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    20.94 ms |  0.52 |  33 | 1589.1 |  418.9 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    65.44 ms |  1.61 |  33 |  508.6 | 1308.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    41.99 ms |  1.03 |  33 |  792.6 |  839.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   120.35 ms |  2.96 |  33 |  276.6 | 2406.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   775.60 ms |  1.00 | 665 |  858.4 |  775.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   386.10 ms |  0.50 | 665 | 1724.4 |  386.1 |  263.07 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,276.57 ms |  1.65 | 665 |  521.6 | 1276.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,451.32 ms |  1.87 | 665 |  458.8 | 1451.3 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,331.93 ms |  3.01 | 665 |  285.5 | 2331.9 |  260.58 MB |        1.00 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.42 ms |  1.00 |  33 |  846.8 |  788.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    23.61 ms |  0.60 |  33 | 1413.5 |  472.3 |   13.59 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    65.68 ms |  1.67 |  33 |  508.2 | 1313.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    83.86 ms |  2.13 |  33 |  398.0 | 1677.2 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   112.64 ms |  2.86 |  33 |  296.3 | 2252.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   853.65 ms |  1.00 | 667 |  782.2 |  853.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   383.82 ms |  0.45 | 667 | 1739.6 |  383.8 |  261.86 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,351.84 ms |  1.58 | 667 |  493.9 | 1351.8 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,173.87 ms |  1.38 | 667 |  568.8 | 1173.9 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,305.37 ms |  2.70 | 667 |  289.6 | 2305.4 |  260.58 MB |        1.00 |

###### AMD.Ryzen.9.5950X - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    25.49 ms |  1.00 |  33 | 1309.5 |  509.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.02 ms |  0.43 |  33 | 3027.8 |  220.5 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    42.03 ms |  1.65 |  33 |  794.1 |  840.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    38.66 ms |  1.52 |  33 |  863.3 |  773.2 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    78.18 ms |  3.07 |  33 |  426.9 | 1563.7 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   519.82 ms |  1.00 | 667 | 1284.5 |  519.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   210.37 ms |  0.40 | 667 | 3173.9 |  210.4 |  261.48 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   870.28 ms |  1.67 | 667 |  767.2 |  870.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   630.49 ms |  1.21 | 667 | 1059.0 |  630.5 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,559.67 ms |  3.00 | 667 |  428.1 | 1559.7 |  260.58 MB |        1.00 |

###### Apple.M1.(Virtual) - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.36 ms |  1.00 |  33 | 1028.5 |  647.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.74 ms |  0.46 |  33 | 2258.4 |  294.7 |   13.59 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    52.76 ms |  1.63 |  33 |  630.8 | 1055.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    40.49 ms |  1.25 |  33 |  821.9 |  809.9 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    84.67 ms |  2.62 |  33 |  393.1 | 1693.3 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   664.17 ms |  1.00 | 665 | 1002.5 |  664.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   353.26 ms |  0.53 | 665 | 1884.7 |  353.3 |  267.27 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,082.34 ms |  1.63 | 665 |  615.1 | 1082.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,249.34 ms |  1.88 | 665 |  532.9 | 1249.3 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,598.74 ms |  2.41 | 665 |  416.5 | 1598.7 |  260.58 MB |        1.00 |


##### PackageAssets with Spaces and Quotes Benchmark Results
Similar to the benchmark related to quotes here spaces ` ` and quotes `"` are
added to relevant columns to benchmark impact of trimming and unescape on low
level column access. That is, basically ` " ` is prepended and appended to each
column. This will test the assumed most common case and fast path part of
trimming and unescaping in Sep. Sep is about 10x faster than CsvHelper for this.
Sylvan does not appear to have support automatic trimming and is, therefore, not
included.

###### AMD.EPYC.7763 - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.91 ms |  1.00 | 41 | 3228.2 |  258.2 |   1.08 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.40 ms |  1.43 | 41 | 2265.4 |  367.9 |   1.11 KB |        1.03 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.79 ms |  1.46 | 41 | 2218.5 |  375.7 |   1.11 KB |        1.03 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.42 ms |  1.58 | 41 | 2041.2 |  408.3 |   1.11 KB |        1.03 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 144.51 ms | 11.19 | 41 |  288.4 | 2890.1 | 451.72 KB |      419.75 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 144.37 ms | 11.18 | 41 |  288.7 | 2887.5 |  446.2 KB |      414.61 |

###### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  13.26 ms |  1.00 | 41 | 3149.0 |  265.3 |   1.22 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.75 ms |  1.41 | 41 | 2228.2 |  374.9 |   1.24 KB |        1.02 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.82 ms |  1.49 | 41 | 2107.2 |  396.5 |   1.25 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  22.54 ms |  1.70 | 41 | 1853.0 |  450.8 |   1.27 KB |        1.04 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 129.86 ms |  9.79 | 41 |  321.7 | 2597.2 | 454.53 KB |      372.95 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 127.98 ms |  9.65 | 41 |  326.4 | 2559.5 | 445.86 KB |      365.83 |

###### AMD.Ryzen.9.5950X - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  8.599 ms |  1.00 | 41 | 4857.5 |  172.0 |   1.04 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 12.402 ms |  1.44 | 41 | 3368.0 |  248.0 |   1.05 KB |        1.01 |
| Sep_TrimUnescape           | Cols  | 50000 | 13.201 ms |  1.54 | 41 | 3164.1 |  264.0 |   1.06 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 14.568 ms |  1.69 | 41 | 2867.3 |  291.4 |   1.07 KB |        1.02 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 96.272 ms | 11.20 | 41 |  433.9 | 1925.4 | 451.52 KB |      432.51 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 95.183 ms | 11.07 | 41 |  438.8 | 1903.7 | 445.86 KB |      427.09 |

###### Apple.M1.(Virtual) - PackageAssets with Spaces and Quotes Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 12.70 ms |  1.00 | 41 | 3282.0 |  254.0 |   1.01 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 15.72 ms |  1.24 | 41 | 2650.5 |  314.5 |   1.19 KB |        1.17 |
| Sep_TrimUnescape           | Cols  | 50000 | 16.28 ms |  1.28 | 41 | 2559.8 |  325.6 |   1.03 KB |        1.02 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 21.21 ms |  1.67 | 41 | 1964.6 |  424.3 |   1.37 KB |        1.35 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 88.96 ms |  7.01 | 41 |  468.5 | 1779.2 |  451.6 KB |      445.94 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 86.97 ms |  6.85 | 41 |  479.2 | 1739.4 | 445.93 KB |      440.34 |


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

##### AMD.EPYC.7763 - FloatsReader Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.913 ms |  1.00 | 20 | 6958.2 |  116.5 |     1.26 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.511 ms |  1.21 | 20 | 5774.0 |  140.4 |    10.71 KB |        8.51 |
| ReadLine_ | Row    | 25000 |  19.754 ms |  6.78 | 20 | 1026.2 |  790.2 | 73489.68 KB |   58,426.57 |
| CsvHelper | Row    | 25000 |  38.285 ms | 13.14 | 20 |  529.5 | 1531.4 |    20.06 KB |       15.95 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.876 ms |  1.00 | 20 | 5230.0 |  155.0 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   5.891 ms |  1.52 | 20 | 3441.4 |  235.6 |    10.72 KB |        8.51 |
| ReadLine_ | Cols   | 25000 |  21.160 ms |  5.46 | 20 |  958.0 |  846.4 | 73489.68 KB |   58,335.99 |
| CsvHelper | Cols   | 25000 |  40.117 ms | 10.35 | 20 |  505.3 | 1604.7 | 21340.29 KB |   16,939.89 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.349 ms |  1.00 | 20 |  646.7 | 1254.0 |     8.08 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  12.954 ms |  0.41 | 20 | 1565.0 |  518.1 |    67.81 KB |        8.40 |
| Sylvan___ | Floats | 25000 |  90.425 ms |  2.88 | 20 |  224.2 | 3617.0 |    18.96 KB |        2.35 |
| ReadLine_ | Floats | 25000 | 112.571 ms |  3.59 | 20 |  180.1 | 4502.9 | 73493.98 KB |    9,101.20 |
| CsvHelper | Floats | 25000 | 160.313 ms |  5.11 | 20 |  126.5 | 6412.5 | 22062.29 KB |    2,732.11 |

##### AMD.Ryzen.7.PRO.7840U.w.Radeon.780M - FloatsReader Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.386 ms |  1.00 | 20 | 6000.5 |  135.5 |     1.41 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.782 ms |  1.12 | 20 | 5372.1 |  151.3 |    10.71 KB |        7.58 |
| ReadLine_ | Row    | 25000 |  15.608 ms |  4.61 | 20 | 1301.9 |  624.3 | 73489.65 KB |   52,006.50 |
| CsvHelper | Row    | 25000 |  39.717 ms | 11.73 | 20 |  511.6 | 1588.7 |    20.03 KB |       14.17 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.509 ms |  1.00 | 20 | 4506.2 |  180.4 |     1.42 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.001 ms |  1.33 | 20 | 3386.0 |  240.0 |    10.71 KB |        7.56 |
| ReadLine_ | Cols   | 25000 |  19.123 ms |  4.24 | 20 | 1062.6 |  764.9 | 73489.66 KB |   51,898.90 |
| CsvHelper | Cols   | 25000 |  42.640 ms |  9.46 | 20 |  476.5 | 1705.6 | 21340.25 KB |   15,070.63 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  30.981 ms |  1.00 | 20 |  655.9 | 1239.2 |      8.2 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   8.686 ms |  0.28 | 20 | 2339.4 |  347.4 |   115.48 KB |       14.08 |
| Sylvan___ | Floats | 25000 |  85.393 ms |  2.76 | 20 |  238.0 | 3415.7 |    18.88 KB |        2.30 |
| ReadLine_ | Floats | 25000 | 104.946 ms |  3.39 | 20 |  193.6 | 4197.8 | 73493.12 KB |    8,960.23 |
| CsvHelper | Floats | 25000 | 156.745 ms |  5.06 | 20 |  129.6 | 6269.8 | 22062.08 KB |    2,689.79 |

##### AMD.Ryzen.9.5950X - FloatsReader Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |  1.909 ms |  1.00 | 20 | 10643.3 |   76.4 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  2.244 ms |  1.18 | 20 |  9054.3 |   89.8 |     10.7 KB |        8.56 |
| ReadLine_ | Row    | 25000 |  9.875 ms |  5.17 | 20 |  2057.8 |  395.0 | 73489.63 KB |   58,791.71 |
| CsvHelper | Row    | 25000 | 24.433 ms | 12.80 | 20 |   831.6 |  977.3 |       20 KB |       16.00 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |  2.615 ms |  1.00 | 20 |  7771.6 |  104.6 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  3.744 ms |  1.43 | 20 |  5427.2 |  149.8 |    10.71 KB |        8.54 |
| ReadLine_ | Cols   | 25000 | 10.169 ms |  3.89 | 20 |  1998.1 |  406.8 | 73489.63 KB |   58,654.23 |
| CsvHelper | Cols   | 25000 | 26.080 ms |  9.97 | 20 |   779.1 | 1043.2 | 21340.22 KB |   17,032.26 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Floats | 25000 | 19.885 ms |  1.00 | 20 |  1021.8 |  795.4 |     7.99 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  3.527 ms |  0.18 | 20 |  5761.0 |  141.1 |   180.36 KB |       22.59 |
| Sylvan___ | Floats | 25000 | 52.686 ms |  2.65 | 20 |   385.7 | 2107.5 |    18.88 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 66.897 ms |  3.36 | 20 |   303.7 | 2675.9 | 73493.12 KB |    9,203.49 |
| CsvHelper | Floats | 25000 | 97.448 ms |  4.90 | 20 |   208.5 | 3897.9 |  22061.7 KB |    2,762.77 |

##### Apple.M1.(Virtual) - FloatsReader Benchmark Results (Sep 0.6.0.0, Sylvan  1.3.9.0, CsvHelper 33.0.1.24)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.887 ms |  1.00 | 20 | 5215.5 |  155.5 |      1.2 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  17.956 ms |  4.62 | 20 | 1129.0 |  718.2 |    10.62 KB |        8.87 |
| ReadLine_ | Row    | 25000 |  14.074 ms |  3.62 | 20 | 1440.4 |  563.0 | 73489.65 KB |   61,381.24 |
| CsvHelper | Row    | 25000 |  27.741 ms |  7.14 | 20 |  730.8 | 1109.6 |    20.28 KB |       16.94 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.726 ms |  1.00 | 20 | 4289.0 |  189.1 |      1.2 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  20.241 ms |  4.28 | 20 | 1001.5 |  809.6 |    10.62 KB |        8.84 |
| ReadLine_ | Cols   | 25000 |  14.976 ms |  3.17 | 20 | 1353.6 |  599.0 | 73489.65 KB |   61,181.63 |
| CsvHelper | Cols   | 25000 |  29.842 ms |  6.31 | 20 |  679.3 | 1193.7 |  21340.5 KB |   17,766.40 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  24.511 ms |  1.00 | 20 |  827.1 |  980.4 |     8.34 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.422 ms |  0.38 | 20 | 2151.5 |  376.9 |    79.89 KB |        9.58 |
| Sylvan___ | Floats | 25000 |  69.902 ms |  2.85 | 20 |  290.0 | 2796.1 |    18.57 KB |        2.23 |
| ReadLine_ | Floats | 25000 |  79.015 ms |  3.22 | 20 |  256.6 | 3160.6 |  73493.2 KB |    8,816.43 |
| CsvHelper | Floats | 25000 | 104.811 ms |  4.28 | 20 |  193.4 | 4192.4 | 22063.34 KB |    2,646.77 |


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

### Example - Use Extension Method Enumerate within async/await Context
Since `SepReader.Row` is a `ref struct` as covered above, one has to avoid
referencing it directly in async context. This can be done in a number of ways,
but one way is to use `Enumerate` extension method to parse/extract data from
row like shown below.

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
        [System.Obsolete(("Types with embedded references are not supported in this version of your compiler" +
            "."), true)]
        [System.Runtime.CompilerServices.CompilerFeatureRequired("RefStructs")]
        [System.Runtime.CompilerServices.IsByRefLike]
        public readonly struct Col
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
        [System.Obsolete(("Types with embedded references are not supported in this version of your compiler" +
            "."), true)]
        [System.Runtime.CompilerServices.CompilerFeatureRequired("RefStructs")]
        [System.Runtime.CompilerServices.IsByRefLike]
        public readonly struct Cols
        {
            public int Count { get; }
            public nietras.SeparatedValues.SepReader.Col this[int index] { get; }
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
        [System.Obsolete(("Types with embedded references are not supported in this version of your compiler" +
            "."), true)]
        [System.Runtime.CompilerServices.CompilerFeatureRequired("RefStructs")]
        [System.Runtime.CompilerServices.IsByRefLike]
        public readonly struct Row
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
        public static nietras.SeparatedValues.SepReader From(in this nietras.SeparatedValues.SepReaderOptions options, byte[] buffer) { }
        public static nietras.SeparatedValues.SepReader From(in this nietras.SeparatedValues.SepReaderOptions options, System.IO.Stream stream) { }
        public static nietras.SeparatedValues.SepReader From(in this nietras.SeparatedValues.SepReaderOptions options, System.IO.TextReader reader) { }
        public static nietras.SeparatedValues.SepReader From(in this nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.Stream> nameToStream) { }
        public static nietras.SeparatedValues.SepReader From(in this nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.TextReader> nameToReader) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, byte[] buffer, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, System.IO.Stream stream, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, System.IO.TextReader reader, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.Stream> nameToStream, System.Threading.CancellationToken cancellationToken = default) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromAsync(this nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.TextReader> nameToReader, System.Threading.CancellationToken cancellationToken = default) { }
        public static nietras.SeparatedValues.SepReader FromFile(in this nietras.SeparatedValues.SepReaderOptions options, string filePath) { }
        public static System.Threading.Tasks.ValueTask<nietras.SeparatedValues.SepReader> FromFileAsync(this nietras.SeparatedValues.SepReaderOptions options, string filePath, System.Threading.CancellationToken cancellationToken = default) { }
        public static nietras.SeparatedValues.SepReader FromText(in this nietras.SeparatedValues.SepReaderOptions options, string text) { }
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
    }
    public readonly struct SepSpec : System.IEquatable<nietras.SeparatedValues.SepSpec>
    {
        public SepSpec() { }
        public SepSpec(nietras.SeparatedValues.Sep sep, System.Globalization.CultureInfo? cultureInfo) { }
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
    public sealed class SepWriter : System.IAsyncDisposable, System.IDisposable
    {
        public nietras.SeparatedValues.SepWriterHeader Header { get; }
        public nietras.SeparatedValues.SepSpec Spec { get; }
        public void Dispose() { }
        public System.Threading.Tasks.ValueTask DisposeAsync() { }
        public void Flush() { }
        public System.Threading.Tasks.Task FlushAsync(System.Threading.CancellationToken cancellationToken = default) { }
        public nietras.SeparatedValues.SepWriter.Row NewRow() { }
        public override string ToString() { }
        [System.Obsolete(("Types with embedded references are not supported in this version of your compiler" +
            "."), true)]
        [System.Runtime.CompilerServices.CompilerFeatureRequired("RefStructs")]
        [System.Runtime.CompilerServices.IsByRefLike]
        public struct Row : System.IAsyncDisposable, System.IDisposable
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
        [System.Obsolete(("Types with embedded references are not supported in this version of your compiler" +
            "."), true)]
        [System.Runtime.CompilerServices.CompilerFeatureRequired("RefStructs")]
        [System.Runtime.CompilerServices.IsByRefLike]
        public readonly struct Col
        {
            public void Format<T>(T value)
                where T : System.ISpanFormattable { }
            public void Set(System.ReadOnlySpan<char> span) { }
            public void Set([System.Runtime.CompilerServices.InterpolatedStringHandlerArgument("")] ref nietras.SeparatedValues.SepWriter.Col.FormatInterpolatedStringHandler handler) { }
            public void Set(System.IFormatProvider? provider, [System.Runtime.CompilerServices.InterpolatedStringHandlerArgument(new string?[]?[] {
                    "",
                    "provider"})] ref nietras.SeparatedValues.SepWriter.Col.FormatInterpolatedStringHandler handler) { }
            [System.Obsolete(("Types with embedded references are not supported in this version of your compiler" +
                "."), true)]
            [System.Runtime.CompilerServices.CompilerFeatureRequired("RefStructs")]
            [System.Runtime.CompilerServices.InterpolatedStringHandler]
            [System.Runtime.CompilerServices.IsByRefLike]
            public struct FormatInterpolatedStringHandler
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
        [System.Obsolete(("Types with embedded references are not supported in this version of your compiler" +
            "."), true)]
        [System.Runtime.CompilerServices.CompilerFeatureRequired("RefStructs")]
        [System.Runtime.CompilerServices.IsByRefLike]
        public readonly struct Cols
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
            public void Set(string[] values) { }
            public void Set(nietras.SeparatedValues.SepReader.Cols cols) { }
        }
        public delegate void ColAction(nietras.SeparatedValues.SepWriter.Col col);
        public delegate void ColAction<T>(nietras.SeparatedValues.SepWriter.Col col, T value);
        public delegate void RowAction(nietras.SeparatedValues.SepWriter.Row row);
    }
    public static class SepWriterExtensions
    {
        public static nietras.SeparatedValues.SepWriter To(this nietras.SeparatedValues.SepWriterOptions options, System.IO.Stream stream) { }
        public static nietras.SeparatedValues.SepWriter To(this nietras.SeparatedValues.SepWriterOptions options, System.IO.TextWriter writer) { }
        public static nietras.SeparatedValues.SepWriter To(this nietras.SeparatedValues.SepWriterOptions options, System.IO.Stream stream, bool leaveOpen) { }
        public static nietras.SeparatedValues.SepWriter To(this nietras.SeparatedValues.SepWriterOptions options, System.IO.TextWriter writer, bool leaveOpen) { }
        public static nietras.SeparatedValues.SepWriter ToFile(this nietras.SeparatedValues.SepWriterOptions options, string filePath) { }
        public static nietras.SeparatedValues.SepWriter ToText(this nietras.SeparatedValues.SepWriterOptions options) { }
        public static nietras.SeparatedValues.SepWriter ToText(this nietras.SeparatedValues.SepWriterOptions options, int capacity) { }
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
