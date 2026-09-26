```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 11.36 GB Available
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
| Sep______    | Row   | 50000   |     4.607 ms |  1.00 |  29 | 6313.6 |   92.1 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.678 ms |  1.02 |  29 | 6217.4 |   93.6 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.212 ms |  1.13 |  29 | 5580.4 |  104.2 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    18.989 ms |  4.12 |  29 | 1531.7 |  379.8 |       7470 B |        7.78 |
| ReadLine_    | Row   | 50000   |    22.103 ms |  4.80 |  29 | 1315.9 |  442.1 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    55.077 ms | 11.96 |  29 |  528.1 | 1101.5 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     6.641 ms |  1.00 |  29 | 4379.7 |  132.8 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.410 ms |  1.12 |  29 | 3925.4 |  148.2 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.358 ms |  3.52 |  29 | 1245.2 |  467.2 |       7478 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    22.468 ms |  3.38 |  29 | 1294.6 |  449.4 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    88.253 ms | 13.29 |  29 |  329.6 | 1765.1 |     456296 B |      475.31 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    34.874 ms |  1.00 |  29 |  834.0 |  697.5 |   14132928 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    18.353 ms |  0.53 |  29 | 1584.8 |  367.1 |   14213961 B |        1.01 |
| Sylvan___    | Asset | 50000   |    56.746 ms |  1.63 |  29 |  512.6 | 1134.9 |   14295531 B |        1.01 |
| ReadLine_    | Asset | 50000   |   118.897 ms |  3.41 |  29 |  244.6 | 2377.9 |  104584256 B |        7.40 |
| CsvHelper    | Asset | 50000   |    99.318 ms |  2.85 |  29 |  292.9 | 1986.4 |   14305232 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   774.577 ms |  1.00 | 581 |  751.2 |  774.6 |  273063736 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   389.384 ms |  0.50 | 581 | 1494.4 |  389.4 |  281115680 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,194.789 ms |  1.54 | 581 |  487.0 | 1194.8 |  273225584 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,565.987 ms |  3.31 | 581 |  226.8 | 2566.0 | 2087766120 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,110.665 ms |  2.72 | 581 |  275.7 | 2110.7 |  273236888 B |        1.00 |
