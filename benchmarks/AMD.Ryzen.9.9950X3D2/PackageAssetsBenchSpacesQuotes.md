```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 53.15 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  3.116 ms |  1.00 | 41 | 13406.1 |   62.3 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  5.923 ms |  1.90 | 41 |  7052.0 |  118.5 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  6.989 ms |  2.24 | 41 |  5976.3 |  139.8 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  7.638 ms |  2.45 | 41 |  5468.5 |  152.8 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 35.344 ms | 11.34 | 41 |  1181.8 |  706.9 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 38.500 ms | 12.36 | 41 |  1084.9 |  770.0 |    1264 B |        4.39 |
