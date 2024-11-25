```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-NMHWMW : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.021 ms |  1.00 | 20 | 6709.4 |  120.9 |     1.26 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.723 ms |  1.23 | 20 | 5444.6 |  148.9 |    10.71 KB |        8.51 |
| ReadLine_ | Row    | 25000 |  19.575 ms |  6.48 | 20 | 1035.6 |  783.0 | 73489.68 KB |   58,426.57 |
| CsvHelper | Row    | 25000 |  37.519 ms | 12.42 | 20 |  540.3 | 1500.8 |    20.06 KB |       15.95 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.050 ms |  1.00 | 20 | 5005.4 |  162.0 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.281 ms |  1.55 | 20 | 3227.7 |  251.2 |    10.72 KB |        8.48 |
| ReadLine_ | Cols   | 25000 |  21.890 ms |  5.41 | 20 |  926.1 |  875.6 | 73489.68 KB |   58,155.67 |
| CsvHelper | Cols   | 25000 |  39.985 ms |  9.87 | 20 |  507.0 | 1599.4 | 21340.28 KB |   16,887.52 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.000 ms |  1.00 | 20 |  653.9 | 1240.0 |     8.08 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  12.715 ms |  0.41 | 20 | 1594.3 |  508.6 |    69.48 KB |        8.60 |
| Sylvan___ | Floats | 25000 |  85.820 ms |  2.77 | 20 |  236.2 | 3432.8 |    18.96 KB |        2.35 |
| ReadLine_ | Floats | 25000 | 114.357 ms |  3.69 | 20 |  177.3 | 4574.3 |  73493.2 KB |    9,101.10 |
| CsvHelper | Floats | 25000 | 160.854 ms |  5.19 | 20 |  126.0 | 6434.2 | 22062.24 KB |    2,732.10 |
