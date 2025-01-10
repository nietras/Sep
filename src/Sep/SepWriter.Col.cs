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
        private const int MinimumLength = 64;
        private char[] _buffer;
        private int _position;

        public ColImpl(SepWriter writer, int index, string name)
        {
            _writer = writer;
            Index = index;
            Name = name;
            _buffer = ArrayPool<char>.Shared.Rent(MinimumLength);
            _position = 0;
        }

        public int Index { get; private set; }
        public string Name { get; }
        public bool HasBeenSet { get; set; } = false;

        public void Clear() { HasBeenSet = false; _position = 0; }

        public void Append(ReadOnlySpan<char> value)
        {
            EnsureCapacity(value.Length);
            value.CopyTo(_buffer.AsSpan(_position));
            _position += value.Length;
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

        [InterpolatedStringHandler]
        public ref struct AppendInterpolatedStringHandler
        {
            private ColImpl _builder;
            private IFormatProvider? _provider;

            public AppendInterpolatedStringHandler(int literalLength, int formattedCount, ColImpl builder, IFormatProvider? provider = null)
            {
                _builder = builder;
                _provider = provider;
            }

            public void AppendLiteral(string value)
            {
                _builder.Append(value.AsSpan());
            }

            public void AppendFormatted<T>(T value)
            {
                if (value is IFormattable formattable)
                {
                    _builder.Append(formattable.ToString(null, _provider).AsSpan());
                }
                else
                {
                    _builder.Append(value?.ToString().AsSpan() ?? ReadOnlySpan<char>.Empty);
                }
            }

            public void AppendFormatted<T>(T value, string? format)
            {
                if (value is IFormattable formattable)
                {
                    _builder.Append(formattable.ToString(format, _provider).AsSpan());
                }
                else
                {
                    _builder.Append(value?.ToString().AsSpan() ?? ReadOnlySpan<char>.Empty);
                }
            }

            public void AppendFormatted<T>(T value, int alignment)
            {
                AppendFormatted(value, alignment, null);
            }

            public void AppendFormatted<T>(T value, int alignment, string? format)
            {
                string? formattedValue;
                if (value is IFormattable formattable)
                {
                    formattedValue = formattable.ToString(format, _provider);
                }
                else
                {
                    formattedValue = value?.ToString();
                }

                if (formattedValue != null)
                {
                    if (alignment > 0)
                    {
                        _builder.Append(formattedValue.PadLeft(alignment).AsSpan());
                    }
                    else if (alignment < 0)
                    {
                        _builder.Append(formattedValue.PadRight(-alignment).AsSpan());
                    }
                    else
                    {
                        _builder.Append(formattedValue.AsSpan());
                    }
                }
            }

            public void AppendFormatted(ReadOnlySpan<char> value)
            {
                _builder.Append(value);
            }

            public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null)
            {
                if (alignment > 0)
                {
                    _builder.Append(value.ToString().PadLeft(alignment).AsSpan());
                }
                else if (alignment < 0)
                {
                    _builder.Append(value.ToString().PadRight(-alignment).AsSpan());
                }
                else
                {
                    _builder.Append(value);
                }
            }

            public void AppendFormatted(string? value)
            {
                if (value != null)
                {
                    _builder.Append(value.AsSpan());
                }
            }

            public void AppendFormatted(string? value, int alignment = 0, string? format = null)
            {
                if (value != null)
                {
                    if (alignment > 0)
                    {
                        _builder.Append(value.PadLeft(alignment).AsSpan());
                    }
                    else if (alignment < 0)
                    {
                        _builder.Append(value.PadRight(-alignment).AsSpan());
                    }
                    else
                    {
                        _builder.Append(value.AsSpan());
                    }
                }
            }

            public void AppendFormatted(object? value, int alignment = 0, string? format = null)
            {
                if (value != null)
                {
                    AppendFormatted(value.ToString().AsSpan(), alignment, format);
                }
            }
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
            var text = _impl;
            text.Clear();
            text.Append(span);
            MarkSet();
        }

        public void Format<T>(T value) where T : ISpanFormattable
        {
            var impl = _impl;
            impl.Clear();
            var handler = new ColImpl.AppendInterpolatedStringHandler(0, 1, impl, impl._writer._cultureInfo);
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
            readonly ColImpl.AppendInterpolatedStringHandler _handler;

            public FormatInterpolatedStringHandler(int literalLength, int formattedCount, Col col)
            {
                _impl = col._impl;
                _impl.Clear();
                _handler = new(literalLength, formattedCount, _impl, _impl._writer._cultureInfo);
            }

            public FormatInterpolatedStringHandler(int literalLength, int formattedCount, Col col, IFormatProvider? provider)
            {
                _impl = col._impl;
                _impl.Clear();
                _handler = new(literalLength, formattedCount, _impl, provider);
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
