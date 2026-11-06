```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 44.57 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  3.451 ms |  1.00 | 41 | 12103.0 |   69.0 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  6.367 ms |  1.84 | 41 |  6560.6 |  127.3 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  7.400 ms |  2.14 | 41 |  5644.7 |  148.0 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  8.010 ms |  2.32 | 41 |  5214.6 |  160.2 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 36.604 ms | 10.61 | 41 |  1141.1 |  732.1 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 36.874 ms | 10.69 | 41 |  1132.8 |  737.5 |    1264 B |        4.39 |
