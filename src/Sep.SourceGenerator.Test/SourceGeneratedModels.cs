using System;
using nietras.SeparatedValues;

namespace nietras.SeparatedValues.SourceGenerator.Test;

[SepSourceGeneration(typeof(DefaultPerson))]
public static partial class DefaultPersonSepExtensions
{
}

public sealed record DefaultPerson
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public int @class { get; init; }
}

public enum RuntimeState
{
    Unknown,
    Ready,
}

[SepSourceGeneration(typeof(RuntimePerson))]
public static partial class RuntimePersonSepExtensions
{
}

public record RuntimePerson
{
    [SepCol("person\"id", 0)]
    public required int Id { get; init; }

    [SepCol("Name", 1)]
    public string? Name { get; init; }

    [SepCol("State", 2)]
    public RuntimeState? State { get; init; }

    [SepCol("Amount", 3, Format = "F2")]
    public decimal? Amount { get; init; }
}

[SepSourceGeneration(typeof(OrdinaryRequiredPerson))]
public static partial class OrdinaryRequiredPersonSepExtensions
{
}

public sealed class OrdinaryRequiredPerson
{
    public OrdinaryRequiredPerson(int id)
    {
    }

    public required int Id { get; init; }
}

[SepSourceGeneration(typeof(SetsRequiredPerson))]
public static partial class SetsRequiredPersonSepExtensions
{
}

public sealed class SetsRequiredPerson
{
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public SetsRequiredPerson(int id)
    {
        Id = id;
    }

    public required int Id { get; init; }
}

public sealed class NullableReferenceValue : ISpanParsable<NullableReferenceValue>, ISpanFormattable
{
    public NullableReferenceValue(string value) => Value = value;

    public string Value { get; }

    public static NullableReferenceValue Parse(string s, IFormatProvider? provider) => new(s);

    public static NullableReferenceValue Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => new(s.ToString());

    public static bool TryParse(string? s, IFormatProvider? provider, out NullableReferenceValue result)
    {
        if (s is null)
        {
            result = null!;
            return false;
        }
        result = new(s);
        return true;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out NullableReferenceValue result)
    {
        result = new(s.ToString());
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider) => Value;

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (Value.AsSpan().TryCopyTo(destination))
        {
            charsWritten = Value.Length;
            return true;
        }
        charsWritten = 0;
        return false;
    }
}

[SepSourceGeneration(typeof(NullableReferencePerson))]
public static partial class NullableReferencePersonSepExtensions
{
}

public sealed class NullableReferencePerson
{
    public NullableReferenceValue? Value { get; set; }
}

[Flags]
public enum ValueFlags
{
    None = 0,
    First = 1,
    Second = 2,
}

/// <summary>
/// Flags with names long enough that a combination of all of them formats to more than the stack
/// allocated buffer of the generated code, which exercises the pooled fallback.
/// </summary>
[Flags]
public enum LongFlags
{
    None = 0,
    AlphaAlphaAlphaAlphaAlphaAlphaAlphaAlphaAlpha = 1 << 0,
    BravoBravoBravoBravoBravoBravoBravoBravoBravo = 1 << 1,
    CharlieCharlieCharlieCharlieCharlieCharlieChar = 1 << 2,
    DeltaDeltaDeltaDeltaDeltaDeltaDeltaDeltaDelta = 1 << 3,
    EchoEchoEchoEchoEchoEchoEchoEchoEchoEchoEchoEc = 1 << 4,
    FoxtrotFoxtrotFoxtrotFoxtrotFoxtrotFoxtrotFoxt = 1 << 5,
    GolfGolfGolfGolfGolfGolfGolfGolfGolfGolfGolfGo = 1 << 6,
}

[SepSourceGeneration(typeof(LongFlagsPerson))]
public static partial class LongFlagsPersonSepExtensions
{
}

public readonly record struct LongFlagsPerson
{
    public LongFlags Flags { get; init; }
}

[SepSourceGeneration(typeof(ValuePerson))]
public static partial class ValuePersonSepExtensions
{
}

/// <summary>
/// Model without any reference type members, so parsing and formatting it must not allocate.
/// </summary>
public readonly record struct ValuePerson
{
    public int Id { get; init; }
    public double Value { get; init; }
    public RuntimeState State { get; init; }
    public ValueFlags Flags { get; init; }
    public int? Optional { get; init; }
}
