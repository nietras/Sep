```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.4 (23H420) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-HYWXRS : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.885 ms |  1.00 | 20 | 5218.3 |  155.4 |     1.21 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  17.967 ms |  4.62 | 20 | 1128.3 |  718.7 |    10.37 KB |        8.54 |
| ReadLine_ | Row    | 25000 |  14.275 ms |  3.67 | 20 | 1420.1 |  571.0 | 73489.65 KB |   60,493.09 |
| CsvHelper | Row    | 25000 |  27.800 ms |  7.16 | 20 |  729.2 | 1112.0 |    20.28 KB |       16.69 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.763 ms |  1.00 | 20 | 4256.3 |  190.5 |     1.21 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  20.663 ms |  4.34 | 20 |  981.1 |  826.5 |    10.62 KB |        8.74 |
| ReadLine_ | Cols   | 25000 |  15.147 ms |  3.18 | 20 | 1338.4 |  605.9 | 73489.65 KB |   60,493.09 |
| CsvHelper | Cols   | 25000 |  29.583 ms |  6.21 | 20 |  685.3 | 1183.3 |  21340.5 KB |   17,566.45 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  24.192 ms |  1.00 | 20 |  838.0 |  967.7 |     8.34 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.410 ms |  0.39 | 20 | 2154.4 |  376.4 |    77.88 KB |        9.34 |
| Sylvan___ | Floats | 25000 |  69.948 ms |  2.89 | 20 |  289.8 | 2797.9 |    18.57 KB |        2.23 |
| ReadLine_ | Floats | 25000 |  78.990 ms |  3.27 | 20 |  256.6 | 3159.6 |  73493.2 KB |    8,816.43 |
| CsvHelper | Floats | 25000 | 103.248 ms |  4.27 | 20 |  196.3 | 4129.9 | 22063.34 KB |    2,646.77 |
