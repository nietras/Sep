using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepMemorySizeTest
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public void SepMemorySizeTest_MemorySize()
    {
        var readerOptionsSize = Unsafe.SizeOf<SepReaderOptions>();
        var writerOptionsSize = Unsafe.SizeOf<SepWriterOptions>();
        var readerSize = GetShallowObjectSize<SepReader>();
        var writerSize = GetShallowObjectSize<SepWriter>();

        TestContext.WriteLine($"SepReaderOptions: {readerOptionsSize} bytes");
        TestContext.WriteLine($"SepWriterOptions: {writerOptionsSize} bytes");
        TestContext.WriteLine($"SepReader shallow object: {readerSize} bytes");
        TestContext.WriteLine($"SepWriter shallow object: {writerSize} bytes");
    }

    static long GetShallowObjectSize<T>() where T : class
    {
        const int objectCount = 1024;
        var objects = new T[objectCount];
        var warmup = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
        var allocatedBytesBefore = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < objectCount; ++i)
        {
            objects[i] = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
        }
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBytesBefore;
        GC.KeepAlive(warmup);
        GC.KeepAlive(objects);
        return allocatedBytes / objectCount;
    }
}
