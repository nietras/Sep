﻿using System.Diagnostics.Contracts;
using System.Threading;

namespace nietras.SeparatedValues;

public static class SepReaderWriterExtensions
{
    public static SepWriter.Row NewRow(this SepWriter writer, SepReader.Row rowToCopy)
    {
        Contract.Assume(writer is not null);
        var row = writer.NewRow();
        rowToCopy.CopyTo(row);
        return row;
    }

    public static SepWriter.Row NewRow(this SepWriter writer, SepReader.Row rowToCopy, CancellationToken cancellationToken)
    {
        Contract.Assume(writer is not null);
        var row = writer.NewRow(cancellationToken);
        rowToCopy.CopyTo(row);
        return row;
    }

    public static void CopyTo(this SepReader.Row readerRow, SepWriter.Row writerRow)
    {
        var colNames = readerRow._state._header.ColNames;
        for (var i = 0; i < colNames.Count; i++)
        {
            var colName = colNames[i];
            writerRow[colName].Set(readerRow[colName].Span);
        }
    }
}
