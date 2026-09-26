```

BenchmarkDotNet v0.16.0-preview.1, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
Memory: 7 GB Total, 0.09 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.752 ms |  1.00 | 20 | 7366.7 |  110.1 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  18.644 ms |  6.80 | 20 | 1087.3 |  745.8 |    12.12 KB |       10.34 |
| ReadLine_ | Row    | 25000 |  16.884 ms |  6.16 | 20 | 1200.6 |  675.4 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  28.057 ms | 10.24 | 20 |  722.5 | 1122.3 |    19.95 KB |       17.02 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.405 ms |  1.00 | 20 | 5954.3 |  136.2 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  21.411 ms |  6.29 | 20 |  946.8 |  856.4 |    12.13 KB |       10.35 |
| ReadLine_ | Cols   | 25000 |  15.591 ms |  4.58 | 20 | 1300.2 |  623.6 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  35.517 ms | 10.44 | 20 |  570.8 | 1420.7 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  28.701 ms |  1.00 | 20 |  706.3 | 1148.0 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  11.531 ms |  0.40 | 20 | 1758.0 |  461.2 |    70.47 KB |        9.01 |
| Sylvan___ | Floats | 25000 |  79.649 ms |  2.78 | 20 |  254.5 | 3186.0 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  90.234 ms |  3.15 | 20 |  224.7 | 3609.4 | 73492.93 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 124.076 ms |  4.33 | 20 |  163.4 | 4963.0 | 22061.55 KB |    2,821.06 |
