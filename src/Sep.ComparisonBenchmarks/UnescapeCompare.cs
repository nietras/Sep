using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Sylvan.Data.Csv;

namespace nietras.SeparatedValues.ComparisonBenchmarks;

public static class UnescapeCompare
{
    record UnescapeTest(string ColText, bool IsValid = false);

    public static void CompareUnescape()
    {
        var tests = new UnescapeTest[]
        {
            new("a", IsValid: true),
            new("\"\"", IsValid: true),
            new("\"\"\"\"", IsValid: true),
            new("\"\"\"\"\"\"", IsValid: true),
            new("\"a\"", IsValid: true),
            new("\"a\"\"a\"", IsValid: true),
            new("\"a\"\"a\"\"a\"", IsValid: true),

            // No start quote
            new("a\"\"a"),
            new("a\"a\"a"),
            new(" \"\" "),
            new(" \"a\" "),
            new(" \"\""),
            new(" \"a\""),
            new("a\"\"\"a"),

            new("\"a\"a\"a\""),
            new("\"\" "),
            new("\"a\" "),
            new("\"a\"\"\"a"),

            new("\"a\"\"\"a\""),
            new("\"\"a\""),
            new("\"a\"a\""),
            new("\"\"a\"a\"\""),

            new("\"\"\""),
            new("\"\"\"\"\""),
        };
        var runners = new Dictionary<string, Func<UnescapeTest, string>>()
        {
            { nameof(CsvHelper), t => UnescapeCsvHelper(ConfigurationFunctions.BadDataFound, t.ColText) },
            { nameof(CsvHelper) + "¹", t => UnescapeCsvHelper(null, t.ColText) },
            { nameof(Sylvan), t => UnescapeSylvan(t.ColText) },
            { nameof(Sep) + "²", t => UnescapeSep(t.ColText) },
        };
        var sb = new StringBuilder();
        var outputCsharp = false;
        sb.Append($"| Input |");
        if (outputCsharp) { sb.Append($" Input (C#) |"); }
        sb.Append($" Valid |");
        foreach (var (name, _) in runners)
        {
            sb.Append($" {name} |");
        }
        sb.AppendLine();
        sb.Append($"|-|");
        if (outputCsharp) { sb.Append($"-|"); }
        sb.Append($"-|");
        foreach (var (_, _) in runners)
        {
            sb.Append($"-|");
        }
        sb.AppendLine();
        foreach (var test in tests)
        {
            sb.Append($"| `{test.ColText.Replace(" ", "·")}` |");
            var csharpColText = test.ColText.Replace(" ", "·").Replace("\"", "\\\"");
            if (outputCsharp) { sb.Append($" `{csharpColText}` |"); }
            sb.Append($" {test.IsValid} |");

            var csharpColTextResult = UnescapeSep(test.ColText).Replace("\"", "\\\"");
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
        sb.AppendLine($"¹ CsvHelper with `BadDataFound = null`");
        sb.AppendLine();
        sb.AppendLine($"² Sep with `{nameof(SepReaderOptions.Unescape)} = true` in `{nameof(SepReaderOptions)}`");

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

    static string UnescapeSep(string colText)
    {
        using var reader = Sep.Reader(o => o with { HasHeader = false, Unescape = true }).FromText(colText);
        SepAssert.Assert(reader.MoveNext());
        return reader.Current[0].ToString();
    }
}
