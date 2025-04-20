```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-XBPEID : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-XBPEID  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    11.24 ms |  1.00 |  33 | 2970.2 |  224.7 |       1.06 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    13.00 ms |  1.16 |  33 | 2567.1 |  260.0 |       1.06 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.34 ms |  0.92 |  33 | 3229.0 |  206.7 |       1.04 KB |        0.99 |
| Sylvan___    | Row   | 50000   |    26.59 ms |  2.37 |  33 | 1255.1 |  531.9 |       7.72 KB |        7.31 |
| ReadLine_    | Row   | 50000   |    24.04 ms |  2.14 |  33 | 1388.3 |  480.8 |  108778.78 KB |  103,042.99 |
| CsvHelper    | Row   | 50000   |    71.28 ms |  6.34 |  33 |  468.3 | 1425.6 |      20.21 KB |       19.14 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    12.42 ms |  1.00 |  33 | 2686.7 |  248.5 |       1.05 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.22 ms |  1.06 |  33 | 2524.0 |  264.5 |       1.07 KB |        1.01 |
| Sylvan___    | Cols  | 50000   |    30.14 ms |  2.43 |  33 | 1107.3 |  602.8 |       7.73 KB |        7.35 |
| ReadLine_    | Cols  | 50000   |    26.19 ms |  2.11 |  33 | 1274.3 |  523.8 |  108778.79 KB |  103,425.70 |
| CsvHelper    | Cols  | 50000   |   101.12 ms |  8.14 |  33 |  330.1 | 2022.4 |     445.86 KB |      423.92 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    59.60 ms |  1.00 |  33 |  560.0 | 1192.0 |   13803.13 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    42.87 ms |  0.72 |  33 |  778.6 |  857.4 |      13947 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    79.36 ms |  1.33 |  33 |  420.6 | 1587.2 |    13962.4 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   197.46 ms |  3.32 |  33 |  169.0 | 3949.2 |  122304.88 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   126.66 ms |  2.13 |  33 |  263.5 | 2533.2 |   13971.72 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,114.98 ms |  1.00 | 667 |  598.9 | 1115.0 |  266678.59 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   666.45 ms |  0.60 | 667 | 1001.9 |  666.4 |   268990.3 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,613.38 ms |  1.45 | 667 |  413.9 | 1613.4 |  266825.61 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,194.02 ms |  3.76 | 667 |  159.2 | 4194.0 | 2442318.66 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,549.93 ms |  2.29 | 667 |  261.9 | 2549.9 |  266834.65 KB |        1.00 |
