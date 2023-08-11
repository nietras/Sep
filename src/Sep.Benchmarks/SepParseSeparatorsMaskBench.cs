using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace nietras.SeparatedValues.Benchmarks;

public unsafe class SepParseSeparatorsMaskBench
{
    public readonly record struct MaskSpec(uint Mask)
    {
        public override string ToString() => Convert.ToString(Mask, 2).PadLeft(32, '0');
    }

    readonly MaskSpec[] _masks;
    MaskSpec _mask;
    uint _maskValue;
    readonly int _dataIndex = 17;
    readonly int* _colEnds = (int*)NativeMemory.Alloc(32, sizeof(int));

    public SepParseSeparatorsMaskBench()
    {
        _masks = new MaskSpec[]
        {
            new(0b0000_0000_0000_0000_0000_0000_0000_0100),
            new(0b0000_0000_0000_0000_0000_0010_0000_0100),
            new(0b0001_0000_0000_0100_0000_0010_0000_0100),
            new(0b0001_0100_0100_0100_0100_0010_0010_0100),
        };
        Mask = _masks.First();
    }

    [ParamsSource(nameof(Masks))]
    public MaskSpec Mask
    {
        get => _mask;
        set { _mask = value; _maskValue = value.Mask; }
    }
    public IEnumerable<MaskSpec> Masks() => _masks;

    [Benchmark]
    public unsafe ref int DummyForWarmup()
    {
        return ref SepParseMask.ParseSeparatorsMask(
            _maskValue, _dataIndex, ref *_colEnds);
    }

    [Benchmark(Baseline = true)]
    public unsafe ref int ParseSeparatorsMask()
    {
        return ref SepParseMask.ParseSeparatorsMask(
            _maskValue, _dataIndex, ref *_colEnds);
    }

    [Benchmark]
    public unsafe ref int ParseSeparatorsMaskLong()
    {
        return ref SepParseMask.ParseSeparatorsMaskLong(
            _maskValue, _dataIndex, ref *_colEnds);
    }
}
