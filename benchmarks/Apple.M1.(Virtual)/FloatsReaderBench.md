```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  Job-HKRCZO : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.887 ms |  1.00 | 20 | 5215.5 |  155.5 |      1.2 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  17.956 ms |  4.62 | 20 | 1129.0 |  718.2 |    10.62 KB |        8.87 |
| ReadLine_ | Row    | 25000 |  14.074 ms |  3.62 | 20 | 1440.4 |  563.0 | 73489.65 KB |   61,381.24 |
| CsvHelper | Row    | 25000 |  27.741 ms |  7.14 | 20 |  730.8 | 1109.6 |    20.28 KB |       16.94 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.726 ms |  1.00 | 20 | 4289.0 |  189.1 |      1.2 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  20.241 ms |  4.28 | 20 | 1001.5 |  809.6 |    10.62 KB |        8.84 |
| ReadLine_ | Cols   | 25000 |  14.976 ms |  3.17 | 20 | 1353.6 |  599.0 | 73489.65 KB |   61,181.63 |
| CsvHelper | Cols   | 25000 |  29.842 ms |  6.31 | 20 |  679.3 | 1193.7 |  21340.5 KB |   17,766.40 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  24.511 ms |  1.00 | 20 |  827.1 |  980.4 |     8.34 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.422 ms |  0.38 | 20 | 2151.5 |  376.9 |    79.89 KB |        9.58 |
| Sylvan___ | Floats | 25000 |  69.902 ms |  2.85 | 20 |  290.0 | 2796.1 |    18.57 KB |        2.23 |
| ReadLine_ | Floats | 25000 |  79.015 ms |  3.22 | 20 |  256.6 | 3160.6 |  73493.2 KB |    8,816.43 |
| CsvHelper | Floats | 25000 | 104.811 ms |  4.28 | 20 |  193.4 | 4192.4 | 22063.34 KB |    2,646.77 |
