```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.56GHz), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    11.85 ms |  1.00 |  33 | 2809.5 |  236.9 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    12.36 ms |  1.04 |  33 | 2693.0 |  247.2 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    11.98 ms |  1.01 |  33 | 2777.9 |  239.6 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    24.61 ms |  2.08 |  33 | 1352.1 |  492.3 |       8.47 KB |        8.34 |
| ReadLine_    | Row   | 50000   |    31.06 ms |  2.62 |  33 | 1071.6 |  621.2 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    79.24 ms |  6.69 |  33 |  420.0 | 1584.8 |      20.02 KB |       19.71 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    13.36 ms |  1.00 |  33 | 2491.5 |  267.2 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    15.05 ms |  1.13 |  33 | 2211.8 |  301.0 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.46 ms |  2.13 |  33 | 1169.4 |  569.2 |       8.47 KB |        8.33 |
| ReadLine_    | Cols  | 50000   |    29.74 ms |  2.23 |  33 | 1119.2 |  594.8 |  108778.73 KB |  106,899.63 |
| CsvHelper    | Cols  | 50000   |   107.80 ms |  8.07 |  33 |  308.7 | 2155.9 |     445.68 KB |      437.98 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    49.62 ms |  1.00 |  33 |  670.8 |  992.4 |   13801.77 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    33.60 ms |  0.68 |  33 |  990.4 |  672.1 |   13867.27 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    67.42 ms |  1.36 |  33 |  493.7 | 1348.3 |    13961.7 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   172.00 ms |  3.47 |  33 |  193.5 | 3439.9 |  122304.16 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   125.83 ms |  2.54 |  33 |  264.5 | 2516.5 |   13970.03 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,044.64 ms |  1.00 | 665 |  637.3 | 1044.6 |  266665.39 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   642.83 ms |  0.62 | 665 | 1035.7 |  642.8 |  271608.64 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,455.86 ms |  1.39 | 665 |  457.3 | 1455.9 |  266823.45 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,811.63 ms |  3.65 | 665 |  174.7 | 3811.6 | 2442317.53 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,616.99 ms |  2.51 | 665 |  254.4 | 2617.0 |  266831.77 KB |        1.00 |
