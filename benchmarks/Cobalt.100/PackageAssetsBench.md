```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 11.72 GB Available
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
| Sep______    | Row   | 50000   |     5.207 ms |  1.00 |  29 | 5604.0 |  104.1 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.164 ms |  0.99 |  29 | 5650.5 |  103.3 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.148 ms |  0.99 |  29 | 5668.7 |  103.0 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    18.840 ms |  3.62 |  29 | 1548.9 |  376.8 |       7470 B |        7.78 |
| ReadLine_    | Row   | 50000   |    20.230 ms |  3.89 |  29 | 1442.5 |  404.6 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    55.063 ms | 10.58 |  29 |  530.0 | 1101.3 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     6.691 ms |  1.00 |  29 | 4361.3 |  133.8 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.731 ms |  1.16 |  29 | 3774.6 |  154.6 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.552 ms |  3.52 |  29 | 1239.0 |  471.0 |       7478 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    20.730 ms |  3.10 |  29 | 1407.7 |  414.6 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    89.192 ms | 13.33 |  29 |  327.2 | 1783.8 |     456296 B |      475.31 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    36.087 ms |  1.00 |  29 |  808.6 |  721.7 |   14132928 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    18.321 ms |  0.51 |  29 | 1592.8 |  366.4 |   14206752 B |        1.01 |
| Sylvan___    | Asset | 50000   |    54.732 ms |  1.52 |  29 |  533.2 | 1094.6 |   14295531 B |        1.01 |
| ReadLine_    | Asset | 50000   |   121.569 ms |  3.37 |  29 |  240.0 | 2431.4 |  104584240 B |        7.40 |
| CsvHelper    | Asset | 50000   |    98.581 ms |  2.73 |  29 |  296.0 | 1971.6 |   14305232 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   789.400 ms |  1.00 | 583 |  739.5 |  789.4 |  273063688 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   368.128 ms |  0.47 | 583 | 1585.8 |  368.1 |  284999704 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,206.335 ms |  1.53 | 583 |  483.9 | 1206.3 |  273225560 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,705.934 ms |  3.43 | 583 |  215.7 | 2705.9 | 2087766448 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,141.202 ms |  2.71 | 583 |  272.6 | 2141.2 |  273235168 B |        1.00 |
