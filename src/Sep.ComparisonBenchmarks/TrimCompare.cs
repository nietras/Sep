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

public static class TrimCompare
{
    record TrimTest(string ColText, bool IsValid = false);

    public static void CompareTrim()
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
            { nameof(CsvHelper) + " Trim", t => TrimCsvHelper(TrimOptions.Trim, null, t.ColText) },
            { nameof(CsvHelper) + " InsideQuotes", t => TrimCsvHelper(TrimOptions.InsideQuotes, null, t.ColText) },
            { nameof(CsvHelper) + " All¹", t => TrimCsvHelper(TrimOptions.Trim | TrimOptions.InsideQuotes, null, t.ColText) },
            // Sylvan does not appear to have Trim support
            //{ nameof(Sylvan), t => TrimSylvan(t.ColText) },
            { nameof(Sep) + " Outer", t => TrimSep(SepTrim.Outer, unescape: false, t.ColText) },
            { nameof(Sep) + " AfterUnescape", t => TrimSep(SepTrim.AfterUnescape, unescape: true, t.ColText) },
            { nameof(Sep) + " All²", t => TrimSep(SepTrim.Outer | SepTrim.AfterUnescape, unescape: true, t.ColText) },
        };
        var sb = new StringBuilder();
        var outputCsharp = false;
        sb.Append($"| Input |");
        if (outputCsharp) { sb.Append($" Input (C#) |"); }
        //sb.Append($" Valid |");
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
            //sb.Append($" {test.IsValid} |");

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
        sb.AppendLine($"¹ CsvHelper with `TrimOptions.Trim | TrimOptions.InsideQuotes`");
        sb.AppendLine();
        sb.AppendLine($"² Sep with `SepTrim.Outer | SepTrim.AfterUnescape, unescape: true` in `{nameof(SepReaderOptions)}`");

        var text = sb.ToString();
        Trace.WriteLine(text);
        File.WriteAllText("TrimCompare.md", text, Encoding.UTF8);
    }

    static string TrimCsvHelper(TrimOptions trimOptions, BadDataFound? badDataFound, string colText)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            BadDataFound = badDataFound,
            TrimOptions = trimOptions,
        };
        using var reader = new StringReader(colText);
        using var csvParser = new CsvParser(reader, config);
        SepAssert.Assert(csvParser.Read());
        return csvParser[0];
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
