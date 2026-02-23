using System;

namespace nietras.SeparatedValues;

interface ISepParser<TChar, TCharInfo>
    where TChar : unmanaged, IEquatable<TChar>
    where TCharInfo : struct, ISepCharInfo<TChar>
{
    int PaddingLength { get; }
    int QuoteCount { get; }
    void ParseColEnds(SepReaderStateBase<TChar, TCharInfo> s);
    void ParseColInfos(SepReaderStateBase<TChar, TCharInfo> s);
}

interface ISepParser : ISepParser<char, SepCharInfoUtf16>
{
}
