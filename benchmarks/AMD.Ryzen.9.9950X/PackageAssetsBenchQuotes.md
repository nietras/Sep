```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 9950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.103
  [Host]     : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-MIRFZN : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-MIRFZN  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     4.256 ms |  1.00 |  33 | 7841.7 |   85.1 |       1.18 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     4.542 ms |  1.07 |  33 | 7348.9 |   90.8 |       1.18 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.412 ms |  1.04 |  33 | 7565.9 |   88.2 |       1.18 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    11.254 ms |  2.64 |  33 | 2965.7 |  225.1 |       7.67 KB |        6.49 |
| ReadLine_    | Row   | 50000   |    10.395 ms |  2.44 |  33 | 3210.8 |  207.9 |  108778.75 KB |   92,057.39 |
| CsvHelper    | Row   | 50000   |    27.967 ms |  6.57 |  33 | 1193.5 |  559.3 |         20 KB |       16.93 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.027 ms |  1.00 |  33 | 6639.8 |  100.5 |       1.18 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.045 ms |  1.20 |  33 | 5521.9 |  120.9 |       1.18 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    12.869 ms |  2.56 |  33 | 2593.6 |  257.4 |       7.68 KB |        6.51 |
| ReadLine_    | Cols  | 50000   |    11.911 ms |  2.37 |  33 | 2802.1 |  238.2 |  108778.73 KB |   92,133.52 |
| CsvHelper    | Cols  | 50000   |    42.657 ms |  8.49 |  33 |  782.5 |  853.1 |     445.66 KB |      377.47 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    28.635 ms |  1.00 |  33 | 1165.6 |  572.7 |   13802.66 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.550 ms |  0.75 |  33 | 1548.8 |  431.0 |   13996.79 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    37.887 ms |  1.32 |  33 |  881.0 |  757.7 |   13962.27 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   100.608 ms |  3.51 |  33 |  331.8 | 2012.2 |   122304.8 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    52.582 ms |  1.84 |  33 |  634.8 | 1051.6 |   13971.77 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   520.781 ms |  1.00 | 667 | 1282.1 |  520.8 |  266669.03 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   269.269 ms |  0.52 | 667 | 2479.7 |  269.3 |  269483.11 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   773.159 ms |  1.48 | 667 |  863.6 |  773.2 |     266825 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,805.081 ms |  3.47 | 667 |  369.9 | 1805.1 | 2442319.44 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,105.261 ms |  2.12 | 667 |  604.1 | 1105.3 |  266833.18 KB |        1.00 |
