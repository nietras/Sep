using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepWriterRowTest
{
    static readonly string NL = Environment.NewLine;

    // TODO: ColIndex only if no header

    [TestMethod]
    public void SepWriterRowTest_Indexer_ColIndex_After_ColName()
    {
        Run(row => { row["A"].Set("1"); row[0].Set("11"); }, $"A{NL}11{NL}");
        Run(row =>
            {
                row["A"].Set("1"); row["B"].Set("2");
                row[1].Set("22");
            },
            $"A;B{NL}1;22{NL}");
    }

    [TestMethod]
    public void SepWriterRowTest_Indexer_ColIndices_After_ColNames()
    {
        var colNames = new string[] { "A", "B" };
        var colValues0 = new string[] { "1", "2" };
        var colIndices = new int[] { 1, 0 };
        var colValues1 = new string[] { "22", "11" };
        Run(row =>
            {
                row[colNames.AsSpan()].Set(colValues0.AsSpan());
                row[colIndices.AsSpan()].Set(colValues1.AsSpan());
            }, $"A;B{NL}11;22{NL}");
    }
    [TestMethod]
    public void SepWriterRowTest_Indexer_ColIndices_After_ColNames_Params_Set()
    {
        Run(row =>
        {
            row["A", "B"].Set("1", "2");
            row[1, 0].Set("22", "11");
        }, $"A;B{NL}11;22{NL}");
    }
    [TestMethod]
    public void SepWriterRowTest_Indexer_ColIndices_After_ColNames_Params_Format()
    {
        Run(row =>
        {
            row["A", "B"].Set("1", "2");
            row[1, 0].Format(22, 11);
        }, $"A;B{NL}11;22{NL}");
    }

    [TestMethod]
    public void SepWriterRowTest_Indexer_ColIndices_Empty_After_ColNames()
    {
        var colNames = new string[] { "A", "B" };
        var colValues0 = new string[] { "1", "2" };
        var colIndices = Array.Empty<int>();
        var colValues1 = Array.Empty<string>();
        Run(row =>
        {
            row[colNames.AsSpan()].Set(colValues0.AsSpan());
            row[colIndices.AsSpan()].Set(colValues1.AsSpan());
        }, $"A;B{NL}1;2{NL}");
    }

    [TestMethod]
    public void SepWriterRowTest_Indexer_ColName()
    {
        Run(row => row["A"].Set("1"), $"A{NL}1{NL}");
        Run(row => { row["A"].Set("1"); row["B"].Set("2"); },
            $"A;B{NL}1;2{NL}");
    }

    [TestMethod]
    public void SepWriterRowTest_Indexer_ColNames_Params_ReadOnlySpan_Set()
    {
        Run(row => row["A", "B"].Set("1", "2"), $"A;B{NL}1;2{NL}");
    }
    [TestMethod]
    public void SepWriterRowTest_Indexer_ColNames_Params_ReadOnlySpan_Format()
    {
        Run(row => row["A", "B"].Format(1, 2), $"A;B{NL}1;2{NL}");
    }

    [TestMethod]
    public void SepWriterRowTest_Indexer_ColNames_ReadOnlySpan()
    {
        var colNames = new string[] { "A", "B" };
        var colValues = new string[] { "1", "2" };
        Run(row => row[colNames.AsSpan()].Set(colValues.AsSpan()), $"A;B{NL}1;2{NL}");
    }

    [TestMethod]
    public void SepWriterRowTest_Indexer_ColNames_ReadOnlySpan_Empty()
    {
        var colNames = Array.Empty<string>();
        var colValues = Array.Empty<string>();
        Run(row => row[colNames.AsSpan()].Set(colValues.AsSpan()), $"{NL}{NL}");
    }

    [TestMethod]
    public void SepWriterRowTest_Indexer_ColNames_IReadOnlyList()
    {
        IReadOnlyList<string> colNames = ["A", "B"];
        IReadOnlyList<string> colValues = ["1", "2"];
        Run(row => row[colNames].Set(colValues), $"A;B{NL}1;2{NL}");
    }

    [TestMethod]
    public void SepWriterRowTest_Indexer_ColNames_IReadOnlyList_Empty()
    {
        IReadOnlyList<string> colNames = Array.Empty<string>();
        IReadOnlyList<string> colValues = Array.Empty<string>();
        Run(row => row[colNames].Set(colValues), $"{NL}{NL}");
    }

    [TestMethod]
    public void SepWriterRowTest_Indexer_ColNames_IReadOnlyList_Null()
    {
        IReadOnlyList<string>? colNames = null;
        IReadOnlyList<string> colValues = Array.Empty<string>();
        RunThrows<ArgumentNullException>(row => row[colNames!].Set(colValues),
            "Value cannot be null. (Parameter 'colNames')");
    }

    [TestMethod]
    public void SepWriterRowTest_Indexer_ColNames_Array()
    {
        var colNames = new string[] { "A", "B" };
        var colValues = new string[] { "1", "2" };
        Run(row => row[colNames].Set(colValues), $"A;B{NL}1;2{NL}");
    }

    [TestMethod]
    public void SepWriterRowTest_Indexer_ColNames_Array_Empty()
    {
        var colNames = Array.Empty<string>();
        var colValues = Array.Empty<string>();
        Run(row => row[colNames].Set(colValues), $"{NL}{NL}");
    }

    [TestMethod]
    public void SepWriterRowTest_Indexer_ColNames_Array_Null()
    {
        string[]? colNames = null;
        string[] colValues = Array.Empty<string>();
        RunThrows<ArgumentNullException>(row => row[colNames!].Set(colValues),
            "Value cannot be null. (Parameter 'colNames')");
    }

    [TestMethod]
    public void SepWriterRowTest_Dispose_Twice()
    {
        using var writer = Sep.Writer().ToText();
        var row = writer.NewRow();
        row.Dispose();
        row.Dispose();
    }

    [TestMethod]
    public void SepWriterRowTest_Escape_SpecialCharacters()
    {
        var options = new SepWriterOptions { Escape = true };
        using var writer = Sep.Writer(options).ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("Value with, comma");
            row["B"].Set("Value with; semicolon");
            row["C"].Set("Value with\nnewline");
        }
        var expected = $"A;B;C{NL}\"Value with, comma\";\"Value with; semicolon\";\"Value with{NL}newline\"{NL}";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterRowTest_Escape_NestedQuotes()
    {
        var options = new SepWriterOptions { Escape = true };
        using var writer = Sep.Writer(options).ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("He said \"Hello\"");
            row["B"].Set("She replied \"Hi\"");
        }
        var expected = $"A;B{NL}\"He said \"\"Hello\"\"\";\"She replied \"\"Hi\"\"\"{NL}";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterRowTest_Escape_MultilineValues()
    {
        var options = new SepWriterOptions { Escape = true };
        using var writer = Sep.Writer(options).ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("Line1\nLine2");
            row["B"].Set("Single line");
        }
        var expected = $"A;B{NL}\"Line1{NL}Line2\";Single line{NL}";
        Assert.AreEqual(expected, writer.ToString());
    }

    static void Run(SepWriter.RowAction action, string expected) =>
        Run(action, actual => Assert.AreEqual(expected, actual));

    static void Run(SepWriter.RowAction action, Action<string>? assert = null)
    {
        using var writer = Sep.Writer().ToText();
        {
            using var row = writer.NewRow();
            action(row);
        }
        assert?.Invoke(writer.ToString());
    }

    static void RunThrows<TException>(SepWriter.RowAction action, string expectedMessage)
        where TException : Exception
    {
        using var writer = Sep.Writer().ToText();
        {
            using var row = writer.NewRow();
            try
            {
                action(row);
                Assert.Fail($"{nameof(action)} did not fail as expected.");
            }
            catch (TException e)
            {
                Assert.AreEqual(expectedMessage, e.Message);
            }
        }
    }
}
