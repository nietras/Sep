using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SourceFilesAsyncSyncTest
{
    [DataRow("Sep/SepReader.IO.Async.cs", "Sep/SepReader.IO.Sync.cs")]
    [DataRow("Sep/SepReaderExtensions.IO.Async.cs", "Sep/SepReaderExtensions.IO.Sync.cs")]
    [DataRow("Sep/SepWriter.IO.Async.cs", "Sep/SepWriter.IO.Sync.cs")]
    [DataTestMethod]
    public void SourceFilesAsyncSyncTest_Verify(string asyncFilePath, string syncFilePath)
    {
        var sourceDirectory = Path.GetDirectoryName(SourceFile()) + @"../../../src/";

        var asyncFile = Path.Combine(sourceDirectory, asyncFilePath);
        var syncFile = Path.Combine(sourceDirectory, syncFilePath);

        var asyncLines = File.ReadAllLines(asyncFile);
        var syncLines = File.ReadAllLines(syncFile);

        Assert.AreEqual(asyncLines.Length, syncLines.Length);
        for (var i = 1; i < asyncLines.Length; i++)
        {
            Assert.AreEqual(asyncLines[i], syncLines[i]);
        }

        static string SourceFile([CallerFilePath] string sourceFilePath = "") => sourceFilePath;
    }
}
