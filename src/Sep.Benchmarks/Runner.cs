using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace nietras.SeparatedValues.Benchmarks;

public static class Runner
{
    public static void ParseAndFormatGenerated(Action<string> log)
    {
        ArgumentNullException.ThrowIfNull(log);

        var lines = 500000;
        var text = SepReaderBench.GenerateSCSV(lines, seed: 42);
        var currentCulture = CultureInfo.CurrentCulture;

        log($"Parsing {lines} rows with header... ");

        var UseFiles = false;
        const string ReadFilePath = "ReadTest.scsv";
        const string WriteFilePath = "WriteTest.scsv";
        if (UseFiles) { File.WriteAllText(ReadFilePath, text); }

        for (var i = 0; i < 2; i++)
        {
            // Ideally would have loved static indexers for syntax for separator
            //using var reader = Sep[';'].Reader().FromFile(filePath);
            //using var writer = Sep[';'].Writer().ToFile(newFilePath);

            using var reader = UseFiles
                ? Sep.Reader().FromFile(ReadFilePath)
                : Sep.Reader().FromText(text);
            using var writer = UseFiles
                ? reader.Spec.Writer().ToFile(WriteFilePath)
                : reader.Spec.Writer().ToText(text.Length);

            var nameIndex = reader.Header.IndexOf(C.Name);
            var sbyteIndex = reader.Header.IndexOf(C.SByte);
            var shortIndex = reader.Header.IndexOf(C.Short);
            var intIndex = reader.Header.IndexOf(C.Int);
            var longIndex = reader.Header.IndexOf(C.Long);
            var floatIndex = reader.Header.IndexOf(C.Float);
            var doubleIndex = reader.Header.IndexOf(C.Double);

            Span<int> colIndeces = new[] { sbyteIndex, shortIndex, intIndex, longIndex, floatIndex, doubleIndex };
            Span<string> colNames = new[] { C.SByte, C.Short, C.Int, C.Long, C.Float, C.Double };

            var b = Stopwatch.GetTimestamp();
            var line = 0;
            foreach (var readRow in reader)
            {
                var name = readRow[nameIndex].Span;
                //var i8 = readRow[sbyteIndex].Parse<sbyte>();
                //var i16 = readRow[shortIndex].Parse<short>();
                //var i32 = readRow[intIndex].Parse<int>();
                //var i64 = readRow[longIndex].Parse<long>();
                //var f32 = readRow[floatIndex].Parse<float>();
                //var f64 = readRow[doubleIndex].Parse<double>();
                var i8 = readRow[C.SByte].Parse<sbyte>();
                var i16 = readRow[C.Short].Parse<short>();
                var i32 = readRow[C.Int].Parse<int>();
                var i64 = readRow[C.Long].Parse<long>();
                var f32 = readRow[C.Float].Parse<float>();
                var f64 = readRow[C.Double].Parse<double>();

                var colsFromIndeces = readRow[colIndeces];
                var colsFromNames = readRow[colNames];

                var valuesFromIndeces = colsFromIndeces.Parse<double>();
                var valuesFromNames = colsFromNames.Parse<double>();

                //if (line == 10)
                //    Debugger.Break();

                using var writeRow = writer.NewRow();
                writeRow[C.Name].Set(name);
                writeRow[C.SByte].Format(i8);
                writeRow[C.Short].Format(i16);
                writeRow[C.Int].Format(i32);
                writeRow[C.Long].Format(i64);
                writeRow[C.Float].Format(f32);
                writeRow[C.Double].Format(f64);

                var writeColsFromIndeces = writeRow[colIndeces];
                var writeColsFromNames = writeRow[colNames];

                // Writing values as double will not match input due to double formatting
                //writeColsFromIndeces.Format(valuesFromIndeces);
                //writeColsFromNames.Format(valuesFromNames);
                //writeColsFromNames.Format(valuesFromNames, (v, col) => col.Format($"{v}"));

                // OR
                //writeRow[C.SByte].Format($"{i8}");
                //writeRow[C.Short].Format($"{i16}");
                //writeRow[C.Int].Format($"{i32}");
                //writeRow[C.Long].Format($"{i64}");
                //writeRow[C.Float].Format($"{f32}");
                //writeRow[C.Double].Format($"{f64}");
                ++line;
            }
            var a = Stopwatch.GetTimestamp();
            var s = (a - b) * 1.0 / Stopwatch.Frequency;
            log($"Read/Write {lines} rows of {text.Length} chars in {s:F3} s or {text.Length / s:F0} chars/s or {text.Length * 2 / s:F0} bytes/s");
            var writeText = writer.ToString();
            var diffIndex = DiffersAtIndex(text, writeText);
            if (diffIndex > 0)
            {
                throw new InvalidOperationException($"Write {writeText.Length} differs from read {text.Length} at {diffIndex}");
            }
        }

        int DiffersAtIndex(string s1, string s2)
        {
            var min = Math.Min(s1.Length, s2.Length);
            var index = 0;
            while (index < min && s1[index] == s2[index])
            {
                ++index;
            }
            return (index == min && s1.Length == s2.Length) ? -1 : index;
        }
    }
}
