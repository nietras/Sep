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
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    10.032 ms |  1.00 |  33 | 3317.6 |  200.6 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     9.780 ms |  0.99 |  33 | 3402.9 |  195.6 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     8.938 ms |  0.90 |  33 | 3723.8 |  178.8 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    24.628 ms |  2.49 |  33 | 1351.4 |  492.6 |       7483 B |        7.79 |
| ReadLine_    | Row   | 50000   |    29.970 ms |  3.03 |  33 | 1110.5 |  599.4 |  111389416 B |  116,030.64 |
| CsvHelper    | Row   | 50000   |    64.365 ms |  6.50 |  33 |  517.1 | 1287.3 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.079 ms |  1.00 |  33 | 3004.0 |  221.6 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    11.982 ms |  1.09 |  33 | 2777.7 |  239.6 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    29.497 ms |  2.67 |  33 | 1128.3 |  589.9 |       7481 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    29.260 ms |  2.65 |  33 | 1137.4 |  585.2 |  111389416 B |  116,030.64 |
| CsvHelper    | Cols  | 50000   |    93.398 ms |  8.47 |  33 |  356.3 | 1868.0 |     456296 B |      475.31 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    57.797 ms |  1.00 |  33 |  575.8 | 1155.9 |   14133789 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    48.696 ms |  0.85 |  33 |  683.5 |  973.9 |   14211899 B |        1.01 |
| Sylvan___    | Asset | 50000   |    89.788 ms |  1.57 |  33 |  370.7 | 1795.8 |   14296236 B |        1.01 |
| ReadLine_    | Asset | 50000   |   180.323 ms |  3.16 |  33 |  184.6 | 3606.5 |  125240968 B |        8.86 |
| CsvHelper    | Asset | 50000   |   123.221 ms |  2.16 |  33 |  270.1 | 2464.4 |   14307020 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 | 1,372.191 ms |  1.00 | 665 |  485.2 | 1372.2 |  273070008 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   881.489 ms |  0.66 | 665 |  755.3 |  881.5 |  282546824 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,701.514 ms |  1.27 | 665 |  391.3 | 1701.5 |  273228744 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,291.345 ms |  2.46 | 665 |  202.3 | 3291.3 | 2500937216 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,224.096 ms |  1.66 | 665 |  299.4 | 2224.1 |  273242128 B |        1.00 |
