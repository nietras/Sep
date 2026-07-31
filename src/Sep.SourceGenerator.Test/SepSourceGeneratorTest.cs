using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.SourceGenerator.Test;

[TestClass]
public class SepSourceGeneratorTest
{
    const string SourcePrefix = """
        using System;
        using nietras.SeparatedValues;

        """;

    static readonly ImmutableArray<MetadataReference> s_references = CreateReferences();

    [TestMethod]
    public void SepSourceGeneratorTest_GeneratesCompleteIndexedLayout()
    {
        var result = Run("""
            [SepSourceGeneration(typeof(Person))]
            public static partial class PersonSep
            {
            }

            public class Person
            {
                [SepCol("third", 2)]
                public int Id { get; set; }

                [SepCol("first", 0)]
                public string Name { get; set; } = "";

                [SepCol("second", 1)]
                public int Age { get; set; }
            }
            """);

        Assert.IsEmpty(result.Diagnostics,
            string.Join(Environment.NewLine, result.Diagnostics.Select(static diagnostic => diagnostic.ToString())));
        Assert.IsEmpty(result.CompilationDiagnostics);
        var source = result.GeneratedSource;
        StringAssert.Contains(source, "@Id = row[2].Parse<int>()");
        StringAssert.Contains(source, "@Name = row[0].ToString()");
        var first = source.IndexOf("row[\"first\"]", StringComparison.Ordinal);
        var second = source.IndexOf("row[\"second\"]", StringComparison.Ordinal);
        var third = source.IndexOf("row[\"third\"]", StringComparison.Ordinal);
        Assert.IsTrue(first >= 0 && first < second && second < third);
    }

