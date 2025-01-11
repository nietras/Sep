using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test.Internals;

public static class DefaultInterpolatedStringHandlerAccessor
{
#if NET8_0_OR_GREATER
    public struct Class
    {
        static void StaticPrivateMethod() { }
        static int StaticPrivateField;
        public Class(int i) { PrivateField = i; }
        void PrivateMethod() { }
        int PrivateField;
        int PrivateProperty { get => PrivateField; }
    }


    public static void GetSetPrivateField(Class c)
    {
        ref int f = ref GetSetPrivateField(ref c);
        f = 43;

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "PrivateField")]
        extern static ref int GetSetPrivateField(ref Class c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_pos")]
    static extern ref int Position(ref this DefaultInterpolatedStringHandler handler);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_arrayToReturnToPool")]
    static extern ref char[]? ArrayToReturnToPool(ref this DefaultInterpolatedStringHandler handler);

    public static string Test()
    {
        var c = new Class(42);
        GetSetPrivateField(c);

        var buffer = new char[16];
        var handler = new DefaultInterpolatedStringHandler(literalLength: 10, formattedCount: 2, provider: null, buffer);
        ref var position = ref handler.Position();
        position = 2;
        handler.AppendFormatted(42);
        return handler.ToStringAndClear();
    }
#endif
}

[TestClass]
public class InterpolatedStringHandlerTest
{
#if NET8_0_OR_GREATER
    [TestMethod]
    public void InterpolatedStringHandlerTest_DefaultInterpolatedStringHandler_Accessor()
    {
        DefaultInterpolatedStringHandlerAccessor.Test();
    }
#endif

    [TestMethod]
    public void InterpolatedStringHandlerTest_Log()
    {
        var logger = new Logger();
        logger.LogMessage(LogLevel.Trace, $"test {2}");
    }

    [TestMethod]
    public void InterpolatedStringHandlerTest_Row()
    {
        var row = new InterpolatedStringHandlerRow();
        row["C"] = $"test {2}";
        row.set_Item2("C", $"test {2}");
    }

    public enum LogLevel
    {
        Off,
        Critical,
        Error,
        Warning,
        Information,
        Trace
    }

    public class Logger
    {
        public LogLevel EnabledLevel { get; init; } = LogLevel.Error;

        //public void LogMessage(LogLevel level, string msg)
        //{
        //    if (EnabledLevel < level) return;
        //    Console.WriteLine(msg);
        //}

        public void LogMessage(LogLevel level, LogInterpolatedStringHandler builder)
        {
            if (EnabledLevel < level) return;
            Trace.WriteLine(builder.GetFormattedText());
        }
    }

    [InterpolatedStringHandler]
    public ref struct LogInterpolatedStringHandler
    {
        // Storage for the built-up string
        readonly StringBuilder builder;

        public LogInterpolatedStringHandler(int literalLength, int formattedCount)
        {
            builder = new StringBuilder(literalLength);
            Trace.WriteLine($"\tliteral length: {literalLength}, formattedCount: {formattedCount}");
        }

        public void AppendLiteral(string s)
        {
            Trace.WriteLine($"\tAppendLiteral called: {{{s}}}");

            builder.Append(s);
            Trace.WriteLine($"\tAppended the literal string");
        }

        public void AppendFormatted<T>(T t)
        {
            Trace.WriteLine($"\tAppendFormatted called: {{{t}}} is of type {typeof(T)}");

            builder.Append(t?.ToString());
            Trace.WriteLine($"\tAppended the formatted object");
        }

        internal string GetFormattedText() => builder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [InterpolatedStringHandler]
    public readonly struct FormatInterpolatedStringHandler
    {
        readonly StringBuilder.AppendInterpolatedStringHandler _handler;

        public FormatInterpolatedStringHandler(int literalLength, int formattedCount, InterpolatedStringHandlerRow row)
        {
            //col._colText.Clear();
            _handler = new(literalLength, formattedCount, new StringBuilder());
        }

        public FormatInterpolatedStringHandler(int literalLength, int formattedCount, InterpolatedStringHandlerRow row, IFormatProvider? provider)
        {
            //col._colText.Clear();
            _handler = new(literalLength, formattedCount, new StringBuilder(), provider);
        }

        public void AppendLiteral(string value) =>
            _handler.AppendLiteral(value);

        public void AppendFormatted<T>(T value) =>
            _handler.AppendFormatted(value);

        public void AppendFormatted<T>(T value, string? format) =>
            _handler.AppendFormatted(value, format);

        public void AppendFormatted<T>(T value, int alignment) =>
            _handler.AppendFormatted(value, alignment);

        public void AppendFormatted<T>(T value, int alignment, string? format) =>
            _handler.AppendFormatted(value, alignment, format);

        public void AppendFormatted(ReadOnlySpan<char> value) =>
            _handler.AppendFormatted(value);

        public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null) =>
            _handler.AppendFormatted(value, alignment, format);

        public void AppendFormatted(string? value) =>
            _handler.AppendFormatted(value);

        public void AppendFormatted(string? value, int alignment = 0, string? format = null) =>
            _handler.AppendFormatted(value, alignment, format);

        public void AppendFormatted(object? value, int alignment = 0, string? format = null) =>
            _handler.AppendFormatted(value, alignment, format);
    }
}

