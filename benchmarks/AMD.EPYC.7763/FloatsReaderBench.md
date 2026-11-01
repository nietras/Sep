```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD EPYC 7763 3.21GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.77 GB Available
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
| Sep______ | Row    | 25000 |   3.126 ms |  1.00 | 20 | 6484.8 |  125.0 |     1280 B |        1.00 |
| Sylvan___ | Row    | 25000 |   3.642 ms |  1.17 | 20 | 5565.7 |  145.7 |    12802 B |       10.00 |
| ReadLine_ | Row    | 25000 |  17.772 ms |  5.69 | 20 | 1140.7 |  710.9 | 75253368 B |   58,791.69 |
| CsvHelper | Row    | 25000 |   2.227 ms |  0.71 | 20 | 9102.5 |   89.1 |      776 B |        0.61 |
|           |        |       |            |       |    |        |        |            |             |
| Sep______ | Cols   | 25000 |   4.290 ms |  1.00 | 20 | 4725.2 |  171.6 |     1280 B |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.178 ms |  1.44 | 20 | 3281.6 |  247.1 |    12803 B |       10.00 |
| ReadLine_ | Cols   | 25000 |  19.426 ms |  4.53 | 20 | 1043.5 |  777.1 | 75253368 B |   58,791.69 |
| CsvHelper | Cols   | 25000 |   2.434 ms |  0.57 | 20 | 8327.9 |   97.4 |      776 B |        0.61 |
|           |        |       |            |       |    |        |        |            |             |
| Sep______ | Floats | 25000 |  34.036 ms |  1.00 | 20 |  595.6 | 1361.4 |     8088 B |        1.00 |
| Sep_MT___ | Floats | 25000 |  15.313 ms |  0.45 | 20 | 1323.9 |  612.5 |    74085 B |        9.16 |
| Sylvan___ | Floats | 25000 |  86.491 ms |  2.54 | 20 |  234.4 | 3459.6 |    19076 B |        2.36 |
| ReadLine_ | Floats | 25000 | 118.887 ms |  3.49 | 20 |  170.5 | 4755.5 | 75256680 B |    9,304.73 |
| CsvHelper | Floats | 25000 |  33.816 ms |  0.99 | 20 |  599.5 | 1352.6 |     7800 B |        0.96 |
