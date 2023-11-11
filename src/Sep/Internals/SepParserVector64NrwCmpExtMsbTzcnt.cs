using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using static System.Runtime.CompilerServices.Unsafe;
using static nietras.SeparatedValues.SepDefaults;
using static nietras.SeparatedValues.SepParseMaskGeneric;
using Vec = System.Runtime.Intrinsics.Vector64;
using VecUI16 = System.Runtime.Intrinsics.Vector64<ushort>;
using VecUI8 = System.Runtime.Intrinsics.Vector64<byte>;

namespace nietras.SeparatedValues;

sealed class SepParserVector64NrwCmpExtMsbTzcnt : ISepParser
{
    readonly byte _separator;
    readonly VecUI16 _max = Vec.Create((ushort)(Sep.Max.Separator + 1));
    readonly VecUI8 _nls = Vec.Create(LineFeedByte);
    readonly VecUI8 _crs = Vec.Create(CarriageReturnByte);
    readonly VecUI8 _qts = Vec.Create(QuoteByte);
    readonly VecUI8 _sps;
    internal nuint _quoteCount = 0;

    public unsafe SepParserVector64NrwCmpExtMsbTzcnt(Sep sep)
    {
        _separator = (byte)sep.Separator;
        _sps = Vec.Create(_separator);
    }

    // Parses 2 x char vectors e.g. 1 byte vector
    public int PaddingLength => VecUI8.Count;

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public int ParseColEnds(SepReaderState s)
    {
        return Parse<int, SepColEndMethods>(s);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public int ParseColInfos(SepReaderState s)
    {
        return Parse<SepColInfo, SepColInfoMethods>(s);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int Parse<TColInfo, TColInfoMethods>(SepReaderState s)
        where TColInfo : unmanaged
        where TColInfoMethods : unmanaged, ISepColInfoMethods<TColInfo>
    {
        // Method should **not** call other non-inlined methods, since this
        // impacts code-generation severely.

        var separator = (char)_separator;
        var quoteCount = _quoteCount;

        var chars = s._chars;
        var charsIndex = s._charsParseStart;
        var charsEnd = s._charsDataEnd;
        var colInfos = s._colEnds;
        var colCount = s._colCount;
        var lineNumber = s._lineNumber;

        var rowLineEndingOffset = 0;

        var colInfosLength = colInfos.Length / (SizeOf<TColInfo>() / SizeOf<int>());

        chars.CheckPaddingAndIsZero(charsEnd, PaddingLength);
        SepArrayExtensions.CheckPadding(colInfosLength, colCount, PaddingLength);

        A.Assert(charsIndex <= charsEnd);
        A.Assert(charsEnd <= (chars.Length - PaddingLength));
        ref var charsRef = ref Add(ref MemoryMarshal.GetArrayDataReference(chars), charsIndex);

        ref var colInfosRef = ref As<int, TColInfo>(ref MemoryMarshal.GetArrayDataReference(colInfos));
        ref var colInfosRefCurrent = ref Add(ref colInfosRef, colCount);
        ref var colInfosRefStop = ref Add(ref colInfosRef, colInfosLength - VecUI8.Count);

        // Use instance fields to force values into registers
        var max = _max;
        var nls = _nls; //Vec.Create(LineFeedByte);
        var crs = _crs; //Vec.Create(CarriageReturnByte);
        var qts = _qts; //Vec.Create(QuoteByte);
        var sps = _sps; //Vec.Create(_separator);

        for (; charsIndex < charsEnd; charsIndex += VecUI8.Count,
             charsRef = ref Add(ref charsRef, VecUI8.Count))
        {
            ref var byteRef = ref As<char, byte>(ref charsRef);
            var v0 = ReadUnaligned<VecUI16>(ref byteRef);
            var v1 = ReadUnaligned<VecUI16>(ref Add(ref byteRef, VecUI8.Count));
            var limit0 = Vec.Min(v0, max);
            var limit1 = Vec.Min(v1, max);
            var bytes = Vec.Narrow(limit0, limit1);

            var nlsEq = Vec.Equals(bytes, nls);
            var crsEq = Vec.Equals(bytes, crs);
            var qtsEq = Vec.Equals(bytes, qts);
            var spsEq = Vec.Equals(bytes, sps);

            var lineEndings = nlsEq | crsEq;
            var lineEndingsSeparators = spsEq | lineEndings;
            var specialChars = lineEndingsSeparators | qtsEq;

            // Optimize for the case of no special character
            var specialCharMask = specialChars.ExtractMostSignificantBits();
            if (specialCharMask != 0)
            {
                var separatorsMask = spsEq.ExtractMostSignificantBits();
                // Optimize for case of only separators i.e. no endings or quotes.
                // Add quoting flags to mask as hack to skip if quoting.
                var testMask = specialCharMask + quoteCount;
                if (separatorsMask == testMask)
                {
                    colInfosRefCurrent = ref ParseSeparatorsMask<TColInfo, TColInfoMethods>(separatorsMask, charsIndex,
                        ref colInfosRefCurrent);
                }
                else
                {
                    var separatorLineEndingsMask = lineEndingsSeparators.ExtractMostSignificantBits();
                    if (separatorLineEndingsMask == testMask)
                    {
                        colInfosRefCurrent = ref ParseSeparatorsLineEndingsMasks<TColInfo, TColInfoMethods>(
                            separatorsMask, separatorLineEndingsMask,
                            ref charsRef, ref charsIndex, separator,
                            ref colInfosRefCurrent, ref rowLineEndingOffset, ref lineNumber);
                        break;
                    }
                    else
                    {
                        colInfosRefCurrent = ref ParseAnyCharsMask<TColInfo, TColInfoMethods>(specialCharMask,
                            separator, ref charsRef, charsIndex,
                            ref rowLineEndingOffset, ref quoteCount,
                            ref colInfosRefCurrent, ref lineNumber);
                        // Used both to indicate row ended and if need to step +2 due to '\r\n'
                        if (rowLineEndingOffset != 0)
                        {
                            // Must be a col end and last is then dataIndex
                            charsIndex = TColInfoMethods.GetColEnd(colInfosRefCurrent) + rowLineEndingOffset;
                            break;
                        }
                    }
                }
                // If current is greater than or equal than "stop", then break.
                // There is no longer guaranteed space enough for next VecUI8.Count.
                if (IsAddressLessThan(ref colInfosRefStop, ref colInfosRefCurrent))
                {
                    // Move data index so next find starts correctly
                    charsIndex += VecUI8.Count;
                    break;
                }
            }
        }

        // ">> 2" instead of "/ sizeof(int))" // CQ: Weird with div sizeof
        colCount = (int)(ByteOffset(ref colInfosRef, ref colInfosRefCurrent) >> 2);
        // Step is VecUI8.Count so may go past end, ensure limited
        charsIndex = Math.Min(charsEnd, charsIndex);

        _quoteCount = quoteCount;
        s._colCount = colCount;
        s._lineNumber = lineNumber;
        s._charsParseStart = charsIndex;

        return rowLineEndingOffset;
    }
}
