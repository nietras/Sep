```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.68GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.45 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                        | Scope | Rows  | Mean      | MB | MB/s   | ns/row | Allocated |
|------- |---------------------------------------------- |------ |------ |----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt           | Row   | 50000 |  3.016 ms | 29 | 9644.1 |   60.3 |     288 B |
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt        | Row   | 50000 |  3.114 ms | 29 | 9339.8 |   62.3 |     288 B |
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt         | Row   | 50000 |  3.149 ms | 29 | 9238.0 |   63.0 |     288 B |
| Sep_   | SepParserAvx512To2x256To512CmpOrMoveMaskTzcnt | Row   | 50000 |  3.159 ms | 29 | 9207.3 |   63.2 |     288 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt        | Row   | 50000 |  3.223 ms | 29 | 9023.5 |   64.5 |     288 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt           | Row   | 50000 |  3.269 ms | 29 | 8897.6 |   65.4 |     288 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt           | Row   | 50000 |  3.335 ms | 29 | 8720.7 |   66.7 |     288 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt           | Row   | 50000 |  3.495 ms | 29 | 8321.5 |   69.9 |     288 B |
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt           | Row   | 50000 |  4.513 ms | 29 | 6445.5 |   90.3 |     288 B |
| Sep_   | SepParserIndexOfAny                           | Row   | 50000 | 13.953 ms | 29 | 2084.6 |  279.1 |     288 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt            | Row   | 50000 | 89.775 ms | 29 |  324.0 | 1795.5 |     288 B |
