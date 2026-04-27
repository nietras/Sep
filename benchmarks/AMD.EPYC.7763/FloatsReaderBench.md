```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.114 ms |  1.00 | 20 | 6526.0 |  124.5 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.774 ms |  1.21 | 20 | 5383.4 |  151.0 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  15.703 ms |  5.05 | 20 | 1294.0 |  628.1 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  38.299 ms | 12.31 | 20 |  530.6 | 1531.9 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.414 ms |  1.00 | 20 | 4603.9 |  176.5 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.341 ms |  1.44 | 20 | 3204.7 |  253.6 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  16.259 ms |  3.69 | 20 | 1249.8 |  650.4 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  40.699 ms |  9.23 | 20 |  499.3 | 1628.0 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.783 ms |  1.00 | 20 |  639.3 | 1271.3 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  14.164 ms |  0.45 | 20 | 1434.6 |  566.6 |    68.54 KB |        8.68 |
| Sylvan___ | Floats | 25000 |  78.389 ms |  2.47 | 20 |  259.2 | 3135.6 |    18.63 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 100.865 ms |  3.17 | 20 |  201.5 | 4034.6 | 73492.94 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 147.104 ms |  4.63 | 20 |  138.1 | 5884.2 | 22061.22 KB |    2,793.11 |
