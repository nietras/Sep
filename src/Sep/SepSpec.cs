using System.Globalization;

namespace nietras.SeparatedValues;

public readonly record struct SepSpec
{
    public SepSpec() : this(Sep.Default, SepDefaults.CultureInfo, false) { }

    public SepSpec(Sep sep, CultureInfo? cultureInfo, bool asyncContinueOnCapturedContext)
    {
        Sep = sep;
        CultureInfo = cultureInfo;
        AsyncContinueOnCapturedContext = asyncContinueOnCapturedContext;
    }

    public Sep Sep { get; init; }
    public CultureInfo? CultureInfo { get; init; }
    public bool AsyncContinueOnCapturedContext { get; init; } = false;
}
