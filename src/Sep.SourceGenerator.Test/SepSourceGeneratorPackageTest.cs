using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.SourceGenerator.Test;

[TestClass]
public class SepSourceGeneratorPackageTest
{
    [TestMethod]
    public void SepSourceGeneratorPackageTest_ContainsAnalyzerAndSepRuntimeDependency()
    {
        var packageDirectory = typeof(SepSourceGeneratorPackageTest).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(static attribute => attribute.Key == "SepPackageDirectory").Value!;
        var packagePath = Directory.GetFiles(packageDirectory, "Sep.SourceGenerator.*.nupkg")
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .First();

        using var archive = ZipFile.OpenRead(packagePath);
        Assert.IsNotNull(archive.GetEntry("analyzers/dotnet/cs/Sep.SourceGenerator.dll"));
        var nuspecEntry = archive.Entries.Single(static entry => entry.FullName.EndsWith(".nuspec", StringComparison.Ordinal));
        using var nuspecStream = nuspecEntry.Open();
        var document = XDocument.Load(nuspecStream);
        var dependency = document.Descendants().Single(static element =>
            element.Name.LocalName == "dependency" && element.Attribute("id")?.Value == "Sep");

        var excludedAssets = dependency.Attribute("exclude")?.Value ?? string.Empty;
        Assert.IsFalse(excludedAssets.Contains("Compile", StringComparison.Ordinal));
        Assert.IsFalse(excludedAssets.Contains("Runtime", StringComparison.Ordinal));
    }
}
