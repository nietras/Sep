﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class PackageAssetsTest
{
    public static IEnumerable<object[]> ToStrings =>
    [
        [SepToString.Direct],
        [SepToString.OnePool()],
        [SepToString.PoolPerCol()],
        [SepToString.PoolPerColThreadSafe()],
        [SepToString.PoolPerColThreadSafeFixedCapacity()],
    ];

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_Read_NoQuotes(SepCreateToString createToString) => VerifyRead(NoQuotes, createToString);

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_Read_NoQuotes_Unescape(SepCreateToString createToString) => VerifyRead(NoQuotes, createToString, unescape: true);

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_Read_WithQuotes(SepCreateToString createToString) => VerifyRead(WithQuotes, createToString);

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_Read_WithQuotes_Unescape(SepCreateToString createToString) => VerifyRead(WithQuotes, createToString, unescape: true);


    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public async ValueTask PackageAssetsTest_Read_NoQuotes_Async(SepCreateToString createToString) => await VerifyReadAsync(NoQuotes, createToString);

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public async ValueTask PackageAssetsTest_Read_NoQuotes_Unescape_Async(SepCreateToString createToString) => await VerifyReadAsync(NoQuotes, createToString, unescape: true);

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public async ValueTask PackageAssetsTest_Read_WithQuotes_Async(SepCreateToString createToString) => await VerifyReadAsync(WithQuotes, createToString);

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public async ValueTask PackageAssetsTest_Read_WithQuotes_Unescape_Async(SepCreateToString createToString) => await VerifyReadAsync(WithQuotes, createToString, unescape: true);


    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_Enumerate_NoQuotes(SepCreateToString createToString) =>
        VerifyEnumerate(NoQuotes, createToString, (reader, select) => reader.Enumerate(select));

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_Enumerate_NoQuotes_Unescape(SepCreateToString createToString) =>
        VerifyEnumerate(NoQuotes, createToString, (reader, select) => reader.Enumerate(select), unescape: true);

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_Enumerate_WithQuotes(SepCreateToString createToString) =>
        VerifyEnumerate(WithQuotes, createToString, (reader, select) => reader.Enumerate(select));

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_Enumerate_WithQuotes_Unescape(SepCreateToString createToString) =>
        VerifyEnumerate(WithQuotes, createToString, (reader, select) => reader.Enumerate(select), unescape: true);


    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_Enumerate_RowTryFunc_NoQuotes(SepCreateToString createToString) =>
        VerifyEnumerateTry(NoQuotes, createToString, (reader, select) => reader.Enumerate(select));

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_Enumerate_RowTryFunc_NoQuotes_Unescape(SepCreateToString createToString) =>
        VerifyEnumerateTry(NoQuotes, createToString, (reader, select) => reader.Enumerate(select), unescape: true);

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_Enumerate_RowTryFunc_WithQuotes(SepCreateToString createToString) =>
        VerifyEnumerateTry(WithQuotes, createToString, (reader, select) => reader.Enumerate(select));

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_Enumerate_RowTryFunc_WithQuotes_Unescape(SepCreateToString createToString) =>
        VerifyEnumerateTry(WithQuotes, createToString, (reader, select) => reader.Enumerate(select), unescape: true);

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_ParallelEnumerate_Empty(SepCreateToString createToString)
    {
        var text = string.Empty;
        VerifyEnumerate(text, createToString, (reader, select) => reader.ParallelEnumerate(select));
    }
    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_ParallelEnumerate_Empty_DegreeOfParallism(SepCreateToString createToString)
    {
        var text = string.Empty;
        VerifyEnumerate(text, createToString, (reader, select) => reader.ParallelEnumerate(select, degreeOfParallism: 5));
    }
    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_ParallelEnumerate_Empty_RowTryFunc(SepCreateToString createToString)
    {
        var text = string.Empty;
        VerifyEnumerateTry(text, createToString, (reader, select) => reader.ParallelEnumerate(select));
    }
    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_ParallelEnumerate_Empty_RowTryFunc_DegreeOfParallism(SepCreateToString createToString)
    {
        var text = string.Empty;
        VerifyEnumerateTry(text, createToString, (reader, select) => reader.ParallelEnumerate(select, degreeOfParallism: 5));
    }

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_ParallelEnumerate_NoQuotes(SepCreateToString createToString)
    {
#if SEPREADERTRACE
        var text = NoQuotes;
#else
        var text = string.Join(string.Empty, Enumerable.Repeat(NoQuotes, 100));
#endif
        VerifyEnumerate(text, createToString, (reader, select) => reader.ParallelEnumerate(select));
    }

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_ParallelEnumerate_NoQuotes_DegreeOfParallism(SepCreateToString createToString)
    {
#if SEPREADERTRACE
        var text = NoQuotes;
#else
        var text = string.Join(string.Empty, Enumerable.Repeat(NoQuotes, 100));
#endif
        VerifyEnumerate(text, createToString, (reader, select) => reader.ParallelEnumerate(select, degreeOfParallism: 5));
    }

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_ParallelEnumerate_WithQuotes(SepCreateToString createToString)
    {
#if SEPREADERTRACE
                var text = NoQuotes;
#else
        var text = string.Join(string.Empty, Enumerable.Repeat(WithQuotes, 100));
#endif
        VerifyEnumerate(text, createToString, (reader, select) => reader.ParallelEnumerate(select));
    }

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_ParallelEnumerate_RowTryFunc_NoQuotes(SepCreateToString createToString)
    {
#if SEPREADERTRACE
        var text = NoQuotes;
#else
        var text = string.Join(string.Empty, Enumerable.Repeat(NoQuotes, 100));
#endif
        VerifyEnumerateTry(text, createToString, (reader, select) => reader.ParallelEnumerate(select));
    }

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_ParallelEnumerate_RowTryFunc_NoQuotes_DegreeOfParallism(SepCreateToString createToString)
    {
#if SEPREADERTRACE
        var text = NoQuotes;
#else
        var text = string.Join(string.Empty, Enumerable.Repeat(NoQuotes, 100));
#endif
        VerifyEnumerateTry(text, createToString, (reader, select) => reader.ParallelEnumerate(select, degreeOfParallism: 5));
    }

    [DataTestMethod]
    [DynamicData(nameof(ToStrings))]
    public void PackageAssetsTest_ParallelEnumerate_RowTryFunc_WithQuotes(SepCreateToString createToString)
    {
#if SEPREADERTRACE
        var text = NoQuotes;
#else
        var text = string.Join(string.Empty, Enumerable.Repeat(WithQuotes, 100));
#endif
        VerifyEnumerateTry(text, createToString, (reader, select) => reader.ParallelEnumerate(select));
    }

    static void VerifyRead(string text, SepCreateToString createToString, bool unescape = false)
    {
        var expected = ReadLineSplitAsList(text);
        var reader = Sep.Reader(o => o with { HasHeader = false, CreateToString = createToString, Unescape = unescape }).FromText(text);
        var rowIndex = 0;
        foreach (var row in reader)
        {
            AssertCols(row, rowIndex, unescape, expected);
            ++rowIndex;
        }
        Assert.AreEqual(expected.Count, rowIndex);
    }

    static async ValueTask VerifyReadAsync(string text, SepCreateToString createToString, bool unescape = false)
    {
        var expected = ReadLineSplitAsList(text);
        using var reader = await Sep
            .Reader(o => o with { HasHeader = false, CreateToString = createToString, Unescape = unescape })
            .FromTextAsync(text);
        var rowIndex = 0;
        await foreach (var row in reader)
        {
            AssertCols(row, rowIndex, unescape, expected);
            ++rowIndex;
        }
        Assert.AreEqual(expected.Count, rowIndex);
    }

    static void AssertCols(SepReader.Row row, int rowIndex, bool unescape, List<string[]> expected)
    {
        var expectedCols = expected[rowIndex];
        expectedCols = unescape ? UnescapeColsByTrim(expectedCols) : expectedCols;
        Assert.AreEqual(expectedCols.Length, row.ColCount);
        CollectionAssert.AreEqual(expectedCols, row[..].ToStringsArray());
    }

    static void VerifyEnumerate(string text, SepCreateToString createToString,
        Func<SepReader, SepReader.RowFunc<string[]>, IEnumerable<string[]>> enumerate,
        bool unescape = false)
    {
        var expected = ReadLineSplitAsList(text);
        var reader = Sep.Reader(o => o with { HasHeader = false, CreateToString = createToString, Unescape = unescape }).FromText(text);
        var rows = enumerate(reader, r => r[0..r.ColCount].ToStringsArray());
        var rowIndex = 0;
        foreach (var cols in rows)
        {
            var expectedCols = expected[rowIndex];
            expectedCols = unescape ? UnescapeColsByTrim(expectedCols) : expectedCols;
            Assert.AreEqual(expectedCols.Length, cols.Length);
            CollectionAssert.AreEqual(expectedCols, cols);
            ++rowIndex;
        }
        Assert.AreEqual(expected.Count, rowIndex);
    }

    static void VerifyEnumerateTry(string text, SepCreateToString createToString,
        Func<SepReader, SepReader.RowTryFunc<string[]>, IEnumerable<string[]>> enumerate,
        bool unescape = false)
    {
        var startsWith = "8";
        var expected = ReadLineSplit(text).Where(cols => cols[0].StartsWith(startsWith)).ToList();
        var reader = Sep.Reader(o => o with { HasHeader = false, CreateToString = createToString, Unescape = unescape }).FromText(text);
        var rows = enumerate(reader, (SepReader.Row r, out string[] cols) =>
        {
            if (r[0].Span.StartsWith(startsWith)) { cols = r[..].ToStringsArray(); return true; }
            cols = default!;
            return false;
        });
        var rowIndex = 0;
        foreach (var cols in rows)
        {
            var expectedCols = expected[rowIndex];
            expectedCols = unescape ? UnescapeColsByTrim(expectedCols) : expectedCols;
            Assert.AreEqual(expectedCols.Length, cols.Length);
            CollectionAssert.AreEqual(expectedCols, cols);
            ++rowIndex;
        }
        Assert.AreEqual(expected.Count, rowIndex);
    }

    static List<string[]> ReadLineSplitAsList(string text, char separator = ',') =>
        ReadLineSplit(text, separator).ToList();

    static IEnumerable<string[]> ReadLineSplit(string text, char separator = ',')
    {
        using var reader = new StringReader(text);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            yield return line.Split(separator);
        }
    }

    static string[] UnescapeColsByTrim(string[] expectedCols) =>
        expectedCols.Select(c => c.Trim('"')).ToArray();

    const string NoQuotes = @"75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,RuntimeAssemblies,,,net5.0,,,,,,lib/net5.0/BlazorGrid.dll,BlazorGrid.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,CompileLibAssemblies,,,net5.0,,,,,,lib/net5.0/BlazorGrid.dll,BlazorGrid.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,ResourceAssemblies,,,net5.0,,,,,,lib/net5.0/de/BlazorGrid.resources.dll,BlazorGrid.resources.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,MSBuildFiles,,,any,,,,,,build/Microsoft.AspNetCore.StaticWebAssets.props,Microsoft.AspNetCore.StaticWebAssets.props,.props,build,any,Any,0.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,MSBuildFiles,,,any,,,,,,build/Akinzekeel.BlazorGrid.props,Akinzekeel.BlazorGrid.props,.props,build,any,Any,0.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,MSBuildMultiTargetingFiles,,,any,,,,,,buildMultiTargeting/Akinzekeel.BlazorGrid.props,Akinzekeel.BlazorGrid.props,.props,buildmultitargeting,any,Any,0.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,ContentFiles,,any,net5.0,,,,,,contentFiles/any/net5.0/libman.json,libman.json,.json,contentfiles,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,ContentFiles,,any,net5.0,,,,,,contentFiles/any/net5.0/package-lock.json,package-lock.json,.json,contentfiles,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,ContentFiles,,any,net5.0,,,,,,contentFiles/any/net5.0/package.json,package.json,.json,contentfiles,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,ContentFiles,,any,net5.0,,,,,,contentFiles/any/net5.0/Content/blazorgrid-bootstrap.scss,blazorgrid-bootstrap.scss,.scss,contentfiles,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,ContentFiles,,any,net5.0,,,,,,contentFiles/any/net5.0/Content/blazorgrid-spectre.scss,blazorgrid-spectre.scss,.scss,contentfiles,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,ContentFiles,,any,net5.0,,,,,,contentFiles/any/net5.0/Content/BlazorGrid.scss,BlazorGrid.scss,.scss,contentfiles,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,ContentFiles,,any,net5.0,,,,,,contentFiles/any/net5.0/Content/bootstrap/scss/_functions.scss,_functions.scss,.scss,contentfiles,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,ContentFiles,,any,net5.0,,,,,,contentFiles/any/net5.0/Content/bootstrap/scss/_variables.scss,_variables.scss,.scss,contentfiles,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,ContentFiles,,any,net5.0,,,,,,contentFiles/any/net5.0/Content/spectre/_variables.scss,_variables.scss,.scss,contentfiles,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,MSBuildTransitiveFiles,,,any,,,,,,buildTransitive/Akinzekeel.BlazorGrid.props,Akinzekeel.BlazorGrid.props,.props,buildtransitive,any,Any,0.0.0.0,,,0.0.0.0
