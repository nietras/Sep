```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-RAOLFZ : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-RAOLFZ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.582 ms |  1.00 |  29 | 8120.5 |   71.6 |       1.03 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.568 ms |  1.00 |  29 | 8151.2 |   71.4 |       1.03 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.468 ms |  1.25 |  29 | 6510.0 |   89.4 |       7.66 KB |        7.47 |
| ReadLine_    | Row   | 50000   |    49.799 ms | 13.91 |  29 |  584.1 |  996.0 |   88608.38 KB |   86,414.26 |
| CsvHelper    | Row   | 50000   |    66.445 ms | 18.55 |  29 |  437.7 | 1328.9 |       23.3 KB |       22.72 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.848 ms |  1.00 |  29 | 5999.4 |   97.0 |        1.2 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.710 ms |  1.18 |  29 | 5093.7 |  114.2 |       1.04 KB |        0.86 |
| Sylvan___    | Cols  | 50000   |     8.223 ms |  1.70 |  29 | 3537.2 |  164.5 |       7.68 KB |        6.41 |
| ReadLine_    | Cols  | 50000   |    25.293 ms |  5.22 |  29 | 1150.0 |  505.9 |   88608.32 KB |   73,888.37 |
| CsvHelper    | Cols  | 50000   |   109.877 ms | 22.66 |  29 |  264.7 | 2197.5 |     445.93 KB |      371.85 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    30.823 ms |  1.00 |  29 |  943.6 |  616.5 |   13802.34 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    23.560 ms |  0.76 |  29 | 1234.6 |  471.2 |   13852.34 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    47.591 ms |  1.54 |  29 |  611.2 |  951.8 |   13961.85 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   116.425 ms |  3.78 |  29 |  249.8 | 2328.5 |  102133.09 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   120.019 ms |  3.89 |  29 |  242.3 | 2400.4 |   13970.29 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   797.462 ms |  1.00 | 581 |  729.7 |  797.5 |  266669.05 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   507.024 ms |  0.64 | 581 | 1147.6 |  507.0 |  275457.14 KB |        1.03 |
| Sylvan___    | Asset | 1000000 |   991.891 ms |  1.25 | 581 |  586.6 |  991.9 |  266826.74 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,259.026 ms |  4.09 | 581 |  178.5 | 3259.0 | 2038846.35 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,527.438 ms |  3.17 | 581 |  230.2 | 2527.4 |  266850.81 KB |        1.00 |
