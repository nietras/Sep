```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4751/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-QAHTUF : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-QAHTUF  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.41 ms |  1.00 |  33 | 3206.3 |  208.2 |       1.21 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    10.75 ms |  1.03 |  33 | 3105.5 |  215.0 |       1.17 KB |        0.97 |
| Sep_Unescape | Row   | 50000   |    10.32 ms |  0.99 |  33 | 3233.3 |  206.5 |       1.21 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    26.60 ms |  2.51 |  33 | 1254.7 |  532.0 |       7.72 KB |        6.39 |
| ReadLine_    | Row   | 50000   |    24.43 ms |  2.30 |  33 | 1366.2 |  488.6 |  108778.79 KB |   90,048.08 |
| CsvHelper    | Row   | 50000   |    71.27 ms |  6.72 |  33 |  468.3 | 1425.5 |      23.22 KB |       19.22 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    12.22 ms |  1.00 |  33 | 2730.9 |  244.4 |       1.22 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.10 ms |  1.07 |  33 | 2547.4 |  262.1 |       1.22 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    30.00 ms |  2.46 |  33 | 1112.4 |  600.1 |       7.73 KB |        6.35 |
| ReadLine_    | Cols  | 50000   |    25.36 ms |  2.08 |  33 | 1316.0 |  507.3 |  108778.78 KB |   89,397.65 |
| CsvHelper    | Cols  | 50000   |   101.58 ms |  8.31 |  33 |  328.6 | 2031.6 |     445.86 KB |      366.42 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    59.31 ms |  1.00 |  33 |  562.8 | 1186.2 |   13803.31 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    41.53 ms |  0.70 |  33 |  803.6 |  830.7 |   13939.88 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    81.00 ms |  1.37 |  33 |  412.1 | 1619.9 |   13962.41 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   244.05 ms |  4.12 |  33 |  136.8 | 4881.0 |  122304.87 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   134.78 ms |  2.27 |  33 |  247.7 | 2695.5 |   13973.52 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,168.49 ms |  1.00 | 667 |  571.4 | 1168.5 |   266670.8 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   670.66 ms |  0.57 | 667 |  995.6 |  670.7 |  268687.83 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,636.51 ms |  1.40 | 667 |  408.0 | 1636.5 |  266825.86 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,203.80 ms |  3.60 | 667 |  158.8 | 4203.8 | 2442318.99 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,561.09 ms |  2.19 | 667 |  260.7 | 2561.1 |  266837.54 KB |        1.00 |
