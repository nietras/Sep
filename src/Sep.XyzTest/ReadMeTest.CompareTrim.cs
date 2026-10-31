using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CsvHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sylvan.Data.Csv;

namespace nietras.SeparatedValues.XyzTest;

public partial class ReadMeTest
{
    record TrimTest(string ColText, bool IsValid = false);

    [TestMethod]
    public void ReadMeTest_CompareTrim()
    {
        var tests = new TrimTest[]
        {
            new("a", IsValid: true),
            new(" a", IsValid: true),
            new("a ", IsValid: true),
            new(" a ", IsValid: true),
            new(" a a ", IsValid: true),

            new("\"a\"", IsValid: true),
            new("\" a\"", IsValid: true),
            new("\"a \"", IsValid: true),
            new("\" a \"", IsValid: true),
            new("\" a a \"", IsValid: true),

            new(" \"a\" ", IsValid: true),
            new(" \" a\" ", IsValid: true),
            new(" \"a \" ", IsValid: true),
            new(" \" a \" ", IsValid: true),
            new(" \" a a \" ", IsValid: true),
        };
        var runners = new Dictionary<string, Func<TrimTest, string>>()
        {
            { nameof(CsvHelper) + " Trim", t => TrimCsvHelper(true, false, t.ColText) },
            { nameof(CsvHelper) + " InsideQuotes", t => TrimCsvHelper(false, true, t.ColText) },
            { nameof(CsvHelper) + " All¹", t => TrimCsvHelper(true, true, t.ColText) },
            // Sylvan does not appear to have Trim support
            //{ nameof(Sylvan), t => TrimSylvan(t.ColText) },
            { nameof(Sep) + " Outer", t => TrimSep(SepTrim.Outer, unescape: true, t.ColText) },
            { nameof(Sep) + " AfterUnescape", t => TrimSep(SepTrim.AfterUnescape, unescape: true, t.ColText) },
            { nameof(Sep) + " All²", t => TrimSep(SepTrim.Outer | SepTrim.AfterUnescape, unescape: true, t.ColText) },
        };
        var sb = new StringBuilder();
        sb.Append($"| Input |");
        foreach (var (name, _) in runners)
        {
            sb.Append($" {name} |");
        }
        sb.AppendLine();
        sb.Append($"|-|");
        foreach (var (_, _) in runners)
        {
            sb.Append($"-|");
        }
        sb.AppendLine();
        foreach (var test in tests)
        {
            sb.Append($"| `{test.ColText.Replace(" ", "·")}` |");

            var csharpColTextResult = TrimSep(SepTrim.Outer, unescape: false, test.ColText).Replace("\"", "\\\"");
            Trace.WriteLine($"new object[] {{ \"{test.ColText.Replace("\"", "\\\"")}\", \"{csharpColTextResult}\" }},");

            foreach (var (_, action) in runners)
            {
                try
                {
                    var outputColText = action(test);
                    if (outputColText.Length > 0)
                    {
                        sb.Append($" `{outputColText.Replace(" ", "·")}`");
                    }
                    sb.Append($" |");
                }
                catch (Exception e)
                {
                    var message = e.Message.ReplaceLineEndings(" ");
                    Trace.WriteLine(message);
                    sb.Append($" EXCEPTION |");
                }
            }
            sb.AppendLine();
        }
        sb.AppendLine();
        sb.AppendLine($"`·` (middle dot) is whitespace to make this visible");
        sb.AppendLine();
        sb.AppendLine($"¹ CsvHelper with `TrimWhitespace = true, TrimInsideQuotes = true`");
        sb.AppendLine();
        sb.AppendLine($"² Sep with `SepTrim.Outer | SepTrim.AfterUnescape, unescape: true` in `{nameof(SepReaderOptions)}`");

        var text = sb.ToString();
        Trace.WriteLine(text);
#if NET9_0
        // Only write to file on latest version to avoid multiple accesses
        File.WriteAllText("../../../CompareTrim.md", text, Encoding.UTF8);
#endif
    }

    [TestMethod]
    [DataRow(false, false, " a ", " a ")]
    [DataRow(true, false, " a ", "a")]
    [DataRow(true, false, " \" a \" ", " a ")]
    [DataRow(false, true, "\" a \"", "a")]
    [DataRow(true, true, " \" a \" ", "a")]
    public void ReadMeTest_CompareTrim_CsvHelper(bool trimWhitespace, bool trimInsideQuotes,
        string colText, string expected)
    {
        Assert.AreEqual(expected, TrimCsvHelper(trimWhitespace, trimInsideQuotes, colText));
    }

    static string TrimCsvHelper(bool trimWhitespace, bool trimInsideQuotes, string colText)
    {
        var config = new CsvReaderOptions
        {
            HasHeader = false,
            Separator = ',',
            TrimWhitespace = trimWhitespace,
            TrimInsideQuotes = trimInsideQuotes,
        };
        using var csvParser = CsvReader.FromText(colText, config);
        Assert.IsTrue(csvParser.Read());
        return csvParser[0].ToString();
    }

    static string TrimSylvan(string colText)
    {
        var options = new CsvDataReaderOptions
        {
            HasHeaders = false,
        };
        using var reader = new StringReader(colText);
        using var csvReader = Sylvan.Data.Csv.CsvDataReader.Create(reader, options);
        SepAssert.Assert(csvReader.Read());
        return csvReader.GetString(0);
    }

    static string TrimSep(SepTrim trim, bool unescape, string colText)
    {
        using var reader = Sep.Reader(o => o with { HasHeader = false, Trim = trim, Unescape = unescape }).FromText(colText);
        SepAssert.Assert(reader.MoveNext());
        return reader.Current[0].ToString();
    }
}
