namespace nietras.SeparatedValues;

static class SepUtf8ParserFactory
{
    internal static ISepParser<byte, SepCharInfoUtf8> Create(SepUtf8ParserOptions options) =>
        new SepParserIndexOfAny<byte, SepCharInfoUtf8>(options.Separator, options.QuotesOrSeparatorIfDisabled);
}
