﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublicApiGenerator;
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
#pragma warning disable CA1822 // Member does not access instance data and can be marked as static

// Only parallelize on class level to avoid multiple writes to README file
[assembly: Parallelize(Workers = 1, Scope = ExecutionScope.ClassLevel)]

namespace nietras.SeparatedValues.XyzTest;

[TestClass]
public partial class ReadMeTest
{
    static readonly string s_testSourceFilePath = SourceFile();
    static readonly string s_rootDirectory = Path.GetDirectoryName(s_testSourceFilePath) + @"../../../";
    static readonly string s_readmeFilePath = s_rootDirectory + @"README.md";

    [TestMethod]
    public void ReadMeTest_()
    {
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
    }

    [TestMethod]
    public void ReadMeTest_SepReader_Debuggability()
    {
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
    }

#if NET9_0_OR_GREATER
    [TestMethod]
    public void ReadMeTest_IEnumerable_But_Not_LINQ_Compatible()
    {
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
    }
#endif

    [TestMethod]
    public void ReadMeTest_LocalFunction_YieldReturn()
    {
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
    }

    [TestMethod]
    public void ReadMeTest_Enumerate()
    {
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
    }

    [TestMethod]
    public void ReadMeTest_EnumerateWhere()
    {
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
    }

    [TestMethod]
    public void ReadMeTest_IteratorWhere()
    {
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
    }

    [TestMethod]
    public void ReadMeTest_EnumerateTrySelect()
    {
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
    }

    [TestMethod]
    public void ReadMeTest_Example_Copy_Rows()
    {
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
    }

    [TestMethod]
    public async ValueTask ReadMeTest_Example_Copy_Rows_Async()
    {
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
    }

    [TestMethod]
    public void ReadMeTest_Example_Skip_Empty_Rows()
    {
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
    }

    [TestMethod]
    public void ReadMeTest_Example_CustomSep_DisableColCountCheck()
    {
        var text = """
                   A;B;C;D;E;F
                   Sep;🚀;1;1.2;0.1
                   CSV;✅;2;2.2;0.2;1.5
                   
                   """; // Empty line at end is for line ending

        using var reader = Sep.New(';').Reader(o => o with { DisableColCountCheck = true }).FromText(text);
        using var writer = reader.Spec.Writer().ToText();
        foreach (var readRow in reader) { }
    }

    [TestMethod]
    public async Task ReadMeTest_Example_AsyncAwaitContext_Enumerate()
    {
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
    }

