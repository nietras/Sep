```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 3.49GHz), 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.5 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                        | Scope | Rows  | Mean       | MB | MB/s   | ns/row | Allocated |
|------- |---------------------------------------------- |------ |------ |-----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt           | Row   | 50000 |   4.387 ms | 29 | 6630.1 |   87.7 |     288 B |
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt         | Row   | 50000 |   4.526 ms | 29 | 6426.6 |   90.5 |     288 B |
| Sep_   | SepParserAvx512To2x256To512CmpOrMoveMaskTzcnt | Row   | 50000 |   4.714 ms | 29 | 6170.4 |   94.3 |     288 B |
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt        | Row   | 50000 |   4.903 ms | 29 | 5931.8 |   98.1 |     288 B |
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt           | Row   | 50000 |   5.189 ms | 29 | 5605.5 |  103.8 |     288 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt           | Row   | 50000 |   5.235 ms | 29 | 5555.6 |  104.7 |     288 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt        | Row   | 50000 |   5.713 ms | 29 | 5091.3 |  114.3 |     288 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt           | Row   | 50000 |   5.812 ms | 29 | 5004.6 |  116.2 |     288 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt           | Row   | 50000 |   5.883 ms | 29 | 4944.0 |  117.7 |     288 B |
| Sep_   | SepParserIndexOfAny                           | Row   | 50000 |  20.137 ms | 29 | 1444.4 |  402.7 |     288 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt            | Row   | 50000 | 103.009 ms | 29 |  282.4 | 2060.2 |     288 B |
