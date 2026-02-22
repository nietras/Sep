using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace nietras.SeparatedValues;

public partial class SepUtf8Reader
{
    public delegate T ColFunc<T>(Col col)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        ;
    public delegate void ColAction(Col col);
    public delegate void ColsAction(Cols col);

    public readonly ref struct Cols
    {
        readonly SepUtf8ReaderState _state;
        readonly ReadOnlySpan<int> _colIndices;
        readonly int _colStartIfRange;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Cols(SepUtf8ReaderState state, ReadOnlySpan<int> indices)
        {
            _state = state;
            _colIndices = indices;
            _colStartIfRange = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Cols(SepUtf8ReaderState state, int colStart, int colCount)
        {
            _state = state;
            _colIndices = MemoryMarshal.CreateSpan(ref Unsafe.NullRef<int>(), colCount);
            _colStartIfRange = colStart;
        }

        public int Count => _colIndices.Length;

        public Col this[int index]
        {
            get => new(_state, IsIndices() ? _colIndices[index] : GetRangeIndex(index));
        }

        public string[] ToStringsArray() => IsIndices()
            ? _state.ToStringsArray(_colIndices)
            : _state.ToStringsArray(_colStartIfRange, _colIndices.Length);

        public T[] ParseToArray<T>() where T : IUtf8SpanParsable<T> => IsIndices()
            ? _state.ParseToArray<T>(_colIndices)
            : _state.ParseToArray<T>(_colStartIfRange, _colIndices.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsIndices() => _colStartIfRange < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetRangeIndex(int index)
        {
            A.Assert(_colStartIfRange >= 0);
            return _colStartIfRange + index;
        }
    }
}
