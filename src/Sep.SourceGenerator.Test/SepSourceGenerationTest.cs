using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.SourceGenerator.Test;

[TestClass]
public class SepSourceGenerationTest
{
    static readonly string s_personCsv =
        "Id;Name;Score\r\n42;Ada;12.5\r\n7;Lin;9.25\r\n".ReplaceLineEndings();
    static readonly string s_singlePersonCsv =
        "Id;Name;Score\r\n42;Ada;12.5\r\n".ReplaceLineEndings();
    static readonly Person s_ada = new(42, "Ada", 12.5m);
    static readonly Person[] s_people = [s_ada, new(7, "Lin", 9.25m)];

    [TestMethod]
    public void SepSourceGenerationTest_Write_UsesGeneratedSpanConversions()
    {
        using var writer = Sep.Writer().ToText();
        Person.Write(writer, s_people);

        Assert.AreEqual(s_personCsv, writer.ToString());
    }

    [TestMethod]
    public void SepSourceGenerationTest_Enumerate_UsesGeneratedSpanConversions()
    {
        using var reader = Sep.Reader().FromText(s_personCsv);

        Assert.AreSequenceEqual(s_people, Person.Enumerate(reader).ToArray());
    }

    [TestMethod]
    public void SepSourceGenerationTest_Parse_ReturnsRecord()
    {
        using var reader = Sep.Reader().FromText(s_singlePersonCsv);
        Assert.IsTrue(reader.MoveNext());

        Assert.AreEqual(s_ada, Person.Parse(reader.Current));
    }

    [TestMethod]
    public void SepSourceGenerationTest_TryParse_ReturnsRecord()
    {
        using var reader = Sep.Reader().FromText(s_singlePersonCsv);
        Assert.IsTrue(reader.MoveNext());

        Assert.IsTrue(Person.TryParse(reader.Current, out var actual));
        Assert.AreEqual(s_ada, actual);
    }

    [TestMethod]
    public void SepSourceGenerationTest_TryParse_ReturnsFalseForInvalidSpanParsableValue()
    {
        using var reader = Sep.Reader().FromText("Id;Name;Score\r\nnot-an-int;Ada;12.5\r\n");
        Assert.IsTrue(reader.MoveNext());

        Assert.IsFalse(Person.TryParse(reader.Current, out var person));
        Assert.IsNull(person);
    }

    [TestMethod]
    public void SepSourceGenerationTest_Format_WritesSingleRow()
    {
        using var writer = Sep.Writer().ToText();
        using (var row = writer.NewRow())
        {
            Person.Format(row, s_ada);
        }

        Assert.AreEqual(s_singlePersonCsv, writer.ToString());
    }

    [TestMethod]
    public async Task SepSourceGenerationTest_WriteAsync_Enumerable()
    {
        await using var writer = Sep.Writer().ToText();

        await Person.WriteAsync(writer, (IEnumerable<Person>)s_people);

        Assert.AreEqual(s_personCsv, writer.ToString());
    }

    [TestMethod]
    public async Task SepSourceGenerationTest_WriteAsync_AsyncEnumerable()
    {
        await using var writer = Sep.Writer().ToText();

        await Person.WriteAsync(writer, AsAsyncEnumerable(s_people));

        Assert.AreEqual(s_personCsv, writer.ToString());
    }

    [TestMethod]
    public async Task SepSourceGenerationTest_EnumerateAsync_ReturnsRecords()
    {
        using var reader = Sep.Reader().FromText(s_personCsv);
        var actual = new List<Person>();
        await foreach (var person in Person.EnumerateAsync(reader))
        {
            actual.Add(person);
        }

        Assert.AreSequenceEqual(s_people, actual);
    }

