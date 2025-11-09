using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nietras.SeparatedValues;

public static class SepUtf8ReaderExtensions
{
    /// <summary>
    /// Creates a UTF-8 reader from UTF-8 encoded text bytes.
    /// </summary>
    /// <param name="options">The reader options.</param>
    /// <param name="utf8Text">The UTF-8 encoded text as a byte array.</param>
    /// <returns>A new SepUtf8Reader instance.</returns>
    public static SepUtf8Reader FromUtf8(this in SepUtf8ReaderOptions options, byte[] utf8Text)
    {
        ArgumentNullException.ThrowIfNull(utf8Text);
        var stream = new MemoryStream(utf8Text);
        Func<SepUtf8Reader.Info, string> display = static info => $"UTF-8 Bytes Length={((byte[])info.Source).Length}";
        return new SepUtf8Reader(new(utf8Text, display), options, stream);
    }

    /// <summary>
    /// Creates a UTF-8 reader from a file path.
    /// </summary>
    /// <param name="options">The reader options.</param>
    /// <param name="filePath">The file path.</param>
    /// <returns>A new SepUtf8Reader instance.</returns>
    public static SepUtf8Reader FromFile(this in SepUtf8ReaderOptions options, string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
        Func<SepUtf8Reader.Info, string> display = static info => $"File='{info.Source}'";
        return new SepUtf8Reader(new(filePath, display), options, stream);
    }

    /// <summary>
    /// Creates a UTF-8 reader from a stream.
    /// </summary>
    /// <param name="options">The reader options.</param>
    /// <param name="stream">The input stream.</param>
    /// <returns>A new SepUtf8Reader instance.</returns>
    public static SepUtf8Reader From(this in SepUtf8ReaderOptions options, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        Func<SepUtf8Reader.Info, string> display = static info => $"Stream={info.Source.GetType().Name}";
        return new SepUtf8Reader(new(stream, display), options, stream);
    }

    /// <summary>
    /// Asynchronously creates a UTF-8 reader from UTF-8 encoded text bytes.
    /// </summary>
    /// <param name="options">The reader options.</param>
    /// <param name="utf8Text">The UTF-8 encoded text as a byte array.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A new SepUtf8Reader instance.</returns>
    public static ValueTask<SepUtf8Reader> FromUtf8Async(this SepUtf8ReaderOptions options, byte[] utf8Text, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(FromUtf8(options, utf8Text));
    }

    /// <summary>
    /// Asynchronously creates a UTF-8 reader from a file path.
    /// </summary>
    /// <param name="options">The reader options.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A new SepUtf8Reader instance.</returns>
    public static ValueTask<SepUtf8Reader> FromFileAsync(this SepUtf8ReaderOptions options, string filePath, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(FromFile(options, filePath));
    }

    /// <summary>
    /// Asynchronously creates a UTF-8 reader from a stream.
    /// </summary>
    /// <param name="options">The reader options.</param>
    /// <param name="stream">The input stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A new SepUtf8Reader instance.</returns>
    public static ValueTask<SepUtf8Reader> FromAsync(this SepUtf8ReaderOptions options, Stream stream, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(From(options, stream));
    }
}
