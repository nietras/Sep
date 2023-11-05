using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Sylvan.Data.Csv;

namespace nietras.SeparatedValues.Benchmarks;

public static class UnescapeCompare
{
    record UnescapeTest(string ColText, bool IsValid = true);

    public static void CompareUnescape()
    {
        var tests = new UnescapeTest[]
        {
            new("a"),
            new("\"\""),
            new("\"a\""),
            new("\"a\"\"a\""),
            new("\"a\"a\"a\""),

            new("a\"\"a"),
            new("a\"a\"a"),
            new(" \"\" "),
            new(" \"a\" "),
            new("\"\" "),
            new("\"a\" "),
            new(" \"\""),
            new(" \"a\""),
            new("\"a\"\"\"a"),

            new("\"a\"\"\"a\""),
            new("\"\"\""),
            new("a\"\"\"a"),
        };
        var runners = new Dictionary<string, Func<UnescapeTest, string>>()
        {
            { nameof(CsvHelper), t => UnescapeCsvHelper(ConfigurationFunctions.BadDataFound, t.ColText) },
            { nameof(CsvHelper) + "¹", t => UnescapeCsvHelper(null, t.ColText) },
            { nameof(Sylvan), t => UnescapeSylvan(t.ColText) },
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
        sb.AppendLine($"¹ CsvHelper BadDataFound = null");
        var text = sb.ToString();
        Trace.WriteLine(text);
        File.WriteAllText("UnescapeCompare.md", text, Encoding.UTF8);
    }

    static string UnescapeCsvHelper(BadDataFound? badDataFound, string colText)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            BadDataFound = badDataFound,
        };
        using var reader = new StringReader(colText);
        using var csvParser = new CsvParser(reader, config);
        SepAssert.Assert(csvParser.Read());
        return csvParser[0];
    }

    static string UnescapeSylvan(string colText)
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
}
