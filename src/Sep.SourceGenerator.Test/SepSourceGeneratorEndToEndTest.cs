using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.SourceGenerator.Test;

[TestClass]
public class SepSourceGeneratorEndToEndTest
{
    static readonly string s_defaultPersonCsv = "Id;Name;class\r\n42;Ada;7\r\n".ReplaceLineEndings();
    static readonly string s_runtimePersonCsv =
        "\"person\"\"id\";Name;State;Amount\r\n1;;Ready;12,50\r\n2;Bob;;\r\n".ReplaceLineEndings();
    static readonly CultureInfo s_danishCulture = CultureInfo.GetCultureInfo("da-DK");
    static readonly DefaultPerson s_defaultPerson = new() { Id = 42, Name = "Ada", @class = 7 };
    static readonly RuntimePerson[] s_runtimePeople =
    [
        new() { Id = 1, Name = null, State = RuntimeState.Ready, Amount = 12.5m },
        new() { Id = 2, Name = "Bob", State = null, Amount = null },
    ];

    [TestMethod]
    public void SepSourceGeneratorEndToEndTest_Write_DefaultNamesAndEscapedIdentifier()
    {
        using var writer = Sep.Writer().ToText();
        DefaultPerson.Write(writer, [s_defaultPerson]);

        Assert.AreEqual(s_defaultPersonCsv, writer.ToString());
    }

    [TestMethod]
    public void SepSourceGeneratorEndToEndTest_Enumerate_DefaultNamesAndEscapedIdentifier()
    {
        using var reader = Sep.Reader().FromText(s_defaultPersonCsv);

        Assert.AreEqual(s_defaultPerson, DefaultPerson.Enumerate(reader).Single());
    }

    [TestMethod]
    public void SepSourceGeneratorEndToEndTest_TryParse_DefaultNamesAndEscapedIdentifier()
    {
        using var reader = Sep.Reader().FromText(s_defaultPersonCsv);
        Assert.IsTrue(reader.MoveNext());

        Assert.IsTrue(DefaultPerson.TryParse(reader.Current, out var actual));
        Assert.AreEqual(s_defaultPerson, actual);
    }

    [TestMethod]
    public void SepSourceGeneratorEndToEndTest_Write_NullableEnumsIndexesAndFormat()
    {
        using var writer = Sep.Writer(options => options with { CultureInfo = s_danishCulture, Escape = true }).ToText();

        RuntimePerson.Write(writer, s_runtimePeople);

        Assert.AreEqual(s_runtimePersonCsv, writer.ToString());
    }

    [TestMethod]
    public void SepSourceGeneratorEndToEndTest_Enumerate_NullableEnumsIndexesAndFormat()
    {
        using var reader = Sep.Reader(options => options with
        {
            CultureInfo = s_danishCulture,
            Unescape = true,
        }).FromText(s_runtimePersonCsv);

        Assert.AreSequenceEqual(s_runtimePeople, RuntimePerson.Enumerate(reader).ToArray());
    }

    [TestMethod]
    public void SepSourceGeneratorEndToEndTest_TryParse_ReturnsFalseForInvalidFormattedValue()
    {
        using var reader = Sep.Reader(options => options with
        {
            CultureInfo = s_danishCulture,
            Unescape = true,
        }).FromText("\"person\"\"id\";Name;State;Amount\n3;Bad;Ready;invalid\n");
        Assert.IsTrue(reader.MoveNext());

        Assert.IsFalse(RuntimePerson.TryParse(reader.Current, out _));
    }

    [TestMethod]
    public async Task SepSourceGeneratorEndToEndTest_EnumerateAsync()
    {
        using var reader = Sep.Reader().FromText("Id;Name;class\n1;Ada;2\n");
        var values = new List<DefaultPerson>();
        await foreach (var value in DefaultPerson.EnumerateAsync(reader))
        {
            values.Add(value);
        }

        Assert.HasCount(1, values);
        Assert.AreEqual("Ada", values[0].Name);
    }

    [TestMethod]
    public void SepSourceGeneratorEndToEndTest_Enumerate_RequiredConstructorModels()
    {
        using var ordinaryWriter = Sep.Writer().ToText();
        OrdinaryRequiredPerson.Write(ordinaryWriter, [new OrdinaryRequiredPerson(0) { Id = 42 }]);
        using var ordinaryReader = Sep.Reader().FromText(ordinaryWriter.ToString());
        Assert.AreEqual(42, OrdinaryRequiredPerson.Enumerate(ordinaryReader).Single().Id);

        using var setsRequiredWriter = Sep.Writer().ToText();
        SetsRequiredPerson.Write(setsRequiredWriter, [new SetsRequiredPerson(7)]);
        using var setsRequiredReader = Sep.Reader().FromText(setsRequiredWriter.ToString());
        Assert.AreEqual(7, SetsRequiredPerson.Enumerate(setsRequiredReader).Single().Id);
    }

    [TestMethod]
    public void SepSourceGeneratorEndToEndTest_MapsNullableReferenceSpanValues()
    {
        using var writer = Sep.Writer().ToText();
        NullableReferencePerson.Write(writer,
        [
            new NullableReferencePerson { Value = null },
            new NullableReferencePerson { Value = new("Ada") },
        ]);

        Assert.AreEqual($"Value{Environment.NewLine}{Environment.NewLine}Ada{Environment.NewLine}", writer.ToString());
        using var reader = Sep.Reader().FromText(writer.ToString());
        var values = NullableReferencePerson.Enumerate(reader).ToArray();
        Assert.IsNull(values[0].Value);
        Assert.AreEqual("Ada", values[1].Value!.Value);
    }
}
