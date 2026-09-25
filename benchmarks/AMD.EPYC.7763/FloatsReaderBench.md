```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.59 GB Available
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
| Sep______ | Row    | 25000 |   3.224 ms |  1.00 | 20 | 6303.3 |  128.9 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.546 ms |  1.10 | 20 | 5730.9 |  141.8 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  14.635 ms |  4.54 | 20 | 1388.5 |  585.4 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  38.018 ms | 11.80 | 20 |  534.5 | 1520.7 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.292 ms |  1.00 | 20 | 4734.5 |  171.7 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   5.938 ms |  1.38 | 20 | 3422.2 |  237.5 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  15.001 ms |  3.50 | 20 | 1354.6 |  600.0 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  53.015 ms | 12.35 | 20 |  383.3 | 2120.6 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.939 ms |  1.00 | 20 |  636.2 | 1277.6 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  14.948 ms |  0.47 | 20 | 1359.4 |  597.9 |    79.27 KB |       10.04 |
| Sylvan___ | Floats | 25000 |  81.521 ms |  2.55 | 20 |  249.3 | 3260.9 |    18.63 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 102.698 ms |  3.22 | 20 |  197.9 | 4107.9 | 73492.93 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 146.445 ms |  4.59 | 20 |  138.8 | 5857.8 | 22061.26 KB |    2,793.12 |
