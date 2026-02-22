namespace nietras.SeparatedValues;

readonly record struct SepUtf8ParserOptions(byte Separator, byte QuotesOrSeparatorIfDisabled)
{
    internal SepUtf8ParserOptions(Sep sep)
        : this((byte)sep.Separator, SepDefaults.QuoteByte)
    { }
    internal SepUtf8ParserOptions(Sep sep, bool disableQuotesParsing)
        : this((byte)sep.Separator, disableQuotesParsing ? (byte)sep.Separator : SepDefaults.QuoteByte)
    { }
}
