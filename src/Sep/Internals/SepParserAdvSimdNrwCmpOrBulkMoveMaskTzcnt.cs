using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using static System.Runtime.CompilerServices.Unsafe;
using static nietras.SeparatedValues.SepDefaults;
using static nietras.SeparatedValues.SepParseMask;
using Vec = System.Runtime.Intrinsics.Vector128;
using VecUI16 = System.Runtime.Intrinsics.Vector128<ushort>;
using VecUI8 = System.Runtime.Intrinsics.Vector128<byte>;

namespace nietras.SeparatedValues;

// Based primarily on Geoff Langdale's:
// "Fitting My Head Through The ARM Holes or:
//  Two Sequences to Substitute for the Missing PMOVMSKB Instruction on ARM NEON"
// https://branchfree.org/2019/04/01/fitting-my-head-through-the-arm-holes-or-two-sequences-to-substitute-for-the-missing-pmovmskb-instruction-on-arm-neon/
// Other sources for inspiration:
// https://lemire.me/blog/2017/07/10/pruning-spaces-faster-on-arm-processors-with-vector-table-lookups/
// https://community.arm.com/arm-community-blogs/b/servers-and-cloud-computing-blog/posts/porting-x86-vector-bitmask-optimizations-to-arm-neon
// https://developer.arm.com/architectures/instruction-sets/intrinsics
sealed class SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt : ISepParser
{
    static readonly int LoopCount = VecUI8.Count * 4;
    readonly char _separator;
    readonly VecUI8 _nls = Vec.Create(LineFeedByte);
    readonly VecUI8 _crs = Vec.Create(CarriageReturnByte);
    readonly VecUI8 _qts;
    readonly VecUI8 _sps;

    nuint _quoteCount = 0;

    public unsafe SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt(SepParserOptions options)
    {
        _separator = options.Separator;
        _sps = Vec.Create((byte)_separator);
        _qts = Vec.Create((byte)options.QuotesOrSeparatorIfDisabled);
    }

