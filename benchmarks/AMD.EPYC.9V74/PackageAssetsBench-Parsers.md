```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.46 GB Available
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
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt        | Row   | 50000 |  2.616 ms | 29 | 11117.5 |   52.3 |     288 B |
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt           | Row   | 50000 |  2.697 ms | 29 | 10783.2 |   53.9 |     288 B |
| Sep_   | SepParserAvx512To2x256To512CmpOrMoveMaskTzcnt | Row   | 50000 |  2.731 ms | 29 | 10652.2 |   54.6 |     288 B |
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt         | Row   | 50000 |  2.759 ms | 29 | 10543.5 |   55.2 |     288 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt           | Row   | 50000 |  2.840 ms | 29 | 10241.3 |   56.8 |     288 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt        | Row   | 50000 |  2.861 ms | 29 | 10167.7 |   57.2 |     288 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt           | Row   | 50000 |  2.903 ms | 29 | 10020.2 |   58.1 |     288 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt           | Row   | 50000 |  3.018 ms | 29 |  9637.6 |   60.4 |     288 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt           | Row   | 50000 |  4.421 ms | 29 |  6579.3 |   88.4 |     288 B |
| Sep_   | SepParserIndexOfAny                           | Row   | 50000 | 13.474 ms | 29 |  2158.7 |  269.5 |     288 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt            | Row   | 50000 | 89.705 ms | 29 |   324.2 | 1794.1 |     288 B |
