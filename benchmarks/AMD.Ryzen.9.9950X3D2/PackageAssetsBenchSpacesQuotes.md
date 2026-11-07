```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 40.79 GB Available
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
| Sep_                       | Cols  | 50000 |  3.144 ms |  1.00 | 41 | 13286.5 |   62.9 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  6.004 ms |  1.91 | 41 |  6956.5 |  120.1 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  7.044 ms |  2.24 | 41 |  5930.1 |  140.9 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  7.673 ms |  2.44 | 41 |  5444.0 |  153.5 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 36.095 ms | 11.48 | 41 |  1157.2 |  721.9 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 38.162 ms | 12.14 | 41 |  1094.5 |  763.2 |    1264 B |        4.39 |
