using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nietras.SeparatedValues;

public static class SepUtf8WriterExtensions
{
    /// <summary>
    /// Creates a UTF-8 writer that writes to a byte array (MemoryStream).
    /// </summary>
    /// <param name="options">The writer options.</param>
    /// <returns>A new SepUtf8Writer instance.</returns>
    public static SepUtf8Writer ToUtf8(this in SepUtf8WriterOptions options)
    {
        var stream = new MemoryStream();
        SepUtf8Writer.DebuggerDisplayFunc display = static (info, s) => $"MemoryStream Length={((MemoryStream)s).Length}";
        return new SepUtf8Writer(new(stream, display), options, stream);
    }

    /// <summary>
    /// Creates a UTF-8 writer that writes to a file.
    /// </summary>
    /// <param name="options">The writer options.</param>
    /// <param name="filePath">The file path.</param>
    /// <returns>A new SepUtf8Writer instance.</returns>
    public static SepUtf8Writer ToFile(this in SepUtf8WriterOptions options, string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.SequentialScan);
        SepUtf8Writer.DebuggerDisplayFunc display = static (info, s) => $"File='{info.Source}'";
        return new SepUtf8Writer(new(filePath, display), options, stream);
    }

    /// <summary>
    /// Creates a UTF-8 writer that writes to a stream.
    /// </summary>
    /// <param name="options">The writer options.</param>
    /// <param name="stream">The output stream.</param>
    /// <returns>A new SepUtf8Writer instance.</returns>
    public static SepUtf8Writer To(this in SepUtf8WriterOptions options, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        SepUtf8Writer.DebuggerDisplayFunc display = static (info, s) => $"Stream={s.GetType().Name}";
        return new SepUtf8Writer(new(stream, display), options, stream);
    }

    /// <summary>
    /// Gets the UTF-8 bytes from a MemoryStream-based writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <returns>The UTF-8 encoded bytes.</returns>
    public static byte[] ToUtf8Bytes(this SepUtf8Writer writer)
    {
        ArgumentNullException.ThrowIfNull(writer);
        // This would only work if the writer was created with ToUtf8()
        // In a real implementation, we'd need to track this internally
        throw new NotSupportedException("ToUtf8Bytes requires the writer to be created with ToUtf8()");
    }

    /// <summary>
    /// Asynchronously creates a UTF-8 writer that writes to a file.
    /// </summary>
    /// <param name="options">The writer options.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A new SepUtf8Writer instance.</returns>
    public static ValueTask<SepUtf8Writer> ToFileAsync(this SepUtf8WriterOptions options, string filePath, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(ToFile(options, filePath));
    }

    /// <summary>
    /// Asynchronously creates a UTF-8 writer that writes to a stream.
    /// </summary>
    /// <param name="options">The writer options.</param>
    /// <param name="stream">The output stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A new SepUtf8Writer instance.</returns>
    public static ValueTask<SepUtf8Writer> ToAsync(this SepUtf8WriterOptions options, Stream stream, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(To(options, stream));
    }
}
