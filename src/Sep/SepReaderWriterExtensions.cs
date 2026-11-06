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
        foreach (var (colName, colIndex) in readerRow._state._header._colNameToIndex)
        {
            writerRow[colName].Set(readerRow[colIndex].Span);
        }
    }
}
