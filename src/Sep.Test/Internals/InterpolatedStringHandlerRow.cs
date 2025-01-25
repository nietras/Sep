using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;

namespace nietras.SeparatedValues.Test.Internals;

public class InterpolatedStringHandlerRow
{
    readonly Dictionary<string, string> _nameToValue = new();
    internal readonly StringBuilder _stringBuilder = new();

    public TraceInterpolatedStringHandler this[string index]
    {
        [param: InterpolatedStringHandlerArgument("")]
        set { _nameToValue[index] = value.GetFormattedText(); }
    }

    [SpecialName]
#pragma warning disable CA1045 // Do not pass types by reference
    public void set_Item2(string index, [InterpolatedStringHandlerArgument("")] ref TraceInterpolatedStringHandler value)
#pragma warning restore CA1045 // Do not pass types by reference
    {
        _nameToValue[index] = value.GetFormattedText();
    }

    [InterpolatedStringHandler]
    public ref struct TraceInterpolatedStringHandler
    {
        static readonly Action<string> Log = t => { };
        // Storage for the built-up string
        readonly StringBuilder _builder;

        public TraceInterpolatedStringHandler(int literalLength, int formattedCount)
        {
            _builder = new StringBuilder(literalLength);
            Log($"\tliteral length: {literalLength}, formattedCount: {formattedCount}");
        }

        public TraceInterpolatedStringHandler(int literalLength, int formattedCount, InterpolatedStringHandlerRow row)
        {
            Contract.Assume(row is not null);
            row._stringBuilder.Clear();
            _builder = row._stringBuilder;
            Log($"\tliteral length: {literalLength}, formattedCount: {formattedCount} REUSE");
        }

        public void AppendLiteral(string s)
        {
            Log($"\tAppendLiteral called: {{{s}}}");

            _builder.Append(s);
            Log($"\tAppended the literal string");
        }

        public void AppendFormatted<T>(T t)
        {
            Trace.WriteLine($"\tAppendFormatted called: {{{t}}} is of type {typeof(T)}");

            _builder.Append(t?.ToString());
            Trace.WriteLine($"\tAppended the formatted object");
        }

        internal string GetFormattedText() => _builder.ToString();
    }
}
