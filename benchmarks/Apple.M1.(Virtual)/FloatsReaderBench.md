```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
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
| Sep______ | Row    | 25000 |   2.399 ms |  1.00 | 20 | 8451.0 |   96.0 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  19.322 ms |  8.06 | 20 | 1049.2 |  772.9 |    12.12 KB |       10.34 |
| ReadLine_ | Row    | 25000 |  19.481 ms |  8.12 | 20 | 1040.6 |  779.2 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  34.636 ms | 14.44 | 20 |  585.3 | 1385.4 |    19.95 KB |       17.02 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.647 ms |  1.00 | 20 | 5559.1 |  145.9 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  23.035 ms |  6.32 | 20 |  880.0 |  921.4 |    12.16 KB |       10.38 |
| ReadLine_ | Cols   | 25000 |  17.658 ms |  4.85 | 20 | 1148.0 |  706.3 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  32.242 ms |  8.85 | 20 |  628.7 | 1289.7 | 21340.23 KB |   18,210.33 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  26.294 ms |  1.00 | 20 |  771.0 | 1051.8 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  24.032 ms |  0.91 | 20 |  843.5 |  961.3 |    82.46 KB |       10.54 |
| Sylvan___ | Floats | 25000 |  69.755 ms |  2.65 | 20 |  290.6 | 2790.2 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  89.421 ms |  3.40 | 20 |  226.7 | 3576.8 | 73492.94 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 135.429 ms |  5.15 | 20 |  149.7 | 5417.2 | 22061.55 KB |    2,821.06 |
