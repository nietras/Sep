using System.Globalization;

namespace nietras.SeparatedValues;

public readonly record struct SepSpec
{
    public SepSpec() : this(Sep.Default, SepDefaults.CultureInfo) { }

    public SepSpec(Sep sep, CultureInfo? cultureInfo)
    {
        Sep = sep;
        CultureInfo = cultureInfo;
    }

    public Sep Sep { get; init; }
    public CultureInfo? CultureInfo { get; init; }
}
