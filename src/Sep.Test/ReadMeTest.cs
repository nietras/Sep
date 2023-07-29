using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class ReadMeTest
{
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

        static IEnumerable<T> Enumerate<T>(SepReader reader, SepReader.RowFunc<T> func)
        {
            foreach (var row in reader)
            {
                yield return func(row);
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
    public void ReadMeTest_UpdateExampleCodeInMarkdown()
    {
        var sourceFile = SourceFile();
        var sourceLines = File.ReadAllLines(sourceFile);

        var readmeFile = Path.GetDirectoryName(sourceFile) + @"../../../README.md";
        var readmeLines = File.ReadAllLines(readmeFile);

        var blocksToUpdate = new (string MethodNameWithParenthesis, string ReadmeLineBeforeCodeBlock)[]
        {
            (nameof(ReadMeTest_) + "()", "## Example"),
            (nameof(ReadMeTest_LocalFunction_YieldReturn) + "()", "If you want to use LINQ"),
            (nameof(ReadMeTest_Enumerate) + "()", "Now if instead refactoring this to something LINQ-compatible"),
            (nameof(ReadMeTest_EnumerateWhere) + "()", "Which discounting the `Enumerate`"),
            (nameof(ReadMeTest_IteratorWhere) + "()", "Instead, you should focus on how to express the enumeration"),
            (nameof(ReadMeTest_Example_Copy_Rows) + "()", "### Example - Copy Rows"),
        };

        foreach (var (methodNameWithParenthesis, readmeLineBeforeCodeBlock) in blocksToUpdate)
        {
            var sourceExampleLines = GetSourceExampleLines(sourceLines, methodNameWithParenthesis);

            var readmeLineBefore = Array.FindIndex(readmeLines,
                l => l.StartsWith(readmeLineBeforeCodeBlock, StringComparison.Ordinal)) + 1;
            if (readmeLineBefore == 0)
            { throw new ArgumentException($"README line '{readmeLineBeforeCodeBlock}' not found."); }

            var readmeCodeStart = Array.FindIndex(readmeLines, readmeLineBefore,
                l => l.StartsWith("```csharp", StringComparison.Ordinal)) + 1;
            var readmeCodeEnd = Array.FindIndex(readmeLines, readmeCodeStart,
                l => l.StartsWith("```", StringComparison.Ordinal));

            readmeLines = readmeLines[..readmeCodeStart].AsEnumerable()
                .Concat(sourceExampleLines)
                .Concat(readmeLines[readmeCodeEnd..]).ToArray();
        }

        var newReadme = string.Join(Environment.NewLine, readmeLines) + Environment.NewLine;
        File.WriteAllText(readmeFile, newReadme, Encoding.UTF8);
    }

    static string[] GetSourceExampleLines(string[] sourceLines, string methodNameWithParenthesis)
    {
        var sourceStartMethodLine = Array.FindIndex(sourceLines,
            l => l.Contains(methodNameWithParenthesis, StringComparison.Ordinal));
        var sourceStartLine = sourceStartMethodLine + 2;
        var sourceEndLine = Array.FindIndex(sourceLines, sourceStartLine,
            l => l.StartsWith("    }", StringComparison.Ordinal));
        var sourceExampleLines = sourceLines[sourceStartLine..sourceEndLine]
            .Select(l => l.Length > 0 ? l.Remove(0, 8) : l).ToArray();
        return sourceExampleLines;
    }

    static string SourceFile([CallerFilePath] string sourceFilePath = "") => sourceFilePath;
}
