using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test.Internals;

[TestClass]
public class InterpolatedStringHandlerIndexerExampleTest
{
    [TestMethod]
    public void InterpolatedStringHandlerIndexerExampleTest_Row()
    {
        M(new InterpolatedStringHandlerRow(), 42);
    }

    static void M(InterpolatedStringHandlerRow row, int value)
    {
        row["C"] = $"Test {value}";
        row.set_Item2("C", $"Test {value}");
    }
}
