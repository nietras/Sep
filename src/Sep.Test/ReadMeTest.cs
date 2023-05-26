using System;
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
                       """;
        Assert.AreEqual(expected, writer.ToString());

        // Above example code is for demonstration purposes only.
        // Short names and repeated constants are only for demonstration.
    }

    [TestMethod]
    public void ReadMeTest_UpdateExampleCodeInMarkdown()
    {
        var sourceFile = SourceFile();
        var sourceLines = File.ReadAllLines(sourceFile);
        var sourceStartMethodLine = Array.FindIndex(sourceLines,
            l => l.Contains(nameof(ReadMeTest_) + "()", StringComparison.Ordinal));
        var sourceStartLine = sourceStartMethodLine + 2;
        var sourceEndLine = Array.FindIndex(sourceLines, sourceStartLine,
            l => l.StartsWith("    }", StringComparison.Ordinal));
        var sourceExampleLines = sourceLines[sourceStartLine..sourceEndLine]
            .Select(l => l.Length > 0 ? l.Remove(0, 8) : l).ToArray();
        var sourceExample = string.Join(Environment.NewLine, sourceExampleLines);

        var readmeFile = Path.GetDirectoryName(sourceFile) + @"../../../README.md";
        var readmeLines = File.ReadAllLines(readmeFile);
        var readmeCodeStart = Array.FindIndex(readmeLines,
            l => l.StartsWith("```csharp", StringComparison.Ordinal)) + 1;
        var readmeCodeEnd = Array.FindIndex(readmeLines, readmeCodeStart,
            l => l.StartsWith("```", StringComparison.Ordinal));
        var readmeBefore = string.Join(Environment.NewLine, readmeLines[..readmeCodeStart]);
        var readmeAfter = string.Join(Environment.NewLine, readmeLines[readmeCodeEnd..]);
        var newReadme = string.Join(Environment.NewLine, readmeBefore, sourceExample, readmeAfter) +
            Environment.NewLine;
        File.WriteAllText(readmeFile, newReadme, Encoding.UTF8);
    }

    static string SourceFile([CallerFilePath] string sourceFilePath = "") => sourceFilePath;
}