    [TestMethod]
    public async Task SepSourceGenerationTest_WriteAsync_PropagatesCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await using var writer = Sep.Writer().ToText();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(async () =>
            await Person.WriteAsync(writer, AsAsyncEnumerable(s_people), cancellation.Token));
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_Name()
    {
        var expected = new NamedPerson { Id = 42, Name = "Ada" };
        using var reader = Sep.Reader().FromText("display_name;person_id\r\nAda;42\r\n");
        var people = NamedPerson.Enumerate(reader).ToArray();

        Assert.AreSequenceEqual(new[] { expected }, people);

        using var writer = Sep.Writer().ToText();
        NamedPerson.Write(writer, people);
        Assert.AreEqual($"person_id;display_name{Environment.NewLine}42;Ada{Environment.NewLine}", writer.ToString());
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_IndexAndName()
    {
        var expected = new IndexedPerson { Id = 42, Name = "Ada", Score = 12.5m };
        var options = Sep.Reader(o => o with { HasHeader = false });
        using var reader = options.FromText("Ada;12.5;42\r\n");
        var people = IndexedPerson.Enumerate(reader).ToArray();

        Assert.AreSequenceEqual(new[] { expected }, people);

        using var writer = Sep.Writer().ToText();
        IndexedPerson.Write(writer, people);
        Assert.AreEqual($"full_name;Score;person_id{Environment.NewLine}Ada;12.5;42{Environment.NewLine}", writer.ToString());
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_NamesAcceptsAlternateHeader()
    {
        using var reader = Sep.Reader().FromText("legacy_id;Name\r\n42;Ada\r\n");

        Assert.AreEqual(new EvolvingPerson(42, "Ada", 7), EvolvingPerson.Enumerate(reader).Single());
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_NameUsesReaderHeaderComparer()
    {
        using var reader = Sep.Reader(options => options with
        {
            ColNameComparer = StringComparer.OrdinalIgnoreCase,
        }).FromText("id;name;version\r\n42;Ada;7\r\n");

        Assert.AreEqual(new EvolvingPerson(42, "Ada", 7), EvolvingPerson.Enumerate(reader).Single());
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_NameUsesReaderHeaderTrimming()
    {
        using var reader = Sep.Reader(options => options with
        {
            Trim = SepTrim.Outer,
        }).FromText(" Id ; Name ; Version \r\n42;Ada;7\r\n");

        Assert.AreEqual(new EvolvingPerson(42, "Ada", 7), EvolvingPerson.Enumerate(reader).Single());
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_OptionalUsesDefaultValue()
    {
        using var reader = Sep.Reader().FromText("Id;Name\r\n42;Ada\r\n");

        Assert.AreEqual(new EvolvingPerson(42, "Ada", 7), EvolvingPerson.Enumerate(reader).Single());
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_OptionalTryParseUsesDefaultValue()
    {
        using var reader = Sep.Reader().FromText("Id;Name\r\n42;Ada\r\n");
        Assert.IsTrue(reader.MoveNext());

        Assert.IsTrue(EvolvingPerson.TryParse(reader.Current, out var actual));
        Assert.AreEqual(new EvolvingPerson(42, "Ada", 7), actual);
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_IgnoreExcludesMember()
    {
        var expected = new EvolvingPerson(42, "Ada", 7);
        using var writer = Sep.Writer().ToText();

        EvolvingPerson.Write(writer, [expected]);

        Assert.AreEqual($"Id;Name;Version{Environment.NewLine}42;Ada;7{Environment.NewLine}", writer.ToString());
    }

    [TestMethod]
    public void SepSourceGenerationTest_ConventionParsesCustomType()
    {
        using var reader = Sep.Reader().FromText("Id\r\n42\r\n");

        Assert.AreEqual(new ConvertedPerson(new StrongId(42)), ConvertedPerson.Enumerate(reader).Single());
    }

    [TestMethod]
    public void SepSourceGenerationTest_ConventionFormatsCustomType()
    {
        using var writer = Sep.Writer().ToText();

        ConvertedPerson.Write(writer, [new(new StrongId(42))]);

        Assert.AreEqual($"Id{Environment.NewLine}42{Environment.NewLine}", writer.ToString());
    }

    [TestMethod]
    public void SepSourceGenerationTest_ConventionTryParseReturnsFalse()
    {
        using var reader = Sep.Reader().FromText("Id\r\ninvalid\r\n");
        Assert.IsTrue(reader.MoveNext());

        Assert.IsFalse(ConvertedPerson.TryParse(reader.Current, out _));
    }

    [TestMethod]
    public void SepSourceGenerationTest_TryParseOnlyConventionThrowsFromParse()
    {
        using var reader = Sep.Reader().FromText("Id\r\ninvalid\r\n");
        Assert.IsTrue(reader.MoveNext());

        Assert.ThrowsExactly<FormatException>(() => TryOnlyConventionPerson.Parse(reader.Current));
    }

    [TestMethod]
    public void SepSourceGenerationTest_RowConventionControlsColumnAccessAndMissingColumns()
    {
        using var reader = Sep.Reader().FromText("RawId\r\n41\r\n");

        Assert.AreEqual(new RowConventionPerson(new StrongId(42)), RowConventionPerson.Enumerate(reader).Single());

        using var writer = Sep.Writer().ToText();
        RowConventionPerson.Write(writer, [new(new StrongId(42))]);
        Assert.AreEqual($"RawId{Environment.NewLine}41{Environment.NewLine}", writer.ToString());
    }

    [TestMethod]
    public void SepSourceGenerationTest_RowConventionTryParseReturnsFalse()
    {
        using var reader = Sep.Reader().FromText("RawId\r\ninvalid\r\n");
        Assert.IsTrue(reader.MoveNext());

        Assert.IsFalse(RowConventionPerson.TryParse(reader.Current, out _));
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_PrefixParsesNestedRecord()
    {
        using var reader = Sep.Reader().FromText("Id;Address.Street;Address.City\r\n42;Main;Aarhus\r\n");

        Assert.AreEqual(
            new NestedPerson(42, new Address("Main", "Aarhus")),
            NestedPerson.Enumerate(reader).Single());
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_PrefixFormatsNestedRecord()
    {
        using var writer = Sep.Writer().ToText();

        NestedPerson.Write(writer, [new(42, new("Main", "Aarhus"))]);

        Assert.AreEqual(
            $"Id;Address.Street;Address.City{Environment.NewLine}42;Main;Aarhus{Environment.NewLine}",
            writer.ToString());
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_PrefixTryParsesNestedRecord()
    {
        using var reader = Sep.Reader().FromText("Id;Address.Street;Address.City\r\n42;Main;Aarhus\r\n");
        Assert.IsTrue(reader.MoveNext());

        Assert.IsTrue(NestedPerson.TryParse(reader.Current, out var actual));
        Assert.AreEqual(new NestedPerson(42, new Address("Main", "Aarhus")), actual);
    }

    static async IAsyncEnumerable<Person> AsAsyncEnumerable(
        IEnumerable<Person> values,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var value in values)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return value;
            await Task.Yield();
        }
    }
}
