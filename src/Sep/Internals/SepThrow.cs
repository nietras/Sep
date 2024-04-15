using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using static nietras.SeparatedValues.SepWriter;

namespace nietras.SeparatedValues;

[StackTraceHidden]
static class SepThrow
{
    static readonly string s_newLine = Environment.NewLine;

    [DoesNotReturn]
    internal static void ArgumentOutOfRangeException_Separator(char separator)
    {
        throw new ArgumentOutOfRangeException(nameof(separator),
            $"'{separator}':{(ushort)separator} is not supported. " +
            $"Must be inside [{(ushort)Sep.Min.Separator}..{(ushort)Sep.Max.Separator}].");
    }

    [DoesNotReturn]
    internal static void ArgumentException_Separator(char separator)
    {
        throw new ArgumentException($"'{separator}':{(ushort)separator} is not supported.",
            nameof(separator));
    }

    [DoesNotReturn]
    internal static void IndexOutOfRangeException()
    {
#pragma warning disable CA2201 // Do not raise reserved exception types
        throw new IndexOutOfRangeException();
#pragma warning restore CA2201 // Do not raise reserved exception types
    }

    [DoesNotReturn]
    internal static void ArgumentException_CountOrLengthSameAsCols(string name, int count, int expectedCount)
    {
        throw new ArgumentException($"'{name}':{count} must have length/count {expectedCount} matching columns selected");
    }

    [DoesNotReturn]
    internal static void ArgumentException_LengthsMustBeSame(string name0, int length0, string name1, int length1)
    {
        throw new ArgumentException($"'{name1}':{length1} must have same length as '{name0}':{length0}");
    }

    [DoesNotReturn]
    internal static void InvalidDataException_ColCountMismatch(
        int colCount, int rowIndex, int lineNumberFrom, int lineNumberToExcl, string row,
        int expectedColCount, string firstRow)
    {
        throw new InvalidDataException(
            $"Found {colCount} column(s) on row {rowIndex}/lines [{lineNumberFrom}..{lineNumberToExcl}]:'{row}'{s_newLine}" +
            $"Expected {expectedColCount} column(s) matching header/first row '{firstRow}'");
    }

    [DoesNotReturn]
    internal static void NotSupportedException_ToStringOnNotStringWriter(TextWriter writer)
    {
        throw new NotSupportedException($"'{nameof(ToString)}' not supported " +
            $"for '{writer.GetType()}' only supported for '{nameof(StringWriter)}'");
    }

    [DoesNotReturn]
    internal static void InvalidOperationException_WriterAlreadyHasActiveRow()
    {
        throw new InvalidOperationException(
            "Writer already has an active new row. Ensure this is disposed before starting next row.");
    }

    [DoesNotReturn]
    internal static void InvalidOperationException_WriterDoesNotHaveActiveRow()
    {
        throw new InvalidOperationException(
            $"Writer does not have an active row. " +
            $"Ensure '{nameof(SepWriter.NewRow)}()' has been called and that the row is only disposed once. " +
            $"I.e. prefer 'using var row = writer.NewRow();'");
    }

    [DoesNotReturn]
    internal static void InvalidOperationException_NotAllColsSet(List<ColImpl> cols, string[] colNamesHeader)
    {
        // TODO: Make detailed exception
        if (colNamesHeader.Length == 0)
        {
            throw new InvalidOperationException($"Not all expected columns have been set.");
        }
        else
        {
            throw new InvalidOperationException($"Not all expected columns '{string.Join(",", colNamesHeader)}' have been set.");
        }
    }

    [DoesNotReturn]
    internal static void NotSupportedException_BufferOrRowLengthExceedsMaximumSupported(int maxLength)
    {
        throw new NotSupportedException(
            $"Buffer or row has reached maximum supported length of {maxLength}. " +
            $"If no such row should exist ensure quotes \" are terminated.");
    }

    [DoesNotReturn]
    internal static void ArgumentException_ColNameAlreadyExists(string colName)
    {
        throw new ArgumentException($"Column name '{colName}' already exists", nameof(colName));
    }

    [DoesNotReturn]
    internal static void InvalidOperationException_CannotAddColNameHeaderAlreadyWritten(string colName)
    {
        throw new InvalidOperationException($"Cannot add column name '{colName} since header or first row already written.");
    }
}
