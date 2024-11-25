```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-YBSRVP : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-YBSRVP  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     4.200 ms |  1.00 |  29 | 6948.2 |   84.0 |      1.18 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.236 ms |  1.01 |  29 | 6888.6 |   84.7 |      1.33 KB |        1.13 |
| Sylvan___    | Row   | 50000   |     4.491 ms |  1.07 |  29 | 6497.9 |   89.8 |      7.66 KB |        6.49 |
| ReadLine_    | Row   | 50000   |    21.321 ms |  5.08 |  29 | 1368.7 |  426.4 |  88608.28 KB |   75,049.53 |
| CsvHelper    | Row   | 50000   |    62.781 ms | 14.95 |  29 |  464.8 | 1255.6 |      20.2 KB |       17.11 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     5.852 ms |  1.00 |  29 | 4986.3 |  117.0 |      1.19 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.573 ms |  1.12 |  29 | 4439.6 |  131.5 |      1.19 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.553 ms |  1.46 |  29 | 3411.8 |  171.1 |      7.67 KB |        6.44 |
| ReadLine_    | Cols  | 50000   |    22.541 ms |  3.85 |  29 | 1294.6 |  450.8 |  88608.29 KB |   74,372.86 |
| CsvHelper    | Cols  | 50000   |   108.328 ms | 18.51 |  29 |  269.4 | 2166.6 |    445.78 KB |      374.16 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    53.003 ms |  1.00 |  29 |  550.6 | 1060.1 |  13803.34 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    38.829 ms |  0.74 |  29 |  751.5 |  776.6 |  13913.62 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    70.050 ms |  1.33 |  29 |  416.6 | 1401.0 |   13962.4 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   233.476 ms |  4.42 |  29 |  125.0 | 4669.5 | 102134.79 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   133.896 ms |  2.54 |  29 |  217.9 | 2677.9 |  13972.19 KB |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   993.034 ms |  1.00 | 583 |  587.9 |  993.0 | 266670.95 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   504.125 ms |  0.51 | 583 | 1158.0 |  504.1 | 268509.69 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,206.663 ms |  1.22 | 583 |  483.8 | 1206.7 | 266825.98 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,256.930 ms |  3.28 | 583 |  179.2 | 3256.9 |   2038848 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,657.874 ms |  2.68 | 583 |  219.6 | 2657.9 | 266843.54 KB |        1.00 |