    [TestMethod]
    public void SepSourceGeneratorTest_GeneratesModernModelsNullableEnumAndAsyncRead()
    {
        var result = Run("""
            namespace Example;

            public enum State
            {
                Unknown,
                Ready,
            }

            [SepSourceGeneration(typeof(ClassPerson))]
            internal static partial class ClassPersonSep { }
            public class ClassPerson
            {
                public required int Id { get; init; }
                public string? Name { get; set; }
                public State? State { get; set; }
                public State StateValue { get; set; }
                [SepCol(Format = "F2")]
                public decimal? Amount { get; set; }
                [SepCol(Format = "O")]
                public DateTime Created { get; set; }
            }

            [SepSourceGeneration(typeof(StructPerson))]
            internal static partial class StructPersonSep { }
            public struct StructPerson
            {
                public int Id { get; set; }
            }

            [SepSourceGeneration(typeof(RecordClassPerson))]
            internal static partial class RecordClassPersonSep { }
            public record class RecordClassPerson
            {
                public required int Id { get; init; }
            }

            [SepSourceGeneration(typeof(RecordStructPerson))]
            internal static partial class RecordStructPersonSep { }
            public record struct RecordStructPerson
            {
                public int Id { get; init; }
            }

            [SepSourceGeneration(typeof(ReadonlyRecordStructPerson))]
            internal static partial class ReadonlyRecordStructPersonSep { }
            public readonly record struct ReadonlyRecordStructPerson
            {
                public int Id { get; init; }
            }

            [SepSourceGeneration(typeof(PositionalPerson))]
            internal static partial class PositionalPersonSep { }
            public record PositionalPerson(
                [property: SepCol("id", 0)] int Id,
                [property: SepCol("name", 1)] string? Name);

            [SepSourceGeneration(typeof(ImmutablePerson))]
            internal static partial class ImmutablePersonSep { }
            public sealed class ImmutablePerson
            {
                public ImmutablePerson(int id, string name)
                {
                    Id = id;
                    Name = name;
                }

                public int Id { get; }
                public string Name { get; }
            }

            [SepSourceGeneration(typeof(FieldPerson))]
            internal static partial class FieldPersonSep { }
            public class FieldPerson
            {
                public FieldPerson(int id, string ignored = "ignored")
                {
                    Id = id;
                }

                public readonly int Id;
                public string Name;
            }

            public class Outer
            {
                public class NestedPerson
                {
                    public int Id { get; init; }
                }
            }

            [SepSourceGeneration(typeof(Outer.NestedPerson))]
            internal static partial class NestedPersonSep { }

            [SepSourceGeneration(typeof(NullableAnnotationPerson))]
            internal static partial class NullableAnnotationPersonSep { }
            public sealed class NullableAnnotationPerson
            {
                public NullableAnnotationPerson(string name)
                {
                    Name = name;
                }

                public string? Name { get; }
            }

            [SepSourceGeneration(typeof(RequiredConstructorPerson))]
            internal static partial class RequiredConstructorPersonSep { }
            public sealed class RequiredConstructorPerson
            {
                public RequiredConstructorPerson(int id)
                {
                }

                public required int Id { get; init; }
            }

            [SepSourceGeneration(typeof(SetsRequiredPerson))]
            internal static partial class SetsRequiredPersonSep { }
            public sealed class SetsRequiredPerson
            {
                [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
                public SetsRequiredPerson(int id)
                {
                    Id = id;
                }

                public required int Id { get; init; }
            }

            public sealed class NullableReference : ISpanParsable<NullableReference>, ISpanFormattable
            {
                public static NullableReference Parse(string s, IFormatProvider? provider) => new();
                public static NullableReference Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => new();
                public static bool TryParse(string? s, IFormatProvider? provider, out NullableReference result)
                {
                    result = new();
                    return true;
                }
                public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out NullableReference result)
                {
                    result = new();
                    return true;
                }
                public string ToString(string? format, IFormatProvider? formatProvider) => "";
                public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
                {
                    charsWritten = 0;
                    return true;
                }
            }

            [SepSourceGeneration(typeof(NullableReferencePerson))]
            internal static partial class NullableReferencePersonSep { }
            public sealed class NullableReferencePerson
            {
                [SepCol(Format = "X")]
                public NullableReference? Value { get; set; }
            }

            [SepSourceGeneration(typeof(RequiredFieldPerson))]
            internal static partial class RequiredFieldPersonSep { }
            public sealed class RequiredFieldPerson
            {
                public required int Id;
            }
            """);

        Assert.IsEmpty(result.Diagnostics,
            string.Join(Environment.NewLine, result.Diagnostics.Select(static diagnostic => diagnostic.ToString())));
        Assert.IsEmpty(result.CompilationDiagnostics,
            string.Join(Environment.NewLine, result.CompilationDiagnostics.Select(static diagnostic => diagnostic.ToString())));
        StringAssert.Contains(result.GeneratedSource, "global::System.Enum.Parse<global::Example.State>");
        StringAssert.Contains(result.GeneratedSource, "global::System.Enum.TryParse<global::Example.State>");
        StringAssert.Contains(result.GeneratedSource, "EnumerateAsync(global::nietras.SeparatedValues.SepReader reader)");
        StringAssert.Contains(result.GeneratedSource, "extension(global::Example.ClassPerson)");
        StringAssert.Contains(result.GeneratedSource, ".Format(value.@Created, \"O\")");
        StringAssert.Contains(result.GeneratedSource, ".Span.IsEmpty ? null");
        StringAssert.Contains(result.GeneratedSource,
            "new global::Example.RequiredConstructorPerson(@id: row[\"Id\"].Parse<int>()) { @Id = row[\"Id\"].Parse<int>() }");
        StringAssert.Contains(result.GeneratedSource,
            "new global::Example.SetsRequiredPerson(@id: row[\"Id\"].Parse<int>())");
        Assert.IsFalse(result.GeneratedSource.Contains(
            "new global::Example.SetsRequiredPerson(@id: row[\"Id\"].Parse<int>()) { @Id",
            StringComparison.Ordinal));
        StringAssert.Contains(result.GeneratedSource, "@Id = row[\"Id\"].Parse<int>()");
        StringAssert.Contains(result.GeneratedSource,
            "new global::Example.StructPerson() { @Id = row[\"Id\"].Parse<int>() }");
        StringAssert.Contains(result.GeneratedSource, "value.@Value is not null");
        Assert.IsFalse(result.GeneratedSource.Contains(".ToString(\"", StringComparison.Ordinal),
            "Generated code must format via Sep columns instead of allocating with ToString.");
    }

    [TestMethod]
    public void SepSourceGeneratorTest_AllowsCaseSensitiveColumnNames()
    {
        var result = Run("""
            [SepSourceGeneration(typeof(Person))]
            public static partial class PersonSep { }

            public class Person
            {
                [SepCol("Id")]
                public int Id { get; set; }

                [SepCol("id")]
                public int OtherId { get; set; }
            }
            """);

        Assert.IsEmpty(result.Diagnostics);
        Assert.IsEmpty(result.CompilationDiagnostics);
    }

