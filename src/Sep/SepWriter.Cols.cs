using System;
using System.Collections.Generic;

namespace nietras.SeparatedValues;

public partial class SepWriter
{
    // Problem here is Col is a ref struct so can't use Action<Col,...>
    public delegate void ColAction(Col col);
    public delegate void ColAction<T>(Col col, T value);

    public readonly ref struct Cols
    {
        readonly ReadOnlySpan<ColImpl> _cols;

        internal Cols(ReadOnlySpan<ColImpl> cols)
        {
            _cols = cols;
        }

        public int Count => _cols.Length;

        public Col this[int colIndex] => new(_cols[colIndex]);

        public void Set(SepReader.Cols cols)
        {
            SepCheck.LengthSameAsCols(_cols.Length, nameof(cols), cols.Count);
            for (var i = 0; i < cols.Count; i++)
            {
                new Col(_cols[i]).Set(cols[i].Span);
            }
        }

        public void Set(IReadOnlyList<string> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            SepCheck.LengthSameAsCols(_cols.Length, nameof(values), values.Count);
            for (var i = 0; i < values.Count; i++)
            {
                new Col(_cols[i]).Set(values[i]);
            }
        }

        // Overload needed since otherwise ambiguous call for array between
        // ReadOnlySpan<> and IReadOnlyList<>
        public void Set(string[] values) => Set(values.AsSpan());

        public void Set(ReadOnlySpan<string> values)
        {
            SepCheck.LengthSameAsCols(_cols.Length, nameof(values), values.Length);
            for (var i = 0; i < values.Length; i++)
            {
                new Col(_cols[i]).Set(values[i]);
            }
        }

        public void Format<T>(IReadOnlyList<T> values) where T : ISpanFormattable
        {
            ArgumentNullException.ThrowIfNull(values);
            SepCheck.LengthSameAsCols(_cols.Length, nameof(values), values.Count);
            for (var i = 0; i < values.Count; i++)
            {
                new Col(_cols[i]).Format(values[i]);
            }
        }

        public void Format<T>(T[] values) where T : ISpanFormattable =>
            Format(values.AsSpan());

        // C# type inference is poor so overload needed
        public void Format<T>(Span<T> values) where T : ISpanFormattable =>
            Format((ReadOnlySpan<T>)values);

        public void Format<T>(ReadOnlySpan<T> values) where T : ISpanFormattable
        {
            SepCheck.LengthSameAsCols(_cols.Length, nameof(values), values.Length);
            for (var i = 0; i < values.Length; i++)
            {
                new Col(_cols[i]).Format(values[i]);
            }
        }

        // TODO: Add overloads for Format with Action

        public void Format<T>(ReadOnlySpan<T> values, ColAction<T> format)
        {
            ArgumentNullException.ThrowIfNull(format);
            SepCheck.LengthSameAsCols(_cols.Length, nameof(values), values.Length);
            for (var i = 0; i < values.Length; i++)
            {
                format(new(_cols[i]), values[i]);
            }
        }

        // TODO: Support interpolated string formatting for spans
    }
}
