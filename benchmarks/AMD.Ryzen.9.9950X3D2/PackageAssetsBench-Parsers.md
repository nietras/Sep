```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 37.47 GB Available
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
| Sep_   | SepParserAvx512To2x256To512CmpOrMoveMaskTzcnt | Row   | 50000 |  1.162 ms | 29 | 25113.0 |   23.2 |     288 B |
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt         | Row   | 50000 |  1.180 ms | 29 | 24725.3 |   23.6 |     288 B |
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt        | Row   | 50000 |  1.238 ms | 29 | 23568.3 |   24.8 |     288 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt           | Row   | 50000 |  1.284 ms | 29 | 22729.6 |   25.7 |     288 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt           | Row   | 50000 |  1.300 ms | 29 | 22445.7 |   26.0 |     288 B |
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt           | Row   | 50000 |  1.313 ms | 29 | 22228.0 |   26.3 |     288 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt           | Row   | 50000 |  1.455 ms | 29 | 20053.7 |   29.1 |     288 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt        | Row   | 50000 |  1.493 ms | 29 | 19549.8 |   29.9 |     288 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt           | Row   | 50000 |  1.552 ms | 29 | 18801.6 |   31.0 |     288 B |
| Sep_   | SepParserIndexOfAny                           | Row   | 50000 |  8.952 ms | 29 |  3259.9 |  179.0 |     288 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt            | Row   | 50000 | 51.351 ms | 29 |   568.3 | 1027.0 |     288 B |
