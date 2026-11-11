```

BenchmarkDotNet v0.16.0-preview.2, macOS Tahoe 26.7.2 (25G332) [Darwin 25.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
Memory: 7 GB Total, 0.08 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |-----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |   9.228 ms |  1.00 | 41 | 4516.0 |  184.6 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  12.763 ms |  1.40 | 41 | 3265.3 |  255.3 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  15.231 ms |  1.67 | 41 | 2736.2 |  304.6 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  17.833 ms |  1.96 | 41 | 2336.9 |  356.7 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 105.243 ms | 11.56 | 41 |  396.0 | 2104.9 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 |  78.654 ms |  8.64 | 41 |  529.8 | 1573.1 |    1264 B |        4.39 |
