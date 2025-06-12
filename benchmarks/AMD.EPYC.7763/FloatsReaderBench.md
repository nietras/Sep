```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-BFPPER : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.936 ms |  1.00 | 20 | 6905.5 |  117.4 |     1.26 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.499 ms |  1.19 | 20 | 5793.9 |  140.0 |    10.71 KB |        8.51 |
| ReadLine_ | Row    | 25000 |  18.762 ms |  6.39 | 20 | 1080.5 |  750.5 | 73489.67 KB |   58,426.57 |
| CsvHelper | Row    | 25000 |  38.301 ms | 13.05 | 20 |  529.3 | 1532.0 |    19.95 KB |       15.86 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.919 ms |  1.00 | 20 | 5172.2 |  156.8 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.266 ms |  1.60 | 20 | 3235.1 |  250.7 |    10.72 KB |        8.51 |
| ReadLine_ | Cols   | 25000 |  19.850 ms |  5.06 | 20 | 1021.3 |  794.0 | 73489.71 KB |   58,336.02 |
| CsvHelper | Cols   | 25000 |  39.623 ms | 10.11 | 20 |  511.6 | 1584.9 | 21340.29 KB |   16,939.89 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.224 ms |  1.00 | 20 |  649.2 | 1249.0 |     8.08 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  12.827 ms |  0.41 | 20 | 1580.4 |  513.1 |     68.8 KB |        8.52 |
| Sylvan___ | Floats | 25000 |  84.321 ms |  2.70 | 20 |  240.4 | 3372.8 |    18.96 KB |        2.35 |
| ReadLine_ | Floats | 25000 | 113.918 ms |  3.65 | 20 |  178.0 | 4556.7 |  73493.2 KB |    9,101.10 |
| CsvHelper | Floats | 25000 | 166.182 ms |  5.32 | 20 |  122.0 | 6647.3 | 22062.48 KB |    2,732.13 |
