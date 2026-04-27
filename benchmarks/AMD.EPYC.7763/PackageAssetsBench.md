```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.24GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.593 ms |  1.00 |  29 | 8094.2 |   71.9 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.886 ms |  1.08 |  29 | 7484.8 |   77.7 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.586 ms |  1.00 |  29 | 8110.7 |   71.7 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.341 ms |  1.21 |  29 | 6700.0 |   86.8 |       8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |    22.018 ms |  6.13 |  29 | 1321.0 |  440.4 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    63.984 ms | 17.81 |  29 |  454.6 | 1279.7 |      19.95 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.957 ms |  1.00 |  29 | 5868.2 |   99.1 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.868 ms |  1.18 |  29 | 4956.5 |  117.4 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.174 ms |  1.65 |  29 | 3558.5 |  163.5 |       8.46 KB |        8.32 |
| ReadLine_    | Cols  | 50000   |    23.316 ms |  4.70 |  29 | 1247.5 |  466.3 |   88608.23 KB |   87,161.22 |
| CsvHelper    | Cols  | 50000   |   103.321 ms | 20.85 |  29 |  281.5 | 2066.4 |     445.67 KB |      438.39 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    39.423 ms |  1.00 |  29 |  737.8 |  788.5 |   13802.24 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.737 ms |  0.73 |  29 | 1012.1 |  574.7 |   13861.74 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    48.458 ms |  1.23 |  29 |  600.2 |  969.2 |   13961.98 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   123.884 ms |  3.14 |  29 |  234.8 | 2477.7 |  102133.97 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   123.793 ms |  3.14 |  29 |  235.0 | 2475.9 |   13971.04 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   825.400 ms |  1.00 | 581 |  705.0 |  825.4 |  266668.32 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   506.193 ms |  0.61 | 581 | 1149.5 |  506.2 |  275203.41 KB |        1.03 |
| Sylvan___    | Asset | 1000000 |   998.747 ms |  1.21 | 581 |  582.6 |  998.7 |  266823.98 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,634.271 ms |  3.19 | 581 |  220.9 | 2634.3 | 2038835.21 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,494.118 ms |  3.02 | 581 |  233.3 | 2494.1 |  266839.37 KB |        1.00 |