    [TestMethod]
    public async Task ReadMeTest_Example_AsyncAwaitContext_CustomIterator()
    {
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
    }

#if NET9_0
    // Only update README on latest .NET version to avoid multiple accesses
    [TestMethod]
#endif
    public void ReadMeTest_UpdateBenchmarksInMarkdown()
    {
        var readmeFilePath = s_readmeFilePath;

        var benchmarkFileNameToConfig = new Dictionary<string, (string Description, string ReadmeBefore, string ReadmeEnd, string SectionPrefix)>()
        {
            { "PackageAssetsBench.md", new("PackageAssets Benchmark Results", "##### PackageAssets Benchmark Results", "##### PackageAssets", "###### ") },
            { "PackageAssetsBench-GcServer.md", new("PackageAssets Benchmark Results (SERVER GC)", "##### PackageAssets Benchmark Results (SERVER GC)", "##### ", "###### ") },
            { "PackageAssetsBenchQuotes.md", new("PackageAssets with Quotes Benchmark Results", "##### PackageAssets with Quotes Benchmark Results", "##### PackageAssets", "###### ") },
            { "PackageAssetsBenchQuotes-GcServer.md", new("PackageAssets with Quotes Benchmark Results (SERVER GC)", "##### PackageAssets with Quotes Benchmark Results (SERVER GC)", "##### ", "###### ") },
            { "PackageAssetsBenchSpacesQuotes.md", new("PackageAssets with Spaces and Quotes Benchmark Results", "##### PackageAssets with Spaces and Quotes Benchmark Results", "#### ", "###### ") },
            { "FloatsReaderBench.md", new("FloatsReader Benchmark Results", "#### Floats Reader Comparison Benchmarks", "### Writer", "##### ") },
        };

        var benchmarksDirectory = Path.Combine(s_rootDirectory, "benchmarks");
        var processorDirectories = Directory.EnumerateDirectories(benchmarksDirectory).ToArray();
        var processors = processorDirectories.Select(LastDirectoryName).ToArray();

        var readmeLines = File.ReadAllLines(readmeFilePath);

        foreach (var (fileName, config) in benchmarkFileNameToConfig)
        {
            var description = config.Description;
            var prefix = config.SectionPrefix;
            var readmeBefore = config.ReadmeBefore;
            var readmeEndLine = config.ReadmeEnd;
            var all = "";
            foreach (var processorDirectory in processorDirectories)
            {
                var contentsFilePath = Path.Combine(processorDirectory, fileName);
                if (File.Exists(contentsFilePath))
                {
                    var versionsFilePath = Path.Combine(processorDirectory, "Versions.txt");
                    var versions = File.ReadAllText(versionsFilePath);
                    var contents = File.ReadAllText(contentsFilePath);
                    var processor = LastDirectoryName(processorDirectory);

                    var section = $"{prefix}{processor} - {description} ({versions})";
                    var benchmarkTable = GetBenchmarkTable(contents);
                    var readmeContents = $"{section}{Environment.NewLine}{Environment.NewLine}{benchmarkTable}{Environment.NewLine}";
                    all += readmeContents;
                }
            }
            readmeLines = ReplaceReadmeLines(readmeLines, [all], readmeBefore, prefix, 0, readmeEndLine, 0);
        }

        var newReadme = string.Join(Environment.NewLine, readmeLines) + Environment.NewLine;
        File.WriteAllText(readmeFilePath, newReadme, System.Text.Encoding.UTF8);

        static string LastDirectoryName(string d) =>
            d.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Last();

        static string GetBenchmarkTable(string markdown) =>
            markdown.Substring(markdown.IndexOf('|'));
    }

#if NET9_0
    // Only update README on latest .NET version to avoid multiple accesses
    [TestMethod]
#endif
    public void ReadMeTest_UpdateExampleCodeInMarkdown()
    {
        var testSourceFilePath = s_testSourceFilePath;
        var readmeFilePath = s_readmeFilePath;
        var rootDirectory = s_rootDirectory;

        var readmeLines = File.ReadAllLines(readmeFilePath);

        // Update README examples
        var testSourceLines = File.ReadAllLines(testSourceFilePath);
        var testBlocksToUpdate = new (string StartLineContains, string ReadmeLineBeforeCodeBlock)[]
        {
            (nameof(ReadMeTest_) + "()", "## Example"),
            (nameof(ReadMeTest_SepReader_Debuggability) + "()", "#### SepReader Debuggability"),
#if NET9_0_OR_GREATER
            (nameof(ReadMeTest_IEnumerable_But_Not_LINQ_Compatible) + "()", "The main culprit above is that for example"),
#endif
            (nameof(ReadMeTest_LocalFunction_YieldReturn) + "()", "If you want to use LINQ"),
            (nameof(ReadMeTest_Enumerate) + "()", "Now if instead refactoring this to something LINQ-compatible"),
            (nameof(ReadMeTest_EnumerateWhere) + "()", "In fact, Sep provides such a convenience "),
            (nameof(ReadMeTest_IteratorWhere) + "()", "Instead, you should focus on how to express the enumeration"),
            (nameof(ReadMeTest_EnumerateTrySelect) + "()", "With this the above custom `Enumerate`"),
            (nameof(ReadMeTest_Example_Copy_Rows) + "()", "### Example - Copy Rows"),
            (nameof(ReadMeTest_Example_Copy_Rows_Async) + "()", "### Example - Copy Rows (Async)"),
            (nameof(ReadMeTest_Example_Copy_Rows_Async) + "()", "## Async Support"),
            (nameof(ReadMeTest_Example_Skip_Empty_Rows) + "()", "### Example - Skip Empty Rows"),
            (nameof(ReadMeTest_Example_AsyncAwaitContext_Enumerate) + "()", "### Example - Use Extension Method Enumerate within async/await Context"),
            (nameof(ReadMeTest_Example_AsyncAwaitContext_CustomIterator) + "()", "### Example - Use Local Function within async/await Context"),
        };
        readmeLines = UpdateReadme(testSourceLines, readmeLines, testBlocksToUpdate,
            sourceStartLineOffset: 2, "    }", sourceEndLineOffset: 0, sourceWhitespaceToRemove: 8);

        var readerOptionsSourceLines = File.ReadAllLines(rootDirectory + @"src/Sep/SepReaderOptions.cs");
        var readerOptionsBlocksToUpdate = new (string StartLineContains, string ReadmeLineBeforeCodeBlock)[]
        {
            ("/// <summary>", "#### SepReaderOptions"),
        };
        readmeLines = UpdateReadme(readerOptionsSourceLines, readmeLines, readerOptionsBlocksToUpdate,
            sourceStartLineOffset: 0, "}", sourceEndLineOffset: 0, sourceWhitespaceToRemove: 4);

        var writerOptionsSourceLines = File.ReadAllLines(rootDirectory + @"src/Sep/SepWriterOptions.cs");
        var writerOptionsBlocksToUpdate = new (string StartLineContains, string ReadmeLineBeforeCodeBlock)[]
        {
            ("/// <summary>", "#### SepWriterOptions"),
        };
        readmeLines = UpdateReadme(writerOptionsSourceLines, readmeLines, writerOptionsBlocksToUpdate,
            sourceStartLineOffset: 0, "}", sourceEndLineOffset: 0, sourceWhitespaceToRemove: 4);

        var newReadme = string.Join(Environment.NewLine, readmeLines) + Environment.NewLine;
        File.WriteAllText(readmeFilePath, newReadme, System.Text.Encoding.UTF8);
    }

