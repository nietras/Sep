namespace nietras.SeparatedValues;

readonly record struct SepParserOptions(char Separator, char QuotesOrSeparatorIfDisabled)
{
    internal SepParserOptions(Sep sep)
        : this(sep.Separator, SepDefaults.Quote)
    { }
    internal SepParserOptions(Sep sep, bool disableQuotesParsing)
        : this(sep.Separator, disableQuotesParsing ? sep.Separator : SepDefaults.Quote)
    { }
}
