```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-YBSRVP : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-YBSRVP  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.531 ms |  1.00 |  33 | 3169.5 |  210.6 |       1.21 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     9.910 ms |  0.94 |  33 | 3368.2 |  198.2 |       1.21 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    26.180 ms |  2.49 |  33 | 1274.9 |  523.6 |       7.72 KB |        6.38 |
| ReadLine_    | Row   | 50000   |    24.019 ms |  2.28 |  33 | 1389.6 |  480.4 |  108778.78 KB |   89,902.72 |
| CsvHelper    | Row   | 50000   |    71.228 ms |  6.76 |  33 |  468.6 | 1424.6 |       20.2 KB |       16.70 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.933 ms |  1.00 |  33 | 2797.1 |  238.7 |       1.22 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.486 ms |  1.13 |  33 | 2474.9 |  269.7 |       1.22 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    29.535 ms |  2.48 |  33 | 1130.1 |  590.7 |       7.73 KB |        6.35 |
| ReadLine_    | Cols  | 50000   |    26.438 ms |  2.22 |  33 | 1262.5 |  528.8 |  108778.78 KB |   89,397.65 |
| CsvHelper    | Cols  | 50000   |   104.426 ms |  8.75 |  33 |  319.6 | 2088.5 |     445.78 KB |      366.36 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    57.695 ms |  1.00 |  33 |  578.5 | 1153.9 |    13803.3 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    47.415 ms |  0.82 |  33 |  703.9 |  948.3 |   13941.84 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    92.123 ms |  1.60 |  33 |  362.3 | 1842.5 |   13962.42 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   196.126 ms |  3.40 |  33 |  170.2 | 3922.5 |  122305.15 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   127.767 ms |  2.22 |  33 |  261.2 | 2555.3 |   13972.13 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,129.007 ms |  1.00 | 667 |  591.4 | 1129.0 |  266670.93 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   674.504 ms |  0.60 | 667 |  989.9 |  674.5 |  268208.37 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,656.453 ms |  1.47 | 667 |  403.1 | 1656.5 |  266825.34 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,269.727 ms |  3.78 | 667 |  156.4 | 4269.7 | 2442331.02 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,581.438 ms |  2.29 | 667 |  258.7 | 2581.4 |  266834.02 KB |        1.00 |
