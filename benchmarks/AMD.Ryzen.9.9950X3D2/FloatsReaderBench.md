```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 40.79 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean        | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |------------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |    816.1 μs |  1.00 | 20 | 24897.3 |   32.6 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1,672.3 μs |  2.05 | 20 | 12150.4 |   66.9 |  16.94 KB |        4.44 |
| CsvHelper | Row    | 25000 |  1,087.4 μs |  1.33 | 20 | 18686.9 |   43.5 |    4.3 KB |        1.13 |
|           |        |       |             |       |    |         |        |           |             |
| Sep______ | Cols   | 25000 |  1,241.7 μs |  1.00 | 20 | 16364.1 |   49.7 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2,596.8 μs |  2.09 | 20 |  7824.8 |  103.9 |  16.94 KB |        4.44 |
| CsvHelper | Cols   | 25000 |  1,580.1 μs |  1.27 | 20 | 12860.0 |   63.2 |    4.3 KB |        1.13 |
|           |        |       |             |       |    |         |        |           |             |
| Sep______ | Floats | 25000 | 15,339.6 μs |  1.00 | 20 |  1324.7 |  613.6 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2,036.0 μs |  0.13 | 20 |  9980.1 |   81.4 | 171.51 KB |       29.31 |
| Sylvan___ | Floats | 25000 | 35,578.4 μs |  2.32 | 20 |   571.1 | 1423.1 |   18.6 KB |        3.18 |
| CsvHelper | Floats | 25000 | 15,746.1 μs |  1.03 | 20 |  1290.5 |  629.8 |   6.09 KB |        1.04 |
