using System.Diagnostics.Contracts;
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
        var colNames = readerRow._state._header._colNames;
        for (var colIndex = 0; colIndex < colNames.Length; ++colIndex)
        {
            writerRow[colNames[colIndex]].Set(readerRow[colIndex].Span);
        }
    }
}