    [TestMethod]
    public void SepSourceGeneratorTest_GeneratesDistinctHintNamesForSameNamesInDifferentNamespaces()
    {
        var result = Run("""
            namespace One
            {
                [SepSourceGeneration(typeof(Person))]
                public static partial class PersonSep { }
                public class Person { public int Id { get; set; } }
            }

            namespace Two
            {
                [SepSourceGeneration(typeof(Person))]
                public static partial class PersonSep { }
                public class Person { public int Id { get; set; } }
            }
            """);

        Assert.IsEmpty(result.Diagnostics);
        var sources = result.RunResult.Results.Single().GeneratedSources
            .Where(static source => source.HintName.EndsWith(".Sep.g.cs", StringComparison.Ordinal))
            .ToArray();
        Assert.AreEqual(2, sources.Length);
        Assert.AreNotEqual(sources[0].HintName, sources[1].HintName);
        Assert.IsTrue(sources.All(static source => source.HintName.StartsWith("PersonSep_", StringComparison.Ordinal)),
            string.Join(", ", sources.Select(static source => source.HintName)));
        Assert.IsEmpty(result.CompilationDiagnostics);
    }

    [TestMethod]
    public void SepSourceGeneratorTest_SupportsPartialAdapterDeclaredInMultipleFiles()
    {
        var result = Run(
            """
            [SepSourceGeneration(typeof(Person))]
            public static partial class PersonSep { }
            public class Person { public int Id { get; set; } }
            """,
            """
            public static partial class PersonSep
            {
                public static int Extra => 42;
            }
            """);

        Assert.IsEmpty(result.Diagnostics,
            string.Join(Environment.NewLine, result.Diagnostics.Select(static diagnostic => diagnostic.ToString())));
        Assert.IsEmpty(result.CompilationDiagnostics,
            string.Join(Environment.NewLine, result.CompilationDiagnostics.Select(static diagnostic => diagnostic.ToString())));
    }

    [TestMethod]
    public void SepSourceGeneratorTest_GeneratesConstantEnumNamesAndHonorsExplicitFormat()
    {
        var result = Run("""
            public enum State { Unknown, Ready, Alias = Ready }
            public enum Empty { }

            [SepSourceGeneration(typeof(Person))]
            public static partial class PersonSep { }
            public class Person
            {
                public State Value { get; set; }
                public State? Optional { get; set; }
                [SepCol(Format = "D")]
                public State Numeric { get; set; }
                [SepCol(Format = "g")]
                public State General { get; set; }
                public Empty Nothing { get; set; }
            }
            """);

        Assert.IsEmpty(result.Diagnostics,
            string.Join(Environment.NewLine, result.Diagnostics.Select(static diagnostic => diagnostic.ToString())));
        Assert.IsEmpty(result.CompilationDiagnostics,
            string.Join(Environment.NewLine, result.CompilationDiagnostics.Select(static diagnostic => diagnostic.ToString())));
        StringAssert.Contains(result.GeneratedSource, "case global::State.@Unknown:");
        StringAssert.Contains(result.GeneratedSource, "__col0.Set(global::System.MemoryExtensions.AsSpan(\"Unknown\"));");
        StringAssert.Contains(result.GeneratedSource, "SetEnum(__col0, value.@Value, default);");
        StringAssert.Contains(result.GeneratedSource, "switch (value.@Optional.GetValueOrDefault())");
        StringAssert.Contains(result.GeneratedSource, "SetEnum(__col2, value.@Numeric, \"D\");");
        StringAssert.Contains(result.GeneratedSource, "switch (value.@General)");
        // An enum without any members has no constant names to switch on.
        StringAssert.Contains(result.GeneratedSource, "SetEnum(__col4, value.@Nothing, default);");
        Assert.IsFalse(result.GeneratedSource.Contains("switch (value.@Nothing)", StringComparison.Ordinal));
        StringAssert.Contains(result.GeneratedSource, "global::System.Enum.TryFormat(value, chars, out var charsWritten, format)");
        // Aliases share the value of the member they alias and would be duplicate switch labels.
        Assert.IsFalse(result.GeneratedSource.Contains("@Alias", StringComparison.Ordinal));
        Assert.IsFalse(result.GeneratedSource.Contains("ToString", StringComparison.Ordinal),
            "Generated code must format via Sep columns instead of allocating with ToString.");
    }

