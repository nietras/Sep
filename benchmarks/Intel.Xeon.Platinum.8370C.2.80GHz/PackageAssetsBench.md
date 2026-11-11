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
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     4.538 ms |  1.00 |  29 | 6409.7 |   90.8 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.683 ms |  1.03 |  29 | 6211.4 |   93.7 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.799 ms |  1.06 |  29 | 6061.0 |   96.0 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |     5.382 ms |  1.19 |  29 | 5403.8 |  107.6 |      8659 B |       30.07 |
| CsvHelper    | Row   | 50000   |     6.132 ms |  1.36 |  29 | 4742.9 |  122.6 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     6.230 ms |  1.00 |  29 | 4668.7 |  124.6 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.047 ms |  1.13 |  29 | 4127.6 |  140.9 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.981 ms |  1.44 |  29 | 3238.5 |  179.6 |      8660 B |       30.07 |
| CsvHelper    | Cols  | 50000   |    20.181 ms |  3.24 |  29 | 1441.2 |  403.6 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    45.812 ms |  1.00 |  29 |  634.9 |  916.2 |  14132488 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    29.890 ms |  0.65 |  29 |  973.1 |  597.8 |  14183317 B |        1.00 |
| Sylvan___    | Asset | 50000   |    52.121 ms |  1.14 |  29 |  558.0 | 1042.4 |  14296952 B |        1.01 |
| CsvHelper    | Asset | 50000   |    42.135 ms |  0.92 |  29 |  690.3 |  842.7 |  15111808 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   935.396 ms |  1.00 | 581 |  622.1 |  935.4 | 273065344 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   603.762 ms |  0.65 | 581 |  963.8 |  603.8 | 283492320 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,133.923 ms |  1.21 | 581 |  513.2 | 1133.9 | 273227704 B |        1.00 |
| CsvHelper    | Asset | 1000000 |   934.340 ms |  1.00 | 581 |  622.8 |  934.3 | 274043824 B |        1.00 |
