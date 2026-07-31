using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.SourceGenerator.Test;

[TestClass]
public class SepSourceGeneratorEndToEndTest
{
    [TestMethod]
    public void SepSourceGeneratorEndToEndTest_ReadWriteTryRead_DefaultNamesAndEscapedIdentifier()
    {
        var expected = new DefaultPerson { Id = 42, Name = "Ada", @class = 7 };
        using var writer = Sep.Writer().ToText();
        DefaultPerson.Write(writer, [expected]);

        Assert.AreEqual("Id;Name;class\r\n42;Ada;7\r\n", writer.ToString());

        using var reader = Sep.Reader().FromText(writer.ToString());
        var actual = DefaultPerson.Enumerate(reader).Single();
        Assert.AreEqual(expected.Id, actual.Id);
        Assert.AreEqual(expected.Name, actual.Name);
        Assert.AreEqual(expected.@class, actual.@class);

        using var tryReader = Sep.Reader().FromText(writer.ToString());
        foreach (var row in tryReader)
        {
            Assert.IsTrue(DefaultPerson.TryParse(row, out var tryActual));
            Assert.AreEqual(expected.Name, tryActual.Name);
        }
    }

    [TestMethod]
    public void SepSourceGeneratorEndToEndTest_ReadWriteNullableEnumsIndexesAndFormat()
    {
        var culture = CultureInfo.GetCultureInfo("da-DK");
        var expected = new[]
        {
            new RuntimePerson { Id = 1, Name = null, State = RuntimeState.Ready, Amount = 12.5m },
            new RuntimePerson { Id = 2, Name = "Bob", State = null, Amount = null },
        };
        using var writer = Sep.Writer(options => options with { CultureInfo = culture, Escape = true }).ToText();
        RuntimePerson.Write(writer, expected);

        Assert.AreEqual("\"person\"\"id\";Name;State;Amount\r\n1;;Ready;12,50\r\n2;Bob;;\r\n", writer.ToString());

        using var reader = Sep.Reader(options => options with { CultureInfo = culture, Unescape = true }).FromText(writer.ToString());
        var actual = RuntimePerson.Enumerate(reader).ToArray();
        CollectionAssert.AreEqual(expected, actual);

        using var invalidReader = Sep.Reader(options => options with { CultureInfo = culture, Unescape = true })
            .FromText("\"person\"\"id\";Name;State;Amount\n3;Bad;Ready;invalid\n");
        foreach (var row in invalidReader)
        {
            Assert.IsFalse(RuntimePerson.TryParse(row, out _));
        }
    }

    [TestMethod]
    public async Task SepSourceGeneratorEndToEndTest_ReadAsync()
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
    public void SepSourceGeneratorEndToEndTest_ReadsRequiredConstructorModels()
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

        Assert.AreEqual("Value\r\n\r\nAda\r\n", writer.ToString());
        using var reader = Sep.Reader().FromText(writer.ToString());
        var values = NullableReferencePerson.Enumerate(reader).ToArray();
        Assert.IsNull(values[0].Value);
        Assert.AreEqual("Ada", values[1].Value!.Value);
    }
}
