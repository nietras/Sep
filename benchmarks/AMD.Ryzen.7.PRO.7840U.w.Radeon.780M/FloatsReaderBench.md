```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-YBSRVP : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.386 ms |  1.00 | 20 | 6000.5 |  135.5 |     1.41 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.782 ms |  1.12 | 20 | 5372.1 |  151.3 |    10.71 KB |        7.58 |
| ReadLine_ | Row    | 25000 |  15.608 ms |  4.61 | 20 | 1301.9 |  624.3 | 73489.65 KB |   52,006.50 |
| CsvHelper | Row    | 25000 |  39.717 ms | 11.73 | 20 |  511.6 | 1588.7 |    20.03 KB |       14.17 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.509 ms |  1.00 | 20 | 4506.2 |  180.4 |     1.42 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.001 ms |  1.33 | 20 | 3386.0 |  240.0 |    10.71 KB |        7.56 |
| ReadLine_ | Cols   | 25000 |  19.123 ms |  4.24 | 20 | 1062.6 |  764.9 | 73489.66 KB |   51,898.90 |
| CsvHelper | Cols   | 25000 |  42.640 ms |  9.46 | 20 |  476.5 | 1705.6 | 21340.25 KB |   15,070.63 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  30.981 ms |  1.00 | 20 |  655.9 | 1239.2 |      8.2 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   8.686 ms |  0.28 | 20 | 2339.4 |  347.4 |   115.48 KB |       14.08 |
| Sylvan___ | Floats | 25000 |  85.393 ms |  2.76 | 20 |  238.0 | 3415.7 |    18.88 KB |        2.30 |
| ReadLine_ | Floats | 25000 | 104.946 ms |  3.39 | 20 |  193.6 | 4197.8 | 73493.12 KB |    8,960.23 |
| CsvHelper | Floats | 25000 | 156.745 ms |  5.06 | 20 |  129.6 | 6269.8 | 22062.08 KB |    2,689.79 |
