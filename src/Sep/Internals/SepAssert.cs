global using A = nietras.SeparatedValues.SepAssert;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace nietras.SeparatedValues;

/// <summary>
/// Many unit test frameworks do not handle <see
/// cref="System.Diagnostics.Debug.Assert(bool)"/> very well including MSTest,
/// so instead defining some custom assert code that throws an ordinary
/// exception instead and provides some more details.
/// </summary>
[ExcludeFromCodeCoverage]
static class SepAssert
{
    internal const string Condition = "SEPASSERT";

    [Conditional(Condition)]
    internal static void AssertMaxPosition(int dataIndex, int vectorCount)
    {
        const int MaxPosition = SepDefaults.RowLengthMax;
        A.Assert(dataIndex < (MaxPosition - vectorCount), $"index {dataIndex} must be within limits {(MaxPosition - vectorCount)} and max position {MaxPosition}");
    }

    [Conditional(Condition)]
    internal static void Assert(
        [DoesNotReturnIf(false)] bool condition,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerArgumentExpression("condition")] string conditionExpression = "")
    {
        if (!condition)
        {
            Fail($"{filePath}({lineNumber}): '{conditionExpression}' fails");
        }
    }

    [Conditional(Condition)]
    internal static void Assert(
        [DoesNotReturnIf(false)] bool condition,
        [InterpolatedStringHandlerArgument(nameof(condition))] ref AssertInterpolatedStringHandler message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerArgumentExpression("condition")] string conditionExpression = "")
    {
        if (!condition)
        {
            Fail($"{filePath}({lineNumber}): '{conditionExpression}' fails '{message.ToStringAndClear()}'");
        }
    }

    [Conditional(Condition)]
    static void Fail(string message)
    {
        SepTrace.WriteLine(message);
        Throw_InvalidOperationException_Assert(message);
    }

    [DoesNotReturn]
    internal static void Throw_InvalidOperationException_Assert(string message)
    {
        throw new InvalidOperationException(message);
    }

    /// <summary>
    /// Provides an interpolated string handler for <see
    /// cref="SepAssert.Assert(bool, ref AssertInterpolatedStringHandler,
    /// string, int, string)"/> that only performs formatting if the assert
    /// fails.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [InterpolatedStringHandler]
    internal struct AssertInterpolatedStringHandler
    {
        StringBuilder? _stringBuilder;
        /// <summary>The handler we use to perform the formatting.</summary>
        StringBuilder.AppendInterpolatedStringHandler _stringBuilderHandler;

        /// <summary>Creates an instance of the handler..</summary>
        /// <param name="literalLength">The number of constant characters outside of interpolation expressions in the interpolated string.</param>
        /// <param name="formattedCount">The number of interpolation expressions in the interpolated string.</param>
        /// <param name="condition">The condition Boolean passed to the <see cref="Debug"/> method.</param>
        /// <param name="shouldAppend">A value indicating whether formatting should proceed.</param>
        /// <remarks>This is intended to be called only by compiler-generated code. Arguments are not validated as they'd otherwise be for members intended to be used directly.</remarks>
        public AssertInterpolatedStringHandler(int literalLength, int formattedCount, bool condition, out bool shouldAppend)
        {
            if (condition)
            {
                _stringBuilder = default;
                _stringBuilderHandler = default;
                shouldAppend = false;
            }
            else
            {
                _stringBuilder = new StringBuilder();
                // Only used when failing an assert.  Additional allocation here doesn't matter; just create a new StringBuilder.
                _stringBuilderHandler = new StringBuilder.AppendInterpolatedStringHandler(literalLength, formattedCount, _stringBuilder);
                shouldAppend = true;
            }
        }

        /// <summary>Extracts the built string from the handler.</summary>
        internal string ToStringAndClear()
        {
            // Don't have access to this for some reason, weird to hide this
            // so instead have to add field for StringBuilder 🙈
            //string s = _stringBuilderHandler._stringBuilder is StringBuilder sb ?
            //    sb.ToString() :
            //    string.Empty;
            var s = _stringBuilder?.ToString() ?? string.Empty;
            _stringBuilder = default;
            _stringBuilderHandler = default;
            return s;
        }

        /// <summary>Writes the specified string to the handler.</summary>
        /// <param name="value">The string to write.</param>
        public void AppendLiteral(string value) => _stringBuilderHandler.AppendLiteral(value);

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value) => _stringBuilderHandler.AppendFormatted(value);

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="format">The format string.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value, string? format) => _stringBuilderHandler.AppendFormatted(value, format);

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value, int alignment) => _stringBuilderHandler.AppendFormatted(value, alignment);

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="format">The format string.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value, int alignment, string? format) => _stringBuilderHandler.AppendFormatted(value, alignment, format);

        /// <summary>Writes the specified character span to the handler.</summary>
        /// <param name="value">The span to write.</param>
        public void AppendFormatted(ReadOnlySpan<char> value) => _stringBuilderHandler.AppendFormatted(value);

        /// <summary>Writes the specified string of chars to the handler.</summary>
        /// <param name="value">The span to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <param name="format">The format string.</param>
        public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null) => _stringBuilderHandler.AppendFormatted(value, alignment, format);

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        public void AppendFormatted(string? value) => _stringBuilderHandler.AppendFormatted(value);

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <param name="format">The format string.</param>
        public void AppendFormatted(string? value, int alignment = 0, string? format = null) => _stringBuilderHandler.AppendFormatted(value, alignment, format);

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <param name="format">The format string.</param>
        public void AppendFormatted(object? value, int alignment = 0, string? format = null) => _stringBuilderHandler.AppendFormatted(value, alignment, format);
    }
}
