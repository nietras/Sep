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
    const string PersonCsv = "Id;Name;Score\r\n42;Ada;12.5\r\n7;Lin;9.25\r\n";
    const string SinglePersonCsv = "Id;Name;Score\r\n42;Ada;12.5\r\n";
    static readonly Person s_ada = new(42, "Ada", 12.5m);
    static readonly Person[] s_people = [s_ada, new(7, "Lin", 9.25m)];

    [TestMethod]
    public void SepSourceGenerationTest_Write_UsesGeneratedSpanConversions()
    {
        using var writer = Sep.Default.Writer().ToText();
        Person.Write(writer, s_people);

        Assert.AreEqual(PersonCsv, writer.ToString());
    }

    [TestMethod]
    public void SepSourceGenerationTest_Enumerate_UsesGeneratedSpanConversions()
    {
        using var reader = Sep.Default.Reader().FromText(PersonCsv);

        Assert.AreSequenceEqual(s_people, Person.Enumerate(reader).ToArray());
    }

    [TestMethod]
    public void SepSourceGenerationTest_Parse_ReturnsRecord()
    {
        using var reader = Sep.Default.Reader().FromText(SinglePersonCsv);
        Assert.IsTrue(reader.MoveNext());

        Assert.AreEqual(s_ada, Person.Parse(reader.Current));
    }

    [TestMethod]
    public void SepSourceGenerationTest_TryParse_ReturnsRecord()
    {
        using var reader = Sep.Default.Reader().FromText(SinglePersonCsv);
        Assert.IsTrue(reader.MoveNext());

        Assert.IsTrue(Person.TryParse(reader.Current, out var actual));
        Assert.AreEqual(s_ada, actual);
    }

    [TestMethod]
    public void SepSourceGenerationTest_TryParse_ReturnsFalseForInvalidSpanParsableValue()
    {
        using var reader = Sep.Default.Reader().FromText("Id;Name;Score\r\nnot-an-int;Ada;12.5\r\n");
        Assert.IsTrue(reader.MoveNext());

        Assert.IsFalse(Person.TryParse(reader.Current, out var person));
        Assert.IsNull(person);
    }

    [TestMethod]
    public void SepSourceGenerationTest_Format_WritesSingleRow()
    {
        using var writer = Sep.Default.Writer().ToText();
        using (var row = writer.NewRow())
        {
            Person.Format(row, s_ada);
        }

        Assert.AreEqual(SinglePersonCsv, writer.ToString());
    }

    [TestMethod]
    public async Task SepSourceGenerationTest_WriteAsync_Enumerable()
    {
        await using var writer = Sep.Default.Writer().ToText();

        await Person.WriteAsync(writer, (IEnumerable<Person>)s_people);

        Assert.AreEqual(PersonCsv, writer.ToString());
    }

    [TestMethod]
    public async Task SepSourceGenerationTest_WriteAsync_AsyncEnumerable()
    {
        await using var writer = Sep.Default.Writer().ToText();

        await Person.WriteAsync(writer, AsAsyncEnumerable(s_people));

        Assert.AreEqual(PersonCsv, writer.ToString());
    }

    [TestMethod]
    public async Task SepSourceGenerationTest_EnumerateAsync_ReturnsRecords()
    {
        using var reader = Sep.Default.Reader().FromText(PersonCsv);
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
        await using var writer = Sep.Default.Writer().ToText();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(async () =>
            await Person.WriteAsync(writer, AsAsyncEnumerable(s_people), cancellation.Token));
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_Name()
    {
        var expected = new NamedPerson { Id = 42, Name = "Ada" };
        using var reader = Sep.Default.Reader().FromText("display_name;person_id\r\nAda;42\r\n");
        var people = NamedPerson.Enumerate(reader).ToArray();

        Assert.AreSequenceEqual(new[] { expected }, people);

        using var writer = Sep.Default.Writer().ToText();
        NamedPerson.Write(writer, people);
        Assert.AreEqual($"person_id;display_name{Environment.NewLine}42;Ada{Environment.NewLine}", writer.ToString());
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_IndexAndName()
    {
        var expected = new IndexedPerson { Id = 42, Name = "Ada", Score = 12.5m };
        var options = Sep.Default.Reader(o => o with { HasHeader = false });
        using var reader = options.FromText("Ada;12.5;42\r\n");
        var people = IndexedPerson.Enumerate(reader).ToArray();

        Assert.AreSequenceEqual(new[] { expected }, people);

        using var writer = Sep.Default.Writer().ToText();
        IndexedPerson.Write(writer, people);
        Assert.AreEqual($"full_name;Score;person_id{Environment.NewLine}Ada;12.5;42{Environment.NewLine}", writer.ToString());
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

[SepSourceGeneration(typeof(Person))]
public static partial class PersonSepExtensions
{
}

public sealed record Person(int Id, string Name, decimal Score);

[SepSourceGeneration(typeof(NamedPerson))]
public static partial class NamedPersonSepExtensions
{
}

public sealed record NamedPerson
{
    [SepCol("person_id")]
    public int Id { get; set; }

    [SepCol(Name = "display_name")]
    public string Name { get; set; } = "";
}

[SepSourceGeneration(typeof(IndexedPerson))]
public static partial class IndexedPersonSepExtensions
{
}

public sealed record IndexedPerson
{
    [SepCol("person_id", 2)]
    public int Id { get; set; }

    [SepCol("full_name", 0)]
    public string Name { get; set; } = "";

    [SepCol(1)]
    public decimal Score { get; set; }
}