    [TestMethod]
    public void SepSourceGeneratorTest_ReadsEachColumnOnlyOnceForNullableMembers()
    {
        var result = Run("""
            [SepSourceGeneration(typeof(Person))]
            public static partial class PersonSep { }
            public class Person
            {
                public int? Id { get; set; }
                public string? Name { get; set; }
            }
            """);

        Assert.IsEmpty(result.Diagnostics);
        Assert.IsEmpty(result.CompilationDiagnostics,
            string.Join(Environment.NewLine, result.CompilationDiagnostics.Select(static diagnostic => diagnostic.ToString())));
        StringAssert.Contains(result.GeneratedSource, "var __col0 = row[\"Id\"];");
        StringAssert.Contains(result.GeneratedSource, "int? __sep0 = __col0.Span.IsEmpty ? null : __col0.Parse<int>();");
        StringAssert.Contains(result.GeneratedSource, "__sep1 = __col1.ToString();");
        Assert.AreEqual(0, CountOccurrences(result.GeneratedSource, "row[\"Id\"].Span.IsEmpty"));
        // One column lookup per nullable member in each of Read, TryRead and Write.
        Assert.AreEqual(3, CountOccurrences(result.GeneratedSource, "= row[\"Id\"];"));
    }

    static int CountOccurrences(string source, string value)
    {
        var count = 0;
        for (var index = source.IndexOf(value, StringComparison.Ordinal); index >= 0;
             index = source.IndexOf(value, index + value.Length, StringComparison.Ordinal))
        {
            ++count;
        }
        return count;
    }

    [TestMethod]
    public void SepSourceGeneratorTest_EscapesKeywordNamespacesAndIdentifiers()
    {
        var result = Run("""
            namespace @class;

            [SepSourceGeneration(typeof(@record))]
            public static partial class @static { }

            public class @record
            {
                public int @event { get; set; }
            }
            """);

        Assert.IsEmpty(result.Diagnostics);
        Assert.IsEmpty(result.CompilationDiagnostics);
        StringAssert.Contains(result.GeneratedSource, "namespace @class;");
        StringAssert.Contains(result.GeneratedSource, "partial class @static");
        StringAssert.Contains(result.GeneratedSource, "@event");
    }

    [TestMethod]
    public void SepSourceGeneratorTest_RejectsFileLocalAdapter()
    {
        var result = Run("""
            [SepSourceGeneration(typeof(Person))]
            file static partial class PersonSep { }

            public class Person
            {
                public int Id { get; set; }
            }
            """);

        var diagnostic = result.Diagnostics.Single(static diagnostic => diagnostic.Id == "SEPGEN001");
        StringAssert.Contains(diagnostic.GetMessage(), "non-file-local");
    }

    [TestMethod]
    public void SepSourceGeneratorTest_SupportsProtectedInternalAccessors()
    {
        var result = Run("""
            [SepSourceGeneration(typeof(Person))]
            public static partial class PersonSep { }

            public class Person
            {
                public int ReadValue { get; protected internal set; }
                public int WriteValue { protected internal get; set; }
            }
            """);

        Assert.IsEmpty(result.Diagnostics);
        Assert.IsEmpty(result.CompilationDiagnostics,
            string.Join(Environment.NewLine, result.CompilationDiagnostics.Select(static diagnostic => diagnostic.ToString())));
    }

    [TestMethod]
    public void SepSourceGeneratorTest_UpdatesSameIdDiagnosticAcrossIncrementalEdits()
    {
        var invalid = """
            [SepSourceGeneration(typeof(Person))]
            public static partial class PersonSep { }
            public class Person
            {
                [SepCol("")]
                public int Id { get; set; }
            }
            """;
        var compilation = CreateCompilation(invalid);
        GeneratorDriver driver = CreateDriver();
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

        var edited = invalid.Replace("Id { get; set; }", "Value { get; set; }", StringComparison.Ordinal)
            .Replace("[SepCol(\"\")]", "[SepCol(\" \")]", StringComparison.Ordinal);
        driver = driver.RunGeneratorsAndUpdateCompilation(CreateCompilation(edited), out _, out _);
        var diagnostic = driver.GetRunResult().Diagnostics.Single(static diagnostic => diagnostic.Id == "SEPGEN005");

        StringAssert.Contains(diagnostic.GetMessage(), "Value");
        Assert.IsFalse(diagnostic.GetMessage().Contains("Id", StringComparison.Ordinal));
    }

