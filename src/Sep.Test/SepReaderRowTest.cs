using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderRowTest
{
    const string _colName0 = "C1";
    const string _colName1 = "C2";
    const string _colName2 = "C3";
    const string _colName3 = "C4";
    const string _colValue0 = " ";
    const string _colValue1 = "";
    const string _colValue2 = "1.23456789";
    const string _colValue3 = "abcdefgh\t, ._";
    const int _cols = 4;
    static readonly string[] _colNames = [_colName0, _colName1, _colName2, _colName3];
    static readonly string[] _colValues = [_colValue0, _colValue1, _colValue2, _colValue3];
    static readonly string _headerText = string.Join(';', _colNames);
    static readonly string _rowText = string.Join(';', _colValues);
    static readonly string _text = $"{_headerText}\r{_rowText}\r";

    readonly SepReader _reader = Sep.Reader().FromText(_text);
    readonly SepReader _enumerator;

    public SepReaderRowTest()
    {
        var enumerator = _reader.GetEnumerator();
        Assert.IsTrue(enumerator.MoveNext(), "MoveNext should be true");
        _enumerator = enumerator;
    }

    [DataRow(false)]
    [DataRow(true)]
    [DataTestMethod]
    public void SepReaderRowTest_EmptyString_Properties(bool unescape)
    {
        using var reader = Sep.Reader(o => o with { Unescape = unescape }).FromText("");
        using var enumerator = reader.GetEnumerator();
        Assert.IsFalse(enumerator.MoveNext());
        // enumerator.Current should not be called if MoveNext false,
        // but here state is asserted anyway.
        var row = enumerator.Current;
        Assert.AreEqual(-1, row.RowIndex);
        Assert.AreEqual(1, row.LineNumberFrom);
        Assert.AreEqual(1, row.LineNumberToExcl);
        Assert.AreEqual(string.Empty, row.ToString());
        Assert.AreEqual(0, row.Span.Length);
        Assert.IsNotNull(row.UnsafeToStringDelegate);
    }

    [DataRow(false)]
    [DataRow(true)]
    [DataTestMethod]
    public void SepReaderRowTest_EmptyRow_Properties(bool unescape)
    {
        using var reader = Sep.Reader(o => o with { Unescape = unescape }).FromText("\n\n");
        using var enumerator = reader.GetEnumerator();
        Assert.IsTrue(enumerator.MoveNext());
        var row = enumerator.Current;
        Assert.AreEqual(1, row.RowIndex);
        Assert.AreEqual(2, row.LineNumberFrom);
        Assert.AreEqual(3, row.LineNumberToExcl);
        Assert.AreEqual(string.Empty, row.ToString());
        Assert.AreEqual(0, row.Span.Length);
        Assert.IsNotNull(row.UnsafeToStringDelegate);
        Assert.AreEqual(string.Empty, row.UnsafeToStringDelegate(0));
    }

    [DataRow(false)]
    [DataRow(true)]
    [DataTestMethod]
    public void SepReaderRowTest_Row_Properties(bool unescape)
    {
        using var reader = Sep.Reader(o => o with { Unescape = unescape }).FromText(_text);
        using var enumerator = reader.GetEnumerator();
        Assert.IsTrue(enumerator.MoveNext());
        var row = enumerator.Current;
        Assert.AreEqual(1, row.RowIndex);
        Assert.AreEqual(2, row.LineNumberFrom);
        Assert.AreEqual(3, row.LineNumberToExcl);
        Assert.AreEqual(_rowText, row.ToString());
        Assert.IsTrue(_rowText.AsSpan().Equals(row.Span, StringComparison.Ordinal));
    }

    [TestMethod]
    public void SepReaderRowTest_Row_Indexer_Single()
    {
        var row = _enumerator.Current;
        SepReaderRowTest_Row_Indexer_Single(ref row);
    }
    [TestMethod]
    public void SepReaderRowTest_Row_Indexer_Single_CallManyTimesToExhaustColNameCache()
    {
        var row = _enumerator.Current;
        for (var i = 0; i < 512; i++)
        {
            SepReaderRowTest_Row_Indexer_Single(ref row);
        }
    }
    static void SepReaderRowTest_Row_Indexer_Single(ref SepReader.Row row)
    {
        var toString = row.UnsafeToStringDelegate;
        for (var index = 0; index < _cols; index++)
        {
            var name = _colNames[index];
            var expected = _colValues[index];

            Assert.AreEqual(expected, row[index].ToString());
            Assert.AreEqual(expected, toString(index));
            Assert.AreEqual(expected, row[new Index(index)].ToString());
            Assert.AreEqual(expected, row[name].ToString());
            // fromEnd = true for Index
            Assert.AreEqual(_colValues[^(index + 1)], row[^(index + 1)].ToString());
        }
    }

    [TestMethod]
    public void SepReaderRowTest_Row_Indexer_Single_ColIndex_OutOfRange()
    {
        var invalidIndices = new[] { -1, _cols, int.MinValue, int.MaxValue };
        foreach (var index in invalidIndices)
        {
            Assert.ThrowsException<IndexOutOfRangeException>(
                () => { var col = _enumerator.Current[index].Span; });
            // Index only takes non-negative numbers
            if (index >= 0)
            {
                Assert.ThrowsException<IndexOutOfRangeException>(
                    () => { var col = _enumerator.Current[new Index(index)].Span; });
            }
        }
    }

    [TestMethod]
    public void SepReaderRowTest_Row_Indexer_Multiple()
    {
        var row = _enumerator.Current;
        SepReaderRowTest_Row_Indexer_Multiple(ref row);
    }
    [TestMethod]
    public void SepReaderRowTest_Row_Indexer_Multiple_CallManyTimesToExhaustColNameCache()
    {
        var row = _enumerator.Current;
        for (var i = 0; i < 512; i++)
        {
            SepReaderRowTest_Row_Indexer_Multiple(ref row);
        }
    }

    [TestMethod]
    public void SepReaderRowTest_Row_Indexer_Multiple_Range()
    {
        var row = _enumerator.Current;
        SepReaderRowTest_Row_Indexer_Multiple_Range(ref row);
    }

    static void SepReaderRowTest_Row_Indexer_Multiple(ref SepReader.Row row)
    {
        var indicesSet = new int[][]
        {
            [0],
            [0, 1],
            [0, 2],
            [1, 3],
            [1, 2, 3],
        };
        foreach (var indices in indicesSet)
        {
            var names = indices.Select(i => _colNames[i]).ToArray();
            var expected = indices.Select(i => _colValues[i]).ToArray();

            AssertCols(expected, row[indices.AsSpan()]);
            AssertCols(expected, row[(IReadOnlyList<int>)indices]);
            AssertCols(expected, row[indices]);

            AssertCols(expected, row[names.AsSpan()]);
            AssertCols(expected, row[(IReadOnlyList<string>)names]);
            AssertCols(expected, row[names]);
        }
    }

    static void SepReaderRowTest_Row_Indexer_Multiple_Range(ref SepReader.Row row)
    {
        var ranges = new Range[]
        {
            0..0,
            1..2,
            1..3,
            0.._cols,
        };
        foreach (var range in ranges)
        {
            AssertCols(_colValues[range], row[range]);
        }
    }

    [TestMethod]
    public void SepReaderRowTest_Row_Indexer_Multiple_ColIndices_OutOfRange()
    {
        const int count = 4;
        var invalidIndices = new int[count] { -1, _cols, int.MinValue, int.MaxValue };

        for (var i = 0; i < count; i++)
        {
            for (var c = 0; c < _cols; c++)
            {
                var indices = Enumerable.Range(0, _cols).ToArray();
                indices[c] = invalidIndices[i];
                Assert.ThrowsException<IndexOutOfRangeException>(
                    () => { var cols = _enumerator.Current[indices]; cols.Select(col => col.ToString()); });
            }
        }
    }

    [TestMethod]
    public void SepReaderRowTest_Row_Indexer_Multiple_ColNames_OutOfRange()
    {
        const int count = 4;
        var invalidNames = new string[count] { "x", "", "Y", "adljakldjaldjal" };

        for (var i = 0; i < count; i++)
        {
            for (var c = 0; c < _cols; c++)
            {
                var names = _colNames.ToArray();
                names[c] = invalidNames[i];
                Assert.ThrowsException<KeyNotFoundException>(
                    () => { var col = _enumerator.Current[names]; });
            }
        }
    }

    [TestMethod]
    public void SepReaderRowTest_Row_DebuggerDisplayPrefix()
    {
        Assert.AreEqual("  1:[2..3] = ", _enumerator.Current.DebuggerDisplayPrefix);
    }

    [TestMethod]
    public void SepReaderRowTest_Row_DebugView()
    {
        var rowDebugView = new SepReader.Row.DebugView(_enumerator.Current);
        var cols = rowDebugView.Cols;
        Assert.AreEqual(_cols, cols.Length);
        for (var colIndex = 0; colIndex < cols.Length; colIndex++)
        {
            var col = cols[colIndex];
            Assert.AreEqual(colIndex, col.ColIndex);
            Assert.AreEqual(_colNames[colIndex], col.ColName);
            Assert.AreEqual(_colValues[colIndex], col.ColValue);
            Assert.AreEqual($"{colIndex:D2}:'{_colNames[colIndex]}'", col.ColIndexName);
        }
    }

    static void AssertCols(ReadOnlySpan<string> expected, in SepReader.Cols cols)
    {
        Assert.AreEqual(expected.Length, cols.Count);
        for (var i = 0; i < expected.Length; i++)
        {
            Assert.AreEqual(expected[i], cols[i].ToString(), i.ToString());
        }
        CollectionAssert.AreEqual(expected.ToArray(), cols.ToStringsArray());
#if NET8_0_OR_GREATER
        CollectionAssert.AreEqual(expected.ToArray(), cols.ParseToArray<string>());
#endif
    }
}
