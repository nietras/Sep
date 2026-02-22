using System;

namespace nietras.SeparatedValues;

internal interface ISepCharInfo<TChar> where TChar : unmanaged, IEquatable<TChar>
{
    static abstract TChar LineFeed { get; }
    static abstract TChar CarriageReturn { get; }
    static abstract TChar Quote { get; }
    static abstract TChar Space { get; }
}

internal readonly struct SepCharInfoUtf16 : ISepCharInfo<char>
{
    public static char LineFeed => SepDefaults.LineFeed;
    public static char CarriageReturn => SepDefaults.CarriageReturn;
    public static char Quote => SepDefaults.Quote;
    public static char Space => SepDefaults.Space;
}

internal readonly struct SepCharInfoUtf8 : ISepCharInfo<byte>
{
    public static byte LineFeed => SepDefaults.LineFeedByte;
    public static byte CarriageReturn => SepDefaults.CarriageReturnByte;
    public static byte Quote => SepDefaults.QuoteByte;
    public static byte Space => SepDefaults.SpaceByte;
}
