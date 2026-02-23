namespace nietras.SeparatedValues;

static class SepUtf8ParserFactory
{
    internal static ISepParser<byte, SepCharInfoUtf8> Create(SepUtf8ParserOptions options) =>
        new SepUtf8ParserIndexOfAny(options);
}
