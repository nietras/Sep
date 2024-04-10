using System;
using System.Collections.Generic;

namespace nietras.SeparatedValues;

public sealed class SepWriterHeader
{
    readonly SepWriter _writer;

    internal SepWriterHeader(SepWriter writer) => _writer = writer;

    public void Add(IReadOnlyList<string> colNames)
    {
        ArgumentNullException.ThrowIfNull(colNames);
        for (var i = 0; i < colNames.Count; i++) { Add(colNames[i]); }
    }

    public void Add(string[] colNames)
    {
        ArgumentNullException.ThrowIfNull(colNames);
        foreach (var colName in colNames) { Add(colName); }
    }

    public void Add(ReadOnlySpan<string> colNames)
    {
        foreach (var colName in colNames) { Add(colName); }
    }

    public void Add(string colName)
    {
        if (_writer._headerWrittenOrSkipped)
        {
            SepThrow.InvalidOperationException_CannotAddColNameHeaderAlreadyWritten(colName);
        }
        if (_writer._colNameToCol.ContainsKey(colName))
        {
            SepThrow.ArgumentException_ColNameAlreadyExists(colName);
        }
        _writer.AddCol(colName);
    }
}
