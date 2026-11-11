```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 10.73 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |  3.199 ms |  1.00 | 20 | 6337.3 |  128.0 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 | 22.851 ms |  7.14 | 20 |  887.1 |  914.1 |  16.56 KB |        4.34 |
| CsvHelper | Row    | 25000 |  7.490 ms |  2.34 | 20 | 2706.4 |  299.6 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Cols   | 25000 |  4.268 ms |  1.00 | 20 | 4750.2 |  170.7 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 | 25.611 ms |  6.00 | 20 |  791.5 | 1024.5 |  16.56 KB |        4.34 |
| CsvHelper | Cols   | 25000 |  8.570 ms |  2.01 | 20 | 2365.4 |  342.8 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Floats | 25000 | 31.733 ms |  1.00 | 20 |  638.8 | 1269.3 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  9.397 ms |  0.30 | 20 | 2157.3 |  375.9 |  50.67 KB |        8.66 |
| Sylvan___ | Floats | 25000 | 89.031 ms |  2.81 | 20 |  227.7 | 3561.2 |  18.24 KB |        3.12 |
| CsvHelper | Floats | 25000 | 38.299 ms |  1.21 | 20 |  529.3 | 1532.0 |   6.09 KB |        1.04 |
