using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.SourceGenerator.Test;

[TestClass]
public class SepSourceGenerationTest
{
    [TestMethod]
    public void SepSourceGenerationTest_ReadAndWrite_UsesGeneratedSpanConversions()
    {
        using var writer = Sep.Default.Writer().ToText();
        PersonSep.Write(writer,
        [
            new() { Id = 42, Name = "Ada", Score = 12.5m },
            new() { Id = 7, Name = "Lin", Score = 9.25m },
        ]);

        var text = writer.ToString();
        Assert.AreEqual("Id;Name;Score\r\n42;Ada;12.5\r\n7;Lin;9.25\r\n", text);

        using var reader = Sep.Default.Reader().FromText(text);
        var people = PersonSep.Read(reader).ToArray();

        Assert.HasCount(2, people);
        Assert.AreEqual(42, people[0].Id);
        Assert.AreEqual("Ada", people[0].Name);
        Assert.AreEqual(12.5m, people[0].Score);
        Assert.AreEqual(7, people[1].Id);
        Assert.AreEqual("Lin", people[1].Name);
        Assert.AreEqual(9.25m, people[1].Score);
    }

    [TestMethod]
    public void SepSourceGenerationTest_TryRead_ReturnsFalseForInvalidSpanParsableValue()
    {
        using var reader = Sep.Default.Reader().FromText("Id;Name;Score\r\nnot-an-int;Ada;12.5\r\n");
        Assert.IsTrue(reader.MoveNext());

        Assert.IsFalse(PersonSep.TryRead(reader.Current, out var person));
        Assert.IsNull(person);
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_Name()
    {
        using var reader = Sep.Default.Reader().FromText("display_name;person_id\r\nAda;42\r\n");
        var people = NamedPersonSep.Read(reader).ToArray();

        Assert.HasCount(1, people);
        Assert.AreEqual(42, people[0].Id);
        Assert.AreEqual("Ada", people[0].Name);

        using var writer = Sep.Default.Writer().ToText();
        NamedPersonSep.Write(writer, people);
        Assert.AreEqual($"person_id;display_name{Environment.NewLine}42;Ada{Environment.NewLine}", writer.ToString());
    }

    [TestMethod]
    public void SepSourceGenerationTest_SepCol_IndexAndName()
    {
        var options = Sep.Default.Reader(o => o with { HasHeader = false });
        using var reader = options.FromText("Ada;12.5;42\r\n");
        var people = IndexedPersonSep.Read(reader).ToArray();

        Assert.HasCount(1, people);
        Assert.AreEqual(42, people[0].Id);
        Assert.AreEqual("Ada", people[0].Name);
        Assert.AreEqual(12.5m, people[0].Score);

        using var writer = Sep.Default.Writer().ToText();
        IndexedPersonSep.Write(writer, people);
        Assert.AreEqual($"full_name;Score;person_id{Environment.NewLine}Ada;12.5;42{Environment.NewLine}", writer.ToString());
    }
}

[SepSourceGeneration(typeof(Person))]
public static partial class PersonSep
{
}

public sealed record Person
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Score { get; set; }
}

[SepSourceGeneration(typeof(NamedPerson))]
public static partial class NamedPersonSep
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
public static partial class IndexedPersonSep
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