f64dfed9-0ef8-41d2-9bdf-fcb0de148f00,2020-11-28T01:50:26.3420052+00:00,Akinzekeel.BlazorGrid.Abstractions,0.9.1-beta,2020-11-27T22:07:11.7570000+00:00,AvailableAssets,RuntimeAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/BlazorGrid.Abstractions.dll,BlazorGrid.Abstractions.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
f64dfed9-0ef8-41d2-9bdf-fcb0de148f00,2020-11-28T01:50:26.3420052+00:00,Akinzekeel.BlazorGrid.Abstractions,0.9.1-beta,2020-11-27T22:07:11.7570000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/BlazorGrid.Abstractions.dll,BlazorGrid.Abstractions.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
e63dd786-26a1-46b1-a32b-ae265dbfbcb9,2020-11-28T01:50:23.3615495+00:00,AmbientServices,3.1.49,2020-11-27T21:59:41.5900000+00:00,AvailableAssets,RuntimeAssemblies,,,netstandard2.0,,,,,,lib/netstandard2.0/AmbientServices.dll,AmbientServices.dll,.dll,lib,netstandard2.0,.NETStandard,2.0.0.0,,,0.0.0.0
e63dd786-26a1-46b1-a32b-ae265dbfbcb9,2020-11-28T01:50:23.3615495+00:00,AmbientServices,3.1.49,2020-11-27T21:59:41.5900000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.0,,,,,,lib/netstandard2.0/AmbientServices.dll,AmbientServices.dll,.dll,lib,netstandard2.0,.NETStandard,2.0.0.0,,,0.0.0.0
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,Analogy.LogViewer.GitHubActionLogs,0.1.0,2020-11-27T20:16:53.5800000+00:00,AvailableAssets,RuntimeAssemblies,,,net471,,,,,,lib/net471/Analogy.LogViewer.GitHubActionLogs.dll,Analogy.LogViewer.GitHubActionLogs.dll,.dll,lib,net471,.NETFramework,4.7.1.0,,,0.0.0.0
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,Analogy.LogViewer.GitHubActionLogs,0.1.0,2020-11-27T20:16:53.5800000+00:00,AvailableAssets,RuntimeAssemblies,,,net472,,,,,,lib/net472/Analogy.LogViewer.GitHubActionLogs.dll,Analogy.LogViewer.GitHubActionLogs.dll,.dll,lib,net472,.NETFramework,4.7.2.0,,,0.0.0.0
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,Analogy.LogViewer.GitHubActionLogs,0.1.0,2020-11-27T20:16:53.5800000+00:00,AvailableAssets,RuntimeAssemblies,,,net48,,,,,,lib/net48/Analogy.LogViewer.GitHubActionLogs.dll,Analogy.LogViewer.GitHubActionLogs.dll,.dll,lib,net48,.NETFramework,4.8.0.0,,,0.0.0.0
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,Analogy.LogViewer.GitHubActionLogs,0.1.0,2020-11-27T20:16:53.5800000+00:00,AvailableAssets,RuntimeAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/Analogy.LogViewer.GitHubActionLogs.dll,Analogy.LogViewer.GitHubActionLogs.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,Analogy.LogViewer.GitHubActionLogs,0.1.0,2020-11-27T20:16:53.5800000+00:00,AvailableAssets,CompileLibAssemblies,,,net471,,,,,,lib/net471/Analogy.LogViewer.GitHubActionLogs.dll,Analogy.LogViewer.GitHubActionLogs.dll,.dll,lib,net471,.NETFramework,4.7.1.0,,,0.0.0.0
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,Analogy.LogViewer.GitHubActionLogs,0.1.0,2020-11-27T20:16:53.5800000+00:00,AvailableAssets,CompileLibAssemblies,,,net472,,,,,,lib/net472/Analogy.LogViewer.GitHubActionLogs.dll,Analogy.LogViewer.GitHubActionLogs.dll,.dll,lib,net472,.NETFramework,4.7.2.0,,,0.0.0.0
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,Analogy.LogViewer.GitHubActionLogs,0.1.0,2020-11-27T20:16:53.5800000+00:00,AvailableAssets,CompileLibAssemblies,,,net48,,,,,,lib/net48/Analogy.LogViewer.GitHubActionLogs.dll,Analogy.LogViewer.GitHubActionLogs.dll,.dll,lib,net48,.NETFramework,4.8.0.0,,,0.0.0.0
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,Analogy.LogViewer.GitHubActionLogs,0.1.0,2020-11-27T20:16:53.5800000+00:00,AvailableAssets,CompileLibAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/Analogy.LogViewer.GitHubActionLogs.dll,Analogy.LogViewer.GitHubActionLogs.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-194202,2020-11-27T19:42:13.5670000+00:00,AvailableAssets,RuntimeAssemblies,,,net45,,,,,,lib/net45/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,net45,.NETFramework,4.5.0.0,,,0.0.0.0
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-194202,2020-11-27T19:42:13.5670000+00:00,AvailableAssets,RuntimeAssemblies,,,net5.0,,,,,,lib/net5.0/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-194202,2020-11-27T19:42:13.5670000+00:00,AvailableAssets,RuntimeAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-194202,2020-11-27T19:42:13.5670000+00:00,AvailableAssets,RuntimeAssemblies,,,netstandard2.0,,,,,,lib/netstandard2.0/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,netstandard2.0,.NETStandard,2.0.0.0,,,0.0.0.0
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-194202,2020-11-27T19:42:13.5670000+00:00,AvailableAssets,CompileLibAssemblies,,,net45,,,,,,lib/net45/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,net45,.NETFramework,4.5.0.0,,,0.0.0.0
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-194202,2020-11-27T19:42:13.5670000+00:00,AvailableAssets,CompileLibAssemblies,,,net5.0,,,,,,lib/net5.0/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-194202,2020-11-27T19:42:13.5670000+00:00,AvailableAssets,CompileLibAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-194202,2020-11-27T19:42:13.5670000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.0,,,,,,lib/netstandard2.0/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,netstandard2.0,.NETStandard,2.0.0.0,,,0.0.0.0
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-220435,2020-11-27T22:04:46.4130000+00:00,AvailableAssets,RuntimeAssemblies,,,net45,,,,,,lib/net45/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,net45,.NETFramework,4.5.0.0,,,0.0.0.0
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-220435,2020-11-27T22:04:46.4130000+00:00,AvailableAssets,RuntimeAssemblies,,,net5.0,,,,,,lib/net5.0/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-220435,2020-11-27T22:04:46.4130000+00:00,AvailableAssets,RuntimeAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-220435,2020-11-27T22:04:46.4130000+00:00,AvailableAssets,RuntimeAssemblies,,,netstandard2.0,,,,,,lib/netstandard2.0/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,netstandard2.0,.NETStandard,2.0.0.0,,,0.0.0.0
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-220435,2020-11-27T22:04:46.4130000+00:00,AvailableAssets,CompileLibAssemblies,,,net45,,,,,,lib/net45/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,net45,.NETFramework,4.5.0.0,,,0.0.0.0
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-220435,2020-11-27T22:04:46.4130000+00:00,AvailableAssets,CompileLibAssemblies,,,net5.0,,,,,,lib/net5.0/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-220435,2020-11-27T22:04:46.4130000+00:00,AvailableAssets,CompileLibAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-220435,2020-11-27T22:04:46.4130000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.0,,,,,,lib/netstandard2.0/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,netstandard2.0,.NETStandard,2.0.0.0,,,0.0.0.0
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-221610,2020-11-27T22:16:20.0370000+00:00,AvailableAssets,RuntimeAssemblies,,,net45,,,,,,lib/net45/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,net45,.NETFramework,4.5.0.0,,,0.0.0.0
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-221610,2020-11-27T22:16:20.0370000+00:00,AvailableAssets,RuntimeAssemblies,,,net5.0,,,,,,lib/net5.0/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-221610,2020-11-27T22:16:20.0370000+00:00,AvailableAssets,RuntimeAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-221610,2020-11-27T22:16:20.0370000+00:00,AvailableAssets,RuntimeAssemblies,,,netstandard2.0,,,,,,lib/netstandard2.0/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,netstandard2.0,.NETStandard,2.0.0.0,,,0.0.0.0
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-221610,2020-11-27T22:16:20.0370000+00:00,AvailableAssets,CompileLibAssemblies,,,net45,,,,,,lib/net45/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,net45,.NETFramework,4.5.0.0,,,0.0.0.0
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-221610,2020-11-27T22:16:20.0370000+00:00,AvailableAssets,CompileLibAssemblies,,,net5.0,,,,,,lib/net5.0/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-221610,2020-11-27T22:16:20.0370000+00:00,AvailableAssets,CompileLibAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,Aptacode.PathFinder,1.0.4-CI-20201127-221610,2020-11-27T22:16:20.0370000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.0,,,,,,lib/netstandard2.0/Aptacode.PathFinder.dll,Aptacode.PathFinder.dll,.dll,lib,netstandard2.0,.NETStandard,2.0.0.0,,,0.0.0.0
81a91bdd-d4d0-4e05-babc-1d4c5367acc5,2020-11-28T01:45:36.0526687+00:00,Archimedes.Library,1.0.245,2020-11-27T19:33:25.6870000+00:00,AvailableAssets,RuntimeAssemblies,,,netstandard2.0,,,,,,lib/netstandard2.0/Archimedes.Library.dll,Archimedes.Library.dll,.dll,lib,netstandard2.0,.NETStandard,2.0.0.0,,,0.0.0.0
81a91bdd-d4d0-4e05-babc-1d4c5367acc5,2020-11-28T01:45:36.0526687+00:00,Archimedes.Library,1.0.245,2020-11-27T19:33:25.6870000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.0,,,,,,lib/netstandard2.0/Archimedes.Library.dll,Archimedes.Library.dll,.dll,lib,netstandard2.0,.NETStandard,2.0.0.0,,,0.0.0.0
bdfcfee8-0f34-45b2-9bc2-7075284de5f4,2020-11-28T01:45:42.1469270+00:00,AspNetCore.Mvc.RangedStreamResult,1.3.0,2020-11-27T19:37:05.2530000+00:00,AvailableAssets,RuntimeAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/AspNetCore.Mvc.RangedStreamResult.dll,AspNetCore.Mvc.RangedStreamResult.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
bdfcfee8-0f34-45b2-9bc2-7075284de5f4,2020-11-28T01:45:42.1469270+00:00,AspNetCore.Mvc.RangedStreamResult,1.3.0,2020-11-27T19:37:05.2530000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/AspNetCore.Mvc.RangedStreamResult.dll,AspNetCore.Mvc.RangedStreamResult.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
38769756-9bb4-4ba9-a9a5-40362d1fd96f,2020-11-28T01:48:51.2932584+00:00,AspNetCore.Mvc.RangedStreamResult,1.3.1,2020-11-27T20:28:44.2430000+00:00,AvailableAssets,RuntimeAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/AspNetCore.Mvc.RangedStreamResult.dll,AspNetCore.Mvc.RangedStreamResult.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
38769756-9bb4-4ba9-a9a5-40362d1fd96f,2020-11-28T01:48:51.2932584+00:00,AspNetCore.Mvc.RangedStreamResult,1.3.1,2020-11-27T20:28:44.2430000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/AspNetCore.Mvc.RangedStreamResult.dll,AspNetCore.Mvc.RangedStreamResult.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
2b53c7f1-a200-490d-8826-d18d54d0e467,2020-11-28T01:48:59.1698829+00:00,AspNetCore.Mvc.RangedStreamResult,1.3.2,2020-11-27T20:34:58.2530000+00:00,AvailableAssets,RuntimeAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/AspNetCore.Mvc.RangedStreamResult.dll,AspNetCore.Mvc.RangedStreamResult.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
2b53c7f1-a200-490d-8826-d18d54d0e467,2020-11-28T01:48:59.1698829+00:00,AspNetCore.Mvc.RangedStreamResult,1.3.2,2020-11-27T20:34:58.2530000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/AspNetCore.Mvc.RangedStreamResult.dll,AspNetCore.Mvc.RangedStreamResult.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
f2ae9835-5378-4ee6-8340-d53ae34ff4c0,2020-11-28T01:46:00.0316805+00:00,Avalonia.SKPictureImage,0.4.2-preview9,2020-11-27T20:07:16.2070000+00:00,AvailableAssets,RuntimeAssemblies,,,net461,,,,,,lib/net461/Avalonia.SKPictureImage.dll,Avalonia.SKPictureImage.dll,.dll,lib,net461,.NETFramework,4.6.1.0,,,0.0.0.0
f2ae9835-5378-4ee6-8340-d53ae34ff4c0,2020-11-28T01:46:00.0316805+00:00,Avalonia.SKPictureImage,0.4.2-preview9,2020-11-27T20:07:16.2070000+00:00,AvailableAssets,RuntimeAssemblies,,,net5.0,,,,,,lib/net5.0/Avalonia.SKPictureImage.dll,Avalonia.SKPictureImage.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
f2ae9835-5378-4ee6-8340-d53ae34ff4c0,2020-11-28T01:46:00.0316805+00:00,Avalonia.SKPictureImage,0.4.2-preview9,2020-11-27T20:07:16.2070000+00:00,AvailableAssets,RuntimeAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/Avalonia.SKPictureImage.dll,Avalonia.SKPictureImage.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
f2ae9835-5378-4ee6-8340-d53ae34ff4c0,2020-11-28T01:46:00.0316805+00:00,Avalonia.SKPictureImage,0.4.2-preview9,2020-11-27T20:07:16.2070000+00:00,AvailableAssets,RuntimeAssemblies,,,netstandard2.0,,,,,,lib/netstandard2.0/Avalonia.SKPictureImage.dll,Avalonia.SKPictureImage.dll,.dll,lib,netstandard2.0,.NETStandard,2.0.0.0,,,0.0.0.0
";

    const string WithQuotes = @"75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""net5.0"","""","""","""","""","""",""lib/net5.0/BlazorGrid.dll"",""BlazorGrid.dll"","".dll"",""lib"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""net5.0"","""","""","""","""","""",""lib/net5.0/BlazorGrid.dll"",""BlazorGrid.dll"","".dll"",""lib"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""ResourceAssemblies"","""","""",""net5.0"","""","""","""","""","""",""lib/net5.0/de/BlazorGrid.resources.dll"",""BlazorGrid.resources.dll"","".dll"",""lib"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""MSBuildFiles"","""","""",""any"","""","""","""","""","""",""build/Microsoft.AspNetCore.StaticWebAssets.props"",""Microsoft.AspNetCore.StaticWebAssets.props"","".props"",""build"",""any"",""Any"",""0.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""MSBuildFiles"","""","""",""any"","""","""","""","""","""",""build/Akinzekeel.BlazorGrid.props"",""Akinzekeel.BlazorGrid.props"","".props"",""build"",""any"",""Any"",""0.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""MSBuildMultiTargetingFiles"","""","""",""any"","""","""","""","""","""",""buildMultiTargeting/Akinzekeel.BlazorGrid.props"",""Akinzekeel.BlazorGrid.props"","".props"",""buildmultitargeting"",""any"",""Any"",""0.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""ContentFiles"","""",""any"",""net5.0"","""","""","""","""","""",""contentFiles/any/net5.0/libman.json"",""libman.json"","".json"",""contentfiles"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""ContentFiles"","""",""any"",""net5.0"","""","""","""","""","""",""contentFiles/any/net5.0/package-lock.json"",""package-lock.json"","".json"",""contentfiles"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""ContentFiles"","""",""any"",""net5.0"","""","""","""","""","""",""contentFiles/any/net5.0/package.json"",""package.json"","".json"",""contentfiles"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""ContentFiles"","""",""any"",""net5.0"","""","""","""","""","""",""contentFiles/any/net5.0/Content/blazorgrid-bootstrap.scss"",""blazorgrid-bootstrap.scss"","".scss"",""contentfiles"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""ContentFiles"","""",""any"",""net5.0"","""","""","""","""","""",""contentFiles/any/net5.0/Content/blazorgrid-spectre.scss"",""blazorgrid-spectre.scss"","".scss"",""contentfiles"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""ContentFiles"","""",""any"",""net5.0"","""","""","""","""","""",""contentFiles/any/net5.0/Content/BlazorGrid.scss"",""BlazorGrid.scss"","".scss"",""contentfiles"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""ContentFiles"","""",""any"",""net5.0"","""","""","""","""","""",""contentFiles/any/net5.0/Content/bootstrap/scss/_functions.scss"",""_functions.scss"","".scss"",""contentfiles"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""ContentFiles"","""",""any"",""net5.0"","""","""","""","""","""",""contentFiles/any/net5.0/Content/bootstrap/scss/_variables.scss"",""_variables.scss"","".scss"",""contentfiles"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""ContentFiles"","""",""any"",""net5.0"","""","""","""","""","""",""contentFiles/any/net5.0/Content/spectre/_variables.scss"",""_variables.scss"","".scss"",""contentfiles"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,""Akinzekeel.BlazorGrid"",""0.9.1-preview"",2020-11-27T22:42:54.3100000+00:00,""AvailableAssets"",""MSBuildTransitiveFiles"","""","""",""any"","""","""","""","""","""",""buildTransitive/Akinzekeel.BlazorGrid.props"",""Akinzekeel.BlazorGrid.props"","".props"",""buildtransitive"",""any"",""Any"",""0.0.0.0"","""","""",""0.0.0.0""
f64dfed9-0ef8-41d2-9bdf-fcb0de148f00,2020-11-28T01:50:26.3420052+00:00,""Akinzekeel.BlazorGrid.Abstractions"",""0.9.1-beta"",2020-11-27T22:07:11.7570000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netstandard2.1"","""","""","""","""","""",""lib/netstandard2.1/BlazorGrid.Abstractions.dll"",""BlazorGrid.Abstractions.dll"","".dll"",""lib"",""netstandard2.1"","".NETStandard"",""2.1.0.0"","""","""",""0.0.0.0""
f64dfed9-0ef8-41d2-9bdf-fcb0de148f00,2020-11-28T01:50:26.3420052+00:00,""Akinzekeel.BlazorGrid.Abstractions"",""0.9.1-beta"",2020-11-27T22:07:11.7570000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""netstandard2.1"","""","""","""","""","""",""lib/netstandard2.1/BlazorGrid.Abstractions.dll"",""BlazorGrid.Abstractions.dll"","".dll"",""lib"",""netstandard2.1"","".NETStandard"",""2.1.0.0"","""","""",""0.0.0.0""
e63dd786-26a1-46b1-a32b-ae265dbfbcb9,2020-11-28T01:50:23.3615495+00:00,""AmbientServices"",""3.1.49"",2020-11-27T21:59:41.5900000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netstandard2.0"","""","""","""","""","""",""lib/netstandard2.0/AmbientServices.dll"",""AmbientServices.dll"","".dll"",""lib"",""netstandard2.0"","".NETStandard"",""2.0.0.0"","""","""",""0.0.0.0""
e63dd786-26a1-46b1-a32b-ae265dbfbcb9,2020-11-28T01:50:23.3615495+00:00,""AmbientServices"",""3.1.49"",2020-11-27T21:59:41.5900000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""netstandard2.0"","""","""","""","""","""",""lib/netstandard2.0/AmbientServices.dll"",""AmbientServices.dll"","".dll"",""lib"",""netstandard2.0"","".NETStandard"",""2.0.0.0"","""","""",""0.0.0.0""
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,""Analogy.LogViewer.GitHubActionLogs"",""0.1.0"",2020-11-27T20:16:53.5800000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""net471"","""","""","""","""","""",""lib/net471/Analogy.LogViewer.GitHubActionLogs.dll"",""Analogy.LogViewer.GitHubActionLogs.dll"","".dll"",""lib"",""net471"","".NETFramework"",""4.7.1.0"","""","""",""0.0.0.0""
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,""Analogy.LogViewer.GitHubActionLogs"",""0.1.0"",2020-11-27T20:16:53.5800000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""net472"","""","""","""","""","""",""lib/net472/Analogy.LogViewer.GitHubActionLogs.dll"",""Analogy.LogViewer.GitHubActionLogs.dll"","".dll"",""lib"",""net472"","".NETFramework"",""4.7.2.0"","""","""",""0.0.0.0""
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,""Analogy.LogViewer.GitHubActionLogs"",""0.1.0"",2020-11-27T20:16:53.5800000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""net48"","""","""","""","""","""",""lib/net48/Analogy.LogViewer.GitHubActionLogs.dll"",""Analogy.LogViewer.GitHubActionLogs.dll"","".dll"",""lib"",""net48"","".NETFramework"",""4.8.0.0"","""","""",""0.0.0.0""
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,""Analogy.LogViewer.GitHubActionLogs"",""0.1.0"",2020-11-27T20:16:53.5800000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netcoreapp3.1"","""","""","""","""","""",""lib/netcoreapp3.1/Analogy.LogViewer.GitHubActionLogs.dll"",""Analogy.LogViewer.GitHubActionLogs.dll"","".dll"",""lib"",""netcoreapp3.1"","".NETCoreApp"",""3.1.0.0"","""","""",""0.0.0.0""
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,""Analogy.LogViewer.GitHubActionLogs"",""0.1.0"",2020-11-27T20:16:53.5800000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""net471"","""","""","""","""","""",""lib/net471/Analogy.LogViewer.GitHubActionLogs.dll"",""Analogy.LogViewer.GitHubActionLogs.dll"","".dll"",""lib"",""net471"","".NETFramework"",""4.7.1.0"","""","""",""0.0.0.0""
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,""Analogy.LogViewer.GitHubActionLogs"",""0.1.0"",2020-11-27T20:16:53.5800000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""net472"","""","""","""","""","""",""lib/net472/Analogy.LogViewer.GitHubActionLogs.dll"",""Analogy.LogViewer.GitHubActionLogs.dll"","".dll"",""lib"",""net472"","".NETFramework"",""4.7.2.0"","""","""",""0.0.0.0""
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,""Analogy.LogViewer.GitHubActionLogs"",""0.1.0"",2020-11-27T20:16:53.5800000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""net48"","""","""","""","""","""",""lib/net48/Analogy.LogViewer.GitHubActionLogs.dll"",""Analogy.LogViewer.GitHubActionLogs.dll"","".dll"",""lib"",""net48"","".NETFramework"",""4.8.0.0"","""","""",""0.0.0.0""
160f0c42-96fe-4969-bcc3-1232d213a653,2020-11-28T01:48:44.4908775+00:00,""Analogy.LogViewer.GitHubActionLogs"",""0.1.0"",2020-11-27T20:16:53.5800000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""netcoreapp3.1"","""","""","""","""","""",""lib/netcoreapp3.1/Analogy.LogViewer.GitHubActionLogs.dll"",""Analogy.LogViewer.GitHubActionLogs.dll"","".dll"",""lib"",""netcoreapp3.1"","".NETCoreApp"",""3.1.0.0"","""","""",""0.0.0.0""
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-194202"",2020-11-27T19:42:13.5670000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""net45"","""","""","""","""","""",""lib/net45/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""net45"","".NETFramework"",""4.5.0.0"","""","""",""0.0.0.0""
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-194202"",2020-11-27T19:42:13.5670000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""net5.0"","""","""","""","""","""",""lib/net5.0/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-194202"",2020-11-27T19:42:13.5670000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netcoreapp3.1"","""","""","""","""","""",""lib/netcoreapp3.1/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""netcoreapp3.1"","".NETCoreApp"",""3.1.0.0"","""","""",""0.0.0.0""
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-194202"",2020-11-27T19:42:13.5670000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netstandard2.0"","""","""","""","""","""",""lib/netstandard2.0/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""netstandard2.0"","".NETStandard"",""2.0.0.0"","""","""",""0.0.0.0""
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-194202"",2020-11-27T19:42:13.5670000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""net45"","""","""","""","""","""",""lib/net45/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""net45"","".NETFramework"",""4.5.0.0"","""","""",""0.0.0.0""
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-194202"",2020-11-27T19:42:13.5670000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""net5.0"","""","""","""","""","""",""lib/net5.0/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-194202"",2020-11-27T19:42:13.5670000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""netcoreapp3.1"","""","""","""","""","""",""lib/netcoreapp3.1/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""netcoreapp3.1"","".NETCoreApp"",""3.1.0.0"","""","""",""0.0.0.0""
811da741-3b8e-4131-8c8f-ce87728a9af9,2020-11-28T01:45:44.7439483+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-194202"",2020-11-27T19:42:13.5670000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""netstandard2.0"","""","""","""","""","""",""lib/netstandard2.0/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""netstandard2.0"","".NETStandard"",""2.0.0.0"","""","""",""0.0.0.0""
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-220435"",2020-11-27T22:04:46.4130000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""net45"","""","""","""","""","""",""lib/net45/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""net45"","".NETFramework"",""4.5.0.0"","""","""",""0.0.0.0""
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-220435"",2020-11-27T22:04:46.4130000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""net5.0"","""","""","""","""","""",""lib/net5.0/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-220435"",2020-11-27T22:04:46.4130000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netcoreapp3.1"","""","""","""","""","""",""lib/netcoreapp3.1/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""netcoreapp3.1"","".NETCoreApp"",""3.1.0.0"","""","""",""0.0.0.0""
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-220435"",2020-11-27T22:04:46.4130000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netstandard2.0"","""","""","""","""","""",""lib/netstandard2.0/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""netstandard2.0"","".NETStandard"",""2.0.0.0"","""","""",""0.0.0.0""
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-220435"",2020-11-27T22:04:46.4130000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""net45"","""","""","""","""","""",""lib/net45/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""net45"","".NETFramework"",""4.5.0.0"","""","""",""0.0.0.0""
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-220435"",2020-11-27T22:04:46.4130000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""net5.0"","""","""","""","""","""",""lib/net5.0/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-220435"",2020-11-27T22:04:46.4130000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""netcoreapp3.1"","""","""","""","""","""",""lib/netcoreapp3.1/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""netcoreapp3.1"","".NETCoreApp"",""3.1.0.0"","""","""",""0.0.0.0""
8bfd92bf-9534-49c3-a5a7-16b092bdc341,2020-11-28T01:50:25.6550904+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-220435"",2020-11-27T22:04:46.4130000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""netstandard2.0"","""","""","""","""","""",""lib/netstandard2.0/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""netstandard2.0"","".NETStandard"",""2.0.0.0"","""","""",""0.0.0.0""
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-221610"",2020-11-27T22:16:20.0370000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""net45"","""","""","""","""","""",""lib/net45/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""net45"","".NETFramework"",""4.5.0.0"","""","""",""0.0.0.0""
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-221610"",2020-11-27T22:16:20.0370000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""net5.0"","""","""","""","""","""",""lib/net5.0/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-221610"",2020-11-27T22:16:20.0370000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netcoreapp3.1"","""","""","""","""","""",""lib/netcoreapp3.1/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""netcoreapp3.1"","".NETCoreApp"",""3.1.0.0"","""","""",""0.0.0.0""
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-221610"",2020-11-27T22:16:20.0370000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netstandard2.0"","""","""","""","""","""",""lib/netstandard2.0/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""netstandard2.0"","".NETStandard"",""2.0.0.0"","""","""",""0.0.0.0""
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-221610"",2020-11-27T22:16:20.0370000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""net45"","""","""","""","""","""",""lib/net45/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""net45"","".NETFramework"",""4.5.0.0"","""","""",""0.0.0.0""
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-221610"",2020-11-27T22:16:20.0370000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""net5.0"","""","""","""","""","""",""lib/net5.0/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-221610"",2020-11-27T22:16:20.0370000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""netcoreapp3.1"","""","""","""","""","""",""lib/netcoreapp3.1/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""netcoreapp3.1"","".NETCoreApp"",""3.1.0.0"","""","""",""0.0.0.0""
0a0726b6-aeae-4d8c-9e41-125ebb54f54c,2020-11-28T01:50:30.5021763+00:00,""Aptacode.PathFinder"",""1.0.4-CI-20201127-221610"",2020-11-27T22:16:20.0370000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""netstandard2.0"","""","""","""","""","""",""lib/netstandard2.0/Aptacode.PathFinder.dll"",""Aptacode.PathFinder.dll"","".dll"",""lib"",""netstandard2.0"","".NETStandard"",""2.0.0.0"","""","""",""0.0.0.0""
81a91bdd-d4d0-4e05-babc-1d4c5367acc5,2020-11-28T01:45:36.0526687+00:00,""Archimedes.Library"",""1.0.245"",2020-11-27T19:33:25.6870000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netstandard2.0"","""","""","""","""","""",""lib/netstandard2.0/Archimedes.Library.dll"",""Archimedes.Library.dll"","".dll"",""lib"",""netstandard2.0"","".NETStandard"",""2.0.0.0"","""","""",""0.0.0.0""
81a91bdd-d4d0-4e05-babc-1d4c5367acc5,2020-11-28T01:45:36.0526687+00:00,""Archimedes.Library"",""1.0.245"",2020-11-27T19:33:25.6870000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""netstandard2.0"","""","""","""","""","""",""lib/netstandard2.0/Archimedes.Library.dll"",""Archimedes.Library.dll"","".dll"",""lib"",""netstandard2.0"","".NETStandard"",""2.0.0.0"","""","""",""0.0.0.0""
bdfcfee8-0f34-45b2-9bc2-7075284de5f4,2020-11-28T01:45:42.1469270+00:00,""AspNetCore.Mvc.RangedStreamResult"",""1.3.0"",2020-11-27T19:37:05.2530000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netstandard2.1"","""","""","""","""","""",""lib/netstandard2.1/AspNetCore.Mvc.RangedStreamResult.dll"",""AspNetCore.Mvc.RangedStreamResult.dll"","".dll"",""lib"",""netstandard2.1"","".NETStandard"",""2.1.0.0"","""","""",""0.0.0.0""
bdfcfee8-0f34-45b2-9bc2-7075284de5f4,2020-11-28T01:45:42.1469270+00:00,""AspNetCore.Mvc.RangedStreamResult"",""1.3.0"",2020-11-27T19:37:05.2530000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""netstandard2.1"","""","""","""","""","""",""lib/netstandard2.1/AspNetCore.Mvc.RangedStreamResult.dll"",""AspNetCore.Mvc.RangedStreamResult.dll"","".dll"",""lib"",""netstandard2.1"","".NETStandard"",""2.1.0.0"","""","""",""0.0.0.0""
38769756-9bb4-4ba9-a9a5-40362d1fd96f,2020-11-28T01:48:51.2932584+00:00,""AspNetCore.Mvc.RangedStreamResult"",""1.3.1"",2020-11-27T20:28:44.2430000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netstandard2.1"","""","""","""","""","""",""lib/netstandard2.1/AspNetCore.Mvc.RangedStreamResult.dll"",""AspNetCore.Mvc.RangedStreamResult.dll"","".dll"",""lib"",""netstandard2.1"","".NETStandard"",""2.1.0.0"","""","""",""0.0.0.0""
38769756-9bb4-4ba9-a9a5-40362d1fd96f,2020-11-28T01:48:51.2932584+00:00,""AspNetCore.Mvc.RangedStreamResult"",""1.3.1"",2020-11-27T20:28:44.2430000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""netstandard2.1"","""","""","""","""","""",""lib/netstandard2.1/AspNetCore.Mvc.RangedStreamResult.dll"",""AspNetCore.Mvc.RangedStreamResult.dll"","".dll"",""lib"",""netstandard2.1"","".NETStandard"",""2.1.0.0"","""","""",""0.0.0.0""
2b53c7f1-a200-490d-8826-d18d54d0e467,2020-11-28T01:48:59.1698829+00:00,""AspNetCore.Mvc.RangedStreamResult"",""1.3.2"",2020-11-27T20:34:58.2530000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netstandard2.1"","""","""","""","""","""",""lib/netstandard2.1/AspNetCore.Mvc.RangedStreamResult.dll"",""AspNetCore.Mvc.RangedStreamResult.dll"","".dll"",""lib"",""netstandard2.1"","".NETStandard"",""2.1.0.0"","""","""",""0.0.0.0""
2b53c7f1-a200-490d-8826-d18d54d0e467,2020-11-28T01:48:59.1698829+00:00,""AspNetCore.Mvc.RangedStreamResult"",""1.3.2"",2020-11-27T20:34:58.2530000+00:00,""AvailableAssets"",""CompileLibAssemblies"","""","""",""netstandard2.1"","""","""","""","""","""",""lib/netstandard2.1/AspNetCore.Mvc.RangedStreamResult.dll"",""AspNetCore.Mvc.RangedStreamResult.dll"","".dll"",""lib"",""netstandard2.1"","".NETStandard"",""2.1.0.0"","""","""",""0.0.0.0""
f2ae9835-5378-4ee6-8340-d53ae34ff4c0,2020-11-28T01:46:00.0316805+00:00,""Avalonia.SKPictureImage"",""0.4.2-preview9"",2020-11-27T20:07:16.2070000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""net461"","""","""","""","""","""",""lib/net461/Avalonia.SKPictureImage.dll"",""Avalonia.SKPictureImage.dll"","".dll"",""lib"",""net461"","".NETFramework"",""4.6.1.0"","""","""",""0.0.0.0""
f2ae9835-5378-4ee6-8340-d53ae34ff4c0,2020-11-28T01:46:00.0316805+00:00,""Avalonia.SKPictureImage"",""0.4.2-preview9"",2020-11-27T20:07:16.2070000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""net5.0"","""","""","""","""","""",""lib/net5.0/Avalonia.SKPictureImage.dll"",""Avalonia.SKPictureImage.dll"","".dll"",""lib"",""net5.0"","".NETCoreApp"",""5.0.0.0"","""","""",""0.0.0.0""
f2ae9835-5378-4ee6-8340-d53ae34ff4c0,2020-11-28T01:46:00.0316805+00:00,""Avalonia.SKPictureImage"",""0.4.2-preview9"",2020-11-27T20:07:16.2070000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netcoreapp3.1"","""","""","""","""","""",""lib/netcoreapp3.1/Avalonia.SKPictureImage.dll"",""Avalonia.SKPictureImage.dll"","".dll"",""lib"",""netcoreapp3.1"","".NETCoreApp"",""3.1.0.0"","""","""",""0.0.0.0""
f2ae9835-5378-4ee6-8340-d53ae34ff4c0,2020-11-28T01:46:00.0316805+00:00,""Avalonia.SKPictureImage"",""0.4.2-preview9"",2020-11-27T20:07:16.2070000+00:00,""AvailableAssets"",""RuntimeAssemblies"","""","""",""netstandard2.0"","""","""","""","""","""",""lib/netstandard2.0/Avalonia.SKPictureImage.dll"",""Avalonia.SKPictureImage.dll"","".dll"",""lib"",""netstandard2.0"","".NETStandard"",""2.0.0.0"","""","""",""0.0.0.0""
";
}
