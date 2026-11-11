```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 3.49GHz), 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.5 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     7.311 ms |  1.00 |  33 | 4552.6 |  146.2 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.393 ms |  1.01 |  33 | 4501.7 |  147.9 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.855 ms |  1.07 |  33 | 4236.9 |  157.1 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    24.101 ms |  3.30 |  33 | 1380.9 |  482.0 |      8670 B |       30.10 |
| CsvHelper    | Row   | 50000   |     8.561 ms |  1.17 |  33 | 3887.6 |  171.2 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     8.617 ms |  1.00 |  33 | 3862.5 |  172.3 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    11.059 ms |  1.28 |  33 | 3009.5 |  221.2 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.866 ms |  3.24 |  33 | 1194.3 |  557.3 |      8673 B |       30.11 |
| CsvHelper    | Cols  | 50000   |    25.245 ms |  2.93 |  33 | 1318.4 |  504.9 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    47.470 ms |  1.00 |  33 |  701.1 |  949.4 |  14132565 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    32.179 ms |  0.68 |  33 | 1034.3 |  643.6 |  14198447 B |        1.00 |
| Sylvan___    | Asset | 50000   |    70.475 ms |  1.48 |  33 |  472.3 | 1409.5 |  14296789 B |        1.01 |
| CsvHelper    | Asset | 50000   |    51.334 ms |  1.08 |  33 |  648.3 | 1026.7 |  15111774 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 | 1,021.018 ms |  1.00 | 665 |  652.1 | 1021.0 | 273065352 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   647.559 ms |  0.63 | 665 | 1028.2 |  647.6 | 280929848 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,527.328 ms |  1.50 | 665 |  435.9 | 1527.3 | 273227648 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,051.857 ms |  1.03 | 665 |  633.0 | 1051.9 | 274043832 B |        1.00 |
