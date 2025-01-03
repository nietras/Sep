using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepWriterColTest
{
    const string ColName = "A";
    const int ColValue = 123456;
    const string ColText = "123456";

    [TestMethod]
    public void SepWriterColTest_ColIndex()
    {
        Run(col => Assert.AreEqual(0, col.ColIndex), null);
    }

    [TestMethod]
    public void SepWriterColTest_ColName()
    {
        Run(col => Assert.AreEqual(ColName, col.ColName), null);
    }

    [TestMethod]
    public void SepWriterColTest_Set_String()
    {
        Run(col => col.Set(ColText));
    }

    [TestMethod]
    public void SepWriterColTest_Set_Span()
    {
        Run(col => col.Set(ColText.AsSpan()));
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString()
    {
        Run(col => col.Set($"{ColValue}"));
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_F2()
    {
        Run(col => col.Set($"{ColValue:F2}"), ColText + ".00");
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_F2_CultureInfoAsConfig()
    {
        Run(col => col.Set($"{ColValue:F2}"), ColText + ",00", CultureInfo.GetCultureInfo("da-DK"));
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_F2_CultureInfoAsParam()
    {
        Run(col => col.Set(CultureInfo.GetCultureInfo("da-DK"), $"{ColValue:F2}"), ColText + ",00");
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_AppendLiteral()
    {
        Run(col => col.Set($"{ColValue} {"Literal"}"), ColText + " Literal");
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_AppendFormatted_Format_Alignment()
    {
        Run(col => col.Set($"{ColValue,16:F2}"), new string(' ', 16 - ColText.Length - 3) + ColText + ".00");
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_AppendFormatted_Alignment()
    {
        Run(col => col.Set($"{ColValue,16}"), new string(' ', 16 - ColText.Length) + ColText);
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_AppendFormatted_Span()
    {
        Run(col => col.Set($"{ColText.AsSpan()}"), ColText);
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_AppendFormatted_Span_Alignment()
    {
        Run(col => col.Set($"{ColText.AsSpan(),16}"), new string(' ', 16 - ColText.Length) + ColText);
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_AppendFormatted_String_Alignment()
    {
        string? nullableString = ColText;
        Run(col => col.Set($"{nullableString,16:s}"), new string(' ', 16 - ColText.Length) + ColText);
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_AppendFormatted_Object_Alignment()
    {
        object? nullableObject = ColText;
        Run(col => col.Set($"{nullableObject,16:s}"), new string(' ', 16 - ColText.Length) + ColText);
    }

    [TestMethod]
    public void SepWriterColTest_Format()
    {
        Run(col => col.Format(ColValue));
    }

    [TestMethod]
    public void SepWriterColTest_Escape_SpecialCharacters()
    {
        var options = new SepWriterOptions { Escape = true };
        using var writer = Sep.Writer(options).ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("Value with, comma");
            row["B"].Set("Value with; semicolon");
            row["C"].Set("Value with\nnewline");
        }
        var expected = $"A;B;C{Environment.NewLine}\"Value with, comma\";\"Value with; semicolon\";\"Value with{Environment.NewLine}newline\"{Environment.NewLine}";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColTest_Escape_NestedQuotes()
    {
        var options = new SepWriterOptions { Escape = true };
        using var writer = Sep.Writer(options).ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("He said \"Hello\"");
            row["B"].Set("She replied \"Hi\"");
        }
        var expected = $"A;B{Environment.NewLine}\"He said \"\"Hello\"\"\";\"She replied \"\"Hi\"\"\"{Environment.NewLine}";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColTest_Escape_MultilineValues()
    {
        var options = new SepWriterOptions { Escape = true };
        using var writer = Sep.Writer(options).ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("Line1\nLine2");
            row["B"].Set("Single line");
        }
        var expected = $"A;B{Environment.NewLine}\"Line1{Environment.NewLine}Line2\";Single line{Environment.NewLine}";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColTest_Escape_OnlyIfContainsSeparator()
    {
        var options = new SepWriterOptions { Escape = true };
        using var writer = Sep.Writer(options).ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("Value with comma,");
            row["B"].Set("Value without comma");
        }
        var expected = $"A;B{Environment.NewLine}\"Value with comma,\";Value without comma{Environment.NewLine}";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColTest_Escape_DifferentSeparator()
    {
        var options = new SepWriterOptions { Escape = true, Sep = new Sep('|') };
        using var writer = Sep.Writer(options).ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("Value with pipe|");
            row["B"].Set("Value without pipe");
        }
        var expected = $"A|B{Environment.NewLine}\"Value with pipe|\"|Value without pipe{Environment.NewLine}";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColTest_Escape_LineEndings()
    {
        var options = new SepWriterOptions { Escape = true };
        using var writer = Sep.Writer(options).ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("Value with\r carriage return");
            row["B"].Set("Value with\n line feed");
            row["C"].Set("Value with\r\n carriage return and line feed");
        }
        var expected = $"A;B;C{Environment.NewLine}\"Value with\r carriage return\";\"Value with\n line feed\";\"Value with\r\n carriage return and line feed\"{Environment.NewLine}";
        Assert.AreEqual(expected, writer.ToString());
    }

    static void Run(SepWriter.ColAction action, string? expectedColValue = ColText, CultureInfo? cultureInfo = null)
    {
        Func<SepWriter>[] createWriters =
        [
            () => Sep.Writer(o => o with { CultureInfo = cultureInfo ?? SepDefaults.CultureInfo }).ToText(),
            () => Sep.Default.Writer(o => o with { CultureInfo = cultureInfo ?? SepDefaults.CultureInfo }).ToText(),
            () => new SepSpec(Sep.Default, cultureInfo ?? SepDefaults.CultureInfo).Writer(o => o with { }).ToText(),
        ];
        foreach (var createWriter in createWriters)
        {
            using var writer = createWriter();
            {
                using var row = writer.NewRow();
                action(row[ColName]);
            }
            if (expectedColValue is not null)
            {
                var expectedText = $"{ColName}{Environment.NewLine}{expectedColValue}{Environment.NewLine}";
                var actualText = writer.ToString();
                Assert.AreEqual(expectedText, actualText);
            }
        }
    }
}
