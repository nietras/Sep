using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace nietras.SeparatedValues;

[DebuggerDisplay("{DebuggerDisplayPrefix,nq}")]
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

    internal string DebuggerDisplayPrefix => $"Count = {GetColNames().Count}";

    IReadOnlyCollection<string> GetColNames() =>
        _writer._colNamesHeader is null ? _writer._colNameToCol.Keys : _writer._colNamesHeader;

    internal class DebugView
    {
        internal DebugView(SepWriterHeader header) => ColNames = header.GetColNames().ToArray();

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        internal string[] ColNames { get; }
    }
}
