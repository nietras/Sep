```

BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.90GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.002 ms |  1.00 | 20 | 6753.9 |  120.1 |     1.24 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.305 ms |  1.10 | 20 | 6133.3 |  132.2 |     10.7 KB |        8.61 |
| ReadLine_ | Row    | 25000 |  18.502 ms |  6.16 | 20 | 1095.7 |  740.1 | 73489.62 KB |   59,161.45 |
| CsvHelper | Row    | 25000 |  37.670 ms | 12.55 | 20 |  538.1 | 1506.8 |    19.95 KB |       16.06 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.076 ms |  1.00 | 20 | 4973.3 |  163.0 |     1.24 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   5.792 ms |  1.42 | 20 | 3500.2 |  231.7 |    10.71 KB |        8.62 |
| ReadLine_ | Cols   | 25000 |  18.372 ms |  4.51 | 20 | 1103.4 |  734.9 | 73489.62 KB |   59,161.45 |
| CsvHelper | Cols   | 25000 |  40.450 ms |  9.92 | 20 |  501.2 | 1618.0 | 21340.16 KB |   17,179.50 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.470 ms |  1.00 | 20 |  644.2 | 1258.8 |     7.89 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  12.902 ms |  0.41 | 20 | 1571.2 |  516.1 |    68.47 KB |        8.68 |
| Sylvan___ | Floats | 25000 |  82.451 ms |  2.62 | 20 |  245.9 | 3298.0 |    21.71 KB |        2.75 |
| ReadLine_ | Floats | 25000 | 110.689 ms |  3.52 | 20 |  183.1 | 4427.6 | 73492.96 KB |    9,313.96 |
| CsvHelper | Floats | 25000 | 157.042 ms |  4.99 | 20 |  129.1 | 6281.7 | 22061.51 KB |    2,795.91 |
