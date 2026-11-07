```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 35.35 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                        | Scope | Rows  | Mean      | MB | MB/s    | ns/row | Allocated |
|------- |---------------------------------------------- |------ |------ |----------:|---:|--------:|-------:|----------:|
| Sep_   | SepParserAvx512To2x256To512CmpOrMoveMaskTzcnt | Row   | 50000 |  1.169 ms | 29 | 24964.5 |   23.4 |     288 B |
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt         | Row   | 50000 |  1.173 ms | 29 | 24868.9 |   23.5 |     288 B |
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt        | Row   | 50000 |  1.252 ms | 29 | 23316.3 |   25.0 |     288 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt           | Row   | 50000 |  1.275 ms | 29 | 22881.5 |   25.5 |     288 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt           | Row   | 50000 |  1.300 ms | 29 | 22445.2 |   26.0 |     288 B |
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt           | Row   | 50000 |  1.302 ms | 29 | 22411.0 |   26.0 |     288 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt           | Row   | 50000 |  1.446 ms | 29 | 20175.6 |   28.9 |     288 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt        | Row   | 50000 |  1.489 ms | 29 | 19602.2 |   29.8 |     288 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt           | Row   | 50000 |  1.544 ms | 29 | 18903.4 |   30.9 |     288 B |
| Sep_   | SepParserIndexOfAny                           | Row   | 50000 |  8.954 ms | 29 |  3258.9 |  179.1 |     288 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt            | Row   | 50000 | 51.141 ms | 29 |   570.6 | 1022.8 |     288 B |
