```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-AKIMDM : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.471 ms |  1.00 | 20 | 8203.5 |   98.8 |     1.18 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  18.834 ms |  7.63 | 20 | 1076.3 |  753.4 |    10.62 KB |        9.01 |
| ReadLine_ | Row    | 25000 |  17.200 ms |  6.97 | 20 | 1178.6 |  688.0 | 73489.65 KB |   62,399.17 |
| CsvHelper | Row    | 25000 |  32.108 ms | 13.01 | 20 |  631.4 | 1284.3 |    20.21 KB |       17.16 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.581 ms |  1.00 | 20 | 5660.5 |  143.3 |     1.18 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  21.596 ms |  6.03 | 20 |  938.7 |  863.8 |    10.38 KB |        8.79 |
| ReadLine_ | Cols   | 25000 |  15.587 ms |  4.35 | 20 | 1300.6 |  623.5 | 73489.65 KB |   62,192.89 |
| CsvHelper | Cols   | 25000 |  30.036 ms |  8.39 | 20 |  674.9 | 1201.4 |  21340.5 KB |   18,060.06 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  24.179 ms |  1.00 | 20 |  838.4 |  967.2 |     8.32 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.109 ms |  0.38 | 20 | 2225.6 |  364.3 |    90.77 KB |       10.91 |
| Sylvan___ | Floats | 25000 |  69.538 ms |  2.88 | 20 |  291.5 | 2781.5 |    18.57 KB |        2.23 |
| ReadLine_ | Floats | 25000 |  78.501 ms |  3.25 | 20 |  258.2 | 3140.1 |  73493.2 KB |    8,832.99 |
| CsvHelper | Floats | 25000 | 110.386 ms |  4.57 | 20 |  183.6 | 4415.5 | 22063.34 KB |    2,651.74 |
