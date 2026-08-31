using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.SourceGenerator.Test;

/// <summary>
/// Sep promises best in class performance and zero allocation, so source generated code must not
/// allocate when parsing or formatting models consisting only of value types.
/// </summary>
[TestClass]
public class SepSourceGeneratorAllocationTest
{
    const int RowCount = 64;
    static readonly string s_csv = CreateCsv();
    static readonly ValuePerson[] s_values = CreateValues();

    [TestMethod]
    public void SepSourceGeneratorAllocationTest_ParseDoesNotAllocate()
    {
        var allocated = MeasureParsing(static reader =>
        {
            var total = 0;
            foreach (var row in reader)
            {
                total += ValuePerson.Parse(row).Id;
            }
            return total;
        });

        Assert.AreEqual(0, allocated);
    }

    [TestMethod]
    public void SepSourceGeneratorAllocationTest_TryParseDoesNotAllocate()
    {
        var allocated = MeasureParsing(static reader =>
        {
            var total = 0;
            foreach (var row in reader)
            {
                if (ValuePerson.TryParse(row, out var value))
                {
                    total += value.Id;
                }
            }
            return total;
        });

        Assert.AreEqual(0, allocated);
    }

    [TestMethod]
    public void SepSourceGeneratorAllocationTest_ColumnConventionDoesNotAllocate()
    {
        var allocated = MeasureParsing(
            static reader =>
            {
                var total = 0;
                foreach (var value in ColumnConventionValue.Enumerate(reader))
                {
                    total += value.Id;
                }
                return total;
            },
            "Value;Id\n0.5;0\n1.5;1\n",
            static options => options);

        Assert.AreEqual(0, allocated);
    }

    [TestMethod]
    public void SepSourceGeneratorAllocationTest_EnumerateDoesNotAllocate()
    {
        var allocated = MeasureParsing(static reader =>
        {
            var total = 0;
            // Must bind to the generated struct enumerator, not to IEnumerable<T>.
            foreach (var value in ValuePerson.Enumerate(reader))
            {
                total += value.Id;
            }
            return total;
        });

        Assert.AreEqual(0, allocated);
    }

    [TestMethod]
    public void SepSourceGeneratorAllocationTest_EnumeratorIsValueType()
    {
        Assert.IsTrue(typeof(ValuePersonSepExtensions.ModelEnumerable).IsValueType);
        Assert.IsTrue(typeof(ValuePersonSepExtensions.ModelEnumerator).IsValueType);
        using var reader = Sep.Reader().FromText(s_csv);
        // Resolved by the compiler via the pattern based foreach, so no interface dispatch.
        Assert.AreEqual(
            typeof(ValuePersonSepExtensions.ModelEnumerator),
            ValuePerson.Enumerate(reader).GetEnumerator().GetType());
    }

    [TestMethod]
    public void SepSourceGeneratorAllocationTest_FormatDoesNotAllocate()
    {
        using var textWriter = new DiscardingTextWriter();
        using var writer = Sep.Writer().To(textWriter);
        var allocated = Measure(() =>
        {
            for (var index = 0; index < s_values.Length; ++index)
            {
                using var row = writer.NewRow();
                ValuePerson.Format(row, s_values[index]);
            }
            return s_values.Length;
        });

        Assert.AreEqual(0, allocated);
    }

    [TestMethod]
    public void SepSourceGeneratorAllocationTest_WriteSpanDoesNotAllocate()
    {
        using var textWriter = new DiscardingTextWriter();
        using var writer = Sep.Writer().To(textWriter);
        var allocated = Measure(() =>
        {
            ValuePerson.Write(writer, new ReadOnlySpan<ValuePerson>(s_values));
            return s_values.Length;
        });

        Assert.AreEqual(0, allocated);
    }

    [TestMethod]
    public void SepSourceGeneratorAllocationTest_WriteArrayDoesNotAllocate()
    {
        // Arrays bind to the dedicated array overload, which must forward to the span path rather
        // than fall back to enumerating and allocating an enumerator.
        using var textWriter = new DiscardingTextWriter();
        using var writer = Sep.Writer().To(textWriter);
        var allocated = Measure(() =>
        {
            ValuePerson.Write(writer, s_values);
            return s_values.Length;
        });

        Assert.AreEqual(0, allocated);
    }

