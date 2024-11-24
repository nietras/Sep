using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace nietras.SeparatedValues;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DebuggerTypeProxy(typeof(DebugView))]
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

    public void Add(params ReadOnlySpan<string> colNames)
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

    public void Write() => _writer.WriteHeader();

    internal string DebuggerDisplay =>
        $"Count = {DebugColNames().Length} State = '{(_writer._headerWrittenOrSkipped ? (_writer._writeHeader ? "Written" : "Skipped") :
                                                     (_writer._writeHeader ? "Not yet written" : "To be skipped"))}'";

    string[] DebugColNames() =>
        _writer._colNamesHeader.Length != _writer._colNameToCol.Count
        ? _writer._colNameToCol.Keys.ToArray() : _writer._colNamesHeader;

    internal class DebugView
    {
        readonly SepWriterHeader _header;

        internal DebugView(SepWriterHeader header) => _header = header;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        internal string[] ColNames => _header.DebugColNames();
    }
}
