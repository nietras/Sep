```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 38.36 GB Available
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
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt | Row   | 50000 |  1.264 ms | 29 | 23080.4 |   25.3 |     856 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.299 ms | 29 | 22464.0 |   26.0 |     856 B |
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.314 ms | 29 | 22201.3 |   26.3 |     856 B |
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt  | Row   | 50000 |  1.350 ms | 29 | 21614.8 |   27.0 |     856 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.396 ms | 29 | 20907.4 |   27.9 |     856 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.442 ms | 29 | 20240.8 |   28.8 |     856 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt | Row   | 50000 |  1.452 ms | 29 | 20098.3 |   29.0 |     856 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.496 ms | 29 | 19509.3 |   29.9 |     856 B |
| Sep_   | SepParserIndexOfAny                    | Row   | 50000 |  8.994 ms | 29 |  3244.6 |  179.9 |     856 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt     | Row   | 50000 | 51.440 ms | 29 |   567.3 | 1028.8 |     856 B |
