```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 10.67 GB Available
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
| Sep_                       | Cols  | 50000 |  9.379 ms |  1.00 | 41 | 4443.6 |  187.6 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 13.689 ms |  1.46 | 41 | 3044.3 |  273.8 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 16.367 ms |  1.75 | 41 | 2546.3 |  327.3 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 18.058 ms |  1.93 | 41 | 2307.8 |  361.2 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 74.516 ms |  7.95 | 41 |  559.3 | 1490.3 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 80.380 ms |  8.57 | 41 |  518.5 | 1607.6 |    1264 B |        4.39 |
