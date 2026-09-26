```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.61 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.985 ms |  1.00 | 20 | 6807.6 |  119.4 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.515 ms |  1.18 | 20 | 5780.6 |  140.6 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  14.663 ms |  4.91 | 20 | 1385.8 |  586.5 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  38.236 ms | 12.81 | 20 |  531.4 | 1529.4 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.535 ms |  1.00 | 20 | 4481.1 |  181.4 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.150 ms |  1.36 | 20 | 3303.9 |  246.0 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  15.416 ms |  3.40 | 20 | 1318.1 |  616.6 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  41.365 ms |  9.12 | 20 |  491.2 | 1654.6 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  32.311 ms |  1.00 | 20 |  628.9 | 1292.4 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  14.850 ms |  0.46 | 20 | 1368.3 |  594.0 |    83.54 KB |       10.58 |
| Sylvan___ | Floats | 25000 |  78.932 ms |  2.44 | 20 |  257.4 | 3157.3 |    18.63 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 101.023 ms |  3.13 | 20 |  201.1 | 4040.9 | 73492.93 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 145.633 ms |  4.51 | 20 |  139.5 | 5825.3 | 22061.26 KB |    2,793.12 |
