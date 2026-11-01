using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.Unsafe;
using static nietras.SeparatedValues.SepDefaults;
using static nietras.SeparatedValues.SepParseMask;
using ISA = System.Runtime.Intrinsics.X86.Avx512BW;
using Vec = System.Runtime.Intrinsics.Vector256;
using VecUI16 = System.Runtime.Intrinsics.Vector512<ushort>;
using VecUI8 = System.Runtime.Intrinsics.Vector256<byte>;

namespace nietras.SeparatedValues;

[ExcludeFromCodeCoverage]
#if SEPUSESTRUCTFORPARSERSFORDISASMO
struct
#else
sealed class
#endif
SepParserAvx512To256CmpOrMoveMaskTzcnt : ISepParser
{
    readonly char _separator;
    readonly VecUI8 _nls = Vec.Create(LineFeedByte);
    readonly VecUI8 _crs = Vec.Create(CarriageReturnByte);
    readonly VecUI8 _qts;
    readonly VecUI8 _sps;
    nuint _quoteCount = 0;

    public SepParserAvx512To256CmpOrMoveMaskTzcnt(SepParserOptions options)
    {
        _separator = options.Separator;
        _sps = Vec.Create((byte)_separator);
        _qts = Vec.Create((byte)options.QuotesOrSeparatorIfDisabled);
    }

    // Quoted blocks combine two byte-vector masks on 64-bit processes.
    public int PaddingLength => SizeOf<nuint>() == sizeof(ulong) ? VecUI8.Count * 2 : VecUI8.Count;
    public int QuoteCount => (int)_quoteCount;

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void ParseColEnds(SepReaderState s)
    {
        _quoteCount = Parse<int, SepColEndMethods>(s);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void ParseColInfos(SepReaderState s)
    {
        _quoteCount = Parse<SepColInfo, SepColInfoMethods>(s);
    }

    // Return quote state so the parser reference need not stay live throughout either parsing loop.
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    nuint Parse<TColInfo, TColInfoMethods>(SepReaderState s)
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
        var colInfosStopLength = colInfosLength - PaddingLength - SepReaderState.ColEndsOrInfosExtraEndCount;
        ref var colInfosRefStop = ref Add(ref colInfosRefOrigin, colInfosStopLength);

        charsIndex -= VecUI8.Count;
    LOOPSTEP:
        charsIndex += VecUI8.Count;
    LOOPNOSTEP:
        if (charsIndex < charsEnd &&
            // If current is greater than or equal than "stop", then there is no
            // longer guaranteed space enough for a full quoted block + next row start.
            !IsAddressLessThan(ref colInfosRefStop, ref colInfosRefCurrent))
        {
            ref var charsRef = ref Add(ref charsOriginRef, (uint)charsIndex);
            ref var byteRef = ref As<char, byte>(ref charsRef);
            var v = ReadUnaligned<VecUI16>(ref byteRef);
            var bytes = ISA.ConvertToVector256ByteWithSaturation(v);

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
                // Combine mask mismatch and quote state without overflowing a full-width mask.
                if (((separatorsMask ^ specialCharMask) | quoteCount) == 0)
                {
                    colInfosRefCurrent = ref ParseSeparatorsMask<TColInfo, TColInfoMethods>(
                        separatorsMask, charsIndex, ref colInfosRefCurrent);
                }
                else
                {
                    var separatorLineEndingsMask = MoveMask(lineEndingsSeparators);
                    if (((separatorLineEndingsMask ^ specialCharMask) | quoteCount) == 0)
                    {
                        colInfosRefCurrent = ref ParseSeparatorsLineEndingsMasks<TColInfo, TColInfoMethods>(
                            separatorsMask, separatorLineEndingsMask,
                            ref charsRef, ref charsIndex, separator,
                            ref colInfosRefCurrent, ref lineNumber);
                        goto NEWROW;
                    }
                    else
                    {
                        if (SizeOf<nuint>() == sizeof(ulong))
                        {
                            var next = ReadUnaligned<VecUI16>(ref Add(ref byteRef, VecUI8.Count * sizeof(char)));
                            var nextBytes = ISA.ConvertToVector256ByteWithSaturation(next);
                            var nextSpsEq = Vec.Equals(nextBytes, sps);
                            var nextLineEndings = Vec.Equals(nextBytes, nls) | Vec.Equals(nextBytes, crs);
                            var nextSeparatorLineEndings = nextSpsEq | nextLineEndings;
                            separatorsMask |= MoveMask(nextSpsEq) << VecUI8.Count;
                            separatorLineEndingsMask |= MoveMask(nextSeparatorLineEndings) << VecUI8.Count;
                            specialCharMask |= MoveMask(nextSeparatorLineEndings | Vec.Equals(nextBytes, qts)) << VecUI8.Count;
                        }
                        var rowLineEndingOffset = 0;
                        colInfosRefCurrent = ref ParseQuotedMask<TColInfo, TColInfoMethods>(
                            separatorsMask, separatorLineEndingsMask, specialCharMask,
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
                        if (SizeOf<nuint>() == sizeof(ulong)) { charsIndex += VecUI8.Count; }
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
        s._parsingRowColCount = TColInfoMethods.CountOffset(ref colInfosRef, ref colInfosRefCurrent);
        s._parsingLineNumber = lineNumber;
        // One or two vector steps may go past end, ensure limited
        s._charsParseStart = Math.Min(charsEnd, charsIndex);
        return quoteCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static nuint MoveMask(VecUI8 v) => (uint)ISA.MoveMask(v);
}
