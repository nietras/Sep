# Sep - ~~Possibly~~ the World's Fastest .NET CSV Parser
![.NET](https://img.shields.io/badge/net7.0%20net8.0-5C2D91?logo=.NET&labelColor=gray)
![C#](https://img.shields.io/badge/12.0-239120?logo=csharp&logoColor=white&labelColor=gray)
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
([`ISpanParsable<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.ispanparsable-1)/[`ISpanFormattable`
](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable)), [`ref
struct`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/ref-struct),
[`ArrayPool<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.arraypool-1)
and similar from [.NET 7+ and C#
11+](https://nietras.com/2022/11/26/dotnet-and-csharp-versions/) for a modern
and highly efficient implementation.
* **🔎 Minimal** - a succinct yet expressive API with few options and no hidden
changes to input or output. What you read/write is what you get. E.g. by default
there is no "automatic" escaping/unescaping of quotes. For automatic unescaping
of quotes see [SepReaderOptions](#sepreaderoptions) and [Unescaping](#unescaping).
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
[ReadMeTest.cs](src/Sep.Test/ReadMeTest.cs).

⚠ Note that it is important to understand that Sep `Row`/`Col`/`Cols` are [`ref
struct`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/ref-struct)s
(please follow the `ref struct` link and understand how this limits the usage of
those). This is due to these types being simple *facades* or indirections to the
underlying reader or writer. That means you cannot use LINQ or create an array
of all rows like `reader.ToArray()` as the reader is not `IEnumerable<>` either
since `ref struct`s cannot be used in interfaces, which is in fact the point.
Hence, you need to parse or copy to different types instead. The same applies to
`Col`/`Cols` which point to internal state that is also reused. This is to avoid
repeated allocations for each row and get the best possible performance, while
still defining a well structured and straightforward API that guides users to
relevant functionality.  See [Why SepReader Is Not IEnumerable and LINQ
Compatible](#why-sepreader-is-not-ienumerable-and-linq-compatible) for more.

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

#### Why SepReader Is Not IEnumerable and LINQ Compatible
As mentioned earlier Sep only allows enumeration and access to one row at a time
and `SepReader.Row` is just a simple *facade* or indirection to the underlying
reader. This is why it is defined as a `ref struct`. In fact, the following code:
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
`row` is still basically the `reader` underneath. Hence, let's imagine *if*
`SepReader` did implement `IEnumerable<SepReader.Row>` and the `Row` was *not* a
`ref struct`. Then, you would be able to write something like below:
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

In fact, Sep now provides such a convenience extension method. And, discounting
the `Enumerate` method, this does have less boilerplate, but not really more
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
above can be seen in [ReadMeTest.cs](src/Sep.Test/ReadMeTest.cs).

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
```

## Limitations and Constraints
Sep is designed to be minimal and fast. As such, it has some limitations and
constraints, since these are not needed for the initial intended usage:

* Automatic escaping and unescaping quotes is not supported. Use
   [`Trim`](https://learn.microsoft.com/en-us/dotnet/api/system.memoryextensions.trim)
   extension method to remove surrounding quotes, for example.
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
* `SepWriter` is not yet fully featured and one cannot skip writing a header
   currently.

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

* `NET 8.0.X`

The following platforms are used for benchmarking:

* `AMD 5950X` X64 Platform Information
  ``` ini
  OS=Windows 10 (10.0.19044.2846/21H2/November2021Update)
  AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
  ```
* `Intel Xeon Silver 4316` X64 Platform Information
  ``` ini
  OS=Windows 10 (10.0.17763.3287/1809/October2018Update/Redstone5)
  Intel Xeon Silver 4316 CPU 2.30GHz, 1 CPU, 40 logical and 20 physical cores
  ```
* `Neoverse N1` ARM64 Platform Information (cloud instance)
  ```ini
  OS=ubuntu 22.04
  Neoverse N1, ARM, 4 vCPU
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

###### AMD.Ryzen.9.5950X - PackageAssets Benchmark Results (Sep 0.4.6.0, Sylvan  1.3.7.0, CsvHelper 31.0.2.15)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     2.326 ms |  1.00 |  29 | 12544.5 |   46.5 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     2.387 ms |  1.03 |  29 | 12226.3 |   47.7 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     3.014 ms |  1.30 |  29 |  9682.9 |   60.3 |       7.21 KB |        7.10 |
| ReadLine_    | Row   | 50000   |    12.914 ms |  5.57 |  29 |  2259.6 |  258.3 |   88608.24 KB |   87,329.01 |
| CsvHelper    | Row   | 50000   |    46.935 ms | 20.17 |  29 |   621.7 |  938.7 |         20 KB |       19.71 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     3.151 ms |  1.00 |  29 |  9262.0 |   63.0 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     3.773 ms |  1.20 |  29 |  7734.6 |   75.5 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     5.166 ms |  1.64 |  29 |  5648.5 |  103.3 |       7.21 KB |        7.09 |
| ReadLine_    | Cols  | 50000   |    13.021 ms |  4.12 |  29 |  2241.0 |  260.4 |   88608.24 KB |   87,077.58 |
| CsvHelper    | Cols  | 50000   |    72.451 ms | 22.99 |  29 |   402.8 | 1449.0 |     445.76 KB |      438.06 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    37.506 ms |  1.00 |  29 |   778.0 |  750.1 |    13803.3 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    22.617 ms |  0.60 |  29 |  1290.2 |  452.3 |   13992.22 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    39.622 ms |  1.06 |  29 |   736.5 |  792.4 |   13962.44 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   105.490 ms |  2.98 |  29 |   276.6 | 2109.8 |  102133.28 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    87.642 ms |  2.35 |  29 |   333.0 | 1752.8 |   13971.76 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   622.282 ms |  1.00 | 583 |   938.1 |  622.3 |  266667.45 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   249.164 ms |  0.40 | 583 |  2343.0 |  249.2 |  268111.22 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   771.816 ms |  1.23 | 583 |   756.4 |  771.8 |  266826.86 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,554.977 ms |  2.43 | 583 |   375.4 | 1555.0 | 2038833.85 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,701.078 ms |  2.67 | 583 |   343.2 | 1701.1 |   266838.3 KB |        1.00 |

###### Intel.Xeon.Silver.4316.2.30GHz - PackageAssets Benchmark Results (Sep 0.4.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     5.118 ms |  1.00 |  29 | 5701.4 |  102.4 |       1.18 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.146 ms |  1.01 |  29 | 5670.6 |  102.9 |       1.18 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     5.953 ms |  1.16 |  29 | 4901.9 |  119.1 |       7.21 KB |        6.12 |
| ReadLine_    | Row   | 50000   |    25.973 ms |  5.07 |  29 | 1123.5 |  519.5 |   88608.26 KB |   75,111.64 |
| CsvHelper    | Row   | 50000   |    90.063 ms | 17.60 |  29 |  324.0 | 1801.3 |      20.69 KB |       17.54 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     7.123 ms |  1.00 |  29 | 4096.5 |  142.5 |       1.19 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.943 ms |  1.12 |  29 | 3673.9 |  158.9 |       1.19 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    10.124 ms |  1.42 |  29 | 2882.3 |  202.5 |       7.22 KB |        6.09 |
| ReadLine_    | Cols  | 50000   |    25.928 ms |  3.64 |  29 | 1125.5 |  518.6 |   88608.24 KB |   74,678.88 |
| CsvHelper    | Cols  | 50000   |   140.614 ms | 19.74 |  29 |  207.5 | 2812.3 |     446.45 KB |      376.26 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    53.901 ms |  1.00 |  29 |  541.4 | 1078.0 |   13802.75 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    30.484 ms |  0.57 |  29 |  957.3 |  609.7 |   14030.67 KB |        1.02 |
| Sylvan___    | Asset | 50000   |    67.354 ms |  1.25 |  29 |  433.2 | 1347.1 |   13961.77 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   149.924 ms |  2.78 |  29 |  194.6 | 2998.5 |  102133.61 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   158.310 ms |  2.94 |  29 |  184.3 | 3166.2 |    13970.8 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,122.212 ms |  1.00 | 583 |  520.2 | 1122.2 |  266672.93 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   378.388 ms |  0.34 | 583 | 1542.8 |  378.4 |  267505.55 KB |        1.00 |
| Sylvan___    | Asset | 1000000 | 1,408.440 ms |  1.26 | 583 |  414.5 | 1408.4 |  266826.38 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,962.035 ms |  2.63 | 583 |  197.1 | 2962.0 | 2038832.76 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 3,379.135 ms |  3.00 | 583 |  172.8 | 3379.1 |  266833.95 KB |        1.00 |

###### Neoverse.N1 - PackageAssets Benchmark Results (Sep 0.4.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    12.72 ms |  1.00 |  29 | 2287.5 |  254.3 |       1020 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    12.75 ms |  1.00 |  29 | 2281.2 |  255.0 |       1020 B |        1.00 |
| Sylvan___    | Row   | 50000   |    31.29 ms |  2.46 |  29 |  929.4 |  625.9 |       6269 B |        6.15 |
| ReadLine_    | Row   | 50000   |    37.12 ms |  2.92 |  29 |  783.5 |  742.5 |   90734916 B |   88,955.80 |
| CsvHelper    | Row   | 50000   |    91.34 ms |  7.18 |  29 |  318.4 | 1826.9 |      21272 B |       20.85 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    15.00 ms |  1.00 |  29 | 1938.6 |  300.1 |       1032 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    16.05 ms |  1.07 |  29 | 1811.8 |  321.1 |       1035 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    35.43 ms |  2.36 |  29 |  821.0 |  708.6 |       6288 B |        6.09 |
| ReadLine_    | Cols  | 50000   |    41.72 ms |  2.78 |  29 |  697.2 |  834.3 |   90734929 B |   87,921.44 |
| CsvHelper    | Cols  | 50000   |   137.46 ms |  9.16 |  29 |  211.6 | 2749.3 |     457144 B |      442.97 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    68.24 ms |  1.00 |  29 |  426.2 | 1364.9 |   14134756 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    38.78 ms |  0.57 |  29 |  750.1 |  775.5 |   14191876 B |        1.00 |
| Sylvan___    | Asset | 50000   |   100.82 ms |  1.48 |  29 |  288.5 | 2016.5 |   14295846 B |        1.01 |
| ReadLine_    | Asset | 50000   |   157.86 ms |  2.31 |  29 |  184.2 | 3157.2 |  104585308 B |        7.40 |
| CsvHelper    | Asset | 50000   |   167.11 ms |  2.43 |  29 |  174.1 | 3342.2 |   14309064 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 | 1,357.03 ms |  1.00 | 581 |  428.8 | 1357.0 |  273070824 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   648.57 ms |  0.48 | 581 |  897.2 |  648.6 |  277540480 B |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,990.02 ms |  1.47 | 581 |  292.4 | 1990.0 |  273234920 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,137.22 ms |  2.32 | 581 |  185.5 | 3137.2 | 2087767336 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 3,391.21 ms |  2.50 | 581 |  171.6 | 3391.2 |  273241296 B |        1.00 |


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

###### AMD.Ryzen.9.5950X - PackageAssets Benchmark Results (SERVER GC) (Sep 0.4.6.0, Sylvan  1.3.7.0, CsvHelper 31.0.2.15)

| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    21.051 ms |  1.00 |  29 | 1386.2 |  421.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     5.993 ms |  0.29 |  29 | 4869.4 |  119.9 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    29.301 ms |  1.39 |  29 |  995.9 |  586.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    33.868 ms |  1.60 |  29 |  861.6 |  677.4 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    76.599 ms |  3.64 |  29 |  381.0 | 1532.0 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   425.355 ms |  1.00 | 583 | 1372.5 |  425.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   109.917 ms |  0.26 | 583 | 5311.1 |  109.9 |  261.49 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   588.226 ms |  1.38 | 583 |  992.4 |  588.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   581.137 ms |  1.37 | 583 | 1004.6 |  581.1 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,535.431 ms |  3.60 | 583 |  380.2 | 1535.4 |  260.58 MB |        1.00 |

###### Intel.Xeon.Silver.4316.2.30GHz - PackageAssets Benchmark Results (SERVER GC) (Sep 0.4.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method    | Scope | Rows    | Mean         | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    41.669 ms |  1.00 |    0.00 |  29 |  700.3 |  833.4 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     9.802 ms |  0.24 |    0.00 |  29 | 2977.1 |  196.0 |   13.68 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    56.608 ms |  1.36 |    0.01 |  29 |  515.5 | 1132.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    47.549 ms |  1.14 |    0.01 |  29 |  613.7 |  951.0 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   153.359 ms |  3.68 |    0.02 |  29 |  190.3 | 3067.2 |   13.64 MB |        1.01 |
|           |       |         |              |       |         |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   822.268 ms |  1.00 |    0.00 | 583 |  710.0 |  822.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   161.654 ms |  0.20 |    0.00 | 583 | 3611.3 |  161.7 |  261.31 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,120.677 ms |  1.36 |    0.00 | 583 |  520.9 | 1120.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,081.121 ms |  1.31 |    0.00 | 583 |  540.0 | 1081.1 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 3,051.024 ms |  3.71 |    0.00 | 583 |  191.3 | 3051.0 |  260.58 MB |        1.00 |

###### Neoverse.N1 - PackageAssets Benchmark Results (SERVER GC) (Sep 0.4.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method    | Scope | Rows    | Mean        | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    56.29 ms |  1.00 |    0.00 |  29 |  516.7 | 1125.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    20.66 ms |  0.37 |    0.02 |  29 | 1407.7 |  413.2 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    88.70 ms |  1.58 |    0.08 |  29 |  327.9 | 1773.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    67.01 ms |  1.19 |    0.04 |  29 |  434.1 | 1340.1 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   158.71 ms |  2.83 |    0.10 |  29 |  183.3 | 3174.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |         |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 1,110.22 ms |  1.00 |    0.00 | 581 |  524.1 | 1110.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   402.71 ms |  0.36 |    0.00 | 581 | 1444.9 |  402.7 |  265.42 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,759.98 ms |  1.58 |    0.01 | 581 |  330.6 | 1760.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,575.08 ms |  1.43 |    0.07 | 581 |  369.4 | 1575.1 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 3,167.05 ms |  2.85 |    0.01 | 581 |  183.7 | 3167.1 |  260.58 MB |        1.00 |


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

###### AMD.Ryzen.9.5950X - PackageAssets with Quotes Benchmark Results (Sep 0.4.6.0, Sylvan  1.3.7.0, CsvHelper 31.0.2.15)

| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     6.401 ms |  1.00 |  33 | 5214.3 |  128.0 |       1.03 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     6.785 ms |  1.06 |  33 | 4919.3 |  135.7 |       1.03 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    18.566 ms |  2.91 |  33 | 1797.8 |  371.3 |       7.23 KB |        7.04 |
| ReadLine_    | Row   | 50000   |    14.481 ms |  2.26 |  33 | 2304.8 |  289.6 |  108778.74 KB |  105,883.49 |
| CsvHelper    | Row   | 50000   |    52.862 ms |  8.26 |  33 |  631.4 | 1057.2 |         20 KB |       19.47 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     7.238 ms |  1.00 |  33 | 4611.2 |  144.8 |       1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     8.971 ms |  1.23 |  33 | 3720.6 |  179.4 |       1.03 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    21.286 ms |  2.94 |  33 | 1568.0 |  425.7 |       7.24 KB |        7.02 |
| ReadLine_    | Cols  | 50000   |    14.900 ms |  2.06 |  33 | 2240.1 |  298.0 |  108778.74 KB |  105,482.42 |
| CsvHelper    | Cols  | 50000   |    83.563 ms | 11.54 |  33 |  399.4 | 1671.3 |     445.76 KB |      432.25 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    39.372 ms |  1.00 |  33 |  847.8 |  787.4 |    13802.4 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    24.035 ms |  0.61 |  33 | 1388.7 |  480.7 |   13985.88 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    50.022 ms |  1.26 |  33 |  667.2 | 1000.4 |   13962.17 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   120.676 ms |  3.07 |  33 |  276.6 | 2413.5 |  122304.18 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    96.420 ms |  2.44 |  33 |  346.2 | 1928.4 |   13971.94 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   703.318 ms |  1.00 | 667 |  949.4 |  703.3 |  266667.29 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   332.798 ms |  0.47 | 667 | 2006.3 |  332.8 |  267969.09 KB |        1.00 |
| Sylvan___    | Asset | 1000000 | 1,075.648 ms |  1.53 | 667 |  620.7 | 1075.6 |  266824.34 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,409.387 ms |  3.34 | 667 |  277.1 | 2409.4 | 2442315.91 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,087.482 ms |  2.96 | 667 |  319.9 | 2087.5 |  266832.87 KB |        1.00 |

###### Intel.Xeon.Silver.4316.2.30GHz - PackageAssets with Quotes Benchmark Results (Sep 0.4.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    13.60 ms |  1.00 |  33 | 2453.9 |  272.0 |       1.21 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    12.79 ms |  0.94 |  33 | 2610.7 |  255.7 |        1.2 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    33.79 ms |  2.49 |  33 |  987.8 |  675.8 |       7.26 KB |        6.01 |
| ReadLine_    | Row   | 50000   |    30.72 ms |  2.26 |  33 | 1086.5 |  614.4 |  108778.76 KB |   90,048.06 |
| CsvHelper    | Row   | 50000   |   102.72 ms |  7.55 |  33 |  324.9 | 2054.3 |      20.69 KB |       17.13 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    15.89 ms |  1.00 |  33 | 2100.0 |  317.9 |       1.21 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    16.99 ms |  1.07 |  33 | 1964.6 |  339.8 |       1.22 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    39.57 ms |  2.53 |  33 |  843.6 |  791.3 |       7.27 KB |        5.99 |
| ReadLine_    | Cols  | 50000   |    31.03 ms |  1.95 |  33 | 1075.5 |  620.7 |  108778.74 KB |   89,613.38 |
| CsvHelper    | Cols  | 50000   |   160.67 ms | 10.11 |  33 |  207.7 | 3213.4 |     446.45 KB |      367.79 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    64.27 ms |  1.00 |  33 |  519.4 | 1285.3 |   13804.23 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    36.01 ms |  0.56 |  33 |  926.9 |  720.2 |   14020.14 KB |        1.02 |
| Sylvan___    | Asset | 50000   |    96.43 ms |  1.50 |  33 |  346.1 | 1928.5 |   13962.36 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   198.74 ms |  3.10 |  33 |  167.9 | 3974.7 |  122304.04 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   179.60 ms |  2.79 |  33 |  185.8 | 3591.9 |   13970.63 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,325.26 ms |  1.00 | 667 |  503.8 | 1325.3 |  266667.79 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   573.47 ms |  0.45 | 667 | 1164.3 |  573.5 |  267685.55 KB |        1.00 |
| Sylvan___    | Asset | 1000000 | 1,983.93 ms |  1.50 | 667 |  336.6 | 1983.9 |  266834.56 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,804.46 ms |  2.87 | 667 |  175.5 | 3804.5 | 2442323.66 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 3,767.71 ms |  2.84 | 667 |  177.2 | 3767.7 |  266840.59 KB |        1.00 |

###### Neoverse.N1 - PackageAssets with Quotes Benchmark Results (Sep 0.4.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    25.14 ms |  1.00 |  33 | 1323.9 |  502.8 |       1.04 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    24.79 ms |  0.99 |  33 | 1342.6 |  495.8 |       1.04 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    39.92 ms |  1.59 |  33 |  833.7 |  798.4 |       6.14 KB |        5.88 |
| ReadLine_    | Row   | 50000   |    44.29 ms |  1.76 |  33 |  751.5 |  885.8 |  108778.83 KB |  104,199.74 |
| CsvHelper    | Row   | 50000   |   106.65 ms |  4.24 |  33 |  312.1 | 2133.0 |      20.77 KB |       19.90 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    27.56 ms |  1.00 |  33 | 1207.5 |  551.3 |       1.06 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    29.35 ms |  1.06 |  33 | 1133.8 |  587.1 |       1.06 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    45.55 ms |  1.65 |  33 |  730.7 |  911.0 |       6.17 KB |        5.82 |
| ReadLine_    | Cols  | 50000   |    45.78 ms |  1.66 |  33 |  727.1 |  915.5 |  108778.83 KB |  102,663.15 |
| CsvHelper    | Cols  | 50000   |   152.23 ms |  5.52 |  33 |  218.6 | 3044.5 |     446.61 KB |      421.50 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    80.29 ms |  1.00 |  33 |  414.5 | 1605.8 |   13804.89 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    49.12 ms |  0.61 |  33 |  677.5 |  982.4 |   13858.29 KB |        1.00 |
| Sylvan___    | Asset | 50000   |   110.68 ms |  1.38 |  33 |  300.7 | 2213.6 |   13961.37 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   196.34 ms |  2.44 |  33 |  169.5 | 3926.7 |  122305.09 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   180.62 ms |  2.24 |  33 |  184.3 | 3612.4 |   13974.57 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,619.19 ms |  1.00 | 665 |  411.2 | 1619.2 |  266671.09 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   830.09 ms |  0.52 | 665 |  802.1 |  830.1 |  269668.23 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 2,185.17 ms |  1.35 | 665 |  304.7 | 2185.2 |  266828.83 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,986.74 ms |  2.46 | 665 |  167.0 | 3986.7 | 2442318.74 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 3,682.98 ms |  2.27 | 665 |  180.8 | 3683.0 |  266841.98 KB |        1.00 |


##### PackageAssets with Quotes Benchmark Results (SERVER GC)
Here again are benchmark results with server garbage collection, which provides
significant speedup over workstation garbage collection.

###### AMD.Ryzen.9.5950X - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.4.6.0, Sylvan  1.3.7.0, CsvHelper 31.0.2.15)

| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    25.84 ms |  1.00 |  33 | 1291.8 |  516.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    10.97 ms |  0.42 |  33 | 3041.8 |  219.5 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    45.45 ms |  1.70 |  33 |  734.3 |  909.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    40.09 ms |  1.59 |  33 |  832.6 |  801.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    92.69 ms |  3.59 |  33 |  360.1 | 1853.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   546.80 ms |  1.00 | 667 | 1221.1 |  546.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   212.94 ms |  0.39 | 667 | 3135.7 |  212.9 |  261.47 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   960.19 ms |  1.75 | 667 |  695.4 |  960.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   678.16 ms |  1.24 | 667 |  984.6 |  678.2 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,900.80 ms |  3.47 | 667 |  351.3 | 1900.8 |  260.58 MB |        1.00 |

###### Intel.Xeon.Silver.4316.2.30GHz - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.4.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method    | Scope | Rows    | Mean        | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    52.58 ms |  1.00 |    0.00 |  33 |  634.7 | 1051.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.79 ms |  0.34 |    0.00 |  33 | 1875.9 |  355.9 |   13.67 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    86.54 ms |  1.65 |    0.01 |  33 |  385.7 | 1730.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    55.36 ms |  1.05 |    0.01 |  33 |  602.9 | 1107.1 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   176.28 ms |  3.35 |    0.02 |  33 |  189.3 | 3525.6 |   13.64 MB |        1.01 |
|           |       |         |             |       |         |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 1,043.85 ms |  1.00 |    0.00 | 667 |  639.7 | 1043.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   336.86 ms |  0.32 |    0.00 | 667 | 1982.1 |  336.9 |  261.35 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,697.87 ms |  1.63 |    0.00 | 667 |  393.3 | 1697.9 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,177.60 ms |  1.13 |    0.00 | 667 |  567.0 | 1177.6 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 3,478.19 ms |  3.33 |    0.01 | 667 |  192.0 | 3478.2 |  260.58 MB |        1.00 |

###### Neoverse.N1 - PackageAssets with Quotes Benchmark Results (SERVER GC) (Sep 0.4.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method    | Scope | Rows    | Mean        | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|--------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    70.05 ms |  1.00 |    0.00 |  33 |  475.1 | 1400.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    32.78 ms |  0.47 |    0.02 |  33 | 1015.2 |  655.7 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    98.67 ms |  1.41 |    0.06 |  33 |  337.3 | 1973.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    79.84 ms |  1.14 |    0.08 |  33 |  416.8 | 1596.9 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   173.86 ms |  2.48 |    0.09 |  33 |  191.4 | 3477.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |         |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 1,382.02 ms |  1.00 |    0.00 | 665 |  481.8 | 1382.0 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   589.55 ms |  0.43 |    0.00 | 665 | 1129.3 |  589.6 |  260.97 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,962.01 ms |  1.42 |    0.00 | 665 |  339.3 | 1962.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,744.55 ms |  1.26 |    0.01 | 665 |  381.6 | 1744.6 | 2385.08 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 3,454.73 ms |  2.50 |    0.01 | 665 |  192.7 | 3454.7 |  260.58 MB |        1.00 |


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

##### AMD.Ryzen.9.5950X - FloatsReader Benchmark Results (Sep 0.4.6.0, Sylvan  1.3.7.0, CsvHelper 31.0.2.15)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.061 ms |  1.00 | 20 | 9857.0 |   82.5 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   2.448 ms |  1.19 | 20 | 8299.1 |   97.9 |    10.02 KB |        8.02 |
| ReadLine_ | Row    | 25000 |  11.663 ms |  5.66 | 20 | 1742.2 |  466.5 | 73489.64 KB |   58,791.71 |
| CsvHelper | Row    | 25000 |  26.409 ms | 12.82 | 20 |  769.4 | 1056.3 |       20 KB |       16.00 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   2.696 ms |  1.00 | 20 | 7537.1 |  107.8 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   4.017 ms |  1.49 | 20 | 5058.7 |  160.7 |    10.02 KB |        8.00 |
| ReadLine_ | Cols   | 25000 |  11.638 ms |  4.33 | 20 | 1746.0 |  465.5 | 73489.64 KB |   58,654.24 |
| CsvHelper | Cols   | 25000 |  27.323 ms | 10.14 | 20 |  743.7 | 1092.9 | 21340.22 KB |   17,032.26 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  22.268 ms |  1.00 | 20 |  912.5 |  890.7 |        8 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   3.396 ms |  0.15 | 20 | 5983.3 |  135.8 |   182.55 KB |       22.83 |
| Sylvan___ | Floats | 25000 |  66.977 ms |  3.01 | 20 |  303.4 | 2679.1 |     18.2 KB |        2.28 |
| ReadLine_ | Floats | 25000 |  72.970 ms |  3.27 | 20 |  278.5 | 2918.8 | 73493.12 KB |    9,190.01 |
| CsvHelper | Floats | 25000 | 105.666 ms |  4.75 | 20 |  192.3 | 4226.6 | 22061.92 KB |    2,758.75 |

##### Intel.Xeon.Silver.4316.2.30GHz - FloatsReader Benchmark Results (Sep 0.4.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   4.012 ms |  1.00 | 20 | 5064.2 |  160.5 |     1.41 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   4.607 ms |  1.15 | 20 | 4410.6 |  184.3 |    10.02 KB |        7.11 |
| ReadLine_ | Row    | 25000 |  21.873 ms |  5.46 | 20 |  929.0 |  874.9 | 73489.67 KB |   52,114.56 |
| CsvHelper | Row    | 25000 |  57.624 ms | 14.36 | 20 |  352.6 | 2305.0 |    20.77 KB |       14.73 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   5.841 ms |  1.00 | 20 | 3478.9 |  233.6 |     1.42 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   7.398 ms |  1.27 | 20 | 2746.5 |  295.9 |    10.03 KB |        7.06 |
| ReadLine_ | Cols   | 25000 |  21.529 ms |  3.69 | 20 |  943.8 |  861.2 | 73489.67 KB |   51,756.14 |
| CsvHelper | Cols   | 25000 |  60.610 ms | 10.38 | 20 |  335.3 | 2424.4 | 21340.82 KB |   15,029.57 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  43.612 ms |  1.00 | 20 |  465.9 | 1744.5 |     8.22 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   6.159 ms |  0.14 | 20 | 3299.0 |  246.4 |   213.54 KB |       25.96 |
| Sylvan___ | Floats | 25000 | 142.246 ms |  3.26 | 20 |  142.8 | 5689.8 |    18.43 KB |        2.24 |
| ReadLine_ | Floats | 25000 | 155.347 ms |  3.56 | 20 |  130.8 | 6213.9 |  73493.3 KB |    8,935.78 |
| CsvHelper | Floats | 25000 | 215.336 ms |  4.94 | 20 |   94.4 | 8613.4 | 22062.78 KB |    2,682.53 |

##### Neoverse.N1 - FloatsReader Benchmark Results (Sep 0.4.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   9.537 ms |  1.00 | 20 | 2125.7 |  381.5 |     1.21 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  30.832 ms |  3.23 | 20 |  657.5 | 1233.3 |     9.74 KB |        8.02 |
| ReadLine_ | Row    | 25000 |  33.938 ms |  3.56 | 20 |  597.3 | 1357.5 | 73489.73 KB |   60,493.16 |
| CsvHelper | Row    | 25000 |  59.210 ms |  6.21 | 20 |  342.4 | 2368.4 |    20.71 KB |       17.04 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |  11.460 ms |  1.00 | 20 | 1768.9 |  458.4 |     1.23 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  34.857 ms |  3.04 | 20 |  581.6 | 1394.3 |     9.75 KB |        7.91 |
| ReadLine_ | Cols   | 25000 |  35.410 ms |  3.09 | 20 |  572.5 | 1416.4 | 73489.72 KB |   59,630.33 |
| CsvHelper | Cols   | 25000 |  64.828 ms |  5.65 | 20 |  312.7 | 2593.1 | 21341.07 KB |   17,316.37 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  52.297 ms |  1.00 | 20 |  387.6 | 2091.9 |     8.12 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  15.271 ms |  0.29 | 20 | 1327.5 |  610.9 |    65.28 KB |        8.04 |
| Sylvan___ | Floats | 25000 | 168.664 ms |  3.23 | 20 |  120.2 | 6746.6 |     18.2 KB |        2.24 |
| ReadLine_ | Floats | 25000 | 166.356 ms |  3.18 | 20 |  121.9 | 6654.3 | 73493.46 KB |    9,052.97 |
| CsvHelper | Floats | 25000 | 218.131 ms |  4.17 | 20 |   92.9 | 8725.2 | 22063.34 KB |    2,717.78 |


### Writer Comparison Benchmarks
Writer benchmarks are still pending, but Sep is unlikely to be the fastest here
since it is explicitly designed to make writing more convenient and flexible.
Still efficient, but not necessarily fastest. That is, Sep does not require
writing header up front and hence having to keep header column order and row
values column order the same. This means Sep does not write columns *directly*
upon definition but defers this until a new row has been fully defined and then
is ended.

## Example Catalogue
The following examples are available in [ReadMeTest.cs](src/Sep.Test/ReadMeTest.cs).

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
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v8.0", FrameworkDisplayName=".NET 8.0")]
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
    public delegate nietras.SeparatedValues.SepToString SepCreateToString(nietras.SeparatedValues.SepReaderHeader? maybeHeader, int colCount);
    public static class SepDefaults
    {
        public static System.StringComparer ColNameComparer { get; }
        public static System.Globalization.CultureInfo CultureInfo { get; }
        public static char Separator { get; }
    }
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class SepReader : nietras.SeparatedValues.SepReaderState
    {
        public nietras.SeparatedValues.SepReader.Row Current { get; }
        public bool HasHeader { get; }
        public bool HasRows { get; }
        public nietras.SeparatedValues.SepReaderHeader Header { get; }
        public bool IsEmpty { get; }
        public nietras.SeparatedValues.SepSpec Spec { get; }
        public nietras.SeparatedValues.SepReader GetEnumerator() { }
        public bool MoveNext() { }
        public string ToString(int index) { }
        [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay}")]
        [System.Obsolete("Types with embedded references are not supported in this version of your compiler" +
            ".", true)]
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
        [System.Obsolete("Types with embedded references are not supported in this version of your compiler" +
            ".", true)]
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
        [System.Obsolete("Types with embedded references are not supported in this version of your compiler" +
            ".", true)]
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
        public static nietras.SeparatedValues.SepReader From(this nietras.SeparatedValues.SepReaderOptions options, byte[] buffer) { }
        public static nietras.SeparatedValues.SepReader From(this nietras.SeparatedValues.SepReaderOptions options, System.IO.Stream stream) { }
        public static nietras.SeparatedValues.SepReader From(this nietras.SeparatedValues.SepReaderOptions options, System.IO.TextReader reader) { }
        public static nietras.SeparatedValues.SepReader From(this nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.Stream> nameToStream) { }
        public static nietras.SeparatedValues.SepReader From(this nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.TextReader> nameToReader) { }
        public static nietras.SeparatedValues.SepReader FromFile(this nietras.SeparatedValues.SepReaderOptions options, string filePath) { }
        public static nietras.SeparatedValues.SepReader FromText(this nietras.SeparatedValues.SepReaderOptions options, string text) { }
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
        public int IndexOf(string colName) { }
        public int[] IndicesOf(System.Collections.Generic.IReadOnlyList<string> colNames) { }
        public int[] IndicesOf(System.ReadOnlySpan<string> colNames) { }
        public int[] IndicesOf(params string[] colNames) { }
        public void IndicesOf(System.ReadOnlySpan<string> colNames, System.Span<int> colIndices) { }
        public System.Collections.Generic.IReadOnlyList<string> NamesStartingWith(string prefix, System.StringComparison comparison = 4) { }
        public override string ToString() { }
        public bool TryIndexOf(string colName, out int colIndex) { }
    }
    public readonly struct SepReaderOptions : System.IEquatable<nietras.SeparatedValues.SepReaderOptions>
    {
        public SepReaderOptions() { }
        public SepReaderOptions(nietras.SeparatedValues.Sep? sep) { }
        public System.Collections.Generic.IEqualityComparer<string> ColNameComparer { get; init; }
        public nietras.SeparatedValues.SepCreateToString CreateToString { get; init; }
        public System.Globalization.CultureInfo? CultureInfo { get; init; }
        public bool DisableColCountCheck { get; init; }
        public bool DisableFastFloat { get; init; }
        public bool DisableQuotesParsing { get; init; }
        public bool HasHeader { get; init; }
        public nietras.SeparatedValues.Sep? Sep { get; init; }
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
    public sealed class SepWriter : System.IDisposable
    {
        public nietras.SeparatedValues.SepWriterHeader Header { get; }
        public nietras.SeparatedValues.SepSpec Spec { get; }
        public void Dispose() { }
        public void Flush() { }
        public nietras.SeparatedValues.SepWriter.Row NewRow() { }
        public override string ToString() { }
        [System.Obsolete("Types with embedded references are not supported in this version of your compiler" +
            ".", true)]
        [System.Runtime.CompilerServices.CompilerFeatureRequired("RefStructs")]
        [System.Runtime.CompilerServices.IsByRefLike]
        public struct Row
        {
            public nietras.SeparatedValues.SepWriter.Col this[int colIndex] { get; }
            public nietras.SeparatedValues.SepWriter.Col this[string colName] { get; }
            public nietras.SeparatedValues.SepWriter.Cols this[System.ReadOnlySpan<int> indices] { get; }
            public nietras.SeparatedValues.SepWriter.Cols this[System.ReadOnlySpan<string> colNames] { get; }
            public nietras.SeparatedValues.SepWriter.Cols this[System.Collections.Generic.IReadOnlyList<string> colNames] { get; }
            public nietras.SeparatedValues.SepWriter.Cols this[string[] colNames] { get; }
            public void Dispose() { }
        }
        [System.Obsolete("Types with embedded references are not supported in this version of your compiler" +
            ".", true)]
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
            [System.Runtime.CompilerServices.InterpolatedStringHandler]
            public readonly struct FormatInterpolatedStringHandler
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
        [System.Obsolete("Types with embedded references are not supported in this version of your compiler" +
            ".", true)]
        [System.Runtime.CompilerServices.CompilerFeatureRequired("RefStructs")]
        [System.Runtime.CompilerServices.IsByRefLike]
        public readonly struct Cols
        {
            public int Count { get; }
            public nietras.SeparatedValues.SepWriter.Col this[int colIndex] { get; }
            public void Format<T>(System.Collections.Generic.IReadOnlyList<T> values)
                where T : System.ISpanFormattable { }
            public void Format<T>(System.ReadOnlySpan<T> values)
                where T : System.ISpanFormattable { }
            public void Format<T>(System.Span<T> values)
                where T : System.ISpanFormattable { }
            public void Format<T>(T[] values)
                where T : System.ISpanFormattable { }
            public void Format<T>(System.ReadOnlySpan<T> values, nietras.SeparatedValues.SepWriter.ColAction<T> format) { }
            public void Set(System.Collections.Generic.IReadOnlyList<string> values) { }
            public void Set(System.ReadOnlySpan<string> values) { }
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
        public void Add(System.ReadOnlySpan<string> colNames) { }
        public void Add(string colName) { }
        public void Add(string[] colNames) { }
        public void Write() { }
    }
    public readonly struct SepWriterOptions : System.IEquatable<nietras.SeparatedValues.SepWriterOptions>
    {
        public SepWriterOptions() { }
        public SepWriterOptions(nietras.SeparatedValues.Sep sep) { }
        public System.Globalization.CultureInfo? CultureInfo { get; init; }
        public nietras.SeparatedValues.Sep Sep { get; init; }
        public bool WriteHeader { get; init; }
    }
}
```
