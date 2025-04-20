```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-DRDGJI : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.909 ms |  1.00 | 20 | 6968.1 |  116.4 |     1.26 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.565 ms |  1.23 | 20 | 5687.1 |  142.6 |    10.71 KB |        8.53 |
| ReadLine_ | Row    | 25000 |  18.022 ms |  6.19 | 20 | 1124.9 |  720.9 | 73489.67 KB |   58,517.44 |
| CsvHelper | Row    | 25000 |  37.309 ms | 12.82 | 20 |  543.4 | 1492.3 |    20.06 KB |       15.97 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.065 ms |  1.00 | 20 | 4986.9 |  162.6 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.151 ms |  1.51 | 20 | 3295.9 |  246.0 |     10.8 KB |        8.55 |
| ReadLine_ | Cols   | 25000 |  19.595 ms |  4.82 | 20 | 1034.6 |  783.8 | 73489.68 KB |   58,155.67 |
| CsvHelper | Cols   | 25000 |  40.218 ms |  9.89 | 20 |  504.1 | 1608.7 | 21340.28 KB |   16,887.52 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  30.939 ms |  1.00 | 20 |  655.2 | 1237.6 |     8.08 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  13.164 ms |  0.43 | 20 | 1540.0 |  526.5 |    68.56 KB |        8.49 |
| Sylvan___ | Floats | 25000 |  87.688 ms |  2.83 | 20 |  231.2 | 3507.5 |    18.96 KB |        2.35 |
| ReadLine_ | Floats | 25000 | 113.903 ms |  3.68 | 20 |  178.0 | 4556.1 |  73493.2 KB |    9,101.10 |
| CsvHelper | Floats | 25000 | 160.055 ms |  5.17 | 20 |  126.7 | 6402.2 | 22062.48 KB |    2,732.13 |
