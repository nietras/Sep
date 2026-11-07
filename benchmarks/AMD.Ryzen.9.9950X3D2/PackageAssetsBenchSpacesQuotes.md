```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 44.59 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method               | Scope | Rows  | Mean     | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|--------------------- |------ |------ |---------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep_                 | Cols  | 50000 | 4.008 ms |  1.00 | 41 | 10422.9 |   80.2 |     288 B |        1.00 |
| Sep_Trim             | Cols  | 50000 | 6.742 ms |  1.70 | 41 |  6195.4 |  134.8 |     288 B |        1.00 |
| Sep_TrimUnescape     | Cols  | 50000 | 8.014 ms |  2.02 | 41 |  5212.3 |  160.3 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim | Cols  | 50000 | 8.631 ms |  2.18 | 41 |  4839.6 |  172.6 |     288 B |        1.00 |
