```

BenchmarkDotNet v0.16.0-preview.2, macOS Tahoe 26.7.2 (25G332) [Darwin 25.6.0]
Apple M2 Pro (Virtual), 1 CPU, 5 logical and 5 physical cores
Memory: 14 GB Total, 0.8 GB Available
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
| Sep_                       | Cols  | 50000 |  7.270 ms |  1.00 | 41 | 5732.6 |  145.4 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 10.953 ms |  1.51 | 41 | 3805.0 |  219.1 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 13.378 ms |  1.84 | 41 | 3115.1 |  267.6 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 14.086 ms |  1.94 | 41 | 2958.5 |  281.7 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 63.780 ms |  8.78 | 41 |  653.4 | 1275.6 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 69.826 ms |  9.61 | 41 |  596.8 | 1396.5 |    1264 B |        4.39 |
