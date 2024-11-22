using System;
using System.Collections.Generic;
using System.Globalization;

namespace nietras.SeparatedValues;

public static class SepDefaults
{
    public static char Separator => ';';
    public static CultureInfo CultureInfo { get; } = CultureInfo.InvariantCulture;
    public static StringComparer ColNameComparer { get; } = StringComparer.Ordinal;

    internal static IReadOnlyList<char> AutoDetectSeparators { get; } =
        [Separator, '\t', '|', ','];

#if DEBUG
    // To increase probability of detecting bugs start with short length to
    // force buffer management paths to be used.
    internal const int InitialBufferLength = 64;
#else
    // Based on L1d typically being 32KB-48KB, so aiming for 16K-24K x sizeof(char).
    // Benchmarks show below to be a good minimum length.
    // Currently rented on ArrayPool and will be rounded up to nearest power of 2.
    internal const int InitialBufferLength = 24 * 1024;
#endif

    internal const int RowLengthMax = 1 << 24;

    // Note the following common line feed chars:
    // \n - UNIX   \r\n - DOS   \r - Mac

    internal const char LineFeed = '\n';
    internal const char CarriageReturn = '\r';
    internal const char Quote = '"';
    internal const char EndOfText = '\u0003';
    // Sep does not handle comments currently, this is solely to prevent using
    // `#` as separator
    internal const char Comment = '#';
    internal const char Space = ' ';

    internal const byte LineFeedByte = (byte)LineFeed;
    internal const byte CarriageReturnByte = (byte)CarriageReturn;
    internal const byte QuoteByte = (byte)Quote;
    //internal const byte CommentByte = (byte)Comment;

    internal const byte EmptyFlag = 0b0000_0000;
    internal const byte LineFeedFlag = 0b0000_0001;
    internal const byte CarriageReturnFlag = 0b0000_0010;
    internal const byte QuoteFlag = 0b0000_0100;
    internal const byte SeparatorFlag = 0b0000_1000;
    internal const byte EndOfTextFlag = 0b0001_0000;
    //internal const byte CommentFlag = 0b0010_0000;

    // Make test for building this, define in separate file
    //internal static ReadOnlySpan<byte> CharToFlags => new byte[256];
    //internal readonly byte[] CharToFlags = CreateCharToFlags();
    //static byte[] CreateCharToFlags()
    //{
    //    var bytes = new byte[byte.MaxValue + 1];
    //    bytes[LineFeed] = LineFeedFlag;
    //    bytes[CarriageReturn] = CarriageReturnFlag;
    //    bytes[Quote] = QuoteFlag;
    //    bytes[CommentFlag] = CommentFlag;
    //    bytes[';'] = SeparatorFlag;
    //    bytes[','] = SeparatorFlag;
    //    bytes['\t'] = SeparatorFlag;
    //    bytes['|'] = SeparatorFlag;
    //    // TODO: Handle other separators
    //    return bytes;
    //}
}