    // Only update public API in README for latest .NET version to keep consistent
#if NET9_0
    [TestMethod]
#endif
    public void ReadMeTest_PublicApi()
    {
        var publicApi = typeof(Sep).Assembly.GeneratePublicApi();

        var readmeFilePath = s_readmeFilePath;
        var readmeLines = File.ReadAllLines(readmeFilePath);
        readmeLines = ReplaceReadmeLines(readmeLines, [publicApi],
            "## Public API Reference", "```csharp", 1, "```", 0);

        var newReadme = string.Join(Environment.NewLine, readmeLines) + Environment.NewLine;
        File.WriteAllText(readmeFilePath, newReadme, System.Text.Encoding.UTF8);
    }

    static string[] UpdateReadme(string[] sourceLines, string[] readmeLines,
        (string StartLineContains, string ReadmeLineBefore)[] blocksToUpdate,
        int sourceStartLineOffset, string sourceEndLineStartsWith, int sourceEndLineOffset, int sourceWhitespaceToRemove,
        string readmeStartLineStartsWith = "```csharp", int readmeStartLineOffset = 1,
        string readmeEndLineStartsWith = "```", int readmeEndLineOffset = 0)
    {
        foreach (var (startLineContains, readmeLineBeforeBlock) in blocksToUpdate)
        {
            var sourceExampleLines = SnipLines(sourceLines,
                startLineContains, sourceStartLineOffset,
                sourceEndLineStartsWith, sourceEndLineOffset,
                sourceWhitespaceToRemove);

            readmeLines = ReplaceReadmeLines(readmeLines, sourceExampleLines, readmeLineBeforeBlock,
                readmeStartLineStartsWith, readmeStartLineOffset, readmeEndLineStartsWith, readmeEndLineOffset);
        }

        return readmeLines;
    }

    static string[] ReplaceReadmeLines(string[] readmeLines, string[] newReadmeLines, string readmeLineBeforeBlock,
        string readmeStartLineStartsWith, int readmeStartLineOffset,
        string readmeEndLineStartsWith, int readmeEndLineOffset)
    {
        var readmeLineBeforeIndex = Array.FindIndex(readmeLines,
            l => l.StartsWith(readmeLineBeforeBlock, StringComparison.Ordinal)) + 1;
        if (readmeLineBeforeIndex == 0)
        { throw new ArgumentException($"README line '{readmeLineBeforeBlock}' not found."); }

        return ReplaceReadmeLines(readmeLines, newReadmeLines,
            readmeLineBeforeIndex, readmeStartLineStartsWith, readmeStartLineOffset, readmeEndLineStartsWith, readmeEndLineOffset);
    }

    static string[] ReplaceReadmeLines(string[] readmeLines, string[] newReadmeLines, int readmeLineBeforeIndex,
        string readmeStartLineStartsWith, int readmeStartLineOffset,
        string readmeEndLineStartsWith, int readmeEndLineOffset)
    {
        var readmeLinesSpan = readmeLines.AsSpan(readmeLineBeforeIndex);
        var readmeReplaceStartIndex = Array.FindIndex(readmeLines, readmeLineBeforeIndex,
            l => l.StartsWith(readmeStartLineStartsWith, StringComparison.Ordinal)) + readmeStartLineOffset;
        Debug.Assert(readmeReplaceStartIndex >= 0);
        var readmeReplaceEndIndex = Array.FindIndex(readmeLines, readmeReplaceStartIndex,
            l => l.StartsWith(readmeEndLineStartsWith, StringComparison.Ordinal)) + readmeEndLineOffset;

        readmeLines = readmeLines[..readmeReplaceStartIndex].AsEnumerable()
            .Concat(newReadmeLines)
            .Concat(readmeLines[readmeReplaceEndIndex..]).ToArray();
        return readmeLines;
    }

    static string[] SnipLines(string[] sourceLines,
        string startLineContains, int startLineOffset,
        string endLineStartsWith, int endLineOffset,
        int whitespaceToRemove = 8)
    {
        var sourceStartLine = Array.FindIndex(sourceLines,
            l => l.Contains(startLineContains, StringComparison.Ordinal));
        sourceStartLine += startLineOffset;
        var sourceEndLine = Array.FindIndex(sourceLines, sourceStartLine,
            l => l.StartsWith(endLineStartsWith, StringComparison.Ordinal));
        sourceEndLine += endLineOffset;
        var sourceExampleLines = sourceLines[sourceStartLine..sourceEndLine]
            .Select(l => l.Length > 0 ? l.Remove(0, whitespaceToRemove) : l).ToArray();
        return sourceExampleLines;
    }

    static string SourceFile([CallerFilePath] string sourceFilePath = "") => sourceFilePath;
}
