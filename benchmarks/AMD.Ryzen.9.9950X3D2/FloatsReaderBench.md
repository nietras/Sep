```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 53.15 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean        | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |------------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |    796.0 μs |  1.00 | 20 | 25528.1 |   31.8 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1,660.5 μs |  2.09 | 20 | 12236.8 |   66.4 |  16.94 KB |        4.44 |
| CsvHelper | Row    | 25000 |  1,086.9 μs |  1.37 | 20 | 18694.3 |   43.5 |    4.3 KB |        1.13 |
|           |        |       |             |       |    |         |        |           |             |
| Sep______ | Cols   | 25000 |  1,227.9 μs |  1.00 | 20 | 16548.1 |   49.1 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2,583.2 μs |  2.10 | 20 |  7866.0 |  103.3 |  16.94 KB |        4.44 |
| CsvHelper | Cols   | 25000 |  1,581.5 μs |  1.29 | 20 | 12848.3 |   63.3 |    4.3 KB |        1.13 |
|           |        |       |             |       |    |         |        |           |             |
| Sep______ | Floats | 25000 | 15,489.5 μs |  1.00 | 20 |  1311.8 |  619.6 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  1,733.5 μs |  0.11 | 20 | 11721.6 |   69.3 | 169.55 KB |       28.97 |
| Sylvan___ | Floats | 25000 | 33,565.8 μs |  2.17 | 20 |   605.4 | 1342.6 |   18.6 KB |        3.18 |
| CsvHelper | Floats | 25000 | 15,621.0 μs |  1.01 | 20 |  1300.8 |  624.8 |   6.09 KB |        1.04 |
