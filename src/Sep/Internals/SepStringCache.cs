namespace nietras.SeparatedValues;

static class SepStringCache
{
    internal static readonly string[] SingleCharToString = CreateSingleCharToString(256);

    static string[] CreateSingleCharToString(int maxChar)
    {
        var singleCharToString = new string[maxChar];
        for (var i = 0; i < maxChar; i++)
        {
            singleCharToString[i] = string.Intern(((char)i).ToString());
        }
        return singleCharToString;
    }
}
