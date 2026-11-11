```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26100.33674/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.57 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     7.451 ms |  1.00 |  33 | 4479.5 |  149.0 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.622 ms |  1.02 |  33 | 4378.8 |  152.4 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     8.094 ms |  1.09 |  33 | 4123.7 |  161.9 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    24.471 ms |  3.28 |  33 | 1363.9 |  489.4 |      8670 B |       30.10 |
| CsvHelper    | Row   | 50000   |    10.351 ms |  1.39 |  33 | 3224.7 |  207.0 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     8.772 ms |  1.00 |  33 | 3804.8 |  175.4 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    11.313 ms |  1.29 |  33 | 2950.4 |  226.3 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    29.036 ms |  3.31 |  33 | 1149.5 |  580.7 |      8673 B |       30.11 |
| CsvHelper    | Cols  | 50000   |    31.281 ms |  3.57 |  33 | 1067.0 |  625.6 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    50.097 ms |  1.00 |  33 |  666.3 | 1001.9 |  14132847 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    32.717 ms |  0.65 |  33 | 1020.2 |  654.3 |  14195552 B |        1.00 |
| Sylvan___    | Asset | 50000   |    70.420 ms |  1.41 |  33 |  474.0 | 1408.4 |  14297022 B |        1.01 |
| CsvHelper    | Asset | 50000   |    58.020 ms |  1.16 |  33 |  575.3 | 1160.4 |  15112661 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 | 1,016.106 ms |  1.00 | 667 |  657.1 | 1016.1 | 273067216 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   604.291 ms |  0.59 | 667 | 1104.9 |  604.3 | 283225024 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,482.327 ms |  1.46 | 667 |  450.4 | 1482.3 | 273228872 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,173.731 ms |  1.16 | 667 |  568.9 | 1173.7 | 274045528 B |        1.00 |
