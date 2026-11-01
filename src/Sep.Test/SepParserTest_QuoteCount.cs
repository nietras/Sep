using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

public partial class SepParserTest
{
    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_ParseColEnds_QuoteCountAcrossCalls(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        AssertQuoteCountAcrossCalls((ISepParser)parserObject, colInfos: false);
    }

    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_ParseColInfos_QuoteCountAcrossCalls(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        AssertQuoteCountAcrossCalls((ISepParser)parserObject, colInfos: true);
    }

    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_ParseColEnds_QuoteCountAcrossCalls_Multiline(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        AssertQuoteCountAcrossCalls((ISepParser)parserObject, colInfos: false, multiline: true);
    }

    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_ParseColInfos_QuoteCountAcrossCalls_Multiline(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        AssertQuoteCountAcrossCalls((ISepParser)parserObject, colInfos: true, multiline: true);
    }

    void AssertQuoteCountAcrossCalls(ISepParser parser, bool colInfos, bool multiline = false)
    {
        var s = colInfos ? _stateUnescape : _state;
        s._parsingLineNumber = 17;
        var width = Math.Max(parser.PaddingLength, 4);
        var text = multiline ? "\"" + new string('a', width - 3) + "\r\n" : "\"" + new string('a', width - 1);
        s._charsDataEnd = FillChars(text);
        Parse();
        Assert.AreEqual(1, parser.QuoteCount);
        Assert.AreEqual(width, s._charsParseStart);
        Assert.AreEqual(0, s._parsingRowColCount);
        Assert.AreEqual(0, s._parsedRowsCount);

        Parse();
        Assert.AreEqual(1, parser.QuoteCount);
        Assert.AreEqual(multiline ? 18 : 17, s._parsingLineNumber);

        text += "\"\"" + new string('a', width - 2);
        s._charsDataEnd = FillChars(text);
        Parse();
        Assert.AreEqual(3, parser.QuoteCount);
        Assert.AreEqual(width * 2, s._charsParseStart);

        text += "\";\"" + new string('a', width - 3);
        s._charsDataEnd = FillChars(text);
        Parse();
        Assert.AreEqual(1, parser.QuoteCount);
        Assert.AreEqual(width * 3, s._charsParseStart);
        Assert.AreEqual(1, s._parsingRowColCount);

        text += "\";\r\n";
        s._charsDataEnd = FillChars(text);
        Parse();
        Assert.AreEqual(0, parser.QuoteCount);
        Assert.AreEqual(text.Length, s._charsParseStart);
        Assert.AreEqual(1, s._parsedRowsCount);
        Assert.AreEqual(3, s._parsedRows[0].ColCount);
        Assert.AreEqual(multiline ? 19 : 18, s._parsedRows[0].LineNumberTo);
        if (colInfos)
        {
            var infos = MemoryMarshal.Cast<int, SepColInfo>(_colEndsOrColInfos);
            Assert.AreEqual(new SepColInfo(width * 2 + 1, 4), infos[1]);
            Assert.AreEqual(new SepColInfo(width * 3 + 1, 2), infos[2]);
            Assert.AreEqual(new SepColInfo(width * 3 + 2, 0), infos[3]);
        }
        else
        {
            AreEqual([width * 2 + 1, width * 3 + 1, width * 3 + 2], _colEndsOrColInfos, 0, 3);
        }

        void Parse()
        {
            if (colInfos) { parser.ParseColInfos(s); }
            else { parser.ParseColEnds(s); }
        }
    }
}
