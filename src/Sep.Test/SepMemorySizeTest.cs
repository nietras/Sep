using System;
using System.Reflection;
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
        var readerStateSize = GetShallowObjectSize<SepReaderState>();
        var readerSize = GetShallowObjectSize<SepReader>();
        var writerSize = GetShallowObjectSize<SepWriter>();

        TestContext.WriteLine($"SepReaderOptions: {readerOptionsSize} bytes");
        TestContext.WriteLine($"SepWriterOptions: {writerOptionsSize} bytes");
        TestContext.WriteLine($"SepReaderState shallow object: {readerStateSize} bytes");
        TestContext.WriteLine($"SepReader shallow object: {readerSize} bytes");
        TestContext.WriteLine($"SepWriter shallow object: {writerSize} bytes");
        TestContext.WriteLine("Shallow size includes the CLR object header, instance fields, and alignment padding; referenced objects are excluded.");
        TestContext.WriteLine("Field widths are managed storage sizes. CLR auto-layout does not expose physical field offsets or guarantee field order.");
        TestContext.WriteLine("SepReader also contains the inherited SepReaderState parsing, buffer, row, and cache fields.");
        TestContext.WriteLine("SepWriter contains writer state and references to column caches and its array pool; their backing storage is separate.");
        WriteFieldLayout<SepReaderState>(TestContext, readerStateSize);
        WriteFieldLayout<SepReader>(TestContext, readerSize);
        WriteFieldLayout<SepWriter>(TestContext, writerSize);
    }

    [TestMethod]
    public void SepMemorySizeTest_ConstructorAllocatedWithStringReader_PackageAssets()
    {
        const int colCount = 25;
        var text = new string(',', colCount - 1) + "\n";
        Run();
        var allocatedBytesBefore = GC.GetAllocatedBytesForCurrentThread();
        Run();
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBytesBefore;
        TestContext.WriteLine($"{allocatedBytes} bytes");

        void Run()
        {
            using var reader = Sep.Reader(o => o with { HasHeader = false })
                                  .From(new System.IO.StringReader(text));
            foreach (var row in reader) { }
        }
    }

    [TestMethod]
    public void SepMemorySizeTest_ConstructorAllocatedWithStringReader_Floats()
    {
        const int featuresCount = 20;
        var colNames = new string[featuresCount * 2];
        for (var i = 0; i < featuresCount; i++)
        {
            colNames[i] = $"GT_Feature{i}";
            colNames[featuresCount + i] = $"RE_Feature{i}";
        }
        var separator = Sep.Default.Separator;
        var text = string.Join(separator, colNames) + "\n" +
                   new string(separator, colNames.Length - 1) + "\n";
        Run();
        var allocatedBytesBefore = GC.GetAllocatedBytesForCurrentThread();
        Run();
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBytesBefore;
        TestContext.WriteLine($"{allocatedBytes} bytes");

        void Run()
        {
            using var reader = Sep.Reader(o => o with { HasHeader = true })
                                  .From(new System.IO.StringReader(text));
            foreach (var row in reader) { }
        }
    }

    static readonly MethodInfo s_sizeOfMethod = typeof(SepMemorySizeTest).GetMethod(
    nameof(SizeOf), BindingFlags.Static | BindingFlags.NonPublic)!;

    static void WriteFieldLayout<T>(TestContext testContext, long instanceSize) where T : class
    {
        testContext.WriteLine($"{typeof(T).Name} instance fields, including base classes:");
        long totalFieldSize = 0;
        for (Type? type = typeof(T); type is not null && type != typeof(object); type = type.BaseType)
        {
            long declaredFieldSize = 0;
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            {
                var fieldSize = (int)s_sizeOfMethod.MakeGenericMethod(field.FieldType).Invoke(null, null)!;
                testContext.WriteLine($"  {type.Name}.{field.Name}: {field.FieldType.Name}, {fieldSize} bytes");
                declaredFieldSize += fieldSize;
            }
            totalFieldSize += declaredFieldSize;
            testContext.WriteLine($"  {type.Name} field storage total: {declaredFieldSize} bytes");
        }
        testContext.WriteLine($"  All field storage: {totalFieldSize} bytes; object header and layout padding: {instanceSize - totalFieldSize} bytes");
    }

    static int SizeOf<T>() => Unsafe.SizeOf<T>();

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
