namespace nietras.SeparatedValues;

static class SepUtf8ParserFactory
{
    // For now UTF-8 only supports IndexOfAny parser. SIMD parsers added later.
    internal static ISepUtf8Parser Create(SepUtf8ParserOptions options) =>
        new SepUtf8ParserIndexOfAny(options);
}
