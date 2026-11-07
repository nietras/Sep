```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 38.83 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                        | Scope | Rows  | Mean     | MB | MB/s    | ns/row | Allocated |
|------- |---------------------------------------------- |------ |------ |---------:|---:|--------:|-------:|----------:|
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt         | Row   | 50000 | 1.187 ms | 29 | 24590.4 |   23.7 |     288 B |
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcntUnroll2 | Row   | 50000 | 1.201 ms | 29 | 24290.8 |   24.0 |     288 B |
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt        | Row   | 50000 | 1.254 ms | 29 | 23267.7 |   25.1 |     288 B |
