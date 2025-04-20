```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-XBPEID : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-XBPEID  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.449 ms |  1.00 |  29 | 8461.8 |   69.0 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.650 ms |  1.06 |  29 | 7995.5 |   73.0 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.565 ms |  1.03 |  29 | 8184.9 |   71.3 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.536 ms |  1.32 |  29 | 6432.6 |   90.7 |       7.66 KB |        7.50 |
| ReadLine_    | Row   | 50000   |    20.179 ms |  5.85 |  29 | 1446.1 |  403.6 |   88608.27 KB |   86,744.62 |
| CsvHelper    | Row   | 50000   |    62.865 ms | 18.23 |  29 |  464.2 | 1257.3 |      20.07 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.030 ms |  1.00 |  29 | 5801.5 |  100.6 |       1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.806 ms |  1.15 |  29 | 5025.7 |  116.1 |       1.03 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     7.502 ms |  1.49 |  29 | 3889.7 |  150.0 |       7.67 KB |        7.46 |
| ReadLine_    | Cols  | 50000   |    21.399 ms |  4.25 |  29 | 1363.7 |  428.0 |   88608.28 KB |   86,167.97 |
| CsvHelper    | Cols  | 50000   |   109.575 ms | 21.78 |  29 |  266.3 | 2191.5 |     448.88 KB |      436.51 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    52.163 ms |  1.00 |  29 |  559.4 | 1043.3 |   13803.17 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    32.842 ms |  0.63 |  29 |  888.5 |  656.8 |   13920.79 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    58.391 ms |  1.12 |  29 |  499.8 | 1167.8 |   13962.64 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   171.508 ms |  3.30 |  29 |  170.1 | 3430.2 |  102134.46 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   132.390 ms |  2.55 |  29 |  220.4 | 2647.8 |   13971.94 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   992.044 ms |  1.00 | 583 |  588.5 |  992.0 |  266670.83 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   487.203 ms |  0.49 | 583 | 1198.2 |  487.2 |  269058.01 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,195.871 ms |  1.21 | 583 |  488.2 | 1195.9 |  266826.02 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,254.174 ms |  3.28 | 583 |  179.4 | 3254.2 | 2038843.73 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,644.197 ms |  2.67 | 583 |  220.8 | 2644.2 |  266841.13 KB |        1.00 |
