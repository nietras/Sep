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
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     7.713 ms |  1.00 |  33 | 4315.3 |  154.3 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     8.293 ms |  1.08 |  33 | 4013.3 |  165.9 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     8.712 ms |  1.13 |  33 | 3820.2 |  174.2 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    18.814 ms |  2.44 |  33 | 1769.0 |  376.3 |       8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |    19.747 ms |  2.56 |  33 | 1685.4 |  394.9 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    55.713 ms |  7.22 |  33 |  597.4 | 1114.3 |      19.95 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     9.451 ms |  1.00 |  33 | 3521.6 |  189.0 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    10.546 ms |  1.12 |  33 | 3156.0 |  210.9 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    21.870 ms |  2.32 |  33 | 1521.8 |  437.4 |       8.47 KB |        8.34 |
| ReadLine_    | Cols  | 50000   |    20.099 ms |  2.13 |  33 | 1655.9 |  402.0 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |    78.704 ms |  8.33 |  33 |  422.9 | 1574.1 |      445.6 KB |      438.75 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    42.341 ms |  1.00 |  33 |  786.0 |  846.8 |   13802.27 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.411 ms |  0.67 |  33 | 1171.5 |  568.2 |   13868.78 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    57.851 ms |  1.37 |  33 |  575.3 | 1157.0 |    13962.7 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   143.487 ms |  3.39 |  33 |  232.0 | 2869.7 |  122304.61 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    98.119 ms |  2.32 |  33 |  339.2 | 1962.4 |   13970.22 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   830.435 ms |  1.00 | 665 |  801.7 |  830.4 |  266667.91 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   517.765 ms |  0.62 | 665 | 1285.9 |  517.8 |  270096.88 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,139.542 ms |  1.37 | 665 |  584.3 | 1139.5 |     266825 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,428.533 ms |  4.13 | 665 |  194.2 | 3428.5 | 2442319.34 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,973.817 ms |  2.38 | 665 |  337.3 | 1973.8 |  266835.77 KB |        1.00 |
