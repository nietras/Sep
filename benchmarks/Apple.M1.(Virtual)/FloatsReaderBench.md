```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  Job-PJJVEM : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.874 ms |  1.00 | 20 | 5232.6 |  155.0 |     1.21 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  18.028 ms |  4.65 | 20 | 1124.5 |  721.1 |    10.62 KB |        8.74 |
| ReadLine_ | Row    | 25000 |  14.229 ms |  3.67 | 20 | 1424.7 |  569.1 | 73489.65 KB |   60,493.09 |
| CsvHelper | Row    | 25000 |  28.100 ms |  7.25 | 20 |  721.4 | 1124.0 |    20.03 KB |       16.48 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.756 ms |  1.00 | 20 | 4262.5 |  190.2 |     1.21 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  20.478 ms |  4.31 | 20 |  989.9 |  819.1 |    10.62 KB |        8.74 |
| ReadLine_ | Cols   | 25000 |  15.217 ms |  3.20 | 20 | 1332.2 |  608.7 | 73489.65 KB |   60,493.09 |
| CsvHelper | Cols   | 25000 |  29.650 ms |  6.23 | 20 |  683.7 | 1186.0 |  21340.5 KB |   17,566.45 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  24.583 ms |  1.00 | 20 |  824.6 |  983.3 |     8.34 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.584 ms |  0.39 | 20 | 2115.1 |  383.4 |    73.79 KB |        8.85 |
| Sylvan___ | Floats | 25000 |  70.365 ms |  2.86 | 20 |  288.1 | 2814.6 |    18.57 KB |        2.23 |
| ReadLine_ | Floats | 25000 |  79.305 ms |  3.23 | 20 |  255.6 | 3172.2 |  73493.2 KB |    8,816.43 |
| CsvHelper | Floats | 25000 | 103.810 ms |  4.22 | 20 |  195.3 | 4152.4 | 22063.34 KB |    2,646.77 |
