using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.SourceGenerator.Test;

/// <summary>
/// Accessibility must be judged from the adapter, which may live in a different assembly than the
/// model. A hand rolled check on DeclaredAccessibility cannot see assembly identity and would let
/// the generator emit code referencing members it cannot actually touch.
/// </summary>
[TestClass]
public class SepSourceGeneratorAccessibilityTest
{
    static readonly ImmutableArray<MetadataReference> s_references = CreateReferences();

    [TestMethod]
    public void SepSourceGeneratorAccessibilityTest_RejectsInternalSetterInOtherAssembly()
    {
        var result = Run("""
            public class Person
            {
                public int Id { get; internal set; }
            }
            """);

        Assert.IsTrue(result.Diagnostics.Any(static diagnostic => diagnostic.Id == "SEPGEN012"),
            string.Join(", ", result.Diagnostics.Select(static diagnostic => diagnostic.Id)));
        Assert.IsEmpty(result.CompilationErrors);
    }

    [TestMethod]
    public void SepSourceGeneratorAccessibilityTest_RejectsInternalModelInOtherAssembly()
    {
        var result = Run("""
            internal class Person
            {
                public int Id { get; set; }
            }
            """);

        // Either way the generator must never emit code naming a type it cannot reference.
        Assert.IsEmpty(result.GeneratedSource);
        Assert.IsEmpty(result.CompilationErrors);
    }

    [TestMethod]
    public void SepSourceGeneratorAccessibilityTest_AllowsInternalSetterWithInternalsVisibleTo()
    {
        var result = Run("""
            [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Consumer")]
            public class Person
            {
                public int Id { get; internal set; }
            }
            """);

        Assert.IsEmpty(result.Diagnostics,
            string.Join(", ", result.Diagnostics.Select(static diagnostic => diagnostic.ToString())));
        Assert.IsEmpty(result.CompilationErrors,
            string.Join(", ", result.CompilationErrors.Select(static diagnostic => diagnostic.ToString())));
        StringAssert.Contains(result.GeneratedSource, "@Id = ");
    }

    [TestMethod]
    public void SepSourceGeneratorAccessibilityTest_RejectsInternalConstructorInOtherAssembly()
    {
        var result = Run("""
            public class Person
            {
                internal Person() { }
                public int Id { get; set; }
            }
            """);

        Assert.IsTrue(result.Diagnostics.Any(static diagnostic => diagnostic.Id == "SEPGEN008"),
            string.Join(", ", result.Diagnostics.Select(static diagnostic => diagnostic.Id)));
        Assert.IsEmpty(result.CompilationErrors);
    }

    static RunResult Run(string modelSource, string modelTypeName = "Person")
    {
        var modelCompilation = CSharpCompilation.Create("ModelAssembly",
            [Parse(modelSource)],
            s_references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Enable));
        using var peStream = new MemoryStream();
        var emitResult = modelCompilation.Emit(peStream);
        Assert.IsTrue(emitResult.Success, string.Join(Environment.NewLine, emitResult.Diagnostics));
        peStream.Position = 0;

        var consumerSource = $$"""
            using nietras.SeparatedValues;
            [SepSourceGeneration(typeof({{modelTypeName}}))]
            public static partial class PersonSep { }
            """;
        var compilation = CSharpCompilation.Create("Consumer",
            [Parse(consumerSource)],
            s_references.Add(MetadataReference.CreateFromStream(peStream)),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Enable));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new SepSourceGenerator().AsSourceGenerator()],
            parseOptions: CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview));
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var output, out _);
        var runResult = driver.GetRunResult();

        // Errors about the model type itself are expected when the model is not accessible, only
        // errors inside the generated source indicate the generator emitted uncompilable code.
        var generatedErrors = output.GetDiagnostics()
            .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error &&
                diagnostic.Location.SourceTree?.FilePath.EndsWith(".Sep.g.cs", StringComparison.Ordinal) == true)
            .ToImmutableArray();
        var generatedSource = string.Concat(runResult.Results.Single().GeneratedSources
            .Where(static source => source.HintName.EndsWith(".Sep.g.cs", StringComparison.Ordinal))
            .Select(static source => source.SourceText.ToString()));
        return new(runResult.Diagnostics, generatedErrors, generatedSource);
    }

    static SyntaxTree Parse(string source) =>
        CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview));

    static ImmutableArray<MetadataReference> CreateReferences()
    {
        var trustedPlatformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?? throw new InvalidOperationException("Trusted platform assemblies are unavailable.");
        return trustedPlatformAssemblies.Split(Path.PathSeparator)
            .Select(static path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .Append(MetadataReference.CreateFromFile(typeof(Sep).Assembly.Location))
            .ToImmutableArray();
    }

    readonly record struct RunResult(
        ImmutableArray<Diagnostic> Diagnostics,
        ImmutableArray<Diagnostic> CompilationErrors,
        string GeneratedSource);
}
