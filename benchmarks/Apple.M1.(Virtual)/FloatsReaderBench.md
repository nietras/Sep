```

BenchmarkDotNet v0.16.0-preview.2, macOS Tahoe 26.7.2 (25G332) [Darwin 25.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
Memory: 7 GB Total, 0.08 GB Available
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
| Sep______ | Row    | 25000 |  2.922 ms |  1.00 | 20 | 6937.3 |  116.9 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 | 22.341 ms |  7.70 | 20 |  907.4 |  893.6 |  16.55 KB |        4.34 |
| CsvHelper | Row    | 25000 |  4.940 ms |  1.70 | 20 | 4103.8 |  197.6 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Cols   | 25000 |  4.066 ms |  1.00 | 20 | 4985.8 |  162.6 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 | 24.689 ms |  6.10 | 20 |  821.1 |  987.6 |  16.56 KB |        4.34 |
| CsvHelper | Cols   | 25000 |  5.961 ms |  1.47 | 20 | 3401.0 |  238.4 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Floats | 25000 | 27.362 ms |  1.00 | 20 |  740.9 | 1094.5 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 | 13.097 ms |  0.48 | 20 | 1547.8 |  523.9 |  62.07 KB |       10.61 |
| Sylvan___ | Floats | 25000 | 71.263 ms |  2.63 | 20 |  284.5 | 2850.5 |  18.23 KB |        3.12 |
| CsvHelper | Floats | 25000 | 35.538 ms |  1.31 | 20 |  570.4 | 1421.5 |   6.09 KB |        1.04 |
