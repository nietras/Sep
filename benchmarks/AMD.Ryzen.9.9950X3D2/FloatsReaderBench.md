```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 38.3 GB Available
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
| Sep______ | Row    | 25000 |  1.275 ms |  1.00 | 20 | 15939.4 |   51.0 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.771 ms |  1.39 | 20 | 11471.0 |   70.9 |  16.94 KB |        4.44 |
| CsvHelper | Row    | 25000 |  1.084 ms |  0.85 | 20 | 18751.0 |   43.3 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Cols   | 25000 |  1.729 ms |  1.00 | 20 | 11750.3 |   69.2 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.692 ms |  1.56 | 20 |  7546.9 |  107.7 |  16.94 KB |        4.44 |
| CsvHelper | Cols   | 25000 |  1.686 ms |  0.97 | 20 | 12053.5 |   67.4 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Floats | 25000 | 15.998 ms |  1.00 | 20 |  1270.1 |  639.9 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.724 ms |  0.17 | 20 |  7458.4 |  109.0 | 169.07 KB |       28.89 |
| Sylvan___ | Floats | 25000 | 35.346 ms |  2.21 | 20 |   574.9 | 1413.9 |   18.6 KB |        3.18 |
| CsvHelper | Floats | 25000 | 15.802 ms |  0.99 | 20 |  1285.9 |  632.1 |   6.09 KB |        1.04 |
