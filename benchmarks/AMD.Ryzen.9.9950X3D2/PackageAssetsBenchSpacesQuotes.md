```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 52.22 GB Available
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
| Sep_                       | Cols  | 50000 |  3.415 ms |  1.00 | 41 | 12229.9 |   68.3 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  6.255 ms |  1.83 | 41 |  6677.3 |  125.1 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  7.213 ms |  2.11 | 41 |  5790.6 |  144.3 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  7.835 ms |  2.29 | 41 |  5330.9 |  156.7 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 34.735 ms | 10.17 | 41 |  1202.5 |  694.7 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 40.334 ms | 11.81 | 41 |  1035.6 |  806.7 |    1264 B |        4.39 |