    [TestMethod]
    public void SepSourceGeneratorAllocationTest_WriteUndefinedAndFlagsEnumDoesNotAllocate()
    {
        var values = new[]
        {
            // Neither is a defined enum member, so both take the Enum.TryFormat fallback.
            new ValuePerson { State = (RuntimeState)42 },
            new ValuePerson { Flags = ValueFlags.First | ValueFlags.Second },
        };
        using var textWriter = new DiscardingTextWriter();
        using var writer = Sep.Writer().To(textWriter);
        var allocated = Measure(() =>
        {
            ValuePerson.Write(writer, new ReadOnlySpan<ValuePerson>(values));
            return values.Length;
        });

        Assert.AreEqual(0, allocated);
    }

    [TestMethod]
    public void SepSourceGeneratorAllocationTest_RoundTripsUndefinedAndFlagsEnums()
    {
        var expected = new[]
        {
            new ValuePerson { Id = 1, State = (RuntimeState)42, Flags = ValueFlags.First | ValueFlags.Second },
            new ValuePerson { Id = 2, State = RuntimeState.Ready, Flags = ValueFlags.None, Optional = 3 },
        };
        using var writer = Sep.Writer().ToText();
        ValuePerson.Write(writer, new ReadOnlySpan<ValuePerson>(expected));

        Assert.AreEqual(
            $"Id;Value;State;Flags;Optional{Environment.NewLine}" +
            $"1;0;42;First, Second;{Environment.NewLine}" +
            $"2;0;Ready;None;3{Environment.NewLine}",
            writer.ToString());

        using var reader = Sep.Reader().FromText(writer.ToString());
        var index = 0;
        foreach (var actual in ValuePerson.Enumerate(reader))
        {
            Assert.AreEqual(expected[index], actual);
            ++index;
        }
        Assert.AreEqual(expected.Length, index);
    }

    [TestMethod]
    public void SepSourceGeneratorAllocationTest_RoundTripsFlagsLongerThanStackallocBuffer()
    {
        const LongFlags all =
            LongFlags.AlphaAlphaAlphaAlphaAlphaAlphaAlphaAlphaAlpha |
            LongFlags.BravoBravoBravoBravoBravoBravoBravoBravoBravo |
            LongFlags.CharlieCharlieCharlieCharlieCharlieCharlieChar |
            LongFlags.DeltaDeltaDeltaDeltaDeltaDeltaDeltaDeltaDelta |
            LongFlags.EchoEchoEchoEchoEchoEchoEchoEchoEchoEchoEchoEc |
            LongFlags.FoxtrotFoxtrotFoxtrotFoxtrotFoxtrotFoxtrotFoxt |
            LongFlags.GolfGolfGolfGolfGolfGolfGolfGolfGolfGolfGolfGo;
        var expected = new LongFlagsPerson { Flags = all };
        using var writer = Sep.Writer(static options => options with { Escape = true }).ToText();
        LongFlagsPerson.Write(writer, new ReadOnlySpan<LongFlagsPerson>([expected]));

        var text = writer.ToString();
        Assert.IsGreaterThan(256, text.Length);

        using var reader = Sep.Reader(static options => options with { Unescape = true }).FromText(text);
        foreach (var actual in LongFlagsPerson.Enumerate(reader))
        {
            Assert.AreEqual(expected, actual);
        }
    }

    /// <summary>
    /// Documents why the generated code cannot just use Col.Format for enums like it does for every
    /// other ISpanFormattable type. ISpanFormattable is implemented by System.Enum rather than by
    /// each enum type, so the constrained call in a generic Format&lt;T&gt; has to box. If this ever
    /// starts failing because the boxing is gone, the generated enum handling can be simplified.
    /// </summary>
    [TestMethod]
    public void SepSourceGeneratorAllocationTest_ColFormatBoxesEnumButGeneratedWriteDoesNot()
    {
        using var formatTextWriter = new DiscardingTextWriter();
        using var formatWriter = Sep.Writer().To(formatTextWriter);
        var formatted = Measure(() =>
        {
            using var row = formatWriter.NewRow();
            row["State"].Format(RuntimeState.Ready);
            return 1;
        });

        using var generatedTextWriter = new DiscardingTextWriter();
        using var generatedWriter = Sep.Writer().To(generatedTextWriter);
        var generated = Measure(() =>
        {
            using var row = generatedWriter.NewRow();
            ValuePerson.Format(row, default);
            return 1;
        });

        Assert.IsGreaterThan(0, formatted, "Col.Format is expected to box the enum.");
        Assert.AreEqual(0, generated);
    }

