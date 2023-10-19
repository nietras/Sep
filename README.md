# Sep - Possibly the World's Fastest .NET CSV Parser
![.NET](https://img.shields.io/badge/net7.0%20net8.0-5C2D91?logo=.NET&labelColor=gray)
![C#](https://img.shields.io/badge/11.0-239120?logo=c-sharp&logoColor=white&labelColor=gray)
[![Build Status](https://github.com/nietras/Sep/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/nietras/Sep/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/nietras/Sep/branch/main/graph/badge.svg?token=WN56CR3X0D)](https://codecov.io/gh/nietras/Sep)
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
changes to input or output. What you read/write is what you get. This means
there is no "automatic" escaping/unescaping of quotes, for example.
* **🚀 Fast** - blazing fast with both architecture specific and cross-platform
SIMD vectorized parsing incl. 64/128/256/512-bit paths e.g. AVX2, AVX-512 (.NET
8.0+), NEON. Uses [csFastFloat](https://github.com/CarlVerret/csFastFloat) for
fast parsing of floating points. Reads or writes one row at a time efficiently
with [detailed benchmarks](#comparison-benchmarks) to prove it.
* **🗑️ Zero allocation** - intelligent and efficient memory management allowing
for zero allocations after warmup incl. supporting use cases of reading or
writing arrays of values (e.g. features) easily without repeated allocations.
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

[Example](#Example) | [Naming and Terminology](#naming-and-terminology) | [API](#application-programming-interface-api) | [Limitations and Constraints](#limitations-and-constraints) | [Comparison Benchmarks](#comparison-benchmarks) | [Example Catalogue](#example-catalogue) | [RFC-4180](#rfc-4180) | [FAQ](#frequently-asked-questions-faq)  | [Public API Reference](#public-api-reference)

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
|`Number` | 1-based that is `LineNumber` will be 1 for the first line (as in `notepad`). Given a row may span multiple lines a row can have a *From* line number and an *ToExcl* line number matching the C# range indexing syntax `[LineNumberFrom..LineNumberToExcl]`. |

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
```
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
public Sep? Sep { get; init; }
/// <summary>
/// Specifies the culture used for parsing. 
/// May be `null` for default culture.
/// </summary>
public CultureInfo? CultureInfo { get; init; }
/// <summary>
/// Indicates whether the first row is a header row.
/// </summary>
public bool HasHeader { get; init; }
/// <summary>
/// Specifies the method factory used to convert a column span 
/// of `char`s to a `string`.
/// </summary>
public SepCreateToString CreateToString { get; init; }
/// <summary>
/// Disables using [csFastFloat](https://github.com/CarlVerret/csFastFloat)
/// for parsing `float` and `double`.
/// </summary>
public bool DisableFastFloat { get; init; }
/// <summary>
/// Disables checking if column count is the same for all rows.
/// </summary>
public bool DisableColCountCheck { get; init; }
```

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
```
String Length=55
```
That is, it will show information of the source for the reader, in this case a
string of length 55.

##### SepReader.Row Debuggability
If you are hovering over `row` then this will show something like:
```
  2:[5..9] = "B;\"Apple\r\nBanana\r\nOrange\r\nPear\""
```
This has the format shown below. 
```
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
```
00:'Key'   = "B"
01:'Value' = "\"Apple\r\nBanana\r\nOrange\r\nPear\""
```

##### SepReader.Col Debuggability
If you hover over `col` you should see:
```
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

static IEnumerable<T> Enumerate<T>(SepReader reader, SepReader.RowFunc<T> func)
{
    foreach (var row in reader)
    {
        yield return func(row);
    }
}
```
Which discounting the `Enumerate` method (which could naturally be an extension
method), does have less boilerplate, but not really more effective lines of
code. The issue here is that this tends to favor factoring code in a way that
can become very inefficient quickly. Consider if one wanted to only enumerate
rows matching a predicate on `Key` which meant only 1% of rows were to be
enumerated e.g.:
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
var actual = Enumerate(reader,
    row => (row["Key"].ToString(), row["Value"].Parse<double>()))
    .Where(kv => kv.Item1.StartsWith("B", StringComparison.Ordinal))
    .ToArray();

CollectionAssert.AreEqual(expected, actual);

static IEnumerable<T> Enumerate<T>(SepReader reader, SepReader.RowFunc<T> func)
{
    foreach (var row in reader)
    {
        yield return func(row);
    }
}
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
This does not take significantly longer to write and is a lot more efficient
(also avoids allocating a string for key for each row) and is easier to debug
and perhaps even read. All examples above can be seen in
[ReadMeTest.cs](src/Sep.Test/ReadMeTest.cs).

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
* `NET 7.0.X`
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
```
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

##### PackageAssets Benchmark Results
The results below show Sep is now **the fastest .NET CSV Parser** (for this
benchmark on these platforms and machines 😀). While for pure parsing allocating
only a fraction of the memory due to extensive use of pooling and the
`ArrayPool<T>`.

This is in many aspects due to Sep having extremely optimized string pooling and
optimized hashing of `ReadOnlySpan<char>`, and thus not really due the the
csv-parsing itself, since that is not a big part of the time consumed. At least
not for a decently fast csv-parser.

###### AMD.Ryzen.9.5950X - PackageAssets Benchmark Results (Sep 0.2.8.0, Sylvan  1.3.3.0, CsvHelper 30.0.1.0)

| Method    | Runtime  | Scope | Rows  | Mean       | Ratio | MB | MB/s    | ns/row | Allocated    | Alloc Ratio |
|---------- |--------- |------ |------ |-----------:|------:|---:|--------:|-------:|-------------:|------------:|
| Sep______ | .NET 7.0 | Row   | 50000 |   2.481 ms |  1.00 | 29 | 11761.3 |   49.6 |      1.13 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Row   | 50000 |   3.117 ms |  1.26 | 29 |  9360.8 |   62.3 |      7.17 KB |        6.34 |
| ReadLine_ | .NET 7.0 | Row   | 50000 |  13.023 ms |  5.20 | 29 |  2240.8 |  260.5 |  88608.25 KB |   78,287.19 |
| CsvHelper | .NET 7.0 | Row   | 50000 |  51.579 ms | 20.76 | 29 |   565.8 | 1031.6 |     20.65 KB |       18.25 |
| Sep______ | .NET 8.0 | Row   | 50000 |   2.436 ms |  0.98 | 29 | 11978.3 |   48.7 |      1.13 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Row   | 50000 |   2.929 ms |  1.18 | 29 |  9962.0 |   58.6 |      7.17 KB |        6.33 |
| ReadLine_ | .NET 8.0 | Row   | 50000 |  11.788 ms |  4.76 | 29 |  2475.5 |  235.8 |  88608.24 KB |   78,287.18 |
| CsvHelper | .NET 8.0 | Row   | 50000 |  42.562 ms | 17.15 | 29 |   685.6 |  851.2 |     20.59 KB |       18.19 |
|           |          |       |       |            |       |    |         |        |              |             |
| Sep______ | .NET 7.0 | Cols  | 50000 |   3.166 ms |  1.00 | 29 |  9218.1 |   63.3 |      1.13 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Cols  | 50000 |   5.460 ms |  1.72 | 29 |  5344.6 |  109.2 |      7.18 KB |        6.33 |
| ReadLine_ | .NET 7.0 | Cols  | 50000 |  13.603 ms |  4.26 | 29 |  2145.1 |  272.1 |  88608.25 KB |   78,152.32 |
| CsvHelper | .NET 7.0 | Cols  | 50000 |  83.833 ms | 26.47 | 29 |   348.1 | 1676.7 |    446.31 KB |      393.65 |
| Sep______ | .NET 8.0 | Cols  | 50000 |   3.142 ms |  0.99 | 29 |  9288.8 |   62.8 |      1.13 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Cols  | 50000 |   5.181 ms |  1.64 | 29 |  5632.6 |  103.6 |      7.18 KB |        6.33 |
| ReadLine_ | .NET 8.0 | Cols  | 50000 |  12.208 ms |  3.85 | 29 |  2390.4 |  244.2 |  88608.24 KB |   78,152.32 |
| CsvHelper | .NET 8.0 | Cols  | 50000 |  70.302 ms | 22.22 | 29 |   415.1 | 1406.0 |    446.35 KB |      393.68 |
|           |          |       |       |            |       |    |         |        |              |             |
| Sep______ | .NET 7.0 | Asset | 50000 |  38.120 ms |  1.00 | 29 |   765.5 |  762.4 |  13800.21 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Asset | 50000 |  44.675 ms |  1.17 | 29 |   653.2 |  893.5 |     14025 KB |        1.02 |
| ReadLine_ | .NET 7.0 | Asset | 50000 | 113.648 ms |  2.98 | 29 |   256.8 | 2273.0 | 102133.41 KB |        7.40 |
| CsvHelper | .NET 7.0 | Asset | 50000 | 105.184 ms |  2.77 | 29 |   277.4 | 2103.7 |  13971.28 KB |        1.01 |
| Sep______ | .NET 8.0 | Asset | 50000 |  30.393 ms |  0.80 | 29 |   960.1 |  607.9 |  13799.66 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 50000 |  38.855 ms |  1.02 | 29 |   751.0 |  777.1 |  14025.03 KB |        1.02 |
| ReadLine_ | .NET 8.0 | Asset | 50000 | 121.473 ms |  3.19 | 29 |   240.2 | 2429.5 | 102133.36 KB |        7.40 |
| CsvHelper | .NET 8.0 | Asset | 50000 |  93.300 ms |  2.45 | 29 |   312.8 | 1866.0 |  13972.05 KB |        1.01 |

###### Intel.Xeon.Silver.4316.2.30GHz - PackageAssets Benchmark Results (Sep 0.2.3, Sylvan  1.3.2.0, CsvHelper 30.0.1.0)

|    Method |  Runtime | Scope |  Rows |       Mean | Ratio | MB |   MB/s | ns/row |    Allocated | Alloc Ratio |
|---------- |--------- |------ |------ |-----------:|------:|---:|-------:|-------:|-------------:|------------:|
| Sep______ | .NET 7.0 |   Row | 50000 |   5.948 ms |  1.00 | 29 | 4905.8 |  119.0 |      1.14 KB |        1.00 |
| Sylvan___ | .NET 7.0 |   Row | 50000 |   6.733 ms |  1.13 | 29 | 4334.0 |  134.7 |      7.18 KB |        6.29 |
| ReadLine_ | .NET 7.0 |   Row | 50000 |  27.988 ms |  4.70 | 29 | 1042.6 |  559.8 |  88608.29 KB |   77,617.53 |
| CsvHelper | .NET 7.0 |   Row | 50000 | 114.902 ms | 19.31 | 29 |  254.0 | 2298.0 |     20.65 KB |       18.09 |
| Sep______ | .NET 8.0 |   Row | 50000 |   5.440 ms |  0.91 | 29 | 5363.8 |  108.8 |      1.29 KB |        1.13 |
| Sylvan___ | .NET 8.0 |   Row | 50000 |   6.383 ms |  1.07 | 29 | 4571.5 |  127.7 |      7.18 KB |        6.29 |
| ReadLine_ | .NET 8.0 |   Row | 50000 |  26.933 ms |  4.53 | 29 | 1083.5 |  538.7 |  88608.26 KB |   77,617.50 |
| CsvHelper | .NET 8.0 |   Row | 50000 |  90.332 ms | 15.18 | 29 |  323.0 | 1806.6 |     20.69 KB |       18.12 |
|           |          |       |       |            |       |    |        |        |              |             |
| Sep______ | .NET 7.0 |  Cols | 50000 |   7.401 ms |  1.00 | 29 | 3942.7 |  148.0 |      1.15 KB |        1.00 |
| Sylvan___ | .NET 7.0 |  Cols | 50000 |  11.545 ms |  1.56 | 29 | 2527.5 |  230.9 |      7.19 KB |        6.28 |
| ReadLine_ | .NET 7.0 |  Cols | 50000 |  28.892 ms |  3.90 | 29 | 1010.0 |  577.8 |  88608.29 KB |   77,352.84 |
| CsvHelper | .NET 7.0 |  Cols | 50000 | 157.705 ms | 21.31 | 29 |  185.0 | 3154.1 |     446.5 KB |      389.78 |
| Sep______ | .NET 8.0 |  Cols | 50000 |   6.683 ms |  0.90 | 29 | 4366.8 |  133.7 |       1.3 KB |        1.13 |
| Sylvan___ | .NET 8.0 |  Cols | 50000 |  10.417 ms |  1.41 | 29 | 2801.2 |  208.3 |      7.19 KB |        6.28 |
| ReadLine_ | .NET 8.0 |  Cols | 50000 |  27.292 ms |  3.69 | 29 | 1069.2 |  545.8 |  88608.26 KB |   77,352.82 |
| CsvHelper | .NET 8.0 |  Cols | 50000 | 141.559 ms | 19.11 | 29 |  206.1 | 2831.2 |    446.45 KB |      389.74 |
|           |          |       |       |            |       |    |        |        |              |             |
| Sep______ | .NET 7.0 | Asset | 50000 |  58.708 ms |  1.00 | 29 |  497.1 | 1174.2 |  13799.65 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Asset | 50000 |  76.436 ms |  1.31 | 29 |  381.8 | 1528.7 |  14025.71 KB |        1.02 |
| ReadLine_ | .NET 7.0 | Asset | 50000 | 174.574 ms |  3.00 | 29 |  167.2 | 3491.5 | 102133.35 KB |        7.40 |
| CsvHelper | .NET 7.0 | Asset | 50000 | 172.747 ms |  2.95 | 29 |  168.9 | 3454.9 |  13970.85 KB |        1.01 |
| Sep______ | .NET 8.0 | Asset | 50000 |  55.672 ms |  0.95 | 29 |  524.2 | 1113.4 |  13800.53 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 50000 |  71.428 ms |  1.22 | 29 |  408.5 | 1428.6 |  14026.38 KB |        1.02 |
| ReadLine_ | .NET 8.0 | Asset | 50000 | 162.203 ms |  2.76 | 29 |  179.9 | 3244.1 | 102133.69 KB |        7.40 |
| CsvHelper | .NET 8.0 | Asset | 50000 | 161.805 ms |  2.76 | 29 |  180.3 | 3236.1 |   13970.8 KB |        1.01 |

###### Neoverse.N1 - PackageAssets Benchmark Results (Sep 0.2.3, Sylvan  1.3.2.0, CsvHelper 30.0.1.0)

|    Method |  Runtime | Scope |  Rows |      Mean | Ratio | MB |   MB/s | ns/row |    Allocated | Alloc Ratio |
|---------- |--------- |------ |------ |----------:|------:|---:|-------:|-------:|-------------:|------------:|
| Sep______ | .NET 7.0 |   Row | 50000 |  12.15 ms |  1.00 | 29 | 2393.9 |  243.0 |      1.11 KB |        1.00 |
| Sylvan___ | .NET 7.0 |   Row | 50000 |  34.31 ms |  2.82 | 29 |  847.7 |  686.2 |      6.25 KB |        5.64 |
| ReadLine_ | .NET 7.0 |   Row | 50000 |  40.99 ms |  3.37 | 29 |  709.6 |  819.8 |  88608.34 KB |   79,942.68 |
| CsvHelper | .NET 7.0 |   Row | 50000 | 107.97 ms |  8.89 | 29 |  269.4 | 2159.4 |     20.74 KB |       18.71 |
| Sep______ | .NET 8.0 |   Row | 50000 |  12.01 ms |  0.99 | 29 | 2421.2 |  240.3 |       1.1 KB |        1.00 |
| Sylvan___ | .NET 8.0 |   Row | 50000 |  30.14 ms |  2.48 | 29 |  965.0 |  602.8 |      6.09 KB |        5.50 |
| ReadLine_ | .NET 8.0 |   Row | 50000 |  39.05 ms |  3.21 | 29 |  744.9 |  780.9 |  88608.41 KB |   79,942.74 |
| CsvHelper | .NET 8.0 |   Row | 50000 |  91.57 ms |  7.54 | 29 |  317.6 | 1831.5 |     20.77 KB |       18.74 |
|           |          |       |       |           |       |    |        |        |              |             |
| Sep______ | .NET 7.0 |  Cols | 50000 |  14.52 ms |  1.00 | 29 | 2002.8 |  290.5 |      1.12 KB |        1.00 |
| Sylvan___ | .NET 7.0 |  Cols | 50000 |  40.36 ms |  2.78 | 29 |  720.6 |  807.2 |      6.25 KB |        5.57 |
| ReadLine_ | .NET 7.0 |  Cols | 50000 |  41.92 ms |  2.89 | 29 |  693.8 |  838.5 |  88608.36 KB |   78,899.97 |
| CsvHelper | .NET 7.0 |  Cols | 50000 | 156.03 ms | 10.74 | 29 |  186.4 | 3120.6 |    446.66 KB |      397.72 |
| Sep______ | .NET 8.0 |  Cols | 50000 |  14.24 ms |  0.98 | 29 | 2042.8 |  284.8 |      1.11 KB |        0.99 |
| Sylvan___ | .NET 8.0 |  Cols | 50000 |  35.23 ms |  2.43 | 29 |  825.6 |  704.6 |      6.11 KB |        5.44 |
| ReadLine_ | .NET 8.0 |  Cols | 50000 |  37.10 ms |  2.55 | 29 |  784.0 |  742.0 |  88608.32 KB |   78,899.93 |
| CsvHelper | .NET 8.0 |  Cols | 50000 | 138.20 ms |  9.52 | 29 |  210.5 | 2763.9 |    446.61 KB |      397.68 |
|           |          |       |       |           |       |    |        |        |              |             |
| Sep______ | .NET 7.0 | Asset | 50000 |  70.16 ms |  1.00 | 29 |  414.5 | 1403.3 |  13800.75 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Asset | 50000 | 107.28 ms |  1.53 | 29 |  271.1 | 2145.6 |  14025.09 KB |        1.02 |
| ReadLine_ | .NET 7.0 | Asset | 50000 | 159.87 ms |  2.27 | 29 |  181.9 | 3197.4 | 102134.11 KB |        7.40 |
| CsvHelper | .NET 7.0 | Asset | 50000 | 189.44 ms |  2.69 | 29 |  153.5 | 3788.8 |  13973.66 KB |        1.01 |
| Sep______ | .NET 8.0 | Asset | 50000 |  67.72 ms |  0.96 | 29 |  429.5 | 1354.3 |  13800.73 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 50000 | 100.09 ms |  1.43 | 29 |  290.6 | 2001.8 |  14024.23 KB |        1.02 |
| ReadLine_ | .NET 8.0 | Asset | 50000 | 144.19 ms |  2.06 | 29 |  201.7 | 2883.8 | 102134.74 KB |        7.40 |
| CsvHelper | .NET 8.0 | Asset | 50000 | 170.81 ms |  2.43 | 29 |  170.3 | 3416.1 |  13971.32 KB |        1.01 |


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

###### AMD.Ryzen.9.5950X - PackageAssets with Quotes Benchmark Results (Sep 0.2.8.0, Sylvan  1.3.3.0, CsvHelper 30.0.1.0)

| Method    | Runtime  | Scope | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated    | Alloc Ratio |
|---------- |--------- |------ |------ |-----------:|------:|---:|-------:|-------:|-------------:|------------:|
| Sep______ | .NET 7.0 | Row   | 50000 |   7.243 ms |  1.00 | 33 | 4608.3 |  144.9 |      1.14 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Row   | 50000 |  21.395 ms |  2.96 | 33 | 1560.0 |  427.9 |      7.33 KB |        6.41 |
| ReadLine_ | .NET 7.0 | Row   | 50000 |  16.510 ms |  2.27 | 33 | 2021.6 |  330.2 | 108778.76 KB |   95,042.19 |
| CsvHelper | .NET 7.0 | Row   | 50000 |  67.351 ms |  9.26 | 33 |  495.6 | 1347.0 |     20.65 KB |       18.05 |
| Sep______ | .NET 8.0 | Row   | 50000 |   6.567 ms |  0.90 | 33 | 5082.8 |  131.3 |      1.14 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Row   | 50000 |  17.739 ms |  2.43 | 33 | 1881.6 |  354.8 |       7.2 KB |        6.29 |
| ReadLine_ | .NET 8.0 | Row   | 50000 |  14.818 ms |  2.06 | 33 | 2252.6 |  296.4 | 108778.75 KB |   95,042.18 |
| CsvHelper | .NET 8.0 | Row   | 50000 |  52.127 ms |  7.16 | 33 |  640.3 | 1042.5 |      20.6 KB |       18.00 |
|           |          |       |       |            |       |    |        |        |              |             |
| Sep______ | .NET 7.0 | Cols  | 50000 |   7.380 ms |  1.00 | 33 | 4522.8 |  147.6 |      1.15 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Cols  | 50000 |  23.992 ms |  3.25 | 33 | 1391.2 |  479.8 |      7.22 KB |        6.29 |
| ReadLine_ | .NET 7.0 | Cols  | 50000 |  16.336 ms |  2.19 | 33 | 2043.2 |  326.7 | 108778.75 KB |   94,799.53 |
| CsvHelper | .NET 7.0 | Cols  | 50000 |  88.119 ms | 11.94 | 33 |  378.8 | 1762.4 |    446.31 KB |      388.95 |
| Sep______ | .NET 8.0 | Cols  | 50000 |   7.068 ms |  0.96 | 33 | 4722.2 |  141.4 |      1.14 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Cols  | 50000 |  19.952 ms |  2.70 | 33 | 1672.9 |  399.0 |      7.21 KB |        6.29 |
| ReadLine_ | .NET 8.0 | Cols  | 50000 |  14.506 ms |  1.96 | 33 | 2301.0 |  290.1 | 108778.75 KB |   94,799.52 |
| CsvHelper | .NET 8.0 | Cols  | 50000 |  82.632 ms | 11.20 | 33 |  403.9 | 1652.6 |    446.35 KB |      388.99 |
|           |          |       |       |            |       |    |        |        |              |             |
| Sep______ | .NET 7.0 | Asset | 50000 |  38.018 ms |  1.00 | 33 |  877.9 |  760.4 |  13808.03 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Asset | 50000 |  60.345 ms |  1.65 | 33 |  553.1 | 1206.9 |  14026.44 KB |        1.02 |
| ReadLine_ | .NET 7.0 | Asset | 50000 | 126.464 ms |  3.20 | 33 |  263.9 | 2529.3 | 122303.92 KB |        8.86 |
| CsvHelper | .NET 7.0 | Asset | 50000 | 111.102 ms |  2.92 | 33 |  300.4 | 2222.0 |  13970.78 KB |        1.01 |
| Sep______ | .NET 8.0 | Asset | 50000 |  34.347 ms |  0.91 | 33 |  971.8 |  686.9 |  13808.08 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 50000 |  52.851 ms |  1.38 | 33 |  631.5 | 1057.0 |  14026.01 KB |        1.02 |
| ReadLine_ | .NET 8.0 | Asset | 50000 | 122.265 ms |  3.19 | 33 |  273.0 | 2445.3 | 122303.85 KB |        8.86 |
| CsvHelper | .NET 8.0 | Asset | 50000 |  96.526 ms |  2.54 | 33 |  345.8 | 1930.5 |  13971.86 KB |        1.01 |

###### Intel.Xeon.Silver.4316.2.30GHz - PackageAssets with Quotes Benchmark Results (Sep 0.2.3, Sylvan  1.3.2.0, CsvHelper 30.0.1.0)

|    Method |  Runtime | Scope |  Rows |      Mean | Ratio | MB |   MB/s | ns/row |    Allocated | Alloc Ratio |
|---------- |--------- |------ |------ |----------:|------:|---:|-------:|-------:|-------------:|------------:|
| Sep______ | .NET 7.0 |   Row | 50000 |  15.13 ms |  1.00 | 33 | 2206.5 |  302.5 |      1.17 KB |        1.00 |
| Sylvan___ | .NET 7.0 |   Row | 50000 |  42.25 ms |  2.80 | 33 |  790.0 |  845.0 |      7.26 KB |        6.22 |
| ReadLine_ | .NET 7.0 |   Row | 50000 |  32.95 ms |  2.18 | 33 | 1013.0 |  659.0 | 108778.78 KB |   93,135.01 |
| CsvHelper | .NET 7.0 |   Row | 50000 | 121.55 ms |  8.03 | 33 |  274.6 | 2431.1 |     20.84 KB |       17.84 |
| Sep______ | .NET 8.0 |   Row | 50000 |  12.34 ms |  0.82 | 33 | 2705.0 |  246.8 |      1.31 KB |        1.12 |
| Sylvan___ | .NET 8.0 |   Row | 50000 |  35.42 ms |  2.34 | 33 |  942.4 |  708.4 |      7.24 KB |        6.20 |
| ReadLine_ | .NET 8.0 |   Row | 50000 |  32.15 ms |  2.13 | 33 | 1038.3 |  642.9 | 108778.77 KB |   93,135.00 |
| CsvHelper | .NET 8.0 |   Row | 50000 | 104.02 ms |  6.88 | 33 |  320.9 | 2080.4 |     20.69 KB |       17.72 |
|           |          |       |       |           |       |    |        |        |              |             |
| Sep______ | .NET 7.0 |  Cols | 50000 |  15.80 ms |  1.00 | 33 | 2112.6 |  316.0 |      1.18 KB |        1.00 |
| Sylvan___ | .NET 7.0 |  Cols | 50000 |  45.83 ms |  2.91 | 33 |  728.3 |  916.6 |      7.33 KB |        6.24 |
| ReadLine_ | .NET 7.0 |  Cols | 50000 |  33.98 ms |  2.15 | 33 |  982.2 |  679.7 | 108778.78 KB |   92,516.17 |
| CsvHelper | .NET 7.0 |  Cols | 50000 | 177.61 ms | 11.23 | 33 |  187.9 | 3552.2 |    446.43 KB |      379.69 |
| Sep______ | .NET 8.0 |  Cols | 50000 |  15.59 ms |  0.99 | 33 | 2140.6 |  311.8 |       1.5 KB |        1.28 |
| Sylvan___ | .NET 8.0 |  Cols | 50000 |  38.85 ms |  2.46 | 33 |  859.2 |  777.0 |      7.25 KB |        6.16 |
| ReadLine_ | .NET 8.0 |  Cols | 50000 |  32.64 ms |  2.06 | 33 | 1022.6 |  652.8 | 108778.77 KB |   92,516.16 |
| CsvHelper | .NET 8.0 |  Cols | 50000 | 161.84 ms | 10.23 | 33 |  206.2 | 3236.9 |    446.45 KB |      379.70 |
|           |          |       |       |           |       |    |        |        |              |             |
| Sep______ | .NET 7.0 | Asset | 50000 |  71.19 ms |  1.00 | 33 |  468.9 | 1423.8 |  13808.03 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Asset | 50000 | 106.82 ms |  1.50 | 33 |  312.5 | 2136.5 |  14025.33 KB |        1.02 |
| ReadLine_ | .NET 7.0 | Asset | 50000 | 205.79 ms |  2.89 | 33 |  162.2 | 4115.9 | 122304.11 KB |        8.86 |
| CsvHelper | .NET 7.0 | Asset | 50000 | 192.54 ms |  2.71 | 33 |  173.3 | 3850.9 |  13970.85 KB |        1.01 |
| Sep______ | .NET 8.0 | Asset | 50000 |  52.18 ms |  0.74 | 33 |  639.6 | 1043.7 |  13808.73 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 50000 |  96.05 ms |  1.35 | 33 |  347.5 | 1921.0 |  14026.75 KB |        1.02 |
| ReadLine_ | .NET 8.0 | Asset | 50000 | 202.80 ms |  2.82 | 33 |  164.6 | 4056.0 | 122304.21 KB |        8.86 |
| CsvHelper | .NET 8.0 | Asset | 50000 | 179.61 ms |  2.52 | 33 |  185.8 | 3592.1 |   13970.8 KB |        1.01 |

###### Neoverse.N1 - PackageAssets with Quotes Benchmark Results (Sep 0.2.3, Sylvan  1.3.2.0, CsvHelper 30.0.1.0)

|    Method |  Runtime | Scope |  Rows |      Mean | Ratio | MB |   MB/s | ns/row |    Allocated | Alloc Ratio |
|---------- |--------- |------ |------ |----------:|------:|---:|-------:|-------:|-------------:|------------:|
| Sep______ | .NET 7.0 |   Row | 50000 |  25.47 ms |  1.00 | 33 | 1306.5 |  509.5 |      1.39 KB |        1.00 |
| Sylvan___ | .NET 7.0 |   Row | 50000 |  46.21 ms |  1.81 | 33 |  720.2 |  924.2 |      6.25 KB |        4.50 |
| ReadLine_ | .NET 7.0 |   Row | 50000 |  49.46 ms |  1.94 | 33 |  673.0 |  989.1 | 108778.86 KB |   78,333.02 |
| CsvHelper | .NET 7.0 |   Row | 50000 | 121.28 ms |  4.76 | 33 |  274.4 | 2425.6 |     20.74 KB |       14.93 |
| Sep______ | .NET 8.0 |   Row | 50000 |  23.43 ms |  0.92 | 33 | 1420.5 |  468.6 |      1.36 KB |        0.98 |
| Sylvan___ | .NET 8.0 |   Row | 50000 |  39.42 ms |  1.55 | 33 |  844.2 |  788.5 |      6.11 KB |        4.40 |
| ReadLine_ | .NET 8.0 |   Row | 50000 |  47.68 ms |  1.87 | 33 |  698.1 |  953.5 | 108778.91 KB |   78,333.05 |
| CsvHelper | .NET 8.0 |   Row | 50000 | 107.00 ms |  4.20 | 33 |  311.0 | 2140.1 |     20.77 KB |       14.96 |
|           |          |       |       |           |       |    |        |        |              |             |
| Sep______ | .NET 7.0 |  Cols | 50000 |  28.30 ms |  1.00 | 33 | 1175.9 |  566.1 |      1.39 KB |        1.00 |
| Sylvan___ | .NET 7.0 |  Cols | 50000 |  53.39 ms |  1.89 | 33 |  623.4 | 1067.8 |      6.25 KB |        4.50 |
| ReadLine_ | .NET 7.0 |  Cols | 50000 |  50.71 ms |  1.79 | 33 |  656.3 | 1014.2 | 108778.93 KB |   78,333.07 |
| CsvHelper | .NET 7.0 |  Cols | 50000 | 180.08 ms |  6.36 | 33 |  184.8 | 3601.5 |    446.66 KB |      321.65 |
| Sep______ | .NET 8.0 |  Cols | 50000 |  25.68 ms |  0.91 | 33 | 1295.8 |  513.7 |      1.36 KB |        0.98 |
| Sylvan___ | .NET 8.0 |  Cols | 50000 |  45.33 ms |  1.61 | 33 |  734.2 |  906.6 |      6.15 KB |        4.43 |
| ReadLine_ | .NET 8.0 |  Cols | 50000 |  44.96 ms |  1.59 | 33 |  740.3 |  899.2 | 108778.91 KB |   78,333.05 |
| CsvHelper | .NET 8.0 |  Cols | 50000 | 153.04 ms |  5.41 | 33 |  217.5 | 3060.7 |    446.61 KB |      321.61 |
|           |          |       |       |           |       |    |        |        |              |             |
| Sep______ | .NET 7.0 | Asset | 50000 |  88.20 ms |  1.00 | 33 |  377.4 | 1764.0 |  13808.62 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Asset | 50000 | 115.22 ms |  1.31 | 33 |  288.9 | 2304.4 |  14024.52 KB |        1.02 |
| ReadLine_ | .NET 7.0 | Asset | 50000 | 200.14 ms |  2.28 | 33 |  166.3 | 4002.9 | 122305.55 KB |        8.86 |
| CsvHelper | .NET 7.0 | Asset | 50000 | 208.19 ms |  2.35 | 33 |  159.9 | 4163.8 |  13971.42 KB |        1.01 |
| Sep______ | .NET 8.0 | Asset | 50000 |  84.89 ms |  0.96 | 33 |  392.1 | 1697.7 |  13808.47 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 50000 | 109.42 ms |  1.24 | 33 |  304.2 | 2188.4 |   14025.4 KB |        1.02 |
| ReadLine_ | .NET 8.0 | Asset | 50000 | 190.83 ms |  2.19 | 33 |  174.4 | 3816.6 | 122305.06 KB |        8.86 |
| CsvHelper | .NET 8.0 | Asset | 50000 | 185.03 ms |  2.09 | 33 |  179.9 | 3700.5 |  13972.06 KB |        1.01 |


#### Floats Reader Comparison Benchmarks
The [FloatsReaderBench.cs](src/Sep.ComparisonBenchmarks/FloatsReaderBench.cs)
benchmark demonstrates what Sep is built for. Namely parsing 32-bit floating
points or features as in machine learning. Here a simple CSV-file is randomly
generated with `N` ground truth values, `N` predicted result values and some
typical extra columns leading that, but which aren't used as such in the
benchmark. `N = 20` here. For example:
```
Set;FileName;DataSplit;GT_Feature0;GT_Feature1;GT_Feature2;GT_Feature3;GT_Feature4;GT_Feature5;GT_Feature6;GT_Feature7;GT_Feature8;GT_Feature9;GT_Feature10;GT_Feature11;GT_Feature12;GT_Feature13;GT_Feature14;GT_Feature15;GT_Feature16;GT_Feature17;GT_Feature18;GT_Feature19;RE_Feature0;RE_Feature1;RE_Feature2;RE_Feature3;RE_Feature4;RE_Feature5;RE_Feature6;RE_Feature7;RE_Feature8;RE_Feature9;RE_Feature10;RE_Feature11;RE_Feature12;RE_Feature13;RE_Feature14;RE_Feature15;RE_Feature16;RE_Feature17;RE_Feature18;RE_Feature19
SetCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC;wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww.png;Train;0.52276427;0.16843422;0.26259267;0.7244084;0.51292276;0.17365117;0.76125056;0.23458846;0.2573214;0.50560355;0.3202332;0.3809696;0.26024464;0.5174511;0.035318818;0.8141374;0.57719684;0.3974705;0.15219308;0.09011261;0.70515215;0.81618196;0.5399706;0.044147138;0.7111546;0.14776127;0.90621275;0.6925897;0.5164137;0.18637845;0.041509967;0.30819967;0.5831603;0.8210651;0.003954861;0.535722;0.8051845;0.7483589;0.3845737;0.14911908
SetAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA;mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm.png;Test;0.6264564;0.11517637;0.24996082;0.77242833;0.2896067;0.6481459;0.14364648;0.044498358;0.6045593;0.51591337;0.050794687;0.42036617;0.7065823;0.6284636;0.21844554;0.013253775;0.36516154;0.2674384;0.06866083;0.71817476;0.07094294;0.46409357;0.012033525;0.7978093;0.43917948;0.5134962;0.4995968;0.008952909;0.82883793;0.012896823;0.0030740085;0.063773096;0.6541431;0.034539033;0.9135142;0.92897075;0.46119377;0.37533295;0.61660606;0.044443816
SetBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB;lllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll.png;Validation;0.7922863;0.5323656;0.400699;0.29737252;0.9072584;0.58673894;0.73510516;0.019412167;0.88168067;0.9576787;0.33283427;0.7107;0.1623628;0.10314285;0.4521515;0.33324885;0.7761104;0.14854911;0.13469358;0.21566042;0.59166247;0.5128394;0.98702157;0.766223;0.67204326;0.7149494;0.2894748;0.55206;0.9898286;0.65083236;0.02421702;0.34540752;0.92906284;0.027142895;0.21974725;0.26544374;0.03848049;0.2161237;0.59233844;0.42221397
SetAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA;ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss.png;Train;0.10609442;0.32130885;0.32383907;0.7511514;0.8258279;0.00904226;0.0420841;0.84049565;0.8958947;0.23807365;0.92621964;0.8452882;0.2794469;0.545344;0.63447595;0.62532926;0.19230893;0.29726416;0.18304513;0.029583583;0.23084833;0.93346167;0.98742676;0.78163713;0.13521992;0.8833956;0.18670778;0.29476836;0.5599867;0.5562107;0.7124796;0.121927656;0.5981778;0.39144602;0.88092715;0.4449142;0.34820423;0.96379805;0.46364686;0.54301775
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
tiny part of the total runtime for Sep for which the run time is dominated by
parsing the floating points. Since Sep uses
[csFastFloat](https://github.com/CarlVerret/csFastFloat) for an integrated fast
floating point parser, it is **>2x faster than Sylvan** for example. If using
Sylvan one may consider using csFastFloat if that is an option.

CsvHelper suffers from the fact that one can only access the column as a string
so this has to be allocated for each column (ReadLine by definition always
allocates a string per column). Still CsvHelper is significantly slower than the
naive `ReadLine` approach. With Sep being **>3.8x faster than CsvHelper**.

It is a testament to how good the .NET and the .NET GC is that the ReadLine is
pretty good compared to CsvHelper regardless of allocating a lot of strings. 

##### AMD.Ryzen.9.5950X - FloatsReader Benchmark Results (Sep 0.2.8.0, Sylvan  1.3.3.0, CsvHelper 30.0.1.0)

| Method    | Runtime  | Scope  | Rows  | Mean       | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |--------- |------- |------ |-----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | .NET 7.0 | Row    | 25000 |   2.642 ms |  1.00 | 27 | 10318.8 |  105.7 |     1.56 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Row    | 25000 |   3.025 ms |  1.14 | 27 |  9013.6 |  121.0 |    10.55 KB |        6.78 |
| ReadLine_ | .NET 7.0 | Row    | 25000 |  14.068 ms |  5.31 | 27 |  1938.0 |  562.7 | 89986.82 KB |   57,808.35 |
| CsvHelper | .NET 7.0 | Row    | 25000 |  47.926 ms | 18.16 | 27 |   568.9 | 1917.1 |    20.74 KB |       13.32 |
| Sep______ | .NET 8.0 | Row    | 25000 |   2.571 ms |  0.97 | 27 | 10604.7 |  102.8 |     1.56 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Row    | 25000 |   2.925 ms |  1.11 | 27 |  9320.4 |  117.0 |    10.55 KB |        6.78 |
| ReadLine_ | .NET 8.0 | Row    | 25000 |  13.122 ms |  5.01 | 27 |  2077.7 |  524.9 | 89986.83 KB |   57,808.35 |
| CsvHelper | .NET 8.0 | Row    | 25000 |  33.886 ms | 12.83 | 27 |   804.6 | 1355.4 |    20.61 KB |       13.24 |
|           |          |        |       |            |       |    |         |        |             |             |
| Sep______ | .NET 7.0 | Cols   | 25000 |   3.077 ms |  1.00 | 27 |  8860.0 |  123.1 |     1.56 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Cols   | 25000 |   4.852 ms |  1.58 | 27 |  5618.9 |  194.1 |    10.55 KB |        6.77 |
| ReadLine_ | .NET 7.0 | Cols   | 25000 |  14.240 ms |  4.58 | 27 |  1914.5 |  569.6 | 89986.84 KB |   57,735.92 |
| CsvHelper | .NET 7.0 | Cols   | 25000 |  42.056 ms | 13.67 | 27 |   648.3 | 1682.2 | 28451.27 KB |   18,254.45 |
| Sep______ | .NET 8.0 | Cols   | 25000 |   3.095 ms |  1.00 | 27 |  8809.6 |  123.8 |     1.56 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Cols   | 25000 |   4.609 ms |  1.50 | 27 |  5915.6 |  184.3 |    10.55 KB |        6.77 |
| ReadLine_ | .NET 8.0 | Cols   | 25000 |  13.020 ms |  4.22 | 27 |  2093.9 |  520.8 | 89986.83 KB |   57,735.91 |
| CsvHelper | .NET 8.0 | Cols   | 25000 |  36.009 ms | 11.70 | 27 |   757.1 | 1440.4 | 28451.15 KB |   18,254.37 |
|           |          |        |       |            |       |    |         |        |             |             |
| Sep______ | .NET 7.0 | Floats | 25000 |  32.440 ms |  1.00 | 27 |   840.4 | 1297.6 |     8.89 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Floats | 25000 |  68.701 ms |  2.12 | 27 |   396.8 | 2748.1 |    18.86 KB |        2.12 |
| ReadLine_ | .NET 7.0 | Floats | 25000 |  79.471 ms |  2.45 | 27 |   343.1 | 3178.8 | 89993.42 KB |   10,122.28 |
| CsvHelper | .NET 7.0 | Floats | 25000 | 133.372 ms |  4.13 | 27 |   204.4 | 5334.9 | 22039.48 KB |    2,478.96 |
| Sep______ | .NET 8.0 | Floats | 25000 |  21.978 ms |  0.68 | 27 |  1240.5 |  879.1 |     9.11 KB |        1.02 |
| Sylvan___ | .NET 8.0 | Floats | 25000 |  65.359 ms |  2.01 | 27 |   417.1 | 2614.4 |    18.84 KB |        2.12 |
| ReadLine_ | .NET 8.0 | Floats | 25000 |  72.653 ms |  2.24 | 27 |   375.3 | 2906.1 |  89990.3 KB |   10,121.93 |
| CsvHelper | .NET 8.0 | Floats | 25000 | 110.129 ms |  3.39 | 27 |   247.6 | 4405.2 | 22036.58 KB |    2,478.63 |

##### Intel.Xeon.Silver.4316.2.30GHz - FloatsReader Benchmark Results (Sep 0.2.3, Sylvan  1.3.2.0, CsvHelper 30.0.1.0)

|    Method |  Runtime |  Scope |  Rows |       Mean | Ratio | MB |   MB/s |  ns/row |   Allocated | Alloc Ratio |
|---------- |--------- |------- |------ |-----------:|------:|---:|-------:|--------:|------------:|------------:|
| Sep______ | .NET 7.0 |    Row | 25000 |   5.417 ms |  1.00 | 27 | 5033.3 |   216.7 |     1.57 KB |        1.00 |
| Sylvan___ | .NET 7.0 |    Row | 25000 |   6.328 ms |  1.17 | 27 | 4308.3 |   253.1 |    10.56 KB |        6.74 |
| ReadLine_ | .NET 7.0 |    Row | 25000 |  28.805 ms |  5.35 | 27 |  946.5 |  1152.2 |  89986.9 KB |   57,412.20 |
| CsvHelper | .NET 7.0 |    Row | 25000 |  76.930 ms | 14.20 | 27 |  354.4 |  3077.2 |    20.74 KB |       13.23 |
| Sep______ | .NET 8.0 |    Row | 25000 |   5.841 ms |  1.08 | 27 | 4667.7 |   233.6 |     1.72 KB |        1.10 |
| Sylvan___ | .NET 8.0 |    Row | 25000 |   6.526 ms |  1.20 | 27 | 4177.8 |   261.0 |    10.56 KB |        6.74 |
| ReadLine_ | .NET 8.0 |    Row | 25000 |  28.041 ms |  5.18 | 27 |  972.3 |  1121.7 |  89986.9 KB |   57,412.20 |
| CsvHelper | .NET 8.0 |    Row | 25000 |  77.364 ms | 14.27 | 27 |  352.4 |  3094.5 |    20.77 KB |       13.25 |
|           |          |        |       |            |       |    |        |         |             |             |
| Sep______ | .NET 7.0 |   Cols | 25000 |   6.921 ms |  1.00 | 27 | 3939.4 |   276.8 |     1.57 KB |        1.00 |
| Sylvan___ | .NET 7.0 |   Cols | 25000 |  10.300 ms |  1.49 | 27 | 2647.0 |   412.0 |    10.58 KB |        6.72 |
| ReadLine_ | .NET 7.0 |   Cols | 25000 |  29.077 ms |  4.19 | 27 |  937.6 |  1163.1 |  89986.9 KB |   57,198.38 |
| CsvHelper | .NET 7.0 |   Cols | 25000 |  95.212 ms | 13.76 | 27 |  286.3 |  3808.5 | 28451.27 KB |   18,084.48 |
| Sep______ | .NET 8.0 |   Cols | 25000 |   6.685 ms |  0.97 | 27 | 4078.0 |   267.4 |     1.73 KB |        1.10 |
| Sylvan___ | .NET 8.0 |   Cols | 25000 |   9.724 ms |  1.41 | 27 | 2803.8 |   388.9 |    10.57 KB |        6.72 |
| ReadLine_ | .NET 8.0 |   Cols | 25000 |  27.190 ms |  3.94 | 27 | 1002.7 |  1087.6 | 89986.89 KB |   57,198.37 |
| CsvHelper | .NET 8.0 |   Cols | 25000 |  81.925 ms | 11.88 | 27 |  332.8 |  3277.0 | 28451.84 KB |   18,084.85 |
|           |          |        |       |            |       |    |        |         |             |             |
| Sep______ | .NET 7.0 | Floats | 25000 |  61.789 ms |  1.00 | 27 |  441.2 |  2471.6 |      9.1 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Floats | 25000 | 146.737 ms |  2.37 | 27 |  185.8 |  5869.5 |    19.62 KB |        2.16 |
| ReadLine_ | .NET 7.0 | Floats | 25000 | 168.368 ms |  2.73 | 27 |  161.9 |  6734.7 | 89993.63 KB |    9,894.08 |
| CsvHelper | .NET 7.0 | Floats | 25000 | 264.441 ms |  4.28 | 27 |  103.1 | 10577.6 | 22039.48 KB |    2,423.07 |
| Sep______ | .NET 8.0 | Floats | 25000 |  44.122 ms |  0.71 | 27 |  617.9 |  1764.9 |     9.08 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Floats | 25000 | 135.504 ms |  2.19 | 27 |  201.2 |  5420.2 |    19.07 KB |        2.10 |
| ReadLine_ | .NET 8.0 | Floats | 25000 | 151.400 ms |  2.45 | 27 |  180.1 |  6056.0 | 89990.48 KB |    9,893.74 |
| CsvHelper | .NET 8.0 | Floats | 25000 | 231.426 ms |  3.75 | 27 |  117.8 |  9257.1 | 22036.58 KB |    2,422.75 |

##### Neoverse.N1 - FloatsReader Benchmark Results (Sep 0.2.3, Sylvan  1.3.2.0, CsvHelper 30.0.1.0)

|    Method |  Runtime |  Scope |  Rows |      Mean | Ratio | MB |   MB/s |  ns/row |   Allocated | Alloc Ratio |
|---------- |--------- |------- |------ |----------:|------:|---:|-------:|--------:|------------:|------------:|
| Sep______ | .NET 7.0 |    Row | 25000 |  11.54 ms |  1.00 | 27 | 2359.4 |   461.4 |     1.54 KB |        1.00 |
| Sylvan___ | .NET 7.0 |    Row | 25000 |  37.00 ms |  3.21 | 27 |  735.6 |  1479.9 |    10.48 KB |        6.80 |
| ReadLine_ | .NET 7.0 |    Row | 25000 |  40.34 ms |  3.50 | 27 |  674.7 |  1613.6 | 89986.96 KB |   58,394.58 |
| CsvHelper | .NET 7.0 |    Row | 25000 |  84.31 ms |  7.31 | 27 |  322.8 |  3372.4 |    20.82 KB |       13.51 |
| Sep______ | .NET 8.0 |    Row | 25000 |  11.63 ms |  1.01 | 27 | 2339.4 |   465.3 |     1.54 KB |        1.00 |
| Sylvan___ | .NET 8.0 |    Row | 25000 |  38.26 ms |  3.32 | 27 |  711.3 |  1530.4 |     10.3 KB |        6.69 |
| ReadLine_ | .NET 8.0 |    Row | 25000 |  40.77 ms |  3.53 | 27 |  667.5 |  1630.8 | 89987.06 KB |   58,394.64 |
| CsvHelper | .NET 8.0 |    Row | 25000 |  79.69 ms |  6.91 | 27 |  341.5 |  3187.7 |    20.86 KB |       13.53 |
|           |          |        |       |           |       |    |        |         |             |             |
| Sep______ | .NET 7.0 |   Cols | 25000 |  13.37 ms |  1.00 | 27 | 2035.0 |   535.0 |     1.55 KB |        1.00 |
| Sylvan___ | .NET 7.0 |   Cols | 25000 |  41.82 ms |  3.13 | 27 |  650.8 |  1672.8 |    10.48 KB |        6.74 |
| ReadLine_ | .NET 7.0 |   Cols | 25000 |  42.16 ms |  3.15 | 27 |  645.5 |  1686.4 | 89986.99 KB |   57,881.08 |
| CsvHelper | .NET 7.0 |   Cols | 25000 |  90.73 ms |  6.79 | 27 |  300.0 |  3629.2 | 28451.35 KB |   18,300.37 |
| Sep______ | .NET 8.0 |   Cols | 25000 |  13.74 ms |  1.03 | 27 | 1980.9 |   549.6 |     1.55 KB |        0.99 |
| Sylvan___ | .NET 8.0 |   Cols | 25000 |  42.55 ms |  3.18 | 27 |  639.7 |  1701.9 |    10.33 KB |        6.64 |
| ReadLine_ | .NET 8.0 |   Cols | 25000 |  38.88 ms |  2.90 | 27 |  700.0 |  1555.1 | 89987.06 KB |   57,881.12 |
| CsvHelper | .NET 8.0 |   Cols | 25000 |  85.89 ms |  6.42 | 27 |  316.9 |  3435.7 | 28451.39 KB |   18,300.39 |
|           |          |        |       |           |       |    |        |         |             |             |
| Sep______ | .NET 7.0 | Floats | 25000 |  64.82 ms |  1.00 | 27 |  419.9 |  2592.6 |     9.12 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Floats | 25000 | 209.21 ms |  3.23 | 27 |  130.1 |  8368.4 |    19.55 KB |        2.15 |
| ReadLine_ | .NET 7.0 | Floats | 25000 | 214.76 ms |  3.31 | 27 |  126.7 |  8590.6 | 89994.37 KB |    9,872.96 |
| CsvHelper | .NET 7.0 | Floats | 25000 | 297.94 ms |  4.60 | 27 |   91.3 | 11917.5 | 22040.05 KB |    2,417.94 |
| Sep______ | .NET 8.0 | Floats | 25000 |  53.37 ms |  0.82 | 27 |  510.0 |  2134.7 |     8.95 KB |        0.98 |
| Sylvan___ | .NET 8.0 | Floats | 25000 | 164.95 ms |  2.55 | 27 |  165.0 |  6598.0 |    18.84 KB |        2.07 |
| ReadLine_ | .NET 8.0 | Floats | 25000 | 160.20 ms |  2.47 | 27 |  169.9 |  6408.1 | 89990.64 KB |    9,872.55 |
| CsvHelper | .NET 8.0 | Floats | 25000 | 231.15 ms |  3.57 | 27 |  117.7 |  9245.8 | 22037.14 KB |    2,417.62 |


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

## RFC-4180
While the [RFC-4180](https://www.ietf.org/rfc/rfc4180.txt) requires `\r\n`
(CR,LF) as line ending, the well-known line endings (`\r\n`, `\n` and `\r`) are
supported similar to .NET. `Environment.NewLine` is used when writing. Quoting
is supported by simply matching pairs of quotes, no matter what. With no
automatic escaping. Hence, you are responsible and in control of this at this
time. 

Note that some libraries will claim conformance but the RFC is, perhaps
naturally, quite strict e.g. only comma is supported as separator/delimiter. Sep
defaults to using `;` as separator if writing, while auto-detecting supported
separators when reading. This is decidedly non-conforming.

The RFC defines the following condensed [ABNF
grammar](https://en.wikipedia.org/wiki/Augmented_Backus%E2%80%93Naur_form):
```
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
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v7.0", FrameworkDisplayName=".NET 7.0")]
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
    public delegate nietras.SeparatedValues.SepToString SepCreateToString(nietras.SeparatedValues.SepHeader header, int colIndex);
    public static class SepDefaults
    {
        public static System.Globalization.CultureInfo CultureInfo { get; }
        public static char Separator { get; }
    }
    public sealed class SepHeader
    {
        public SepHeader(string row, System.Collections.Generic.Dictionary<string, int> colNameToIndex) { }
        public System.Collections.Generic.IReadOnlyList<string> ColNames { get; }
        public bool IsEmpty { get; }
        public static nietras.SeparatedValues.SepHeader Empty { get; }
        public int IndexOf(string colName) { }
        public int[] IndicesOf(System.Collections.Generic.IReadOnlyList<string> colNames) { }
        public int[] IndicesOf(System.ReadOnlySpan<string> colNames) { }
        public int[] IndicesOf(params string[] colNames) { }
        public void IndicesOf(System.ReadOnlySpan<string> colNames, System.Span<int> colIndices) { }
        public System.Collections.Generic.IReadOnlyList<string> NamesStartingWith(string prefix, System.StringComparison comparison = 4) { }
        public override string ToString() { }
    }
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class SepReader : nietras.SeparatedValues.SepReaderState
    {
        public nietras.SeparatedValues.SepReader.Row Current { get; }
        public bool HasHeader { get; }
        public bool HasRows { get; }
        public nietras.SeparatedValues.SepHeader Header { get; }
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
            public string ToStringRaw() { }
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
            public nietras.SeparatedValues.SepReader.Col this[int index] { get; }
            public int Length { get; }
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
            public override string ToString() { }
        }
        public delegate void ColAction(nietras.SeparatedValues.SepReader.Col col);
        public delegate T ColFunc<T>(nietras.SeparatedValues.SepReader.Col col);
        public delegate void ColsAction(nietras.SeparatedValues.SepReader.Cols col);
        public delegate void RowAction(nietras.SeparatedValues.SepReader.Row row);
        public delegate T RowFunc<T>(nietras.SeparatedValues.SepReader.Row row);
    }
    public static class SepReaderExtensions
    {
        public static nietras.SeparatedValues.SepReader From(this nietras.SeparatedValues.SepReaderOptions options, byte[] buffer) { }
        public static nietras.SeparatedValues.SepReader From(this nietras.SeparatedValues.SepReaderOptions options, System.IO.Stream stream) { }
        public static nietras.SeparatedValues.SepReader From(this nietras.SeparatedValues.SepReaderOptions options, System.IO.TextReader reader) { }
        public static nietras.SeparatedValues.SepReader From(this nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.Stream> nameToStream) { }
        public static nietras.SeparatedValues.SepReader From(this nietras.SeparatedValues.SepReaderOptions options, string name, System.Func<string, System.IO.TextReader> nameToReader) { }
        public static nietras.SeparatedValues.SepReader FromFile(this nietras.SeparatedValues.SepReaderOptions options, string filePath) { }
        public static nietras.SeparatedValues.SepReader FromText(this nietras.SeparatedValues.SepReaderOptions options, string text) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.Sep sep) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.Sep? sep) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.SepSpec spec) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.Sep sep, System.Func<nietras.SeparatedValues.SepReaderOptions, nietras.SeparatedValues.SepReaderOptions> configure) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.Sep? sep, System.Func<nietras.SeparatedValues.SepReaderOptions, nietras.SeparatedValues.SepReaderOptions> configure) { }
        public static nietras.SeparatedValues.SepReaderOptions Reader(this nietras.SeparatedValues.SepSpec spec, System.Func<nietras.SeparatedValues.SepReaderOptions, nietras.SeparatedValues.SepReaderOptions> configure) { }
    }
    public readonly struct SepReaderOptions : System.IEquatable<nietras.SeparatedValues.SepReaderOptions>
    {
        public SepReaderOptions() { }
        public SepReaderOptions(nietras.SeparatedValues.Sep? sep) { }
        public nietras.SeparatedValues.SepCreateToString CreateToString { get; init; }
        public System.Globalization.CultureInfo? CultureInfo { get; init; }
        public bool DisableColCountCheck { get; init; }
        public bool DisableFastFloat { get; init; }
        public bool HasHeader { get; init; }
        public nietras.SeparatedValues.Sep? Sep { get; init; }
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
        public static nietras.SeparatedValues.SepCreateToString Direct { get; }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        protected virtual void DisposeManagedResources() { }
        public abstract string ToString(System.ReadOnlySpan<char> chars);
        public static nietras.SeparatedValues.SepCreateToString OnePool(int maximumStringLength = 32, int initialCapacity = 64, int maximumCapacity = 4096) { }
        public static nietras.SeparatedValues.SepCreateToString PoolPerCol(int maximumStringLength = 32, int initialCapacity = 64, int maximumCapacity = 4096) { }
    }
    public class SepWriter : System.IDisposable
    {
        public SepWriter(nietras.SeparatedValues.SepWriterOptions options, System.IO.TextWriter writer) { }
        public nietras.SeparatedValues.SepSpec Spec { get; }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
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
            public nietras.SeparatedValues.SepWriter.Col this[int colIndex] { get; }
            public int Length { get; }
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
        public static nietras.SeparatedValues.SepWriter ToFile(this nietras.SeparatedValues.SepWriterOptions options, string filePath) { }
        public static nietras.SeparatedValues.SepWriter ToText(this nietras.SeparatedValues.SepWriterOptions options) { }
        public static nietras.SeparatedValues.SepWriter ToText(this nietras.SeparatedValues.SepWriterOptions options, int capacity) { }
        public static nietras.SeparatedValues.SepWriterOptions Writer(this nietras.SeparatedValues.Sep sep) { }
        public static nietras.SeparatedValues.SepWriterOptions Writer(this nietras.SeparatedValues.SepSpec spec) { }
        public static nietras.SeparatedValues.SepWriterOptions Writer(this nietras.SeparatedValues.Sep sep, System.Func<nietras.SeparatedValues.SepWriterOptions, nietras.SeparatedValues.SepWriterOptions> configure) { }
        public static nietras.SeparatedValues.SepWriterOptions Writer(this nietras.SeparatedValues.SepSpec spec, System.Func<nietras.SeparatedValues.SepWriterOptions, nietras.SeparatedValues.SepWriterOptions> configure) { }
    }
    public readonly struct SepWriterOptions : System.IEquatable<nietras.SeparatedValues.SepWriterOptions>
    {
        public SepWriterOptions() { }
        public SepWriterOptions(nietras.SeparatedValues.Sep sep) { }
        public System.Globalization.CultureInfo? CultureInfo { get; init; }
        public nietras.SeparatedValues.Sep Sep { get; init; }
    }
}
```
