```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-SMYKWG : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.538 ms |  1.00 | 20 | 7988.7 |  101.5 |     1.18 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  19.493 ms |  7.69 | 20 | 1039.9 |  779.7 |    10.62 KB |        9.03 |
| ReadLine_ | Row    | 25000 |  14.794 ms |  5.84 | 20 | 1370.3 |  591.8 | 73489.65 KB |   62,502.83 |
| CsvHelper | Row    | 25000 |  28.188 ms | 11.12 | 20 |  719.2 | 1127.5 |    20.28 KB |       17.25 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.265 ms |  1.00 | 20 | 6209.1 |  130.6 |     1.18 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  21.229 ms |  6.50 | 20 |  954.9 |  849.1 |    10.62 KB |        9.00 |
| ReadLine_ | Cols   | 25000 |  15.580 ms |  4.77 | 20 | 1301.2 |  623.2 | 73489.65 KB |   62,295.86 |
| CsvHelper | Cols   | 25000 |  30.142 ms |  9.23 | 20 |  672.5 | 1205.7 |  21340.5 KB |   18,089.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  22.760 ms |  1.00 | 20 |  890.7 |  910.4 |     8.32 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  17.370 ms |  0.76 | 20 | 1167.1 |  694.8 |    53.21 KB |        6.40 |
| Sylvan___ | Floats | 25000 |  69.897 ms |  3.07 | 20 |  290.0 | 2795.9 |    18.57 KB |        2.23 |
| ReadLine_ | Floats | 25000 |  79.960 ms |  3.51 | 20 |  253.5 | 3198.4 |  73493.2 KB |    8,832.99 |
| CsvHelper | Floats | 25000 | 102.602 ms |  4.51 | 20 |  197.6 | 4104.1 | 22063.34 KB |    2,651.74 |
