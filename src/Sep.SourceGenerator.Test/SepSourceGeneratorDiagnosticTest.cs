using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.SourceGenerator.Test;

/// <summary>
/// Diagnostic ids must exist in exactly one place, so these tests verify the single source of truth
/// stays consistent with the descriptors and with the shipped analyzer release notes.
/// </summary>
[TestClass]
public class SepSourceGeneratorDiagnosticTest
{
    static readonly ImmutableArray<SepSourceGenerator.IssueId> s_ids =
        Enum.GetValues<SepSourceGenerator.IssueId>().ToImmutableArray();

    [TestMethod]
    public void SepSourceGeneratorDiagnosticTest_IssueIdsAreContiguousFromOne()
    {
        for (var index = 0; index < s_ids.Length; ++index)
        {
            Assert.AreEqual(index + 1, (int)s_ids[index]);
        }
    }

    [TestMethod]
    public void SepSourceGeneratorDiagnosticTest_EveryIssueIdHasMatchingDescriptor()
    {
        foreach (var id in s_ids)
        {
            var diagnostic = SepSourceGenerator.CreateDiagnostic(
                SepSourceGenerator.Issue.Create(id, Location.None, "a", "b", "c"));

            Assert.AreEqual(SepSourceGenerator.DiagnosticId(id), diagnostic.Id);
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.Severity);
            Assert.AreEqual("Usage", diagnostic.Descriptor.Category);
            Assert.IsNotEmpty(diagnostic.Descriptor.Title.ToString());
            Assert.IsNotEmpty(diagnostic.GetMessage());
        }
    }

    [TestMethod]
    public void SepSourceGeneratorDiagnosticTest_DiagnosticIdsAreFormattedAndUnique()
    {
        var ids = s_ids.Select(SepSourceGenerator.DiagnosticId).ToArray();

        Assert.AreEqual("SEPGEN001", ids[0]);
        Assert.AreEqual(ids.Length, ids.Distinct(StringComparer.Ordinal).Count());
        Assert.IsTrue(ids.All(static id => id.Length == "SEPGEN000".Length),
            string.Join(", ", ids));
    }

    [TestMethod]
    public void SepSourceGeneratorDiagnosticTest_AnalyzerReleasesListsEveryDiagnosticId()
    {
        var releaseNotes = File.ReadAllText(Path.Combine(
            FindRepositoryRoot(), "src", "Sep.SourceGenerator", "AnalyzerReleases.Unshipped.md"));
        var documented = releaseNotes.Split('\n')
            .Select(static line => line.Split('|')[0].Trim())
            .Where(static value => value.StartsWith("SEPGEN", StringComparison.Ordinal))
            .ToArray();

        CollectionAssert.AreEqual(s_ids.Select(SepSourceGenerator.DiagnosticId).ToArray(), documented);
    }

    static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Sep.slnx")))
            {
                return directory.FullName;
            }
        }
        throw new DirectoryNotFoundException("Could not find the repository root.");
    }
}
