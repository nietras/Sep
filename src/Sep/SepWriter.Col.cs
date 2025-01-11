using System;
using System.Buffers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

public partial class SepWriter
{
    internal sealed class ColImpl(SepWriter writer, int index, string name)
    {
        internal const int MinimumLength = 64;
        internal readonly SepWriter _writer = writer;
        internal char[] _buffer = ArrayPool<char>.Shared.Rent(MinimumLength);
        internal int _position = 0;

        public int Index { get; } = index;
        public string Name { get; } = name;
        public bool HasBeenSet { get; set; } = false;

        public void Clear() { HasBeenSet = false; _position = 0; }

        public void Append(ReadOnlySpan<char> value)
        {
            EnsureCapacity(value.Length);
            value.CopyTo(_buffer.AsSpan(_position));
            _position += value.Length;
        }

        public ReadOnlySpan<char> GetSpan()
        {
            return _buffer.AsSpan(0, _position);
        }

        public void Dispose()
        {
            ArrayPool<char>.Shared.Return(_buffer);
            _buffer = null!;
        }

        void EnsureCapacity(int additionalLength)
        {
            if (_position + additionalLength > _buffer.Length)
            {
                GrowBuffer(additionalLength);
            }
        }

        void GrowBuffer(int additionalLength)
        {
            int newSize = Math.Max(_buffer.Length * 2, _position + additionalLength);
            char[] newBuffer = ArrayPool<char>.Shared.Rent(newSize);
            _buffer.AsSpan(0, _position).CopyTo(newBuffer);
            ArrayPool<char>.Shared.Return(_buffer);
            _buffer = newBuffer;
        }
    }

#pragma warning disable CA1815 // Override equals and operator equals on value types
    public readonly ref struct Col
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        readonly ColImpl _impl;

        internal Col(ColImpl impl)
        {
            _impl = impl;
        }

        internal int ColIndex => _impl.Index;
        internal string ColName => _impl.Name;

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA1045 // Do not pass types by reference
        public void Set([InterpolatedStringHandlerArgument("")] ref FormatInterpolatedStringHandler handler)
#pragma warning restore CA1045 // Do not pass types by reference
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
        {
            handler.Finish();
        }

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
        public void Set(IFormatProvider? provider,
#pragma warning disable CA1045 // Do not pass types by reference
            [InterpolatedStringHandlerArgument("", "provider")] ref FormatInterpolatedStringHandler handler)
#pragma warning restore CA1045 // Do not pass types by reference
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
        {
            handler.Finish();
        }

        public void Set(ReadOnlySpan<char> span)
        {
            var text = _impl;
            text.Clear();
            text.Append(span);
            MarkSet();
        }

        public void Format<T>(T value) where T : ISpanFormattable
        {
            var impl = _impl;
            impl.Clear();
            if (value.TryFormat(impl._buffer, out var charsWritten, null, impl._writer._cultureInfo))
            {
                impl._position = charsWritten;
            }
            else
            {
                var handler = new FormatInterpolatedStringHandler(0, 1, this);
                handler.AppendFormatted(value);
                handler.Finish();
            }
            MarkSet();
        }

        /// <summary>Provides a handler used by the language compiler to append interpolated strings into <see cref="Col"/> instances.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [InterpolatedStringHandler]
#pragma warning disable CA1815 // Override equals and operator equals on value types
        public ref struct FormatInterpolatedStringHandler
#pragma warning restore CA1815 // Override equals and operator equals on value types
        {
            readonly ColImpl _impl;
            DefaultInterpolatedStringHandler _handler;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_pos")]
            static extern ref int Position(ref DefaultInterpolatedStringHandler handler);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_arrayToReturnToPool")]
            static extern ref char[]? ArrayToReturnToPool(ref DefaultInterpolatedStringHandler handler);

            public FormatInterpolatedStringHandler(int literalLength, int formattedCount, Col col)
            {
                _impl = col._impl;
                _impl.Clear();
                _handler = new(literalLength, formattedCount, _impl._writer._cultureInfo, _impl._buffer);
                Position(ref _handler) = _impl._position;
                ArrayToReturnToPool(ref _handler) = _impl._buffer;
            }


            public FormatInterpolatedStringHandler(int literalLength, int formattedCount, Col col, IFormatProvider? provider)
            {
                _impl = col._impl;
                _impl.Clear();
                _handler = new(literalLength, formattedCount, provider, _impl._buffer);
                Position(ref _handler) = _impl._position;
                ArrayToReturnToPool(ref _handler) = _impl._buffer;
            }

            public void AppendLiteral(string value)
            {
                _handler.AppendLiteral(value);
                MarkSet();
            }

            public void AppendFormatted<T>(T value)
            {
                _handler.AppendFormatted(value);
                MarkSet();
            }

            public void AppendFormatted<T>(T value, string? format)
            {
                _handler.AppendFormatted(value, format);
                MarkSet();
            }

            public void AppendFormatted<T>(T value, int alignment)
            {
                _handler.AppendFormatted(value, alignment);
                MarkSet();
            }

            public void AppendFormatted<T>(T value, int alignment, string? format)
            {
                _handler.AppendFormatted(value, alignment, format);
                MarkSet();
            }

            public void AppendFormatted(ReadOnlySpan<char> value)
            {
                _handler.AppendFormatted(value);
                MarkSet();
            }

            public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null)
            {
                _handler.AppendFormatted(value, alignment, format);
                MarkSet();
            }

            public void AppendFormatted(string? value)
            {
                _handler.AppendFormatted(value);
                MarkSet();
            }

            public void AppendFormatted(string? value, int alignment = 0, string? format = null)
            {
                _handler.AppendFormatted(value, alignment, format);
                MarkSet();
            }

            public void AppendFormatted(object? value, int alignment = 0, string? format = null)
            {
                _handler.AppendFormatted(value, alignment, format);
                MarkSet();
            }

            void MarkSet() => _impl.HasBeenSet = true;

            internal void Finish()
            {
                ref var handlerArrayRef = ref ArrayToReturnToPool(ref _handler);
                A.Assert(handlerArrayRef is not null);
                _impl._buffer = handlerArrayRef!;
                _impl._position = Position(ref _handler);
                handlerArrayRef = null;
                _handler = default;
            }
        }

        void MarkSet() => _impl.HasBeenSet = true;
    }
}
