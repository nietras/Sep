```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-ZAPULK : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.333 ms |  1.00 | 20 | 6097.2 |  133.3 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.856 ms |  7.16 | 20 |  851.8 |  954.2 |    10.35 KB |        8.82 |
| ReadLine_ | Row    | 25000 |  21.779 ms |  6.54 | 20 |  933.0 |  871.2 | 73489.65 KB |   62,606.82 |
| CsvHelper | Row    | 25000 |  32.463 ms |  9.74 | 20 |  625.9 | 1298.5 |    19.99 KB |       17.03 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.323 ms |  1.00 | 20 | 4700.7 |  172.9 |     1.18 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.767 ms |  6.19 | 20 |  759.1 | 1070.7 |    10.35 KB |        8.79 |
| ReadLine_ | Cols   | 25000 |  22.745 ms |  5.26 | 20 |  893.4 |  909.8 | 73489.67 KB |   62,399.19 |
| CsvHelper | Cols   | 25000 |  34.796 ms |  8.05 | 20 |  584.0 | 1391.9 | 21340.21 KB |   18,119.71 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  30.202 ms |  1.00 | 20 |  672.8 | 1208.1 |     7.98 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   8.424 ms |  0.28 | 20 | 2412.2 |  336.9 |    67.01 KB |        8.39 |
| Sylvan___ | Floats | 25000 |  95.584 ms |  3.16 | 20 |  212.6 | 3823.4 |    18.42 KB |        2.31 |
| ReadLine_ | Floats | 25000 | 103.605 ms |  3.43 | 20 |  196.1 | 4144.2 | 73493.38 KB |    9,206.90 |
| CsvHelper | Floats | 25000 | 144.447 ms |  4.78 | 20 |  140.7 | 5777.9 | 22061.91 KB |    2,763.81 |