    [TestMethod]
    public void SepSourceGeneratorTest_ValueModelsUseCompleteEquality()
    {
        var member = new SepSourceGenerator.Member("Id", "int", "int", false, false, false, true, false, 0)
            .WithMapping(new SepSourceGenerator.ColumnMapping("id", 0, null));
        var equalMember = new SepSourceGenerator.Member("Id", "int", "int", false, false, false, true, false, 0)
            .WithMapping(new SepSourceGenerator.ColumnMapping("id", 0, null));
        var differentMember = new SepSourceGenerator.Member("Name", "string", "string", true, false, true, true, true, 1)
            .WithMapping(new SepSourceGenerator.ColumnMapping("name", 1, "G"));
        var plan = new SepSourceGenerator.ConstructionPlan(
            ImmutableArray.Create(new SepSourceGenerator.ConstructorParameter("@id", 0)),
            ImmutableArray<int>.Empty);
        var equalPlan = new SepSourceGenerator.ConstructionPlan(
            ImmutableArray.Create(new SepSourceGenerator.ConstructorParameter("@id", 0)),
            ImmutableArray<int>.Empty);
        var model = new SepSourceGenerator.Model(
            "Test", "public", "@PersonSep", "global::Test.PersonSep", "global::Test.Person",
            ImmutableArray.Create(member), plan, usesIndexes: true);
        var equalModel = new SepSourceGenerator.Model(
            "Test", "public", "@PersonSep", "global::Test.PersonSep", "global::Test.Person",
            ImmutableArray.Create(equalMember), equalPlan, usesIndexes: true);
        var differentModel = new SepSourceGenerator.Model(
            "Test", "internal", "@PersonSep", "global::Test.PersonSep", "global::Test.Person",
            ImmutableArray.Create(differentMember), SepSourceGenerator.ConstructionPlan.Empty, usesIndexes: false);
        var differentMemberModel = new SepSourceGenerator.Model(
            "Test", "public", "@PersonSep", "global::Test.PersonSep", "global::Test.Person",
            ImmutableArray.Create(differentMember), plan, usesIndexes: true);
        var differentParameterPlan = new SepSourceGenerator.ConstructionPlan(
            ImmutableArray.Create(new SepSourceGenerator.ConstructorParameter("@other", 0)),
            ImmutableArray<int>.Empty);
        var differentInitializerPlan = new SepSourceGenerator.ConstructionPlan(
            ImmutableArray.Create(new SepSourceGenerator.ConstructorParameter("@id", 0)),
            ImmutableArray.Create(1));
        var anotherInitializerPlan = new SepSourceGenerator.ConstructionPlan(
            ImmutableArray.Create(new SepSourceGenerator.ConstructorParameter("@id", 0)),
            ImmutableArray.Create(2));
        var issue = SepSourceGenerator.Issue.Create(SepSourceGenerator.IssueId.InvalidColumn, Location.None, "Id", "name");

        var enumMember = CreateEnumMember("Ready");
        Assert.IsTrue(enumMember.Equals(CreateEnumMember("Ready")));
        Assert.IsFalse(enumMember.Equals(CreateEnumMember("Other")));
        Assert.AreEqual(enumMember.GetHashCode(), CreateEnumMember("Ready").GetHashCode());
        Assert.IsTrue(member.Equals(equalMember));
        Assert.IsFalse(member.Equals(differentMember));
        Assert.IsFalse(member.Equals(new object()));
        Assert.AreEqual(member.GetHashCode(), equalMember.GetHashCode());
        Assert.IsTrue(plan.Equals(equalPlan));
        Assert.IsFalse(plan.Equals(SepSourceGenerator.ConstructionPlan.Empty));
        Assert.IsFalse(plan.Equals(differentParameterPlan));
        Assert.IsFalse(differentInitializerPlan.Equals(anotherInitializerPlan));
        Assert.IsTrue(differentInitializerPlan.Equals(new SepSourceGenerator.ConstructionPlan(
            ImmutableArray.Create(new SepSourceGenerator.ConstructorParameter("@id", 0)),
            ImmutableArray.Create(1))));
        Assert.IsFalse(plan.Equals(new object()));
        Assert.AreNotEqual(plan.GetHashCode(), differentInitializerPlan.GetHashCode());
        Assert.IsTrue(model.Equals(equalModel));
        Assert.IsFalse(model.Equals(differentModel));
        Assert.IsFalse(model.Equals(differentMemberModel));
        Assert.IsFalse(model.Equals(new object()));
        Assert.AreEqual(model.GetHashCode(), equalModel.GetHashCode());
        Assert.IsTrue(issue.Equals(SepSourceGenerator.Issue.Create(SepSourceGenerator.IssueId.InvalidColumn, Location.None, "Id", "name")));
        Assert.IsFalse(issue.Equals(SepSourceGenerator.Issue.Create(SepSourceGenerator.IssueId.InvalidColumn, Location.None, "Name", "name")));
        Assert.IsFalse(issue.Equals(SepSourceGenerator.Issue.Create(SepSourceGenerator.IssueId.NoMembers, Location.None, "Id", "name")));
        Assert.IsFalse(issue.Equals(new object()));
        Assert.AreNotEqual(issue.GetHashCode(), SepSourceGenerator.Issue.Create(SepSourceGenerator.IssueId.InvalidColumn, Location.None, "Name", "name").GetHashCode());
        var location = Location.Create("Test.cs", new TextSpan(1, 2), new LinePositionSpan(new(0, 1), new(0, 3)));
        var locatedIssue = SepSourceGenerator.Issue.Create(SepSourceGenerator.IssueId.InvalidColumn, location, "Id", "name");
        var otherLocation = Location.Create("Other.cs", new TextSpan(1, 2), new LinePositionSpan(new(0, 1), new(0, 3)));
        Assert.IsTrue(locatedIssue.Equals(SepSourceGenerator.Issue.Create(SepSourceGenerator.IssueId.InvalidColumn, location, "Id", "name")));
        Assert.IsFalse(locatedIssue.Equals(issue));
        Assert.IsFalse(locatedIssue.Equals(SepSourceGenerator.Issue.Create(SepSourceGenerator.IssueId.InvalidColumn, otherLocation, "Id", "name")));
        Assert.AreEqual(
            locatedIssue.GetHashCode(),
            SepSourceGenerator.Issue.Create(SepSourceGenerator.IssueId.InvalidColumn, location, "Id", "name").GetHashCode());
        var locationInfo = SepSourceGenerator.LocationInfo.Create(location)!;
        Assert.IsFalse(locationInfo.Equals(new object()));
        Assert.IsNull(SepSourceGenerator.LocationInfo.Create(Location.None));
        Assert.AreEqual(location.GetLineSpan(), locationInfo.ToLocation().GetLineSpan());
        var parameter = new SepSourceGenerator.ConstructorParameter("@id", 0);
        Assert.IsTrue(parameter.Equals(new SepSourceGenerator.ConstructorParameter("@id", 0)));
        Assert.IsFalse(parameter.Equals(new object()));
        Assert.AreNotEqual(parameter.GetHashCode(), new SepSourceGenerator.ConstructorParameter("@name", 0).GetHashCode());
    }

