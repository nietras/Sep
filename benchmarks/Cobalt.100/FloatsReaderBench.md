```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-FXNRMG : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.308 ms |  1.00 | 20 | 6142.8 |  132.3 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.737 ms |  7.18 | 20 |  856.0 |  949.5 |    10.35 KB |        8.81 |
| ReadLine_ | Row    | 25000 |  21.754 ms |  6.58 | 20 |  934.1 |  870.2 | 73489.65 KB |   62,606.82 |
| CsvHelper | Row    | 25000 |  32.397 ms |  9.79 | 20 |  627.2 | 1295.9 |    20.01 KB |       17.05 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.301 ms |  1.00 | 20 | 4724.2 |  172.0 |     1.18 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.464 ms |  6.15 | 20 |  767.8 | 1058.6 |    10.35 KB |        8.79 |
| ReadLine_ | Cols   | 25000 |  22.615 ms |  5.26 | 20 |  898.5 |  904.6 | 73489.67 KB |   62,399.19 |
| CsvHelper | Cols   | 25000 |  35.721 ms |  8.31 | 20 |  568.9 | 1428.8 | 21340.21 KB |   18,119.71 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  30.363 ms |  1.00 | 20 |  669.2 | 1214.5 |     7.96 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   8.361 ms |  0.28 | 20 | 2430.4 |  334.4 |    66.37 KB |        8.34 |
| Sylvan___ | Floats | 25000 |  96.160 ms |  3.17 | 20 |  211.3 | 3846.4 |    18.42 KB |        2.32 |
| ReadLine_ | Floats | 25000 | 103.393 ms |  3.41 | 20 |  196.5 | 4135.7 |    73493 KB |    9,238.50 |
| CsvHelper | Floats | 25000 | 143.722 ms |  4.73 | 20 |  141.4 | 5748.9 | 22061.91 KB |    2,773.31 |
