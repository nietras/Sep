using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepUtf8WriterTest
{
    [TestMethod]
    public void SepUtf8WriterTest_CanCreate()
    {
        using var writer = Sep.Utf8Writer().ToUtf8();
        Assert.IsNotNull(writer);
    }

    [TestMethod]
    public void SepUtf8WriterTest_Spec()
    {
        using var writer = Sep.Utf8Writer().ToUtf8();
        var spec = writer.Spec;
        Assert.AreEqual(';', spec.Sep.Separator);
    }

    [TestMethod]
    public void SepUtf8WriterTest_ToFile()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            using (var writer = Sep.Utf8Writer().ToFile(tempFile))
            {
                Assert.IsNotNull(writer);
            }
            Assert.IsTrue(File.Exists(tempFile));
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [TestMethod]
    public void SepUtf8WriterTest_AddHeader()
    {
        using var writer = Sep.Utf8Writer().ToUtf8();
        var header = writer.Header;
        var index = header.Add("Column1");
        Assert.AreEqual(0, index);
    }
}
