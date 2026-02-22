using System;

namespace nietras.SeparatedValues;

public abstract class SepToBytes : IDisposable
{
    public static SepCreateToBytes Direct { get; } =
        static (maybeHeader, colCount) => SepToBytesDirect.Instance;

    public virtual bool IsThreadSafe => false;

    public abstract ReadOnlyMemory<byte> ToBytes(ReadOnlySpan<byte> colSpan, int colIndex);

    internal virtual void DisposeManaged() { }

    #region Dispose
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!m_disposed)
        {
            if (disposing)
            {
                DisposeManaged();
            }
        }
        m_disposed = true;
    }

    volatile bool m_disposed = false;
    #endregion
}

// Signature to allow customizing per col or similar.
// Note that if source has no header this will be null.
public delegate SepToBytes SepCreateToBytes(SepReaderHeader? maybeHeader, int colCount);
