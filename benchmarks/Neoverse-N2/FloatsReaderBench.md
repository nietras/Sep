```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 10.96 GB Available
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
| Sep______ | Row    | 25000 |  3.209 ms |  1.00 | 20 | 6318.1 |  128.3 |     1200 B |        1.00 |
| Sylvan___ | Row    | 25000 | 22.869 ms |  7.13 | 20 |  886.4 |  914.8 |    12413 B |       10.34 |
| ReadLine_ | Row    | 25000 | 18.671 ms |  5.82 | 20 | 1085.7 |  746.9 | 75253368 B |   62,711.14 |
| CsvHelper | Row    | 25000 |  6.206 ms |  1.93 | 20 | 3266.5 |  248.2 |      776 B |        0.65 |
|           |        |       |           |       |    |        |        |            |             |
| Sep______ | Cols   | 25000 |  4.265 ms |  1.00 | 20 | 4753.6 |  170.6 |     1200 B |        1.00 |
| Sylvan___ | Cols   | 25000 | 25.387 ms |  5.95 | 20 |  798.5 | 1015.5 |    12415 B |       10.35 |
| ReadLine_ | Cols   | 25000 | 18.882 ms |  4.43 | 20 | 1073.6 |  755.3 | 75253368 B |   62,711.14 |
| CsvHelper | Cols   | 25000 |  6.235 ms |  1.46 | 20 | 3251.6 |  249.4 |      776 B |        0.65 |
|           |        |       |           |       |    |        |        |            |             |
| Sep______ | Floats | 25000 | 30.817 ms |  1.00 | 20 |  657.8 | 1232.7 |     8008 B |        1.00 |
| Sep_MT___ | Floats | 25000 |  9.245 ms |  0.30 | 20 | 2192.7 |  369.8 |    73346 B |        9.16 |
| Sylvan___ | Floats | 25000 | 83.439 ms |  2.71 | 20 |  243.0 | 3337.6 |    18676 B |        2.33 |
| ReadLine_ | Floats | 25000 | 97.158 ms |  3.15 | 20 |  208.7 | 3886.3 | 75256680 B |    9,397.69 |
| CsvHelper | Floats | 25000 | 35.865 ms |  1.16 | 20 |  565.2 | 1434.6 |     7800 B |        0.97 |
