```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26200.9711/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 11.88 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|-------:|-------:|-----------:|------------:|
| Sep______ | Row    | 25000 |  3.584 ms |  1.00 | 20 | 5669.5 |  143.4 |     1200 B |        1.00 |
| Sylvan___ | Row    | 25000 | 23.061 ms |  6.44 | 20 |  881.1 |  922.4 |    12413 B |       10.34 |
| ReadLine_ | Row    | 25000 | 18.448 ms |  5.15 | 20 | 1101.5 |  737.9 | 75253368 B |   62,711.14 |
| CsvHelper | Row    | 25000 |  6.993 ms |  1.95 | 20 | 2905.7 |  279.7 |      776 B |        0.65 |
|           |        |       |           |       |    |        |        |            |             |
| Sep______ | Cols   | 25000 |  4.837 ms |  1.00 | 20 | 4200.6 |  193.5 |     1200 B |        1.00 |
| Sylvan___ | Cols   | 25000 | 26.022 ms |  5.38 | 20 |  780.9 | 1040.9 |    12416 B |       10.35 |
| ReadLine_ | Cols   | 25000 | 18.479 ms |  3.82 | 20 | 1099.6 |  739.1 | 75253368 B |   62,711.14 |
| CsvHelper | Cols   | 25000 |  7.341 ms |  1.52 | 20 | 2768.1 |  293.6 |      776 B |        0.65 |
|           |        |       |           |       |    |        |        |            |             |
| Sep______ | Floats | 25000 | 31.046 ms |  1.00 | 20 |  654.5 | 1241.9 |     8008 B |        1.00 |
| Sep_MT___ | Floats | 25000 |  9.464 ms |  0.30 | 20 | 2147.1 |  378.5 |    74120 B |        9.26 |
| Sylvan___ | Floats | 25000 | 83.539 ms |  2.69 | 20 |  243.2 | 3341.6 |    18676 B |        2.33 |
| ReadLine_ | Floats | 25000 | 94.113 ms |  3.03 | 20 |  215.9 | 3764.5 | 75256680 B |    9,397.69 |
| CsvHelper | Floats | 25000 | 36.740 ms |  1.18 | 20 |  553.1 | 1469.6 |     7800 B |        0.97 |
