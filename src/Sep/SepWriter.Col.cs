using System;
using System.Buffers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace nietras.SeparatedValues;

public partial class SepWriter
{
    internal sealed class ColImpl
    {
        internal readonly SepWriter _writer;

        public ColImpl(SepWriter writer, int index, string name, ColBuilder text)
        {
            _writer = writer;
            Index = index;
            Name = name;
            Text = text;
        }

        public int Index { get; private set; }
        public string Name { get; }
        public ColBuilder Text { get; }
        public bool HasBeenSet { get; set; } = false;

        public void Clear() { HasBeenSet = false; Text.Clear(); }
    }

    internal sealed class ColBuilder : IDisposable
    {
        private const int MinimumLength = 64;
        private char[] _buffer;
        private int _position;

        public ColBuilder()
        {
            _buffer = ArrayPool<char>.Shared.Rent(MinimumLength);
            _position = 0;
        }

        public void Append(ReadOnlySpan<char> value)
        {
            EnsureCapacity(value.Length);
            value.CopyTo(_buffer.AsSpan(_position));
            _position += value.Length;
        }

        public void Clear()
        {
            _position = 0;
        }

        public ReadOnlyMemory<char> GetMemory()
        {
            return _buffer.AsMemory(0, _position);
        }

        public ReadOnlySpan<char> GetSpan()
        {
            return _buffer.AsSpan(0, _position);
        }

        private void EnsureCapacity(int additionalLength)
        {
            if (_position + additionalLength > _buffer.Length)
            {
                GrowBuffer(additionalLength);
            }
        }

        private void GrowBuffer(int additionalLength)
        {
            int newSize = Math.Max(_buffer.Length * 2, _position + additionalLength);
            char[] newBuffer = ArrayPool<char>.Shared.Rent(newSize);
            _buffer.AsSpan(0, _position).CopyTo(newBuffer);
            ArrayPool<char>.Shared.Return(_buffer);
            _buffer = newBuffer;
        }

        public void Dispose()
        {
            ArrayPool<char>.Shared.Return(_buffer);
            _buffer = null!;
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
        { }

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
        public void Set(IFormatProvider? provider,
#pragma warning disable CA1045 // Do not pass types by reference
            [InterpolatedStringHandlerArgument("", "provider")] ref FormatInterpolatedStringHandler handler)
#pragma warning restore CA1045 // Do not pass types by reference
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
        { }

        public void Set(ReadOnlySpan<char> span)
        {
            var text = _impl.Text;
            text.Clear();
            text.Append(span);
            MarkSet();
        }

        public void Format<T>(T value) where T : ISpanFormattable
        {
            var impl = _impl;
            var text = impl.Text;
            text.Clear();
            var handler = new ColBuilder.AppendInterpolatedStringHandler(0, 1, text, impl._writer._cultureInfo);
            handler.AppendFormatted(value);
            MarkSet();
        }

        /// <summary>Provides a handler used by the language compiler to append interpolated strings into <see cref="Col"/> instances.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [InterpolatedStringHandler]
#pragma warning disable CA1815 // Override equals and operator equals on value types
        public readonly struct FormatInterpolatedStringHandler
#pragma warning restore CA1815 // Override equals and operator equals on value types
        {
            readonly ColImpl _impl;
            readonly ColBuilder.AppendInterpolatedStringHandler _handler;

            public FormatInterpolatedStringHandler(int literalLength, int formattedCount, Col col)
            {
                _impl = col._impl;
                var text = _impl.Text;
                text.Clear();
                _handler = new(literalLength, formattedCount, text, _impl._writer._cultureInfo);
            }

            public FormatInterpolatedStringHandler(int literalLength, int formattedCount, Col col, IFormatProvider? provider)
            {
                _impl = col._impl;
                var text = _impl.Text;
                text.Clear();
                _handler = new(literalLength, formattedCount, text, provider);
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
        }

        void MarkSet() => _impl.HasBeenSet = true;
    }
}
