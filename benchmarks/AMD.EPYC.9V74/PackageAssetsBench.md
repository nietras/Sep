```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.4 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     2.710 ms |  1.00 |  29 | 10731.8 |   54.2 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     2.781 ms |  1.03 |  29 | 10460.6 |   55.6 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     2.781 ms |  1.03 |  29 | 10457.9 |   55.6 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     3.509 ms |  1.29 |  29 |  8288.5 |   70.2 |       8.46 KB |        8.32 |
| ReadLine_    | Row   | 50000   |    16.379 ms |  6.04 |  29 |  1775.8 |  327.6 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    49.696 ms | 18.34 |  29 |   585.3 |  993.9 |      19.95 KB |       19.64 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     3.847 ms |  1.00 |  29 |  7560.6 |   76.9 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     4.652 ms |  1.21 |  29 |  6252.0 |   93.0 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     6.222 ms |  1.62 |  29 |  4675.0 |  124.4 |       8.46 KB |        8.33 |
| ReadLine_    | Cols  | 50000   |    15.883 ms |  4.13 |  29 |  1831.2 |  317.7 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Cols  | 50000   |    83.149 ms | 21.61 |  29 |   349.8 | 1663.0 |      445.6 KB |      438.75 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    39.102 ms |  1.00 |  29 |   743.8 |  782.0 |    13802.5 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    22.611 ms |  0.58 |  29 |  1286.3 |  452.2 |   13871.69 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    43.001 ms |  1.10 |  29 |   676.4 |  860.0 |   13962.42 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   113.355 ms |  2.90 |  29 |   256.6 | 2267.1 |  102133.71 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   102.028 ms |  2.61 |  29 |   285.1 | 2040.6 |   13971.73 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   705.306 ms |  1.00 | 581 |   825.0 |  705.3 |  266668.02 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   446.398 ms |  0.63 | 581 |  1303.5 |  446.4 |  274850.02 KB |        1.03 |
| Sylvan___    | Asset | 1000000 |   843.816 ms |  1.20 | 581 |   689.6 |  843.8 |  266826.25 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,374.169 ms |  4.78 | 581 |   172.5 | 3374.2 | 2038835.88 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,068.468 ms |  2.93 | 581 |   281.3 | 2068.5 |  266842.03 KB |        1.00 |
