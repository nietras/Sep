```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-MPBGVI : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-MPBGVI  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                              | Scope | Rows  | Mean       | MB | MB/s   | ns/row | Allocated |
|------- |------------------------------------ |------ |------ |-----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.471 ms | 29 | 8378.9 |   69.4 |    1050 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt | Row   | 50000 |   3.496 ms | 29 | 8320.8 |   69.9 |    1081 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt | Row   | 50000 |   3.659 ms | 29 | 7948.9 |   73.2 |    1274 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.794 ms | 29 | 7665.4 |   75.9 |     969 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt | Row   | 50000 |   3.858 ms | 29 | 7539.9 |   77.2 |     987 B |
| Sep_   | SepParserIndexOfAny                 | Row   | 50000 |  13.463 ms | 29 | 2160.5 |  269.3 |    1007 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt  | Row   | 50000 | 134.505 ms | 29 |  216.2 | 2690.1 |    1370 B |
