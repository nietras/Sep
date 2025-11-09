using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepUtf8ReaderTest
{
    [TestMethod]
    public void SepUtf8ReaderTest_CanCreate()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);
        Assert.IsNotNull(reader);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Spec()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);
        var spec = reader.Spec;
        Assert.AreEqual(';', spec.Sep.Separator);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_FromFile()
    {
        var tempFile = System.IO.Path.GetTempFileName();
        try
        {
            System.IO.File.WriteAllBytes(tempFile, Encoding.UTF8.GetBytes("A;B;C\n1;2;3"));
            using var reader = Sep.Utf8Reader().FromFile(tempFile);
            Assert.IsNotNull(reader);
        }
        finally
        {
            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);
            }
        }
    }
}
