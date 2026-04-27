```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.24GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.660 ms | 29 | 7947.0 |   73.2 |    1040 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt | Row   | 50000 |   3.681 ms | 29 | 7900.7 |   73.6 |    1072 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.917 ms | 29 | 7425.8 |   78.3 |     961 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt | Row   | 50000 |   4.237 ms | 29 | 6864.1 |   84.7 |     976 B |
| Sep_   | SepParserIndexOfAny                 | Row   | 50000 |  14.094 ms | 29 | 2063.8 |  281.9 |     938 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt | Row   | 50000 |  21.261 ms | 29 | 1368.0 |  425.2 |    1264 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt  | Row   | 50000 | 106.706 ms | 29 |  272.6 | 2134.1 |     936 B |
