```

BenchmarkDotNet v0.16.0-preview.2, macOS Tahoe 26.7.2 (25G332) [Darwin 25.6.0]
Apple M2 Pro (Virtual), 1 CPU, 5 logical and 5 physical cores
Memory: 14 GB Total, 0.8 GB Available
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
| Sep______ | Row    | 25000 |  2.486 ms |  1.00 | 20 | 8155.4 |   99.4 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 | 20.142 ms |  8.11 | 20 | 1006.5 |  805.7 |  16.55 KB |        4.34 |
| CsvHelper | Row    | 25000 |  4.407 ms |  1.77 | 20 | 4600.4 |  176.3 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Cols   | 25000 |  3.812 ms |  1.00 | 20 | 5318.0 |  152.5 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 | 22.395 ms |  5.88 | 20 |  905.2 |  895.8 |  16.56 KB |        4.34 |
| CsvHelper | Cols   | 25000 |  5.469 ms |  1.44 | 20 | 3707.0 |  218.7 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Floats | 25000 | 25.694 ms |  1.00 | 20 |  789.0 | 1027.8 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  6.853 ms |  0.27 | 20 | 2958.1 |  274.1 |  57.91 KB |        9.90 |
| Sylvan___ | Floats | 25000 | 67.617 ms |  2.63 | 20 |  299.8 | 2704.7 |  18.23 KB |        3.12 |
| CsvHelper | Floats | 25000 | 32.111 ms |  1.25 | 20 |  631.3 | 1284.4 |   6.09 KB |        1.04 |
