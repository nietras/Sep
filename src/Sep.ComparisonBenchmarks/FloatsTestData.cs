﻿using System;
using System.Diagnostics;
using System.IO;

namespace nietras.SeparatedValues.ComparisonBenchmarks;

static class FloatsTestData
{
    const int RandomSeed = 42;
    public const string GroundTruthColNamePrefix = "GT_";
    public const string ResultColNamePrefix = "RE_";

    internal static string GenerateText(int rows, int featuresCount)
    {
        var capacity = rows * (featuresCount * 8 + 512);
        using var writer = Sep.Writer().ToText(capacity);
        Fill(new Random(RandomSeed), rows, featuresCount, writer);
        return writer.ToString();
    }

    internal static byte[] GenerateBytes(int rows, int featuresCount)
    {
        var capacity = rows * (featuresCount * 8 + 512);
        using var stream = new MemoryStream(capacity);
        using var writer = Sep.Writer().To(stream);
        Fill(new Random(RandomSeed), rows, featuresCount, writer);
        writer.Flush();
        return stream.ToArray();
    }

    internal static void Fill(Random random, int rows, int featuresCount, SepWriter writer)
    {
        var b = Stopwatch.GetTimestamp();

        var groundTruthColNames = new string[featuresCount];
        var resultColNames = new string[featuresCount];
        for (var i = 0; i < featuresCount; i++)
        {
            groundTruthColNames[i] = $"{GroundTruthColNamePrefix}Feature{i}";
            resultColNames[i] = $"{ResultColNamePrefix}Feature{i}";
        }

        Span<float> values = stackalloc float[featuresCount];

        //var floatsColValues = GenerateFloatsColValues(random, 256, featuresCount);

        for (var r = 0; r < rows; r++)
        {
            using var row = writer.NewRow();
            //row[groundTruthColNames].Set(floatsColValues[(r * 2) % floatsColValues.Length]);
            //row[resultColNames].Set(floatsColValues[(r * 2 + 1) % floatsColValues.Length]);
            row[groundTruthColNames].Format(Fill(random, values));
            row[resultColNames].Format(Fill(random, values));
        }

        var a = Stopwatch.GetTimestamp();
        var ms = ((a - b) * 1000.0) / Stopwatch.Frequency;
        Console.WriteLine($"// {nameof(Fill)} test data {ms,7:F3} ms");
    }

    static string[][] GenerateFloatsColValues(Random random, int count, int floatsCount)
    {
        var floatsColValues = new string[count][];
        Span<float> floats = stackalloc float[floatsCount];

        for (var i = 0; i < floatsColValues.Length; i++)
        {
            Fill(random, floats);
            var colValues = new string[floats.Length];
            for (var j = 0; j < floats.Length; j++)
            {
                colValues[j] = floats[j].ToString();
            }
            floatsColValues[i] = colValues;
        }
        return floatsColValues;
    }

    static Span<float> Fill(Random random, Span<float> values)
    {
        for (var i = 0; i < values.Length; i++)
        {
            values[i] = random.NextSingle();
        }
        return values;
    }
}
