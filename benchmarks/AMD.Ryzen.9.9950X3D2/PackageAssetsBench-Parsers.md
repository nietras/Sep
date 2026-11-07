```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 44.02 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                 | Scope | Rows  | Mean      | MB | MB/s    | ns/row | Allocated |
|------- |--------------------------------------- |------ |------ |----------:|---:|--------:|-------:|----------:|
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt  | Row   | 50000 |  1.250 ms | 29 | 23343.3 |   25.0 |     288 B |
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt | Row   | 50000 |  1.317 ms | 29 | 22162.9 |   26.3 |     288 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.362 ms | 29 | 21429.4 |   27.2 |     288 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.390 ms | 29 | 20997.2 |   27.8 |     288 B |
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.395 ms | 29 | 20921.4 |   27.9 |     288 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.547 ms | 29 | 18869.1 |   30.9 |     288 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt | Row   | 50000 |  1.580 ms | 29 | 18465.7 |   31.6 |     288 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.620 ms | 29 | 18016.1 |   32.4 |     288 B |
| Sep_   | SepParserIndexOfAny                    | Row   | 50000 |  9.152 ms | 29 |  3188.4 |  183.0 |     288 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt     | Row   | 50000 | 56.265 ms | 29 |   518.6 | 1125.3 |     288 B |
