```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3)
Cobalt 100, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-MYYDFG : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-MYYDFG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     4.893 ms |  1.00 |  29 | 5964.2 |   97.9 |        968 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.150 ms |  1.05 |  29 | 5666.4 |  103.0 |        970 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.886 ms |  1.00 |  29 | 5971.9 |   97.7 |        971 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.349 ms |  4.16 |  29 | 1434.0 |  407.0 |       6695 B |        6.92 |
| ReadLine_    | Row   | 50000   |    24.816 ms |  5.07 |  29 | 1175.9 |  496.3 |   90734858 B |   93,734.36 |
| CsvHelper    | Row   | 50000   |    53.226 ms | 10.88 |  29 |  548.2 | 1064.5 |      20529 B |       21.21 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     6.123 ms |  1.00 |  29 | 4765.7 |  122.5 |        975 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.084 ms |  1.16 |  29 | 4119.2 |  141.7 |        974 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.969 ms |  3.92 |  29 | 1217.4 |  479.4 |       6686 B |        6.86 |
| ReadLine_    | Cols  | 50000   |    25.605 ms |  4.18 |  29 | 1139.7 |  512.1 |   90734858 B |   93,061.39 |
| CsvHelper    | Cols  | 50000   |    89.216 ms | 14.58 |  29 |  327.1 | 1784.3 |     456552 B |      468.26 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    45.041 ms |  1.00 |  29 |  647.9 |  900.8 |   14133894 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    29.386 ms |  0.65 |  29 |  993.0 |  587.7 |   14198826 B |        1.00 |
| Sylvan___    | Asset | 50000   |    68.931 ms |  1.53 |  29 |  423.3 | 1378.6 |   14296154 B |        1.01 |
| ReadLine_    | Asset | 50000   |   128.018 ms |  2.85 |  29 |  227.9 | 2560.4 |  104585240 B |        7.40 |
| CsvHelper    | Asset | 50000   |   111.246 ms |  2.47 |  29 |  262.3 | 2224.9 |   14307390 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   908.323 ms |  1.00 | 583 |  642.7 |  908.3 |  273069736 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   430.643 ms |  0.47 | 583 | 1355.6 |  430.6 |  283496648 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,395.957 ms |  1.54 | 583 |  418.2 | 1396.0 |  273229632 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,270.259 ms |  2.50 | 583 |  257.1 | 2270.3 | 2087766384 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,239.974 ms |  2.47 | 583 |  260.6 | 2240.0 |  273237552 B |        1.00 |
