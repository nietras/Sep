// Copyright (c) 2023, Olaf Kober <olaf.kober@outlook.com>

using System;
using System.Runtime.CompilerServices;
using nietras.SeparatedValues;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;


namespace Sep.Internals;

internal sealed class SepParserSve : ISepParser
{
    public SepParserSve(SepParserOptions options)
    {
        // TODO: Initialize any SVE specific fields based on options if necessary
    }

    public int PaddingLength => throw new NotImplementedException();

    public int QuoteCount => throw new NotImplementedException();


    public void ParseColEnds(SepReaderState s)
    {
        throw new NotImplementedException();
    }

    public void ParseColInfos(SepReaderState s)
    {
        throw new NotImplementedException();
    }
}
