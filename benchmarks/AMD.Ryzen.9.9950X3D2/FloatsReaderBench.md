```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 44.47 GB Available
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
| Sep______ | Row    | 25000 |  1.046 ms |  1.00 | 20 | 19423.9 |   41.8 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.712 ms |  1.64 | 20 | 11868.3 |   68.5 |  16.94 KB |        4.44 |
| CsvHelper | Row    | 25000 |  1.101 ms |  1.05 | 20 | 18458.3 |   44.0 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Cols   | 25000 |  1.471 ms |  1.00 | 20 | 13813.4 |   58.8 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.642 ms |  1.80 | 20 |  7692.2 |  105.7 |  16.94 KB |        4.44 |
| CsvHelper | Cols   | 25000 |  1.613 ms |  1.10 | 20 | 12594.5 |   64.5 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Floats | 25000 | 15.114 ms |  1.00 | 20 |  1344.4 |  604.6 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  1.979 ms |  0.13 | 20 | 10267.5 |   79.2 | 168.67 KB |       28.83 |
| Sylvan___ | Floats | 25000 | 35.733 ms |  2.36 | 20 |   568.7 | 1429.3 |   18.6 KB |        3.18 |
| CsvHelper | Floats | 25000 | 15.804 ms |  1.05 | 20 |  1285.8 |  632.2 |   6.09 KB |        1.04 |