    // Parses 8 x char vectors e.g. 4 byte vector
    public int PaddingLength => LoopCount;
    public int QuoteCount => (int)_quoteCount;

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void ParseColEnds(SepReaderState s)
    {
        Parse<int, SepColEndMethods>(s);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void ParseColInfos(SepReaderState s)
    {
        Parse<SepColInfo, SepColInfoMethods>(s);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Parse<TColInfo, TColInfoMethods>(SepReaderState s)
        where TColInfo : unmanaged
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
    {
        // Method should **not** call other non-inlined methods, since this
        // impacts code-generation severely.

        // Unpack instance fields
        var separator = _separator;
        var quoteCount = _quoteCount;
        // Use instance fields to force values into registers
        var nls = _nls; //Vec.Create(LineFeedByte);
        var crs = _crs; //Vec.Create(CarriageReturnByte);
        var qts = _qts; //Vec.Create(QuoteByte);
        var sps = _sps; //Vec.Create(_separator);

        // Unpack state fields
        var chars = s._chars;
        var charsIndex = s._charsParseStart;
        var charsEnd = s._charsDataEnd;
        var lineNumber = s._parsingLineNumber;
        var colInfos = s._colEndsOrColInfos;

        var colInfosLength = TColInfoMethods.IntsLengthToColInfosLength(colInfos.Length);

        chars.CheckPaddingAndIsZero(charsEnd, PaddingLength);
        SepArrayExtensions.CheckPadding(colInfosLength, s._parsingRowColCount + s._parsingRowColEndsOrInfosStartIndex, PaddingLength);
        A.Assert(charsIndex <= charsEnd);
        A.Assert(charsEnd <= (chars.Length - PaddingLength));

        ref var charsOriginRef = ref MemoryMarshal.GetArrayDataReference(chars);

        ref var colInfosRefOrigin = ref As<int, TColInfo>(ref MemoryMarshal.GetArrayDataReference(colInfos));
        ref var colInfosRef = ref Add(ref colInfosRefOrigin, s._parsingRowColEndsOrInfosStartIndex);
        ref var colInfosRefCurrent = ref Add(ref colInfosRefOrigin, s._parsingRowColCount + s._parsingRowColEndsOrInfosStartIndex);
        ref var colInfosRefEnd = ref Add(ref colInfosRefOrigin, colInfosLength);
        var colInfosStopLength = colInfosLength - LoopCount - SepReaderState.ColEndsOrInfosExtraEndCount;
        ref var colInfosRefStop = ref Add(ref colInfosRefOrigin, colInfosStopLength);

        charsIndex -= LoopCount;
    LOOPSTEP:
        charsIndex += LoopCount;
    LOOPNOSTEP:
        if (charsIndex < charsEnd &&
            // If current is greater than or equal than "stop", then there is no
            // longer guaranteed space enough for next VecUI8.Count + next row start.
            !IsAddressLessThan(ref colInfosRefStop, ref colInfosRefCurrent))
        {
            ref var charsRef = ref Add(ref charsOriginRef, (uint)charsIndex);
            var bytes0 = ReadNarrow(ref charsRef);
            var bytes1 = ReadNarrow(ref Add(ref charsRef, VecUI8.Count * 1));
            var bytes2 = ReadNarrow(ref Add(ref charsRef, VecUI8.Count * 2));
            var bytes3 = ReadNarrow(ref Add(ref charsRef, VecUI8.Count * 3));

            var nlsEq0 = AdvSimd.CompareEqual(bytes0, nls);
            var crsEq0 = AdvSimd.CompareEqual(bytes0, crs);
            var qtsEq0 = AdvSimd.CompareEqual(bytes0, qts);
            var spsEq0 = AdvSimd.CompareEqual(bytes0, sps);
            var lineEndings0 = AdvSimd.Or(nlsEq0, crsEq0);
            var lineEndingsSeparators0 = AdvSimd.Or(spsEq0, lineEndings0);
            var specialChars0 = AdvSimd.Or(lineEndingsSeparators0, qtsEq0);

            var nlsEq1 = AdvSimd.CompareEqual(bytes1, nls);
            var crsEq1 = AdvSimd.CompareEqual(bytes1, crs);
            var qtsEq1 = AdvSimd.CompareEqual(bytes1, qts);
            var spsEq1 = AdvSimd.CompareEqual(bytes1, sps);
            var lineEndings1 = AdvSimd.Or(nlsEq1, crsEq1);
            var lineEndingsSeparators1 = AdvSimd.Or(spsEq1, lineEndings1);
            var specialChars1 = AdvSimd.Or(lineEndingsSeparators1, qtsEq1);

            var nlsEq2 = AdvSimd.CompareEqual(bytes2, nls);
            var crsEq2 = AdvSimd.CompareEqual(bytes2, crs);
            var qtsEq2 = AdvSimd.CompareEqual(bytes2, qts);
            var spsEq2 = AdvSimd.CompareEqual(bytes2, sps);
            var lineEndings2 = AdvSimd.Or(nlsEq2, crsEq2);
            var lineEndingsSeparators2 = AdvSimd.Or(spsEq2, lineEndings2);
            var specialChars2 = AdvSimd.Or(lineEndingsSeparators2, qtsEq2);

            var nlsEq3 = AdvSimd.CompareEqual(bytes3, nls);
            var crsEq3 = AdvSimd.CompareEqual(bytes3, crs);
            var qtsEq3 = AdvSimd.CompareEqual(bytes3, qts);
            var spsEq3 = AdvSimd.CompareEqual(bytes3, sps);
            var lineEndings3 = AdvSimd.Or(nlsEq3, crsEq3);
            var lineEndingsSeparators3 = AdvSimd.Or(spsEq3, lineEndings3);
            var specialChars3 = AdvSimd.Or(lineEndingsSeparators3, qtsEq3);

            // Optimize for the case of no special character
            var specialCharMask = MoveMask(specialChars0, specialChars1,
                                           specialChars2, specialChars3);
            if (specialCharMask != 0u)
            {
                var separatorsMask = MoveMask(spsEq0, spsEq1, spsEq2, spsEq3);
                // Optimize for case of only separators i.e. no endings or quotes.
                // Add quote count to mask as hack to skip if quoting.
                var testMask = specialCharMask + quoteCount;
                if (separatorsMask == testMask)
                {
                    colInfosRefCurrent = ref ParseSeparatorsMask<TColInfo, TColInfoMethods>(
                        separatorsMask, charsIndex, ref colInfosRefCurrent);
                }
                else
                {
                    var separatorLineEndingsMask = MoveMask(
                        lineEndingsSeparators0, lineEndingsSeparators1,
                        lineEndingsSeparators2, lineEndingsSeparators3);
                    if (separatorLineEndingsMask == testMask)
                    {
                        colInfosRefCurrent = ref ParseSeparatorsLineEndingsMasks<TColInfo, TColInfoMethods>(
                            separatorsMask, separatorLineEndingsMask,
                            ref charsRef, ref charsIndex, separator,
                            ref colInfosRefCurrent, ref lineNumber);
                        goto NEWROW;
                    }
                    else
                    {
                        var rowLineEndingOffset = 0;
                        colInfosRefCurrent = ref ParseAnyCharsMask<TColInfo, TColInfoMethods>(specialCharMask,
                            separator, ref charsRef, charsIndex,
                            ref rowLineEndingOffset, ref quoteCount,
                            ref colInfosRefCurrent, ref lineNumber);
                        // Used both to indicate row ended and if need to step +2 due to '\r\n'
                        if (rowLineEndingOffset != 0)
                        {
                            // Must be a col end and last is then dataIndex
                            charsIndex = TColInfoMethods.GetColEnd(colInfosRefCurrent) + rowLineEndingOffset;
                            goto NEWROW;
                        }
                    }
                }
            }
            goto LOOPSTEP;
        NEWROW:
            var colCount = TColInfoMethods.CountOffset(ref colInfosRef, ref colInfosRefCurrent);
            // Add new parsed row
            ref var parsedRowRef = ref MemoryMarshal.GetArrayDataReference(s._parsedRows);
            Add(ref parsedRowRef, s._parsedRowsCount) = new(lineNumber, colCount);
            ++s._parsedRowsCount;
            // Next row start (one before)
            colInfosRefCurrent = ref Add(ref colInfosRefCurrent, 1);
            A.Assert(IsAddressLessThan(ref colInfosRefCurrent, ref colInfosRefEnd));
            colInfosRefCurrent = TColInfoMethods.Create(charsIndex - 1, 0);
            // Update for next row
            colInfosRef = ref colInfosRefCurrent;
            s._parsingRowColEndsOrInfosStartIndex += colCount + 1;
            s._parsingRowCharsStartIndex = charsIndex;
            // Space for more rows?
            if (s._parsedRowsCount < s._parsedRows.Length)
            {
                goto LOOPNOSTEP;
            }
        }
        // Update instance state from enregistered
        _quoteCount = quoteCount;
        s._parsingRowColCount = TColInfoMethods.CountOffset(ref colInfosRef, ref colInfosRefCurrent);
        s._parsingLineNumber = lineNumber;
        // Step is VecUI8.Count so may go past end, ensure limited
        s._charsParseStart = Math.Min(charsEnd, charsIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static VecUI8 ReadNarrow(ref char charsRef)
    {
        ref var byteRef = ref As<char, byte>(ref charsRef);
        var v0 = ReadUnaligned<VecUI16>(ref byteRef);
        var v1 = ReadUnaligned<VecUI16>(ref Add(ref byteRef, VecUI8.Count));

        var r0 = AdvSimd.ExtractNarrowingSaturateLower(v0);
        var r1 = AdvSimd.ExtractNarrowingSaturateUpper(r0, v1);
        return r1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static nuint MoveMask(VecUI8 p0, VecUI8 p1, VecUI8 p2, VecUI8 p3)
    {
        // Combine with shifting to pack into one vector
        var t0 = AdvSimd.ShiftRightAndInsert(p1, p0, 1); // vsriq_n_u8(p1, p0, 1)
        var t1 = AdvSimd.ShiftRightAndInsert(p3, p2, 1); // vsriq_n_u8(p3, p2, 1)
        var t2 = AdvSimd.ShiftRightAndInsert(t1, t0, 2); // vsriq_n_u8(t1, t0, 2)
        var t3 = AdvSimd.ShiftRightAndInsert(t2, t2, 4); // vsriq_n_u8(t2, t2, 4)

        // Narrow to 8 bytes (shift right by 4 bits each 16-bit lane, then take lower half)
        var t4 = AdvSimd.ShiftRightLogicalNarrowingLower(t3.AsUInt16(), 4); // vshrn_n_u16
        return (nuint)t4.AsUInt64().GetElement(0);

        /* Credit aqrit (comment in Geoff Langdale blog post)
        uint64_t NEON_i8x64_MatchMask(const uint8_t* ptr, uint8_t match_byte)
        {
            uint8x16x4_t src = vld4q_u8(ptr); // NOT CURRENTLY AVAILABLE IN .NET 9
            uint8x16_t dup = vdupq_n_u8(match_byte);
            uint8x16_t cmp0 = vceqq_u8(src.val[0], dup);
            uint8x16_t cmp1 = vceqq_u8(src.val[1], dup);
            uint8x16_t cmp2 = vceqq_u8(src.val[2], dup);
            uint8x16_t cmp3 = vceqq_u8(src.val[3], dup);

            uint8x16_t t0 = vsriq_n_u8(cmp1, cmp0, 1);
            uint8x16_t t1 = vsriq_n_u8(cmp3, cmp2, 1);
            uint8x16_t t2 = vsriq_n_u8(t1, t0, 2);
            uint8x16_t t3 = vsriq_n_u8(t2, t2, 4);
            uint8x8_t t4 = vshrn_n_u16(vreinterpretq_u16_u8(t3), 4);
            return vget_lane_u64(vreinterpret_u64_u8(t4), 0);
        }
        */
    }
}
