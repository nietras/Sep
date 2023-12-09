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
changes to input or output. What you read/write is what you get. E.g. by default
there is no "automatic" escaping/unescaping of quotes. For automatic unescaping
of quotes see [SepReaderOptions](#sepreaderoptions) and [Unescaping](#unescaping).
* **🚀 Fast** - blazing fast with both architecture specific and cross-platform
SIMD vectorized parsing incl. 64/128/256/512-bit paths e.g. AVX2, AVX-512 (.NET
8.0+), NEON. Uses [csFastFloat](https://github.com/CarlVerret/csFastFloat) for
fast parsing of floating points. Reads or writes one row at a time efficiently
with [detailed benchmarks](#comparison-benchmarks) to prove it.
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

[Example](#Example) | [Naming and Terminology](#naming-and-terminology) | [API](#application-programming-interface-api) | [Limitations and Constraints](#limitations-and-constraints) | [Comparison Benchmarks](#comparison-benchmarks) | [Example Catalogue](#example-catalogue) | [RFC-4180](#rfc-4180) | [FAQ](#frequently-asked-questions-faq)  | [Public API Reference](#public-api-reference)

## Example
```csharp
var text = """
           A;B;C;D;E;F
           Sep;🚀;1;1.2;0.1;0.5
           CSV;✅;2;2.2;0.2;1.5
           """;

using SepReader reader = Sep.Reader().FromText(text);   // Infers separator 'Sep' from header
using SepWriter writer = reader.Spec.Writer().ToText(); // Writer defined from reader 'Spec'
                                                        // Use .FromFile(...)/ToFile(...) for files
var idx = reader.Header.IndexOf("B");
var nms = new[] { "E", "F" };

foreach (SepReader.Row readRow in reader)           // Read one row at a time
{
    ReadOnlySpan<char> a = readRow["A"].Span;       // Column as ReadOnlySpan<char>
    string b = readRow[idx].ToString();             // Column to string (might be pooled)
    int c = readRow["C"].Parse<int>();              // Parse any T : ISpanParsable<T>
    float d = readRow["D"].Parse<float>();          // Parse float/double fast via csFastFloat
    Span<double> ef = readRow[nms].Parse<double>(); // Parse multiple columns as Span<T>
                                                    // - Sep handles array allocation and reuse
    
    foreach (ref double v in ef) { v *= 10; }       // Modify ef values in-place

    using var writeRow = writer.NewRow();           // Start new row. Row written on Dispose.
    writeRow["A"].Set(a);                           // Set by ReadOnlySpan<char>
    writeRow["B"].Set(b);                           // Set by string
    writeRow["C"].Set($"{c * 2}");                  // Set via InterpolatedStringHandler, no allocs
    writeRow["D"].Format(d / 2);                    // Format any T : ISpanFormattable
    writeRow[nms].Format(ef);                       // Format multiple columns directly
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

Sep supports unescaping via an option, see [SepReaderOptions](#sepreaderoptions)
and [Unescaping](#unescaping). Therefore, Sep has two methods being tested. The
default `Sep` without unescaping and `Sep_Unescape` where unescaping is enabled.
Note that only if there are quotes will there be any unescaping, but to support
unescape one has to track extra state during parsing which means there is a
slight cost no matter what, most notably for the `Cols` scope. Sep is still the
fastest of all (by far in many cases).

##### PackageAssets Benchmark Results
The results below show Sep is now **the fastest .NET CSV Parser** (for this
benchmark on these platforms and machines 😀). While for pure parsing allocating
only a fraction of the memory due to extensive use of pooling and the
`ArrayPool<T>`.

This is in many aspects due to Sep having extremely optimized string pooling and
optimized hashing of `ReadOnlySpan<char>`, and thus not really due the the
csv-parsing itself, since that is not a big part of the time consumed. At least
not for a decently fast csv-parser.

###### AMD.Ryzen.9.5950X - PackageAssets Benchmark Results (Sep 0.3.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method       | Scope | Rows  | Mean       | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |------ |-----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000 |   2.409 ms |  1.00 | 29 | 12111.6 |   48.2 |       933 B |        1.00 |
| Sep_Unescape | Row   | 50000 |   2.499 ms |  1.04 | 29 | 11675.7 |   50.0 |       934 B |        1.00 |
| Sylvan___    | Row   | 50000 |   2.913 ms |  1.21 | 29 | 10016.5 |   58.3 |      7381 B |        7.91 |
| ReadLine_    | Row   | 50000 |  12.198 ms |  5.04 | 29 |  2392.3 |  244.0 |  90734838 B |   97,250.63 |
| CsvHelper    | Row   | 50000 |  42.460 ms | 17.63 | 29 |   687.3 |  849.2 |     21074 B |       22.59 |
|              |       |       |            |       |    |         |        |             |             |
| Sep______    | Cols  | 50000 |   3.480 ms |  1.00 | 29 |  8385.3 |   69.6 |       935 B |        1.00 |
| Sep_Unescape | Cols  | 50000 |   3.959 ms |  1.14 | 29 |  7370.9 |   79.2 |       936 B |        1.00 |
| Sylvan___    | Cols  | 50000 |   5.210 ms |  1.50 | 29 |  5600.5 |  104.2 |      7385 B |        7.90 |
| ReadLine_    | Cols  | 50000 |  14.275 ms |  3.60 | 29 |  2044.3 |  285.5 |  90734826 B |   97,042.59 |
| CsvHelper    | Cols  | 50000 |  69.058 ms | 19.86 | 29 |   422.6 | 1381.2 |    457060 B |      488.83 |
|              |       |       |            |       |    |         |        |             |             |
| Sep______    | Asset | 50000 |  35.052 ms |  1.00 | 29 |   832.5 |  701.0 |  14131121 B |        1.00 |
| Sep_Unescape | Asset | 50000 |  34.813 ms |  1.00 | 29 |   838.2 |  696.3 |  14130768 B |        1.00 |
| Sylvan___    | Asset | 50000 |  38.875 ms |  1.12 | 29 |   750.6 |  777.5 |  14297351 B |        1.01 |
| ReadLine_    | Asset | 50000 | 127.866 ms |  3.76 | 29 |   228.2 | 2557.3 | 104584604 B |        7.40 |
| CsvHelper    | Asset | 50000 |  86.415 ms |  2.47 | 29 |   337.7 | 1728.3 |  14306352 B |        1.01 |

###### Intel.Xeon.Silver.4316.2.30GHz - PackageAssets Benchmark Results (Sep 0.3.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method       | Scope | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000 |   5.253 ms |  1.00 | 29 | 5554.8 |  105.1 |     1.07 KB |        1.00 |
| Sep_Unescape | Row   | 50000 |   5.287 ms |  1.01 | 29 | 5519.5 |  105.7 |     1.07 KB |        1.00 |
| Sylvan___    | Row   | 50000 |   5.925 ms |  1.13 | 29 | 4925.4 |  118.5 |     7.21 KB |        6.72 |
| ReadLine_    | Row   | 50000 |  25.585 ms |  4.87 | 29 | 1140.5 |  511.7 | 88608.26 KB |   82,561.29 |
| CsvHelper    | Row   | 50000 |  89.448 ms | 17.02 | 29 |  326.2 | 1789.0 |    20.69 KB |       19.28 |
|              |       |       |            |       |    |        |        |             |             |
| Sep______    | Cols  | 50000 |   7.123 ms |  1.00 | 29 | 4097.0 |  142.5 |     1.08 KB |        1.00 |
| Sep_Unescape | Cols  | 50000 |   7.953 ms |  1.12 | 29 | 3669.4 |  159.1 |     1.08 KB |        1.00 |
| Sylvan___    | Cols  | 50000 |  10.047 ms |  1.41 | 29 | 2904.6 |  200.9 |     7.22 KB |        6.71 |
| ReadLine_    | Cols  | 50000 |  25.720 ms |  3.61 | 29 | 1134.6 |  514.4 | 88608.26 KB |   82,261.89 |
| CsvHelper    | Cols  | 50000 | 138.163 ms | 19.40 | 29 |  211.2 | 2763.3 |   446.28 KB |      414.31 |
|              |       |       |            |       |    |        |        |             |             |
| Sep______    | Asset | 50000 |  53.365 ms |  1.00 | 29 |  546.8 | 1067.3 | 13799.86 KB |        1.00 |
| Sep_Unescape | Asset | 50000 |  53.388 ms |  1.00 | 29 |  546.6 | 1067.8 | 13800.99 KB |        1.00 |
| Sylvan___    | Asset | 50000 |  67.865 ms |  1.27 | 29 |  430.0 | 1357.3 | 13962.46 KB |        1.01 |
| ReadLine_    | Asset | 50000 | 153.589 ms |  2.88 | 29 |  190.0 | 3071.8 | 102133.4 KB |        7.40 |
| CsvHelper    | Asset | 50000 | 159.288 ms |  2.99 | 29 |  183.2 | 3185.8 |  13970.8 KB |        1.01 |

###### Neoverse.N1 - PackageAssets Benchmark Results (Sep 0.3.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method       | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |------ |----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000 |  12.56 ms |  1.00 | 29 | 2315.4 |  251.2 |       904 B |        1.00 |
| Sep_Unescape | Row   | 50000 |  12.46 ms |  0.99 | 29 | 2334.2 |  249.2 |       902 B |        1.00 |
| Sylvan___    | Row   | 50000 |  31.26 ms |  2.49 | 29 |  930.4 |  625.2 |      6269 B |        6.93 |
| ReadLine_    | Row   | 50000 |  38.28 ms |  3.05 | 29 |  759.8 |  765.6 |  90734916 B |  100,370.48 |
| CsvHelper    | Row   | 50000 |  91.28 ms |  7.27 | 29 |  318.6 | 1825.6 |     21272 B |       23.53 |
|              |       |       |           |       |    |        |        |             |             |
| Sep______    | Cols  | 50000 |  14.96 ms |  1.00 | 29 | 1944.2 |  299.2 |       913 B |        1.00 |
| Sep_Unescape | Cols  | 50000 |  15.70 ms |  1.05 | 29 | 1853.0 |  313.9 |       915 B |        1.00 |
| Sylvan___    | Cols  | 50000 |  35.36 ms |  2.36 | 29 |  822.6 |  707.2 |      6288 B |        6.89 |
| ReadLine_    | Cols  | 50000 |  40.74 ms |  2.72 | 29 |  713.9 |  814.9 |  90734929 B |   99,381.08 |
| CsvHelper    | Cols  | 50000 | 136.92 ms |  9.15 | 29 |  212.4 | 2738.3 |    457144 B |      500.71 |
|              |       |       |           |       |    |        |        |             |             |
| Sep______    | Asset | 50000 |  67.27 ms |  1.00 | 29 |  432.4 | 1345.4 |  14131346 B |        1.00 |
| Sep_Unescape | Asset | 50000 |  67.01 ms |  1.00 | 29 |  434.1 | 1340.2 |  14131792 B |        1.00 |
| Sylvan___    | Asset | 50000 | 100.28 ms |  1.49 | 29 |  290.0 | 2005.7 |  14295848 B |        1.01 |
| ReadLine_    | Asset | 50000 | 152.69 ms |  2.27 | 29 |  190.5 | 3053.8 | 104585572 B |        7.40 |
| CsvHelper    | Asset | 50000 | 166.75 ms |  2.46 | 29 |  174.4 | 3334.9 |  14306628 B |        1.01 |


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

###### AMD.Ryzen.9.5950X - PackageAssets with Quotes Benchmark Results (Sep 0.3.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method       | Scope | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000 |   7.453 ms |  1.00 | 33 | 4478.2 |  149.1 |       944 B |        1.00 |
| Sep_Unescape | Row   | 50000 |   7.381 ms |  0.99 | 33 | 4522.2 |  147.6 |       944 B |        1.00 |
| Sylvan___    | Row   | 50000 |  18.035 ms |  2.40 | 33 | 1850.7 |  360.7 |      7390 B |        7.83 |
| ReadLine_    | Row   | 50000 |  14.689 ms |  1.97 | 33 | 2272.3 |  293.8 | 111389433 B |  117,997.28 |
| CsvHelper    | Row   | 50000 |  52.212 ms |  7.01 | 33 |  639.3 | 1044.2 |     21081 B |       22.33 |
|              |       |       |            |       |    |        |        |             |             |
| Sep______    | Cols  | 50000 |   7.780 ms |  1.00 | 33 | 4290.1 |  155.6 |       946 B |        1.00 |
| Sep_Unescape | Cols  | 50000 |   8.337 ms |  1.07 | 33 | 4003.6 |  166.7 |       946 B |        1.00 |
| Sylvan___    | Cols  | 50000 |  20.329 ms |  2.60 | 33 | 1641.9 |  406.6 |      7411 B |        7.83 |
| ReadLine_    | Cols  | 50000 |  15.540 ms |  1.99 | 33 | 2147.8 |  310.8 | 111389433 B |  117,747.82 |
| CsvHelper    | Cols  | 50000 |  83.149 ms | 10.64 | 33 |  401.4 | 1663.0 |    457060 B |      483.15 |
|              |       |       |            |       |    |        |        |             |             |
| Sep______    | Asset | 50000 |  35.903 ms |  1.00 | 33 |  929.6 |  718.1 |  14139438 B |        1.00 |
| Sep_Unescape | Asset | 50000 |  34.316 ms |  0.93 | 33 |  972.7 |  686.3 |  14130926 B |        1.00 |
| Sylvan___    | Asset | 50000 |  51.465 ms |  1.43 | 33 |  648.5 | 1029.3 |  14296613 B |        1.01 |
| ReadLine_    | Asset | 50000 | 117.914 ms |  3.29 | 33 |  283.1 | 2358.3 | 125239144 B |        8.86 |
| CsvHelper    | Asset | 50000 |  96.670 ms |  2.57 | 33 |  345.3 | 1933.4 |  14307304 B |        1.01 |

###### Intel.Xeon.Silver.4316.2.30GHz - PackageAssets with Quotes Benchmark Results (Sep 0.3.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method       | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |------ |----------:|------:|---:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000 |  13.72 ms |  1.00 | 33 | 2433.1 |  274.4 |      1.09 KB |        1.00 |
| Sep_Unescape | Row   | 50000 |  14.36 ms |  1.05 | 33 | 2325.0 |  287.1 |      1.09 KB |        1.00 |
| Sylvan___    | Row   | 50000 |  34.97 ms |  2.55 | 33 |  954.3 |  699.5 |      7.26 KB |        6.66 |
| ReadLine_    | Row   | 50000 |  29.99 ms |  2.18 | 33 | 1113.0 |  599.8 | 108778.77 KB |   99,721.98 |
| CsvHelper    | Row   | 50000 | 102.27 ms |  7.45 | 33 |  326.4 | 2045.5 |     20.69 KB |       18.97 |
|              |       |       |           |       |    |        |        |              |             |
| Sep______    | Cols  | 50000 |  15.38 ms |  1.00 | 33 | 2169.7 |  307.7 |      1.09 KB |        1.00 |
| Sep_Unescape | Cols  | 50000 |  17.71 ms |  1.15 | 33 | 1884.8 |  354.2 |       1.1 KB |        1.01 |
| Sylvan___    | Cols  | 50000 |  40.92 ms |  2.71 | 33 |  815.6 |  818.5 |      7.27 KB |        6.64 |
| ReadLine_    | Cols  | 50000 |  30.22 ms |  1.96 | 33 | 1104.5 |  604.4 | 108778.77 KB |   99,366.16 |
| CsvHelper    | Cols  | 50000 | 159.90 ms | 10.39 | 33 |  208.7 | 3198.0 |    446.45 KB |      407.81 |
|              |       |       |           |       |    |        |        |              |             |
| Sep______    | Asset | 50000 |  66.17 ms |  1.00 | 33 |  504.4 | 1323.4 |  13809.96 KB |        1.00 |
| Sep_Unescape | Asset | 50000 |  62.28 ms |  0.95 | 33 |  536.0 | 1245.5 |  13799.79 KB |        1.00 |
| Sylvan___    | Asset | 50000 |  95.92 ms |  1.45 | 33 |  348.0 | 1918.4 |  13962.73 KB |        1.01 |
| ReadLine_    | Asset | 50000 | 193.85 ms |  2.96 | 33 |  172.2 | 3876.9 |  122304.1 KB |        8.86 |
| CsvHelper    | Asset | 50000 | 178.36 ms |  2.70 | 33 |  187.1 | 3567.2 |   13970.8 KB |        1.01 |

###### Neoverse.N1 - PackageAssets with Quotes Benchmark Results (Sep 0.3.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method       | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |------ |----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000 |  24.35 ms |  1.00 | 33 | 1366.9 |  487.0 |       941 B |        1.00 |
| Sep_Unescape | Row   | 50000 |  24.21 ms |  0.99 | 33 | 1374.7 |  484.2 |       941 B |        1.00 |
| Sylvan___    | Row   | 50000 |  39.92 ms |  1.64 | 33 |  833.6 |  798.5 |      6288 B |        6.68 |
| ReadLine_    | Row   | 50000 |  45.96 ms |  1.89 | 33 |  724.1 |  919.3 | 111389508 B |  118,373.55 |
| CsvHelper    | Row   | 50000 | 106.99 ms |  4.40 | 33 |  311.1 | 2139.8 |     21272 B |       22.61 |
|              |       |       |           |       |    |        |        |             |             |
| Sep______    | Cols  | 50000 |  26.96 ms |  1.00 | 33 | 1234.3 |  539.3 |       869 B |        1.00 |
| Sep_Unescape | Cols  | 50000 |  28.60 ms |  1.06 | 33 | 1163.6 |  572.1 |       953 B |        1.10 |
| Sylvan___    | Cols  | 50000 |  45.48 ms |  1.69 | 33 |  731.8 |  909.6 |      6318 B |        7.27 |
| ReadLine_    | Cols  | 50000 |  45.95 ms |  1.70 | 33 |  724.4 |  918.9 | 111389521 B |  128,181.27 |
| CsvHelper    | Cols  | 50000 | 152.46 ms |  5.65 | 33 |  218.3 | 3049.1 |    457328 B |      526.27 |
|              |       |       |           |       |    |        |        |             |             |
| Sep______    | Asset | 50000 |  83.86 ms |  1.00 | 33 |  396.9 | 1677.2 |  14139912 B |        1.00 |
| Sep_Unescape | Asset | 50000 |  79.99 ms |  0.96 | 33 |  416.1 | 1599.9 |  14132628 B |        1.00 |
| Sylvan___    | Asset | 50000 | 110.29 ms |  1.31 | 33 |  301.8 | 2205.9 |  14298320 B |        1.01 |
| ReadLine_    | Asset | 50000 | 178.89 ms |  2.13 | 33 |  186.1 | 3577.7 | 125240480 B |        8.86 |
| CsvHelper    | Asset | 50000 | 182.59 ms |  2.18 | 33 |  182.3 | 3651.7 |  14308800 B |        1.01 |


#### Floats Reader Comparison Benchmarks
The [FloatsReaderBench.cs](src/Sep.ComparisonBenchmarks/FloatsReaderBench.cs)
benchmark demonstrates what Sep is built for. Namely parsing 32-bit floating
points or features as in machine learning. Here a simple CSV-file is randomly
generated with `N` ground truth values, `N` predicted result values and nothing
else (note this was changed from version 0.3.0, prior to that there were some
extra leading columns). `N = 20`
here. For example:
```
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
tiny part of the total runtime for Sep for which the run time is dominated by
parsing the floating points. Since Sep uses
[csFastFloat](https://github.com/CarlVerret/csFastFloat) for an integrated fast
floating point parser, it is **>2x faster than Sylvan** for example. If using
Sylvan one may consider using csFastFloat if that is an option.

CsvHelper suffers from the fact that one can only access the column as a string
so this has to be allocated for each column (ReadLine by definition always
allocates a string per column). Still CsvHelper is significantly slower than the
naive `ReadLine` approach. With Sep being **>4x faster than CsvHelper**.

It is a testament to how good the .NET and the .NET GC is that the ReadLine is
pretty good compared to CsvHelper regardless of allocating a lot of strings. 

##### AMD.Ryzen.9.5950X - FloatsReader Benchmark Results (Sep 0.3.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   1.893 ms |  1.00 | 20 | 10733.4 |   75.7 |     1.15 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   2.212 ms |  1.17 | 20 |  9187.9 |   88.5 |    10.02 KB |        8.74 |
| ReadLine_ | Row    | 25000 |  10.913 ms |  5.76 | 20 |  1862.0 |  436.5 | 73489.63 KB |   64,099.99 |
| CsvHelper | Row    | 25000 |  25.079 ms | 13.25 | 20 |   810.2 | 1003.2 |    20.58 KB |       17.95 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |   2.591 ms |  1.00 | 20 |  7843.3 |  103.6 |     1.15 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   3.738 ms |  1.44 | 20 |  5436.2 |  149.5 |    10.03 KB |        8.74 |
| ReadLine_ | Cols   | 25000 |  11.010 ms |  4.25 | 20 |  1845.6 |  440.4 | 73489.64 KB |   64,045.44 |
| CsvHelper | Cols   | 25000 |  27.201 ms | 10.43 | 20 |   747.0 | 1088.0 | 21340.99 KB |   18,598.45 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Floats | 25000 |  21.397 ms |  1.00 | 20 |   949.7 |  855.9 |     7.87 KB |        1.00 |
| Sylvan___ | Floats | 25000 |  65.771 ms |  3.08 | 20 |   308.9 | 2630.8 |     18.2 KB |        2.31 |
| ReadLine_ | Floats | 25000 |  72.513 ms |  3.39 | 20 |   280.2 | 2900.5 | 73493.12 KB |    9,338.25 |
| CsvHelper | Floats | 25000 | 106.198 ms |  4.96 | 20 |   191.3 | 4247.9 | 22062.55 KB |    2,803.33 |

##### Intel.Xeon.Silver.4316.2.30GHz - FloatsReader Benchmark Results (Sep 0.3.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.698 ms |  1.00 | 20 | 5494.3 |  147.9 |     1.31 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   4.570 ms |  1.24 | 20 | 4446.5 |  182.8 |    10.03 KB |        7.67 |
| ReadLine_ | Row    | 25000 |  20.949 ms |  5.66 | 20 |  970.0 |  837.9 | 73489.67 KB |   56,201.21 |
| CsvHelper | Row    | 25000 |  56.975 ms | 15.41 | 20 |  356.6 | 2279.0 |    20.77 KB |       15.89 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   5.087 ms |  1.00 | 20 | 3994.7 |  203.5 |     1.31 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   7.370 ms |  1.45 | 20 | 2757.1 |  294.8 |    10.04 KB |        7.68 |
| ReadLine_ | Cols   | 25000 |  21.340 ms |  4.20 | 20 |  952.2 |  853.6 | 73489.67 KB |   56,243.22 |
| CsvHelper | Cols   | 25000 |  60.316 ms | 11.86 | 20 |  336.9 | 2412.6 | 21340.99 KB |   16,332.72 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  41.747 ms |  1.00 | 20 |  486.7 | 1669.9 |     8.12 KB |        1.00 |
| Sylvan___ | Floats | 25000 | 138.798 ms |  3.33 | 20 |  146.4 | 5551.9 |    18.43 KB |        2.27 |
| ReadLine_ | Floats | 25000 | 148.648 ms |  3.56 | 20 |  136.7 | 5945.9 |  73493.3 KB |    9,054.03 |
| CsvHelper | Floats | 25000 | 212.156 ms |  5.08 | 20 |   95.8 | 8486.2 | 22062.78 KB |    2,718.03 |

##### Neoverse.N1 - FloatsReader Benchmark Results (Sep 0.3.0.0, Sylvan  1.3.5.0, CsvHelper 30.0.1.0)

| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   9.182 ms |  1.00 | 20 | 2207.7 |  367.3 |     1.11 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  30.729 ms |  3.35 | 20 |  659.7 | 1229.1 |     9.74 KB |        8.80 |
| ReadLine_ | Row    | 25000 |  33.696 ms |  3.67 | 20 |  601.6 | 1347.8 | 73489.73 KB |   66,419.67 |
| CsvHelper | Row    | 25000 |  59.282 ms |  6.46 | 20 |  342.0 | 2371.3 |    20.71 KB |       18.71 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |  11.392 ms |  1.00 | 20 | 1779.4 |  455.7 |     1.12 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  34.813 ms |  3.06 | 20 |  582.3 | 1392.5 |     9.75 KB |        8.70 |
| ReadLine_ | Cols   | 25000 |  32.510 ms |  2.85 | 20 |  623.6 | 1300.4 | 73489.75 KB |   65,551.83 |
| CsvHelper | Cols   | 25000 |  64.080 ms |  5.63 | 20 |  316.4 | 2563.2 | 21341.07 KB |   19,035.94 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  52.081 ms |  1.00 | 20 |  389.2 | 2083.2 |     7.97 KB |        1.00 |
| Sylvan___ | Floats | 25000 | 169.677 ms |  3.26 | 20 |  119.5 | 6787.1 |     18.2 KB |        2.28 |
| ReadLine_ | Floats | 25000 | 162.237 ms |  3.11 | 20 |  125.0 | 6489.5 | 73493.46 KB |    9,221.58 |
| CsvHelper | Floats | 25000 | 220.231 ms |  4.23 | 20 |   92.0 | 8809.2 | 22063.34 KB |    2,768.39 |


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
    public delegate nietras.SeparatedValues.SepToString SepCreateToString(nietras.SeparatedValues.SepHeader? maybeHeader, int colCount);
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
        public static nietras.SeparatedValues.SepCreateToString Direct { get; }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        public abstract string ToString(System.ReadOnlySpan<char> colSpan, int colIndex);
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
