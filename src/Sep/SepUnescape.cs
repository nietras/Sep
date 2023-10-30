using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

public enum SepUnescape
{

    No,
    UnescapeOnColAccess,
}

static class SepUnescaping
{
    internal static int Unescape(ref char chars, int length)
    {
        // 0, 1, 3, 5, 7
        var unescapedLength = 0;
        var toggle = 0;
        for (var i = 0; i < length; i++)
        {
            var c = Unsafe.Add(ref chars, i);
            Unsafe.Add(ref chars, unescapedLength) = c;
            var o = c == SepDefaults.Quote ? 0 : 1;
            toggle ^= o;
            var s = toggle & o;
            unescapedLength += s;
        }
        return unescapedLength;
    }
}
