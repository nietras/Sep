```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26100.33650/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.54 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|-----------:|------------:|
| Sep______ | Row    | 25000 |   3.061 ms |  1.00 | 20 | 6638.9 |  122.4 |     1280 B |        1.00 |
| Sylvan___ | Row    | 25000 |   3.584 ms |  1.17 | 20 | 5670.2 |  143.3 |    12802 B |       10.00 |
| ReadLine_ | Row    | 25000 |  15.127 ms |  4.94 | 20 | 1343.3 |  605.1 | 75253368 B |   58,791.69 |
| CsvHelper | Row    | 25000 |   2.137 ms |  0.70 | 20 | 9507.9 |   85.5 |      776 B |        0.61 |
|           |        |       |            |       |    |        |        |            |             |
| Sep______ | Cols   | 25000 |   4.196 ms |  1.00 | 20 | 4842.8 |  167.8 |     1280 B |        1.00 |
| Sylvan___ | Cols   | 25000 |   5.973 ms |  1.42 | 20 | 3402.2 |  238.9 |    12803 B |       10.00 |
| ReadLine_ | Cols   | 25000 |  14.910 ms |  3.55 | 20 | 1362.8 |  596.4 | 75253368 B |   58,791.69 |
| CsvHelper | Cols   | 25000 |   2.190 ms |  0.52 | 20 | 9278.7 |   87.6 |      776 B |        0.61 |
|           |        |       |            |       |    |        |        |            |             |
| Sep______ | Floats | 25000 |  32.628 ms |  1.00 | 20 |  622.8 | 1305.1 |     8088 B |        1.00 |
| Sep_MT___ | Floats | 25000 |  15.278 ms |  0.47 | 20 | 1330.0 |  611.1 |    78238 B |        9.67 |
| Sylvan___ | Floats | 25000 |  76.692 ms |  2.35 | 20 |  265.0 | 3067.7 |    19066 B |        2.36 |
| ReadLine_ | Floats | 25000 | 100.927 ms |  3.09 | 20 |  201.3 | 4037.1 | 75256680 B |    9,304.73 |
| CsvHelper | Floats | 25000 |  42.706 ms |  1.31 | 20 |  475.8 | 1708.2 |     7800 B |        0.96 |
