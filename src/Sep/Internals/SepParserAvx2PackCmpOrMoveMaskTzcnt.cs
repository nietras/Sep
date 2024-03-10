﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using static System.Runtime.CompilerServices.Unsafe;
using static nietras.SeparatedValues.SepDefaults;
using static nietras.SeparatedValues.SepParseMask;
using ISA = System.Runtime.Intrinsics.X86.Avx2;
using Vec = System.Runtime.Intrinsics.Vector256;
using VecI16 = System.Runtime.Intrinsics.Vector256<short>;
using VecUI8 = System.Runtime.Intrinsics.Vector256<byte>;

namespace nietras.SeparatedValues;

sealed class SepParserAvx2PackCmpOrMoveMaskTzcnt : ISepParser
{
    readonly char _separator;
    readonly VecUI8 _nls = Vec.Create(LineFeedByte);
    readonly VecUI8 _crs = Vec.Create(CarriageReturnByte);
    readonly VecUI8 _qts = Vec.Create(QuoteByte);
    readonly VecUI8 _sps;
    nuint _quoteCount = 0;

    public unsafe SepParserAvx2PackCmpOrMoveMaskTzcnt(Sep sep)
    {
        _separator = sep.Separator;
        _sps = Vec.Create((byte)_separator);
    }

    // Parses 2 x char vectors e.g. 1 byte vector
    public int PaddingLength => VecUI8.Count;
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
        var colInfosStopLength = colInfosLength - VecUI8.Count - SepReaderState.ColEndsOrInfosExtraEndCount;
        ref var colInfosRefStop = ref Add(ref colInfosRefOrigin, colInfosStopLength);

        charsIndex -= VecUI8.Count;
    LOOPSTEP:
        charsIndex += VecUI8.Count;
    LOOPNOSTEP:
        if (charsIndex < charsEnd &&
            // If current is greater than or equal than "stop", then there is no
            // longer guaranteed space enough for next VecUI8.Count + next row start.
            !IsAddressLessThan(ref colInfosRefStop, ref colInfosRefCurrent))
        {
            ref var charsRef = ref Add(ref charsOriginRef, (uint)charsIndex);
            ref var byteRef = ref As<char, byte>(ref charsRef);
            var v0 = ReadUnaligned<VecI16>(ref byteRef);
            var v1 = ReadUnaligned<VecI16>(ref Add(ref byteRef, VecUI8.Count));
            var packed = ISA.PackUnsignedSaturate(v0, v1);
            // Pack interleaves the two vectors need to permute them back
            var bytes = ISA.Permute4x64(packed.AsInt64(), 0b_11_01_10_00).AsByte();

            var nlsEq = Vec.Equals(bytes, nls);
            var crsEq = Vec.Equals(bytes, crs);
            var qtsEq = Vec.Equals(bytes, qts);
            var spsEq = Vec.Equals(bytes, sps);

            var lineEndings = nlsEq | crsEq;
            var lineEndingsSeparators = spsEq | lineEndings;
            var specialChars = lineEndingsSeparators | qtsEq;

            // Optimize for the case of no special character
            var specialCharMask = MoveMask(specialChars);
            if (specialCharMask != 0u)
            {
                var separatorsMask = MoveMask(spsEq);
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
                    var separatorLineEndingsMask = MoveMask(lineEndingsSeparators);
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
    static nuint MoveMask(VecUI8 v) => (uint)ISA.MoveMask(v);
}
