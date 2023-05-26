using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

internal class SepCheck
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //[ExcludeFromCodeCoverage] // Block after throw will never be covered
    internal static void LengthSameAsCols(int expectedLength, string name, int actualLength)
    {
        if (expectedLength != actualLength)
        {
            SepThrow.ArgumentException_LengthSameAsCols(name, actualLength, expectedLength);
        }
    }
}
