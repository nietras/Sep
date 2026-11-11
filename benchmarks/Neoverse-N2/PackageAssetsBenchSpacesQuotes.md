```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 10.73 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  9.903 ms |  1.00 | 41 | 4208.2 |  198.1 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 14.692 ms |  1.48 | 41 | 2836.5 |  293.8 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 17.060 ms |  1.72 | 41 | 2442.8 |  341.2 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 19.017 ms |  1.92 | 41 | 2191.5 |  380.3 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 75.755 ms |  7.65 | 41 |  550.1 | 1515.1 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 81.996 ms |  8.28 | 41 |  508.2 | 1639.9 |    1264 B |        4.39 |
