```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26200.9725/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 12.13 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     5.175 ms |  1.00 |  29 | 5638.4 |  103.5 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.124 ms |  0.99 |  29 | 5694.5 |  102.5 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.011 ms |  0.97 |  29 | 5823.5 |  100.2 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    18.772 ms |  3.63 |  29 | 1554.5 |  375.4 |      7470 B |       25.94 |
| CsvHelper    | Row   | 50000   |    12.547 ms |  2.43 |  29 | 2325.7 |  250.9 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     6.483 ms |  1.00 |  29 | 4501.3 |  129.7 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.344 ms |  1.13 |  29 | 3973.7 |  146.9 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.467 ms |  3.62 |  29 | 1243.5 |  469.3 |      7478 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    28.135 ms |  4.34 |  29 | 1037.2 |  562.7 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    36.425 ms |  1.00 |  29 |  801.1 |  728.5 |  14132256 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    18.623 ms |  0.51 |  29 | 1566.9 |  372.5 |  14199780 B |        1.00 |
| Sylvan___    | Asset | 50000   |    55.956 ms |  1.54 |  29 |  521.5 | 1119.1 |  14295531 B |        1.01 |
| CsvHelper    | Asset | 50000   |    43.072 ms |  1.18 |  29 |  677.5 |  861.4 |  15111560 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   776.453 ms |  1.00 | 583 |  751.9 |  776.5 | 273063200 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   366.160 ms |  0.47 | 583 | 1594.3 |  366.2 | 284984336 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,208.903 ms |  1.56 | 583 |  482.9 | 1208.9 | 273225560 B |        1.00 |
| CsvHelper    | Asset | 1000000 |   950.466 ms |  1.22 | 583 |  614.2 |  950.5 | 274042112 B |        1.00 |
