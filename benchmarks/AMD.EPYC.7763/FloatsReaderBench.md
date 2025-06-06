```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-MPBGVI : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.009 ms |  1.00 | 20 | 6737.0 |  120.4 |     1.26 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.550 ms |  1.18 | 20 | 5709.6 |  142.0 |    10.71 KB |        8.53 |
| ReadLine_ | Row    | 25000 |  18.724 ms |  6.22 | 20 | 1082.7 |  748.9 | 73489.67 KB |   58,517.44 |
| CsvHelper | Row    | 25000 |  38.711 ms | 12.86 | 20 |  523.7 | 1548.4 |    19.95 KB |       15.89 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.046 ms |  1.00 | 20 | 5010.7 |  161.8 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.307 ms |  1.56 | 20 | 3214.2 |  252.3 |    10.79 KB |        8.57 |
| ReadLine_ | Cols   | 25000 |  20.016 ms |  4.95 | 20 | 1012.8 |  800.6 | 73489.68 KB |   58,335.99 |
| CsvHelper | Cols   | 25000 |  41.432 ms | 10.24 | 20 |  489.3 | 1657.3 | 21340.28 KB |   16,939.88 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.282 ms |  1.00 | 20 |  648.0 | 1251.3 |     8.08 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  13.373 ms |  0.43 | 20 | 1515.9 |  534.9 |    70.95 KB |        8.79 |
| Sylvan___ | Floats | 25000 |  84.452 ms |  2.70 | 20 |  240.0 | 3378.1 |    19.89 KB |        2.46 |
| ReadLine_ | Floats | 25000 | 111.513 ms |  3.56 | 20 |  181.8 | 4460.5 | 73496.21 KB |    9,101.48 |
| CsvHelper | Floats | 25000 | 160.598 ms |  5.13 | 20 |  126.2 | 6423.9 | 22062.53 KB |    2,732.14 |
