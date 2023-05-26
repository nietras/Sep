global using T = nietras.SeparatedValues.SepTrace;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
//using TImpl = System.Diagnostics.Debug;
using TImpl = System.Diagnostics.Trace;

namespace nietras.SeparatedValues;

[ExcludeFromCodeCoverage]
static class SepTrace
{
    internal const string Condition = "SEPTRACE";

    [Conditional(Condition)]
    internal static void WriteLine(string message) => TImpl.WriteLine(message);
}
