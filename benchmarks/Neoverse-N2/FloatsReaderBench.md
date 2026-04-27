```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.245 ms |  1.00 | 20 | 6246.4 |  129.8 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.418 ms |  7.22 | 20 |  865.6 |  936.7 |    12.12 KB |       10.35 |
| ReadLine_ | Row    | 25000 |  19.347 ms |  5.96 | 20 | 1047.8 |  773.9 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  33.257 ms | 10.25 | 20 |  609.6 | 1330.3 |    19.95 KB |       17.02 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.357 ms |  1.00 | 20 | 4652.4 |  174.3 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.586 ms |  6.10 | 20 |  762.5 | 1063.4 |    12.13 KB |       10.35 |
| ReadLine_ | Cols   | 25000 |  19.951 ms |  4.58 | 20 | 1016.1 |  798.1 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  37.123 ms |  8.52 | 20 |  546.1 | 1484.9 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.829 ms |  1.00 | 20 |  636.9 | 1273.2 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.373 ms |  0.29 | 20 | 2162.8 |  374.9 |    69.44 KB |        8.88 |
| Sylvan___ | Floats | 25000 |  87.467 ms |  2.75 | 20 |  231.8 | 3498.7 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 | 100.218 ms |  3.15 | 20 |  202.3 | 4008.7 | 73492.94 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 138.578 ms |  4.35 | 20 |  146.3 | 5543.1 | 22061.31 KB |    2,821.03 |
