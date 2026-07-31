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
        using var archive = OpenPackage();
        Assert.IsNotNull(archive.GetEntry("analyzers/dotnet/cs/Sep.SourceGenerator.dll"));
        var dependency = ReadNuspec(archive).Descendants().Single(static element =>
            element.Name.LocalName == "dependency" && element.Attribute("id")?.Value == "Sep");

        var excludedAssets = dependency.Attribute("exclude")?.Value ?? string.Empty;
        Assert.IsFalse(excludedAssets.Contains("Compile", StringComparison.Ordinal));
        Assert.IsFalse(excludedAssets.Contains("Runtime", StringComparison.Ordinal));
    }

    /// <summary>
    /// The net8.0 target framework exists only to declare the Sep dependency, since Sep targets
    /// net8.0 and later and therefore cannot be referenced from the netstandard2.0 analyzer. If the
    /// dependency ever moves out of a net8.0 group the extra target framework is pointless.
    /// </summary>
    [TestMethod]
    public void SepSourceGeneratorPackageTest_DeclaresSepOnlyForNet8AndLater()
    {
        using var archive = OpenPackage();
        var groups = ReadNuspec(archive).Descendants()
            .Where(static element => element.Name.LocalName == "group")
            .ToArray();
        var sepGroups = groups
            .Where(static group => group.Elements().Any(static dependency =>
                dependency.Attribute("id")?.Value == "Sep"))
            .ToArray();

        Assert.HasCount(1, sepGroups);
        Assert.AreEqual("net8.0", sepGroups[0].Attribute("targetFramework")?.Value);
        var netStandardGroup = groups.Single(static group =>
            group.Attribute("targetFramework")?.Value == ".NETStandard2.0");
        Assert.IsEmpty(netStandardGroup.Elements());
    }

    /// <summary>
    /// Only the netstandard2.0 build is loaded by Roslyn, so shipping any other assembly would be
    /// dead weight and could be loaded by mistake.
    /// </summary>
    [TestMethod]
    public void SepSourceGeneratorPackageTest_ShipsOnlyTheNetStandardAnalyzerAssembly()
    {
        using var archive = OpenPackage();
        var assemblies = archive.Entries
            .Where(static entry => entry.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            .Select(static entry => entry.FullName)
            .ToArray();

        Assert.AreSequenceEqual(new[] { "analyzers/dotnet/cs/Sep.SourceGenerator.dll" }, assemblies);
    }

    /// <summary>
    /// The generated source calls Sep, so the declared Sep version must be the Sep built alongside
    /// the generator. A hardcoded package reference version would silently go stale and could let a
    /// consumer resolve a Sep too old for the APIs the generated source uses.
    /// </summary>
    [TestMethod]
    public void SepSourceGeneratorPackageTest_DeclaredSepVersionMatchesPackageVersion()
    {
        using var archive = OpenPackage();
        var nuspec = ReadNuspec(archive);
        var packageVersion = nuspec.Descendants()
            .Single(static element => element.Name.LocalName == "version").Value;
        var dependency = nuspec.Descendants().Single(static element =>
            element.Name.LocalName == "dependency" && element.Attribute("id")?.Value == "Sep");

        Assert.AreEqual(packageVersion, dependency.Attribute("version")?.Value);
    }

    static ZipArchive OpenPackage()
    {
        var packageDirectory = typeof(SepSourceGeneratorPackageTest).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(static attribute => attribute.Key == "SepPackageDirectory").Value!;
        var packagePath = Directory.GetFiles(packageDirectory, "Sep.SourceGenerator.*.nupkg")
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .First();
        return ZipFile.OpenRead(packagePath);
    }

    static XDocument ReadNuspec(ZipArchive archive)
    {
        var nuspecEntry = archive.Entries.Single(static entry =>
            entry.FullName.EndsWith(".nuspec", StringComparison.Ordinal));
        using var nuspecStream = nuspecEntry.Open();
        return XDocument.Load(nuspecStream);
    }
}
