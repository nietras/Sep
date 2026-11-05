```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 45.48 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |  1.251 ms |  1.00 | 20 | 16244.8 |   50.0 |   4.95 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.671 ms |  1.34 | 20 | 12157.4 |   66.9 |  16.94 KB |        3.43 |
| CsvHelper | Row    | 25000 |  1.090 ms |  0.87 | 20 | 18638.5 |   43.6 |    4.3 KB |        0.87 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Cols   | 25000 |  1.690 ms |  1.00 | 20 | 12022.6 |   67.6 |   4.95 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.627 ms |  1.55 | 20 |  7734.4 |  105.1 |  16.94 KB |        3.43 |
| CsvHelper | Cols   | 25000 |  1.741 ms |  1.03 | 20 | 11674.5 |   69.6 |    4.3 KB |        0.87 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Floats | 25000 | 14.943 ms |  1.00 | 20 |  1359.8 |  597.7 |   7.63 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.191 ms |  0.15 | 20 |  9272.8 |   87.7 | 178.36 KB |       23.37 |
| Sylvan___ | Floats | 25000 | 35.287 ms |  2.36 | 20 |   575.8 | 1411.5 |   18.6 KB |        2.44 |
| CsvHelper | Floats | 25000 | 15.491 ms |  1.04 | 20 |  1311.7 |  619.6 |   6.09 KB |        0.80 |
