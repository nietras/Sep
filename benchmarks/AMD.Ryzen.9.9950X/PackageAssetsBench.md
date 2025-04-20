```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 9950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-RXSQJG : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-RXSQJG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     1.365 ms |  1.00 |  29 | 21384.9 |   27.3 |       1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     1.455 ms |  1.07 |  29 | 20059.6 |   29.1 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     1.399 ms |  1.02 |  29 | 20865.5 |   28.0 |       1.01 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     1.934 ms |  1.42 |  29 | 15085.4 |   38.7 |       7.72 KB |        7.63 |
| ReadLine_    | Row   | 50000   |     8.833 ms |  6.47 |  29 |  3303.7 |  176.7 |   88608.25 KB |   87,581.89 |
| CsvHelper    | Row   | 50000   |    24.072 ms | 17.64 |  29 |  1212.3 |  481.4 |         20 KB |       19.76 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     1.895 ms |  1.00 |  29 | 15399.8 |   37.9 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     2.321 ms |  1.22 |  29 | 12573.8 |   46.4 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     3.141 ms |  1.66 |  29 |  9290.5 |   62.8 |       7.66 KB |        7.54 |
| ReadLine_    | Cols  | 50000   |     9.248 ms |  4.88 |  29 |  3155.2 |  185.0 |   88608.25 KB |   87,245.04 |
| CsvHelper    | Cols  | 50000   |    44.453 ms | 23.46 |  29 |   656.4 |  889.1 |      445.7 KB |      438.84 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    26.486 ms |  1.00 |  29 |  1101.7 |  529.7 |   13802.57 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    16.693 ms |  0.63 |  29 |  1748.1 |  333.9 |   13992.75 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    29.711 ms |  1.12 |  29 |   982.2 |  594.2 |   13963.15 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    82.744 ms |  3.13 |  29 |   352.7 | 1654.9 |  102134.11 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    54.704 ms |  2.07 |  29 |   533.4 | 1094.1 |   13970.84 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   473.474 ms |  1.00 | 583 |  1233.0 |  473.5 |   266668.7 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   207.790 ms |  0.44 | 583 |  2809.5 |  207.8 |  268542.84 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   573.016 ms |  1.21 | 583 |  1018.8 |  573.0 |  266825.02 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,303.550 ms |  2.75 | 583 |   447.8 | 1303.5 | 2038835.55 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,149.642 ms |  2.43 | 583 |   507.8 | 1149.6 |  266841.26 KB |        1.00 |
