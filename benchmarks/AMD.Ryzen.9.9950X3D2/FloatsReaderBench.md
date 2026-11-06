```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 40.62 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  Error=0.0186 ms  StdDev=0.0097 ms  

```
| Method    | Scope | Rows  | Mean     | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|---------- |------ |------ |---------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep______ | Row   | 25000 | 1.066 ms |  1.00 | 20 | 19066.9 |   42.6 |   3.81 KB |        1.00 |
