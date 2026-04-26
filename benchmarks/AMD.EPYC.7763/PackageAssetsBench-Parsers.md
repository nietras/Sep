```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.82GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                              | Scope | Rows  | Mean       | MB | MB/s   | ns/row | Allocated |
|------- |------------------------------------ |------ |------ |-----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.406 ms | 29 | 8539.8 |   68.1 |    1040 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt | Row   | 50000 |   3.466 ms | 29 | 8392.5 |   69.3 |    1072 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.748 ms | 29 | 7759.5 |   75.0 |     960 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt | Row   | 50000 |   4.014 ms | 29 | 7245.9 |   80.3 |     977 B |
| Sep_   | SepParserIndexOfAny                 | Row   | 50000 |  13.995 ms | 29 | 2078.3 |  279.9 |     936 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt | Row   | 50000 |  20.969 ms | 29 | 1387.1 |  419.4 |    1264 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt  | Row   | 50000 | 106.406 ms | 29 |  273.3 | 2128.1 |     936 B |
