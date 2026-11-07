using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using static System.Runtime.CompilerServices.Unsafe;
using static nietras.SeparatedValues.SepParseMask;
using ISA = System.Runtime.Intrinsics.X86.Avx512BW;
using Vec = System.Runtime.Intrinsics.Vector512;
using VecUI16 = System.Runtime.Intrinsics.Vector512<ushort>;
using VecUI8 = System.Runtime.Intrinsics.Vector512<byte>;

namespace nietras.SeparatedValues;

[ExcludeFromCodeCoverage]
#if SEPUSESTRUCTFORPARSERSFORDISASMO
struct
#else
sealed class
#endif
SepParserAvx512To2x256To512CmpOrMoveMaskTzcnt : ISepParser
{
    // Parses 4 x char vectors e.g. 2 x 1 byte vector
    public int PaddingLength => VecUI8.Count;

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    void ISepParser.ParseColEnds(SepReaderState s, SepParserOptions options)
    {
        ParseColEnds(s, options);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    internal static void ParseColEnds(SepReaderState s, SepParserOptions options)
    {
        Parse<int, SepColEndMethods>(s, options);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    void ISepParser.ParseColInfos(SepReaderState s, SepParserOptions options)
    {
        ParseColInfos(s, options);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    internal static void ParseColInfos(SepReaderState s, SepParserOptions options)
    {
        Parse<SepColInfo, SepColInfoMethods>(s, options);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Parse<TColInfo, TColInfoMethods>(SepReaderState s, SepParserOptions options)
        where TColInfo : unmanaged
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
    {
        // Method should **not** call other non-inlined methods, since this
        // impacts code-generation severely.

        var separator = (char)options.Separator;
        var nls = Vec.Create(options.LineFeed);
        var crs = Vec.Create(options.CarriageReturn);
        var qts = Vec.Create(options.QuotesOrSeparatorIfDisabled);
        var sps = Vec.Create(options.Separator);

        // Unpack state fields
        var quoteCount = s._parsingRowQuoteCount;
        var chars = s._chars;
        var charsIndex = s._charsParseStart;
        var charsEnd = s._charsDataEnd;
        var lineNumber = s._parsingLineNumber;
        var colInfos = s._colEndsOrColInfos;

        var colInfosLength = TColInfoMethods.IntsLengthToColInfosLength(colInfos.Length);

        chars.CheckPaddingAndIsZero(charsEnd, VecUI8.Count);
        SepArrayExtensions.CheckPadding(colInfosLength, s._parsingRowColCount + s._parsingRowColEndsOrInfosStartIndex, VecUI8.Count);
        A.Assert(charsIndex <= charsEnd);
        A.Assert(charsEnd <= (chars.Length - VecUI8.Count));

        ref var colInfosRefOrigin = ref As<int, TColInfo>(ref MemoryMarshal.GetArrayDataReference(colInfos));
        ref var colInfosRefEnd = ref Add(ref colInfosRefOrigin, colInfosLength);
        ref var colInfosRefStop = ref Add(ref colInfosRefEnd, -VecUI8.Count - SepReaderState.ColEndsOrInfosExtraEndCount);
        ref var colInfosRef = ref Add(ref colInfosRefOrigin, s._parsingRowColEndsOrInfosStartIndex);
        ref var colInfosRefCurrent = ref Add(ref colInfosRef, s._parsingRowColCount);

        ref var charsOriginRef = ref MemoryMarshal.GetArrayDataReference(chars);

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
            var v0 = ReadUnaligned<VecUI16>(ref As<char, byte>(ref charsRef));
            var v1 = ReadUnaligned<VecUI16>(ref Add(ref As<char, byte>(ref charsRef), VecUI8.Count));
            var bytes0 = ISA.ConvertToVector256ByteWithSaturation(v0);
            var bytes1 = ISA.ConvertToVector256ByteWithSaturation(v1);
            var bytes = bytes0.ToVector512Unsafe().WithUpper(bytes1);

            var lineEndings = MoveMask(Vec.Equals(bytes, nls)) | MoveMask(Vec.Equals(bytes, crs));
            var separatorsMask = MoveMask(Vec.Equals(bytes, sps));
            var separatorLineEndingsMask = separatorsMask | lineEndings;
            var specialCharMask = separatorLineEndingsMask | MoveMask(Vec.Equals(bytes, qts));

            // Optimize for the case of no special character
            if (specialCharMask != 0u)
            {
                // Optimize for case of only separators i.e. no endings or quotes.
                // Combine mask mismatch and quote state without overflowing a full-width mask.
                if (((separatorsMask ^ specialCharMask) | quoteCount) == 0)
                {
                    colInfosRefCurrent = ref ParseSeparatorsMask<TColInfo, TColInfoMethods>(
                        separatorsMask, charsIndex, ref colInfosRefCurrent);
                }
                else
                {
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
        // Update reader state from enregistered
        s._parsingRowQuoteCount = quoteCount;
        s._parsingRowColCount = TColInfoMethods.CountOffset(ref colInfosRef, ref colInfosRefCurrent);
        s._parsingLineNumber = lineNumber;
        // Step is VecUI8.Count so may go past end, ensure limited
        s._charsParseStart = Math.Min(charsEnd, charsIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static nuint MoveMask(VecUI8 v) => (nuint)Vec.ExtractMostSignificantBits(v);
}
