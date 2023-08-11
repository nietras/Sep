using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using static System.Runtime.CompilerServices.Unsafe;
using static nietras.SeparatedValues.SepDefaults;
using static nietras.SeparatedValues.SepParseMask;
using Vec = System.Runtime.Intrinsics.Vector256;
using VecUI16 = System.Runtime.Intrinsics.Vector256<ushort>;
using VecUI8 = System.Runtime.Intrinsics.Vector256<byte>;

namespace nietras.SeparatedValues;

sealed class SepParserVector256NrwCmpExtMsbTzcnt : ISepParser
{
    readonly byte _separator;
    readonly VecUI16 _max = Vec.Create((ushort)(Sep.Max.Separator + 1));
    readonly VecUI8 _nls = Vec.Create(LineFeedByte);
    readonly VecUI8 _crs = Vec.Create(CarriageReturnByte);
    readonly VecUI8 _qts = Vec.Create(QuoteByte);
    readonly VecUI8 _sps;
    internal nuint _quoting = 0;

    public unsafe SepParserVector256NrwCmpExtMsbTzcnt(Sep sep)
    {
        _separator = (byte)sep.Separator;
        _sps = Vec.Create(_separator);
    }

    // Parses 2 x char vectors e.g. 1 byte vector
    public int PaddingLength => VecUI8.Count;

    [SkipLocalsInit]
    public int Parse(char[] chars, int charsIndex, int charsEnd,
                     int[] colEnds, ref int colEndsEnd,
                     scoped ref int _rowLineEndingOffset, scoped ref int _lineNumber)
    {
        // Method should **not** call other non-inlined methods, since this
        // impacts code-generation severely.

        var separator = (char)_separator;

        var quoting = _quoting;
        var rowLineEndingOffset = _rowLineEndingOffset;
        var lineNumber = _lineNumber;

        chars.CheckPaddingAndIsZero(charsEnd, PaddingLength);
        colEnds.CheckPadding(colEndsEnd, PaddingLength);

        A.Assert(charsIndex <= charsEnd);
        A.Assert(charsEnd <= (chars.Length - PaddingLength));
        ref var charsRef = ref Add(ref MemoryMarshal.GetArrayDataReference(chars), charsIndex);

        ref var colEndsRef = ref MemoryMarshal.GetArrayDataReference(colEnds);
        ref var colEndsRefCurrent = ref Add(ref colEndsRef, colEndsEnd);
        ref var colEndsRefStop = ref Add(ref colEndsRef, colEnds.Length - VecUI8.Count);

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
                var testMask = specialCharMask + quoting;
                if (separatorsMask == testMask)
                {
                    colEndsRefCurrent = ref ParseSeparatorsMask(separatorsMask, charsIndex,
                        ref colEndsRefCurrent);
                }
                else
                {
                    var separatorLineEndingsMask = lineEndingsSeparators.ExtractMostSignificantBits();
                    if (separatorLineEndingsMask == testMask)
                    {
                        colEndsRefCurrent = ref ParseSeparatorsLineEndingsMasks(
                            separatorsMask, separatorLineEndingsMask,
                            ref charsRef, ref charsIndex, separator,
                            ref colEndsRefCurrent, ref rowLineEndingOffset, ref lineNumber);
                        break;
                    }
                    else
                    {
                        colEndsRefCurrent = ref ParseAnyCharsMask(specialCharMask,
                            separator, ref charsRef, charsIndex,
                            ref rowLineEndingOffset, ref quoting,
                            ref colEndsRefCurrent, ref lineNumber);
                        // Used both to indicate row ended and if need to step +2 due to '\r\n'
                        if (rowLineEndingOffset != 0)
                        {
                            // Must be a col end and last is then dataIndex
                            charsIndex = colEndsRefCurrent + rowLineEndingOffset;
                            break;
                        }
                    }
                }
                // If current is greater than or equal than "stop", then break.
                // There is no longer guaranteed space enough for next VecUI8.Count.
                if (IsAddressLessThan(ref colEndsRefStop, ref colEndsRefCurrent))
                {
                    // Move data index so next find starts correctly
                    charsIndex += VecUI8.Count;
                    break;
                }
            }
        }

        // ">> 2" instead of "/ sizeof(int))" // CQ: Weird with div sizeof
        colEndsEnd = (int)(ByteOffset(ref colEndsRef, ref colEndsRefCurrent) >> 2);
        // Step is VecUI8.Count so may go past end, ensure limited
        charsIndex = Math.Min(charsEnd, charsIndex);

        _quoting = quoting;
        _rowLineEndingOffset = rowLineEndingOffset;
        _lineNumber = lineNumber;

        return charsIndex;
    }
}
