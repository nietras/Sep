```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 36.97 GB Available
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
| Sep_   | SepParserAvx512To2x256To512CmpOrMoveMaskTzcnt | Row   | 50000 |  1.171 ms | 29 | 24917.5 |   23.4 |     288 B |
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt         | Row   | 50000 |  1.192 ms | 29 | 24472.0 |   23.8 |     288 B |
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt        | Row   | 50000 |  1.267 ms | 29 | 23038.1 |   25.3 |     288 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt           | Row   | 50000 |  1.273 ms | 29 | 22921.4 |   25.5 |     288 B |
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt           | Row   | 50000 |  1.307 ms | 29 | 22322.6 |   26.1 |     288 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt           | Row   | 50000 |  1.323 ms | 29 | 22064.6 |   26.5 |     288 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt           | Row   | 50000 |  1.455 ms | 29 | 20051.4 |   29.1 |     288 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt        | Row   | 50000 |  1.498 ms | 29 | 19477.5 |   30.0 |     288 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt           | Row   | 50000 |  1.570 ms | 29 | 18589.8 |   31.4 |     288 B |
| Sep_   | SepParserIndexOfAny                           | Row   | 50000 |  8.954 ms | 29 |  3259.0 |  179.1 |     288 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt            | Row   | 50000 | 51.195 ms | 29 |   570.0 | 1023.9 |     288 B |
