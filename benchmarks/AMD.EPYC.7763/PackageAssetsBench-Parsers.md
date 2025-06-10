```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-GLYBTL : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-GLYBTL  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                              | Scope | Rows  | Mean       | MB | MB/s   | ns/row | Allocated |
|------- |------------------------------------ |------ |------ |-----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.502 ms | 29 | 8305.3 |   70.0 |    1050 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt | Row   | 50000 |   3.631 ms | 29 | 8009.6 |   72.6 |    1082 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt | Row   | 50000 |   3.894 ms | 29 | 7469.6 |   77.9 |    1272 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.919 ms | 29 | 7422.3 |   78.4 |     972 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt | Row   | 50000 |   3.953 ms | 29 | 7357.6 |   79.1 |     988 B |
| Sep_   | SepParserIndexOfAny                 | Row   | 50000 |  13.647 ms | 29 | 2131.3 |  272.9 |     988 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt  | Row   | 50000 | 131.143 ms | 29 |  221.8 | 2622.9 |    4460 B |
