```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7184/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                 | Scope | Rows  | Mean      | MB | MB/s    | ns/row | Allocated |
|------- |--------------------------------------- |------ |------ |----------:|---:|--------:|-------:|----------:|
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt | Row   | 50000 |  1.368 ms | 29 | 21329.0 |   27.4 |    1040 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.423 ms | 29 | 20499.9 |   28.5 |    1072 B |
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.431 ms | 29 | 20395.4 |   28.6 |    1040 B |
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt  | Row   | 50000 |  1.473 ms | 29 | 19814.5 |   29.5 |    1200 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.485 ms | 29 | 19646.3 |   29.7 |     960 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt | Row   | 50000 |  1.500 ms | 29 | 19458.1 |   30.0 |     960 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.571 ms | 29 | 18575.7 |   31.4 |     976 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.581 ms | 29 | 18463.1 |   31.6 |    1264 B |
| Sep_   | SepParserIndexOfAny                    | Row   | 50000 |  9.119 ms | 29 |  3200.0 |  182.4 |     937 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt     | Row   | 50000 | 50.667 ms | 29 |   575.9 | 1013.3 |     936 B |
