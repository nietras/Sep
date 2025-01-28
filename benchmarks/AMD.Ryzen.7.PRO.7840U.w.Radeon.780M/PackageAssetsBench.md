```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4751/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-QAHTUF : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-QAHTUF  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     4.250 ms |  1.00 | 29  | 6865.5 |   85.0 |      1.33 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     4.447 ms |  1.05 | 29  | 6562.5 |   88.9 |      1.32 KB |        0.99 |
| Sep_Unescape | Row   | 50000   |     4.278 ms |  1.01 | 29  | 6822.0 |   85.6 |      1.18 KB |        0.89 |
| Sylvan___    | Row   | 50000   |     4.768 ms |  1.07 |  29 | 6119.8 |   95.4 |      7.66 KB |        6.48 |
| ReadLine_    | Row   | 50000   |    20.959 ms |  4.71 |  29 | 1392.3 |  419.2 |  88608.26 KB |   74,925.56 |
| CsvHelper    | Row   | 50000   |    65.193 ms | 14.64 |  29 |  447.6 | 1303.9 |      20.2 KB |       17.08 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     6.747 ms |  1.00 |  29 | 4325.2 |  134.9 |      1.19 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.739 ms |  1.00 |  29 | 4330.0 |  134.8 |      1.19 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     7.509 ms |  1.12 |  29 | 3885.9 |  150.2 |      7.67 KB |        6.46 |
| ReadLine_    | Cols  | 50000   |    23.536 ms |  3.50 |  29 | 1239.9 |  470.7 |  88608.28 KB |   74,617.50 |
| CsvHelper    | Cols  | 50000   |   107.075 ms | 15.94 |  29 |  272.5 | 2141.5 |    448.88 KB |      378.00 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    54.052 ms |  1.00 |  29 |  539.9 | 1081.0 |   13803.3 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    35.594 ms |  0.66 |  29 |  819.8 |  711.9 |  13914.86 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    62.009 ms |  1.15 |  29 |  470.6 | 1240.2 |  13962.68 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   201.825 ms |  3.73 |  29 |  144.6 | 4036.5 | 102134.43 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   135.566 ms |  2.51 |  29 |  215.3 | 2711.3 |  13972.69 KB |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 | 1,047.265 ms |  1.00 | 583 |  557.4 | 1047.3 | 266672.16 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   492.995 ms |  0.47 | 583 | 1184.2 |  493.0 | 267823.63 KB |        1.00 |
| Sylvan___    | Asset | 1000000 | 1,218.367 ms |  1.16 | 583 |  479.2 | 1218.4 | 266825.65 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,276.776 ms |  3.13 | 583 |  178.2 | 3276.8 | 2038836.1 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,683.525 ms |  2.56 | 583 |  217.5 | 2683.5 | 266846.78 KB |        1.00 |
