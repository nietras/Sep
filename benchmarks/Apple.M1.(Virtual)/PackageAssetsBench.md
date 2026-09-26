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
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     2.995 ms |  1.00 |  29 | 9710.3 |   59.9 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.100 ms |  1.03 |  29 | 9382.1 |   62.0 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.082 ms |  1.03 |  29 | 9438.7 |   61.6 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    17.699 ms |  5.91 |  29 | 1643.3 |  354.0 |       7470 B |        7.78 |
| ReadLine_    | Row   | 50000   |    17.413 ms |  5.81 |  29 | 1670.3 |  348.3 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    39.519 ms | 13.19 |  29 |  736.0 |  790.4 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     4.192 ms |  1.00 |  29 | 6937.9 |   83.8 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     4.553 ms |  1.09 |  29 | 6388.4 |   91.1 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    20.472 ms |  4.89 |  29 | 1420.8 |  409.4 |       7477 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    17.586 ms |  4.20 |  29 | 1653.9 |  351.7 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    61.801 ms | 14.77 |  29 |  470.6 | 1236.0 |     456296 B |      475.31 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    40.512 ms |  1.00 |  29 |  718.0 |  810.2 |   14133948 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    31.156 ms |  0.77 |  29 |  933.6 |  623.1 |   14272320 B |        1.01 |
| Sylvan___    | Asset | 50000   |    54.648 ms |  1.35 |  29 |  532.2 | 1093.0 |   14296248 B |        1.01 |
| ReadLine_    | Asset | 50000   |   108.068 ms |  2.67 |  29 |  269.1 | 2161.4 |  104585460 B |        7.40 |
| CsvHelper    | Asset | 50000   |    77.913 ms |  1.93 |  29 |  373.3 | 1558.3 |   14305754 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   685.099 ms |  1.00 | 581 |  849.3 |  685.1 |  273069992 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   421.832 ms |  0.62 | 581 | 1379.4 |  421.8 |  283925624 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,130.919 ms |  1.65 | 581 |  514.5 | 1130.9 |  273228600 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,361.686 ms |  3.45 | 581 |  246.4 | 2361.7 | 2087768080 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,879.568 ms |  2.75 | 581 |  309.6 | 1879.6 |  273245616 B |        1.00 |
