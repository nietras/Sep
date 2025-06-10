```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-BFPPER : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-BFPPER  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                              | Scope | Rows  | Mean       | MB | MB/s   | ns/row | Allocated |
|------- |------------------------------------ |------ |------ |-----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt | Row   | 50000 |   3.451 ms | 29 | 8428.7 |   69.0 |    1082 B |
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.538 ms | 29 | 8221.2 |   70.8 |    1051 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt | Row   | 50000 |   3.821 ms | 29 | 7611.7 |   76.4 |    1275 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt | Row   | 50000 |   3.943 ms | 29 | 7376.9 |   78.9 |     988 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.984 ms | 29 | 7300.1 |   79.7 |     972 B |
| Sep_   | SepParserIndexOfAny                 | Row   | 50000 |  13.558 ms | 29 | 2145.4 |  271.2 |    1007 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt  | Row   | 50000 | 130.677 ms | 29 |  222.6 | 2613.5 |    1370 B |
