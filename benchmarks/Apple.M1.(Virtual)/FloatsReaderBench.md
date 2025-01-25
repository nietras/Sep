```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
  Job-ILBOFO : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.864 ms |  1.00 | 20 | 5245.7 |  154.6 |      1.2 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  17.909 ms |  4.63 | 20 | 1131.9 |  716.4 |    10.62 KB |        8.87 |
| ReadLine_ | Row    | 25000 |  14.045 ms |  3.63 | 20 | 1443.4 |  561.8 | 73489.65 KB |   61,381.24 |
| CsvHelper | Row    | 25000 |  27.822 ms |  7.20 | 20 |  728.6 | 1112.9 |    20.28 KB |       16.94 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.764 ms |  1.00 | 20 | 4255.2 |  190.6 |      1.2 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  19.874 ms |  4.17 | 20 | 1020.0 |  795.0 |    10.38 KB |        8.64 |
| ReadLine_ | Cols   | 25000 |  15.170 ms |  3.18 | 20 | 1336.3 |  606.8 | 73489.65 KB |   61,181.63 |
| CsvHelper | Cols   | 25000 |  29.714 ms |  6.24 | 20 |  682.2 | 1188.6 |  21340.5 KB |   17,766.40 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  24.479 ms |  1.00 | 20 |  828.1 |  979.2 |     8.34 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  11.309 ms |  0.46 | 20 | 1792.6 |  452.4 |    60.77 KB |        7.29 |
| Sylvan___ | Floats | 25000 |  69.253 ms |  2.83 | 20 |  292.7 | 2770.1 |    18.57 KB |        2.23 |
| ReadLine_ | Floats | 25000 |  79.851 ms |  3.26 | 20 |  253.9 | 3194.1 |  73493.2 KB |    8,816.43 |
| CsvHelper | Floats | 25000 | 102.565 ms |  4.19 | 20 |  197.7 | 4102.6 | 22063.34 KB |    2,646.77 |
