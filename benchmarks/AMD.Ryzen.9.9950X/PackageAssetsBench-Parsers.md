```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7184/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                 | Scope | Rows  | Mean      | MB | MB/s    | ns/row | Allocated |
|------- |--------------------------------------- |------ |------ |----------:|---:|--------:|-------:|----------:|
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt | Row   | 50000 |  1.449 ms | 29 | 20145.4 |   29.0 |    1048 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.452 ms | 29 | 20092.1 |   29.0 |    1080 B |
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.472 ms | 29 | 19830.1 |   29.4 |    1048 B |
| Sep_   | SepParserVectorNrwCmpMoveMaskTzcnt     | Row   | 50000 |  1.481 ms | 29 | 19700.3 |   29.6 |    1032 B |
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt  | Row   | 50000 |  1.521 ms | 29 | 19180.4 |   30.4 |    1208 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.530 ms | 29 | 19076.2 |   30.6 |     968 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt | Row   | 50000 |  1.591 ms | 29 | 18344.1 |   31.8 |     968 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.615 ms | 29 | 18066.1 |   32.3 |    1272 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.618 ms | 29 | 18036.9 |   32.4 |     984 B |
| Sep_   | SepParserIndexOfAny                    | Row   | 50000 |  9.415 ms | 29 |  3099.5 |  188.3 |     944 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt     | Row   | 50000 | 51.396 ms | 29 |   567.8 | 1027.9 |     944 B |
