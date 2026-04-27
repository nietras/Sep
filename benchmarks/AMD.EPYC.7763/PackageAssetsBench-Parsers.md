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
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.571 ms | 29 | 8144.5 |   71.4 |    1040 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt | Row   | 50000 |   3.635 ms | 29 | 8002.4 |   72.7 |    1073 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.917 ms | 29 | 7425.8 |   78.3 |     960 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt | Row   | 50000 |   4.161 ms | 29 | 6989.9 |   83.2 |     976 B |
| Sep_   | SepParserIndexOfAny                 | Row   | 50000 |  13.977 ms | 29 | 2081.0 |  279.5 |     938 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt | Row   | 50000 |  20.945 ms | 29 | 1388.7 |  418.9 |    1264 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt  | Row   | 50000 | 106.509 ms | 29 |  273.1 | 2130.2 |     936 B |
