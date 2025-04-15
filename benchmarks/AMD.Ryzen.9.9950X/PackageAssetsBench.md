```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 9950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.103
  [Host]     : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-MIRFZN : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-MIRFZN  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     1.603 ms |  1.00 |  29 | 18202.7 |   32.1 |       1.17 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     1.662 ms |  1.04 |  29 | 17560.2 |   33.2 |       1.17 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     1.621 ms |  1.01 |  29 | 18001.3 |   32.4 |       1.17 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     1.902 ms |  1.19 |  29 | 15339.5 |   38.0 |       7.65 KB |        6.54 |
| ReadLine_    | Row   | 50000   |     8.612 ms |  5.37 |  29 |  3388.5 |  172.2 |   88608.25 KB |   75,675.43 |
| CsvHelper    | Row   | 50000   |    23.988 ms | 14.96 |  29 |  1216.5 |  479.8 |         20 KB |       17.08 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     2.120 ms |  1.00 |  29 | 13763.5 |   42.4 |       1.17 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     2.537 ms |  1.20 |  29 | 11502.2 |   50.7 |       1.17 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     3.003 ms |  1.42 |  29 |  9718.0 |   60.1 |       7.66 KB |        6.53 |
| ReadLine_    | Cols  | 50000   |     9.356 ms |  4.41 |  29 |  3119.2 |  187.1 |   88608.25 KB |   75,549.41 |
| CsvHelper    | Cols  | 50000   |    44.334 ms | 20.91 |  29 |   658.2 |  886.7 |      445.7 KB |      380.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    27.170 ms |  1.00 |  29 |  1074.0 |  543.4 |   13802.77 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    17.175 ms |  0.63 |  29 |  1699.1 |  343.5 |   13995.12 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    29.696 ms |  1.09 |  29 |   982.7 |  593.9 |   13962.13 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    83.845 ms |  3.09 |  29 |   348.0 | 1676.9 |  102133.86 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    54.469 ms |  2.01 |  29 |   535.7 | 1089.4 |   13971.05 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   447.553 ms |  1.00 | 583 |  1304.4 |  447.6 |  266669.24 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   213.917 ms |  0.48 | 583 |  2729.0 |  213.9 |  269030.28 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   572.956 ms |  1.28 | 583 |  1018.9 |  573.0 |  266825.09 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,304.496 ms |  2.92 | 583 |   447.5 | 1304.5 | 2038835.13 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,156.929 ms |  2.59 | 583 |   504.6 | 1156.9 |  266845.88 KB |        1.00 |
