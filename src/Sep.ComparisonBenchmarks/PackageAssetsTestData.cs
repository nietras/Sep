#define USE_STRING_POOLING
//#define PARSE_ASSETS
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace nietras.SeparatedValues.ComparisonBenchmarks;

static class PackageAssetsTestData
{
    static readonly Uri PackageAssetsUrl = new("https://raw.githubusercontent.com/joelverhagen/NCsvPerf/main/NCsvPerf/TestData/PackageAssets.csv");
    const string PackageAssetsFileName = @"PackageAssets.csv";
    const string DirectoryNameToFind = "artifacts";
    static readonly ConcurrentDictionary<(bool quotes, bool spaces), LineCache> _quotesOrNotToLineCache = new();

    internal static LineCache PackageAssets(bool quoteAroundSomeCols = false, bool spacesAroundSomeColsAndInsideQuotes = false)
    {
        var key = (quoteAroundSomeCols, spacesAroundSomeColsAndInsideQuotes);
        if (!_quotesOrNotToLineCache.TryGetValue(key, out var lineCache))
        {
            lineCache = new LineCache(GetPackageAssetsFilePath(),
                quoteAroundSomeCols, spacesAroundSomeColsAndInsideQuotes);
            _quotesOrNotToLineCache.TryAdd(key, lineCache);
        }
        return lineCache;
    }

    internal async static Task EnsurePackageAssets()
    {
        var packageAssetsFilePath = GetPackageAssetsFilePath();
        if (!File.Exists(packageAssetsFilePath))
        {
            using var client = new HttpClient();
            using var s = await client.GetStreamAsync(PackageAssetsUrl).ConfigureAwait(true);
            using var fs = new FileStream(packageAssetsFilePath, FileMode.OpenOrCreate);
            await s.CopyToAsync(fs).ConfigureAwait(true);
        }
    }

    internal static string GetPackageAssetsFilePath()
    {
        Console.WriteLine(typeof(PackageAssetsBench).Assembly.Location);
        var directory = FindParentDirectory();
        var packageAssetsFilePath = Path.Combine(directory, PackageAssetsFileName);
        Console.WriteLine(packageAssetsFilePath);
        return packageAssetsFilePath;
    }

    internal static string FindParentDirectory() => FindParentDirectory(typeof(PackageAssetsBench).Assembly.Location, DirectoryNameToFind);
    internal static string FindParentDirectory(string currentDirectory, string directoryNameToFind)
    {
        var directory = currentDirectory;
        while (!directory.EndsWith(directoryNameToFind, StringComparison.OrdinalIgnoreCase))
        {
            directory = Directory.GetParent(directory)!.FullName;
        }
        return directory;
    }


    public sealed class LineCache
    {
        readonly string[] _sourceLines;
        readonly ConcurrentDictionary<int, string> _rowCountToString = new();
        readonly ConcurrentDictionary<int, byte[]> _rowCountToBytes = new();

        public LineCache(string filePath, bool quoteAroundSomeCols, bool spacesAroundSomeColsAndInsideQuotes)
        {
            _sourceLines = File.ReadAllLines(filePath);
            if (quoteAroundSomeCols)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < _sourceLines.Length; i++)
                {
                    ref var line = ref _sourceLines[i];
                    sb.Clear();
                    AddQuotesAndMaybeSpacesAroundSomeCols(line, sb, spacesAroundSomeColsAndInsideQuotes);
                    var newLine = sb.ToString();
                    //Debug.Assert(line.Split(',').Length == 25);
                    line = newLine;
                }
            }
        }

        static void AddQuotesAndMaybeSpacesAroundSomeCols(ReadOnlySpan<char> span, StringBuilder sb, bool spaces)
        {
            var col = 0;
            var start = 0;
            const char separator = ',';
            while (start <= span.Length)
            {
                var remain = span.Slice(start);
                var foundIndex = remain.IndexOf(separator);
                foundIndex = foundIndex < 0 ? span.Length - start : foundIndex;
                var colSpan = remain.Slice(0, foundIndex);
                if (col > 0)
                {
                    sb.Append(separator);
                }
                // These cols are parsed so not adding quotes around
                if (col == 0 || col == 1 || col == 4)
                {
                    sb.Append(colSpan);
                }
                else
                {
                    if (spaces) { sb.Append(' '); }
                    sb.Append('"');
                    if (spaces) { sb.Append(' '); }
                    sb.Append(colSpan);
                    if (spaces) { sb.Append(' '); }
                    sb.Append('"');
                    if (spaces) { sb.Append(' '); }
                }
                start += foundIndex + 1;
                ++col;
            }
        }

        public string GetString(int lineCount)
        {
            return _rowCountToString.GetOrAdd(lineCount, GetStringUncached);
        }

        public byte[] GetBytes(int lineCount)
        {
            return _rowCountToBytes.GetOrAdd(lineCount, GetBytesUncached);
        }

        string GetStringUncached(int lineCount)
        {
            if (lineCount == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(lineCount * 256);
            sb.AppendLine(_sourceLines[0]);
            var line = 1;
            while (line < lineCount)
            {
                var index = (line - 1) % (_sourceLines.Length - 1) + 1; // +1 skip header
                sb.AppendLine(_sourceLines[index]);
                ++line;
            }
            return sb.ToString();
        }

        byte[] GetBytesUncached(int lineCount)
        {
            if (lineCount == 0)
            {
                return Array.Empty<byte>();
            }

            using var memoryStream = new MemoryStream(lineCount * 256);
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);

            writer.WriteLine(_sourceLines[0]);
            var line = 1;
            while (line < lineCount)
            {
                var index = (line - 1) % (_sourceLines.Length - 1) + 1; // +1 skip header
                writer.WriteLine(_sourceLines[index]);
                ++line;
            }
            writer.Flush();
            return memoryStream.ToArray();
        }
    }
}