    /// <summary>
    /// ISpanFormattable is on System.Enum, not on each enum type, and enums do not implement
    /// ISpanParsable&lt;TSelf&gt; at all. Both are why the generator cannot treat enums like every
    /// other span convertible type and has to know the concrete enum type instead.
    /// </summary>
    [TestMethod]
    public void SepSourceGeneratorAllocationTest_EnumInterfacesExplainWhyGenerationIsSpecialized()
    {
        Assert.IsTrue(typeof(ISpanFormattable).IsAssignableFrom(typeof(RuntimeState)));
        // The implementing method is declared on System.Enum rather than on the enum type itself,
        // so a constrained call through ISpanFormattable has to box.
        var interfaceMap = typeof(RuntimeState).GetInterfaceMap(typeof(ISpanFormattable));
        Assert.AreEqual(typeof(Enum), interfaceMap.TargetMethods[0].DeclaringType);
        // Written with reflection since `ISpanParsable<RuntimeState>` does not even compile as a
        // constraint target, which is exactly the point being asserted.
        Assert.IsFalse(Array.Exists(typeof(RuntimeState).GetInterfaces(),
            static @interface => @interface.IsGenericType &&
                @interface.GetGenericTypeDefinition() == typeof(ISpanParsable<>)),
            "Enums do not implement ISpanParsable<TSelf>, so Col.Parse<T> cannot be used for them.");
    }

    /// <summary>
    /// Formatting an undefined value or a flags combination cannot use a constant name, so it must
    /// still avoid allocating by going through Enum.TryFormat rather than Enum.ToString.
    /// </summary>
    [TestMethod]
    public void SepSourceGeneratorAllocationTest_EnumToStringAllocatesButTryFormatDoesNot()
    {
        const ValueFlags flags = ValueFlags.First | ValueFlags.Second;
        var toStringAllocated = Measure(() => flags.ToString().Length);
        var tryFormatAllocated = Measure(() =>
        {
            Span<char> chars = stackalloc char[256];
            Enum.TryFormat(flags, chars, out var charsWritten);
            return charsWritten;
        });

        Assert.IsGreaterThan(0, toStringAllocated, "Enum.ToString is expected to allocate a string.");
        Assert.AreEqual(0, tryFormatAllocated);
    }

    static long MeasureParsing(Func<SepReader, int> action) =>
        MeasureParsing(action, s_csv, static options => options);

    static long MeasureParsing(
        Func<SepReader, int> action,
        string csv,
        Func<SepReaderOptions, SepReaderOptions> configure)
    {
        // Creating the reader allocates, so only the enumeration itself is measured. Warming up
        // first ensures jitting and array pool rents do not count as allocations either.
        for (var iteration = 0; iteration < 3; ++iteration)
        {
            using var warmupReader = Sep.Reader(configure).FromText(csv);
            action(warmupReader);
        }
        using var reader = Sep.Reader(configure).FromText(csv);
        var before = GC.GetAllocatedBytesForCurrentThread();
        action(reader);
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    static long Measure(Func<int> action)
    {
        // Warm up so jitting, static constructors and array pool rents do not count as allocations.
        for (var iteration = 0; iteration < 3; ++iteration)
        {
            action();
        }
        var before = GC.GetAllocatedBytesForCurrentThread();
        action();
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    static string CreateCsv()
    {
        var builder = new StringBuilder("Id;Value;State;Flags;Optional\n");
        for (var index = 0; index < RowCount; ++index)
        {
            builder.Append(CultureInfo.InvariantCulture, $"{index};{index}.5;Ready;First;{index}\n");
        }
        return builder.ToString();
    }

    static ValuePerson[] CreateValues()
    {
        var values = new ValuePerson[RowCount];
        for (var index = 0; index < values.Length; ++index)
        {
            values[index] = new ValuePerson
            {
                Id = index,
                Value = index + 0.5,
                State = RuntimeState.Ready,
                Flags = ValueFlags.First,
                Optional = index,
            };
        }
        return values;
    }

    sealed class DiscardingTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
        public override void Write(char value) { }
        public override void Write(char[] buffer, int index, int count) { }
        public override void Write(ReadOnlySpan<char> buffer) { }
        public override void Write(string? value) { }
    }
}
