using System;

namespace nietras.SeparatedValues;

public abstract class SepToString : IDisposable
{
    public static SepCreateToString Direct { get; } =
        static (header) => SepToStringDirect.Instance;

    public static SepCreateToString PoolPerCol(
        int maximumStringLength = SepStringHashPool.MaximumStringLengthDefault,
        int initialCapacity = SepStringHashPool.InitialCapacityDefault,
        int maximumCapacity = SepStringHashPool.MaximumCapacityDefault) =>
        header => new SepToStringHashPoolPerCol(header.ColNames.Count,
            maximumStringLength, initialCapacity, maximumCapacity);

    public static SepCreateToString OnePool(
        int maximumStringLength = SepStringHashPool.MaximumStringLengthDefault,
        int initialCapacity = SepStringHashPool.InitialCapacityDefault,
        int maximumCapacity = SepStringHashPool.MaximumCapacityDefault)
    {
        var s = new SepToStringHashPoolSingle(maximumStringLength, initialCapacity, maximumCapacity);
        return header => s;
    }

    public abstract string ToString(ReadOnlySpan<char> colSpan, int colIndex);

    internal virtual void DisposeManaged() { }

    #region Dispose
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Dispose(bool disposing) executes in two distinct scenarios.
    // If disposing equals true, the method has been called directly
    // or indirectly by a user's code. Managed and unmanaged resources
    // can be disposed.
    // If disposing equals false, the method has been called by the 
    // runtime from inside the finalizer and you should not reference 
    // other objects. Only unmanaged resources can be disposed.
    protected virtual void Dispose(bool disposing)
    {
        // Dispose only if we have not already disposed.
        if (!m_disposed)
        {
            // If disposing equals true, dispose all managed and unmanaged resources.
            // I.e. dispose managed resources only if true, unmanaged always.
            if (disposing)
            {
                DisposeManaged();
            }

            // Call the appropriate methods to clean up unmanaged resources here.
            // If disposing is false, only the following code is executed.
        }
        m_disposed = true;
    }

    volatile bool m_disposed = false;
    #endregion
}

// Signature to allow customizing per col or similar
public delegate SepToString SepCreateToString(SepHeader header);
