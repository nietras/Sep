```

BenchmarkDotNet v0.16.0-preview.2, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
Memory: 7 GB Total, 0.56 GB Available
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
| Sep______ | Row    | 25000 |  3.052 ms |  1.00 | 20 | 6642.3 |  122.1 |     1200 B |        1.00 |
| Sylvan___ | Row    | 25000 | 23.449 ms |  7.73 | 20 |  864.5 |  937.9 |    12413 B |       10.34 |
| ReadLine_ | Row    | 25000 | 16.947 ms |  5.59 | 20 | 1196.2 |  677.9 | 75253368 B |   62,711.14 |
| CsvHelper | Row    | 25000 |  4.230 ms |  1.40 | 20 | 4792.8 |  169.2 |      776 B |        0.65 |
|           |        |       |           |       |    |        |        |            |             |
| Sep______ | Cols   | 25000 |  4.814 ms |  1.00 | 20 | 4211.4 |  192.5 |     1200 B |        1.00 |
| Sylvan___ | Cols   | 25000 | 24.106 ms |  5.08 | 20 |  840.9 |  964.3 |    12415 B |       10.35 |
| ReadLine_ | Cols   | 25000 | 17.200 ms |  3.62 | 20 | 1178.6 |  688.0 | 75253368 B |   62,711.14 |
| CsvHelper | Cols   | 25000 |  3.835 ms |  0.81 | 20 | 5286.6 |  153.4 |      776 B |        0.65 |
|           |        |       |           |       |    |        |        |            |             |
| Sep______ | Floats | 25000 | 27.251 ms |  1.00 | 20 |  743.9 | 1090.0 |     8008 B |        1.00 |
| Sep_MT___ | Floats | 25000 | 13.230 ms |  0.49 | 20 | 1532.2 |  529.2 |    77181 B |        9.64 |
| Sylvan___ | Floats | 25000 | 80.115 ms |  2.95 | 20 |  253.0 | 3204.6 |    18676 B |        2.33 |
| ReadLine_ | Floats | 25000 | 89.042 ms |  3.28 | 20 |  227.7 | 3561.7 | 75256680 B |    9,397.69 |
| CsvHelper | Floats | 25000 | 34.028 ms |  1.25 | 20 |  595.7 | 1361.1 |     7800 B |        0.97 |
