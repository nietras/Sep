```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
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
| Sep______ | Row    | 25000 |   3.271 ms |  1.00 | 20 | 6211.5 |  130.9 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.762 ms |  7.27 | 20 |  855.1 |  950.5 |    12.12 KB |       10.35 |
| ReadLine_ | Row    | 25000 |  16.724 ms |  5.12 | 20 | 1215.0 |  668.9 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  33.005 ms | 10.10 | 20 |  615.7 | 1320.2 |    19.95 KB |       17.02 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.532 ms |  1.00 | 20 | 4483.2 |  181.3 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.830 ms |  5.92 | 20 |  757.3 | 1073.2 |    12.13 KB |       10.35 |
| ReadLine_ | Cols   | 25000 |  17.846 ms |  3.94 | 20 | 1138.6 |  713.9 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  36.746 ms |  8.11 | 20 |  553.0 | 1469.9 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.700 ms |  1.00 | 20 |  641.0 | 1268.0 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  15.113 ms |  0.48 | 20 | 1344.5 |  604.5 |    84.36 KB |       10.79 |
| Sylvan___ | Floats | 25000 |  87.584 ms |  2.76 | 20 |  232.0 | 3503.4 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  93.495 ms |  2.95 | 20 |  217.3 | 3739.8 | 73492.94 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 128.695 ms |  4.06 | 20 |  157.9 | 5147.8 | 22060.98 KB |    2,820.98 |
