```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 10.67 GB Available
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
| Sep______ | Row    | 25000 |  3.220 ms |  1.00 | 20 | 6295.9 |  128.8 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 | 22.748 ms |  7.06 | 20 |  891.2 |  909.9 |  16.56 KB |        4.34 |
| CsvHelper | Row    | 25000 |  7.512 ms |  2.33 | 20 | 2698.6 |  300.5 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Cols   | 25000 |  4.252 ms |  1.00 | 20 | 4767.9 |  170.1 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 | 25.762 ms |  6.06 | 20 |  786.9 | 1030.5 |  16.56 KB |        4.34 |
| CsvHelper | Cols   | 25000 |  8.503 ms |  2.00 | 20 | 2384.1 |  340.1 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Floats | 25000 | 31.538 ms |  1.00 | 20 |  642.8 | 1261.5 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  9.304 ms |  0.29 | 20 | 2178.9 |  372.1 |  51.25 KB |        8.76 |
| Sylvan___ | Floats | 25000 | 89.167 ms |  2.83 | 20 |  227.3 | 3566.7 |  18.24 KB |        3.12 |
| CsvHelper | Floats | 25000 | 38.162 ms |  1.21 | 20 |  531.2 | 1526.5 |   6.09 KB |        1.04 |
