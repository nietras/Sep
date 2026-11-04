namespace nietras.SeparatedValues;

readonly struct SepParserOptions
{
    // One scalar field lets older JITs extract every byte without spilling the struct.
    readonly uint _bytes;

    internal uint Bytes => _bytes;

    internal byte Separator
    {
        get => (byte)_bytes;
        init => _bytes = (_bytes & 0xFFFFFF00u) | value;
    }
    internal byte QuotesOrSeparatorIfDisabled
    {
        get => (byte)(_bytes >> 8);
        init => _bytes = (_bytes & 0xFFFF00FFu) | ((uint)value << 8);
    }
    internal byte LineFeed
    {
        get => (byte)(_bytes >> 16);
        init => _bytes = (_bytes & 0xFF00FFFFu) | ((uint)value << 16);
    }
    internal byte CarriageReturn
    {
        get => (byte)(_bytes >> 24);
        init => _bytes = (_bytes & 0x00FFFFFFu) | ((uint)value << 24);
    }

    internal SepParserOptions(uint bytes) => _bytes = bytes;

    internal SepParserOptions(byte separator, byte quotesOrSeparatorIfDisabled,
        byte lineFeed = SepDefaults.LineFeedByte, byte carriageReturn = SepDefaults.CarriageReturnByte)
    {
        _bytes = separator | ((uint)quotesOrSeparatorIfDisabled << 8) |
            ((uint)lineFeed << 16) | ((uint)carriageReturn << 24);
    }
    internal SepParserOptions(Sep sep)
        : this((byte)sep.Separator, SepDefaults.QuoteByte)
    { }
    internal SepParserOptions(Sep sep, bool disableQuotesParsing)
        : this((byte)sep.Separator, disableQuotesParsing ? (byte)sep.Separator : SepDefaults.QuoteByte)
    { }
}