    [TestMethod]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public class PersonSep { }
        public class Person { public int Id { get; set; } }
        """, "SEPGEN001")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public abstract class Person { public int Id { get; set; } }
        """, "SEPGEN002")]
    [DataRow("""
        [SepSourceGeneration(null)]
        public static partial class PersonSep { }
        """, "SEPGEN002")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person { public object Value { get; set; } = new(); }
        """, "SEPGEN003")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person { }
        """, "SEPGEN004")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person { [SepCol("")] public int Id { get; set; } }
        """, "SEPGEN005")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person { [SepCol(Index = -1)] public int Id { get; set; } }
        """, "SEPGEN005")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person
        {
            [SepCol("a")]
            [SepCol("b")]
            public int Id { get; set; }
        }
        """, "SEPGEN005")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person { public int this[int index] { get => index; set { } } }
        """, "SEPGEN003")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person { [SepCol(Format = "X")] public string Name { get; set; } = ""; }
        """, "SEPGEN005")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person { public int Id { private get; private set; } }
        """, "SEPGEN003")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person { public int Id { set { } } }
        """, "SEPGEN003")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person
        {
            [SepCol("id")] public int Id { get; set; }
            [SepCol("id")] public int OtherId { get; set; }
        }
        """, "SEPGEN006")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person
        {
            [SepCol(0)] public int Id { get; set; }
            [SepCol(0)] public int OtherId { get; set; }
        }
        """, "SEPGEN006")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person
        {
            [SepCol(0)] public int Id { get; set; }
            public int OtherId { get; set; }
        }
        """, "SEPGEN007")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person
        {
            private Person() { }
            public int Id { get; set; }
        }
        """, "SEPGEN008")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person
        {
            public Person(int id) { }
            public Person(string name) { }
            public int Id { get; set; }
            public string Name { get; set; } = "";
        }
        """, "SEPGEN009")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person
        {
            public Person() { }
            public int Id { get; }
        }
        """, "SEPGEN012")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person
        {
            public Person(int id, int ID) { }
            public int Id { get; set; }
        }
        """, "SEPGEN010")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person))]
        public static partial class PersonSep { }
        public class Person
        {
            public Person(string name) { }
            public int Id { get; }
        }
        """, "SEPGEN012")]
    [DataRow("""
        [SepSourceGeneration(typeof(Person<>))]
        public static partial class PersonSep { }
        public class Person<T> { public int Id { get; set; } }
        """, "SEPGEN011")]
    [DataRow("""
        [SepSourceGeneration(typeof(Outer<int>.Person))]
        public static partial class PersonSep { }
        public class Outer<T>
        {
            public class Person { public int Id { get; set; } }
        }
        """, "SEPGEN011")]
    public void SepSourceGeneratorTest_ReportsStableDiagnostic(string source, string diagnosticId)
    {
        var result = Run(source);
        Assert.IsTrue(result.Diagnostics.Any(diagnostic => diagnostic.Id == diagnosticId),
            $"Expected {diagnosticId}, actual: {string.Join(", ", result.Diagnostics.Select(static diagnostic => diagnostic.Id))}");
    }

    [TestMethod]
    public void SepSourceGeneratorTest_RejectsISpanParsableForAnotherType()
    {
        var result = Run("""
            [SepSourceGeneration(typeof(Person))]
            public static partial class PersonSep { }

            public readonly struct Value : ISpanParsable<int>, ISpanFormattable
            {
                public static int Parse(string s, IFormatProvider? provider) => 0;
                public static int Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => 0;
                public static bool TryParse(string? s, IFormatProvider? provider, out int result)
                {
                    result = 0;
                    return true;
                }
                public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out int result)
                {
                    result = 0;
                    return true;
                }
                public string ToString(string? format, IFormatProvider? formatProvider) => "";
                public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
                {
                    charsWritten = 0;
                    return true;
                }
            }

            public class Person
            {
                public Value Value { get; set; }
            }
            """);

        Assert.IsTrue(result.Diagnostics.Any(static diagnostic => diagnostic.Id == "SEPGEN003"));
    }

    [TestMethod]
    public void SepSourceGeneratorTest_HashCodesDistinguishNullFromEmptyAndOrder()
    {
        // The analyzer ships as netstandard2.0 where System.HashCode does not exist and a polyfill
        // is used instead. These are the properties the incremental caching relies on.
        var withNullFormat = CreateEnumMember("Ready");
        var withEmptyFormat = CreateEnumMember("Ready").WithMapping(new SepSourceGenerator.ColumnMapping("c", null, ""));
        var reordered = new SepSourceGenerator.ConstructionPlan(
            ImmutableArray.Create(
                new SepSourceGenerator.ConstructorParameter("@a", 0),
                new SepSourceGenerator.ConstructorParameter("@b", 1)),
            ImmutableArray<int>.Empty);
        var swapped = new SepSourceGenerator.ConstructionPlan(
            ImmutableArray.Create(
                new SepSourceGenerator.ConstructorParameter("@b", 1),
                new SepSourceGenerator.ConstructorParameter("@a", 0)),
            ImmutableArray<int>.Empty);

        Assert.AreNotEqual(withNullFormat.GetHashCode(), withEmptyFormat.GetHashCode());
        Assert.AreNotEqual(reordered.GetHashCode(), swapped.GetHashCode());
        Assert.AreEqual(reordered.GetHashCode(), new SepSourceGenerator.ConstructionPlan(
            ImmutableArray.Create(
                new SepSourceGenerator.ConstructorParameter("@a", 0),
                new SepSourceGenerator.ConstructorParameter("@b", 1)),
            ImmutableArray<int>.Empty).GetHashCode());
        Assert.AreEqual(
            SepSourceGenerator.ConstructionPlan.Empty.GetHashCode(),
            new SepSourceGenerator.ConstructionPlan(
                ImmutableArray<SepSourceGenerator.ConstructorParameter>.Empty,
                ImmutableArray<int>.Empty).GetHashCode());
    }

    [TestMethod]
    [DataRow("array", "Person.Write(writer, array);")]
    [DataRow("list", "Person.Write(writer, list);")]
    [DataRow("sequence", "Person.Write(writer, sequence);")]
    [DataRow("span", "Person.Write(writer, new Person[0].AsSpan());")]
    [DataRow("collection expression", "Person.Write(writer, [new Person()]);")]
    public void SepSourceGeneratorTest_WriteOverloadsAreNotAmbiguous(string description, string call)
    {
        // The span overload exists to avoid an enumerator allocation, but it must not make ordinary
        // calls with an array, a list or a sequence ambiguous against the IEnumerable overload.
        var result = Run($$"""
            using System.Collections.Generic;

            [SepSourceGeneration(typeof(Person))]
            public static partial class PersonSep { }
            public class Person { public int Id { get; set; } }

            public static class Caller
            {
                public static void Call(SepWriter writer, Person[] array, List<Person> list, IEnumerable<Person> sequence)
                {
                    {{call}}
                }
            }
            """);

        Assert.IsEmpty(result.Diagnostics);
        Assert.IsEmpty(result.CompilationDiagnostics,
            $"{description}: " +
            string.Join(Environment.NewLine, result.CompilationDiagnostics.Select(static diagnostic => diagnostic.ToString())));
    }

    [TestMethod]
    public void SepSourceGeneratorTest_RequiresCSharp14()
    {
        var result = Run(LanguageVersion.CSharp13, """
            [SepSourceGeneration(typeof(Person))]
            public static partial class PersonSepExtensions { }
            public class Person { public int Id { get; set; } }
            """);

        Assert.AreEqual("SEPGEN013", result.Diagnostics.Single().Id);
        Assert.IsEmpty(result.GeneratedSource);
    }

    static SepSourceGenerator.Member CreateEnumMember(string enumMemberName) =>
        new("State", "global::State", "global::State", "global::State",
            isString: false, isEnum: true, isNullable: false, isNullableValue: false,
            ImmutableArray.Create(enumMemberName), canWrite: true, isProperty: true, isRequired: false, order: 2);

    static DriverResult Run(params string[] sources) => Run(LanguageVersion.Preview, sources);

    static DriverResult Run(LanguageVersion languageVersion, params string[] sources)
    {
        GeneratorDriver driver = CreateDriver(languageVersion);
        driver = driver.RunGeneratorsAndUpdateCompilation(
            CreateCompilation(languageVersion, sources),
            out var outputCompilation,
            out _);
        var runResult = driver.GetRunResult();
        var generatedSource = string.Concat(runResult.Results.Single().GeneratedSources
            .Where(static source => source.HintName.EndsWith(".Sep.g.cs", StringComparison.Ordinal))
            .Select(static source => source.SourceText.ToString()));
        return new(
            runResult,
            runResult.Diagnostics,
            outputCompilation.GetDiagnostics().Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToImmutableArray(),
            generatedSource);
    }

    static CSharpCompilation CreateCompilation(params string[] sources) =>
        CreateCompilation(LanguageVersion.Preview, sources);

    static CSharpCompilation CreateCompilation(LanguageVersion languageVersion, params string[] sources) =>
        CSharpCompilation.Create(
            "SepSourceGeneratorTest",
            sources.Select(source => CSharpSyntaxTree.ParseText(
                SourcePrefix + source, CSharpParseOptions.Default.WithLanguageVersion(languageVersion))),
            s_references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

    static GeneratorDriver CreateDriver() => CreateDriver(LanguageVersion.Preview);

    static GeneratorDriver CreateDriver(LanguageVersion languageVersion) =>
        CSharpGeneratorDriver.Create(
            [new SepSourceGenerator().AsSourceGenerator()],
            parseOptions: CSharpParseOptions.Default.WithLanguageVersion(languageVersion));

    static ImmutableArray<MetadataReference> CreateReferences()
    {
        var trustedPlatformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?? throw new InvalidOperationException("Trusted platform assemblies are unavailable.");
        return trustedPlatformAssemblies.Split(Path.PathSeparator)
            .Select(static path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .Append((MetadataReference)MetadataReference.CreateFromFile(typeof(Sep).Assembly.Location))
            .ToImmutableArray();
    }

    readonly record struct DriverResult(
        GeneratorDriverRunResult RunResult,
        ImmutableArray<Diagnostic> Diagnostics,
        ImmutableArray<Diagnostic> CompilationDiagnostics,
        string GeneratedSource);
}
