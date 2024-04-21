using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IHFood.Sep.Test;

[TestClass]
public static class AssemblyInitializeCultureTest
{
    // Sets up InvariantCulture for all tests
    [AssemblyInitialize]
    public static void AssemblyInitializeCultureTest_InvariantCulture(TestContext _)
    {
        // Change current culture to invariant
        CultureInfo culture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }
}
