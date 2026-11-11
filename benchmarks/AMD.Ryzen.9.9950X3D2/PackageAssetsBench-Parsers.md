```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 53.15 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                        | Scope | Rows  | Mean      | MB | MB/s    | ns/row | Allocated |
|------- |---------------------------------------------- |------ |------ |----------:|---:|--------:|-------:|----------:|
| Sep_   | SepParserAvx512To2x256To512CmpOrMoveMaskTzcnt | Row   | 50000 |  1.137 ms | 29 | 25658.9 |   22.7 |     288 B |
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt         | Row   | 50000 |  1.148 ms | 29 | 25409.9 |   23.0 |     288 B |
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt        | Row   | 50000 |  1.202 ms | 29 | 24283.2 |   24.0 |     288 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt           | Row   | 50000 |  1.254 ms | 29 | 23271.7 |   25.1 |     288 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt           | Row   | 50000 |  1.270 ms | 29 | 22971.5 |   25.4 |     288 B |
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt           | Row   | 50000 |  1.280 ms | 29 | 22804.9 |   25.6 |     288 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt           | Row   | 50000 |  1.424 ms | 29 | 20491.5 |   28.5 |     288 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt        | Row   | 50000 |  1.445 ms | 29 | 20198.1 |   28.9 |     288 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt           | Row   | 50000 |  1.517 ms | 29 | 19231.6 |   30.3 |     288 B |
| Sep_   | SepParserIndexOfAny                           | Row   | 50000 |  8.896 ms | 29 |  3280.4 |  177.9 |     288 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt            | Row   | 50000 | 50.765 ms | 29 |   574.8 | 1015.3 |     288 B |
