using System;

namespace nietras.SeparatedValues;

public abstract class SepToString : IDisposable
{
    public static SepCreateToString Direct { get; } =
        static (header, colIndex) => SepToStringDirect.Instance;

    public static SepCreateToString PoolPerCol(
        int maximumStringLength = SepToStringHashPool.MaximumStringLengthDefault,
        int initialCapacity = SepToStringHashPool.InitialCapacityDefault,
        int maximumCapacity = SepToStringHashPool.MaximumCapacityDefault) =>
        (header, colIndex) => new SepToStringHashPool(maximumStringLength, initialCapacity, maximumCapacity);

    public static SepCreateToString OnePool(
        int maximumStringLength = SepToStringHashPool.MaximumStringLengthDefault,
        int initialCapacity = SepToStringHashPool.InitialCapacityDefault,
        int maximumCapacity = SepToStringHashPool.MaximumCapacityDefault)
    {
        // Issue with code analysis
#pragma warning disable CA2000 // Dispose objects before losing scope
        var s = new SepToStringHashPool(maximumStringLength, initialCapacity, maximumCapacity);
#pragma warning restore CA2000 // Dispose objects before losing scope
        return (header, colIndex) => s;
    }

    public abstract string ToString(ReadOnlySpan<char> chars);

    protected virtual void DisposeManagedResources() { }

    #region Dispose
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    {
        Dispose(true);
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
                DisposeManagedResources();
            }

            // Call the appropriate methods to clean up unmanaged resources here.
            // If disposing is false, only the following code is executed.
        }
        m_disposed = true;
    }

    volatile bool m_disposed = false;
    #endregion
}

// Signature to allow customizing per col
public delegate SepToString SepCreateToString(SepHeader header, int colIndex);
