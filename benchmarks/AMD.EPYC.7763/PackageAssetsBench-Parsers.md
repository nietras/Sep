```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.71GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.734 ms | 29 | 7788.7 |   74.7 |    1048 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt | Row   | 50000 |   3.825 ms | 29 | 7604.0 |   76.5 |    1080 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   4.071 ms | 29 | 7144.7 |   81.4 |     968 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt | Row   | 50000 |   4.296 ms | 29 | 6769.9 |   85.9 |     985 B |
| Sep_   | SepParserIndexOfAny                 | Row   | 50000 |  14.298 ms | 29 | 2034.3 |  286.0 |     946 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt | Row   | 50000 |  21.526 ms | 29 | 1351.2 |  430.5 |    1272 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt  | Row   | 50000 | 106.676 ms | 29 |  272.7 | 2133.5 |     944 B |
