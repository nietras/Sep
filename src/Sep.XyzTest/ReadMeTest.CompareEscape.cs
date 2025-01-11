using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sylvan.Data;
using Sylvan.Data.Csv;

namespace nietras.SeparatedValues.XyzTest;

public partial class ReadMeTest
{
    [TestMethod]
    public void ReadMeTest_CompareEscape()
    {
        var tests = new string[]
        {
            new(""),
            new(" "),
            new("a"),
            new(";"),
            new(","),
            new("\""),
            new("\r"),
            new("\n"),
            new("a\"aa\"aaa"),
            new("a;aa;aaa"),
        };
        var runners = new Dictionary<string, Func<string, string>>()
        {
            { nameof(CsvHelper), EscapeCsvHelper },
            { nameof(Sylvan), EscapeSylvan },
            { nameof(Sep) + "¹", EscapeSep },
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
            var display = ForDisplay(test);

            sb.Append($"| `{display}` |");

            var csharpColText = display.Replace("\"", "\\\"");
            var csharpColTextResult = ForDisplay(EscapeSep(test)).Replace("\"", "\\\"");
            Trace.WriteLine($"new object[] {{ \"{test.Replace("\"", "\\\"")}\", \"{csharpColTextResult}\" }},");

            foreach (var (_, action) in runners)
            {
                try
                {
                    var outputColText = action(test);
                    if (outputColText.Length > 0)
                    {
                        sb.Append($" `{ForDisplay(outputColText)}`");
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
        sb.AppendLine("Separator/delimiter is set to semi-colon `;` (default for Sep)");
        sb.AppendLine();
        sb.AppendLine($"`·` (middle dot) is whitespace to make this visible");
        sb.AppendLine();
        sb.AppendLine($"`\\r`, `\\n` are carriage return and line feed special characters to make these visible");
        sb.AppendLine();
        sb.AppendLine($"¹ Sep with `{nameof(SepWriterOptions.Escape)} = true` in `{nameof(SepWriterOptions)}`");

        var text = sb.ToString();
        Trace.WriteLine(text);
#if NET9_0
        // Only write to file on latest version to avoid multiple accesses
        File.WriteAllText("../../../CompareEscape.md", text, Encoding.UTF8);
#endif
    }

    static string EscapeCsvHelper(string colText)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            Delimiter = ";",
        };
        using var stringWriter = new StringWriter();
        using var writer = new CsvWriter(stringWriter, config);
        writer.WriteField(colText);
        writer.NextRecord();
        return GetCol(stringWriter.ToString());
    }

    static string EscapeSylvan(string colText)
    {
        // Sylvan has to have some kind of type it seems
        var records = new[] { new { Name = colText } };

        // create a DbDataReader over the anonymous records.
        var recordReader = records.AsDataReader();
        var options = new CsvDataWriterOptions { WriteHeaders = false, Delimiter = ';' };
        using var stringWriter = new StringWriter();
        using var csvWriter = Sylvan.Data.Csv.CsvDataWriter.Create(stringWriter, options);
        csvWriter.Write(recordReader);
        return GetCol(stringWriter.ToString());
    }

    static string EscapeSep(string colText)
    {
        using var writer = Sep.Writer(o => o with { WriteHeader = false, Escape = true }).ToText();
        {
            using var row = writer.NewRow();
            row[0].Set(colText);
        }
        return GetCol(writer.ToString());
    }

    static string GetCol(string col)
    {
        using var reader = Sep.Default.Reader(o => o with { HasHeader = false }).FromText(col);
        reader.MoveNext();
        return reader.Current[0].ToString();
    }

    static string ForDisplay(string test) =>
        test.Replace(" ", "·").Replace("\r", "\\r").Replace("\n", "\\n");
}
