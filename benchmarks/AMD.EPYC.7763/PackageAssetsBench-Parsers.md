```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-OFNCWY : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-OFNCWY  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                              | Scope | Rows  | Mean       | MB | MB/s   | ns/row | Allocated |
|------- |------------------------------------ |------ |------ |-----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.437 ms | 29 | 8462.9 |   68.7 |    1050 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt | Row   | 50000 |   3.449 ms | 29 | 8433.1 |   69.0 |    1082 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt | Row   | 50000 |   3.752 ms | 29 | 7752.0 |   75.0 |    1275 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt | Row   | 50000 |   3.933 ms | 29 | 7396.0 |   78.7 |     988 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   4.006 ms | 29 | 7260.4 |   80.1 |     973 B |
| Sep_   | SepParserIndexOfAny                 | Row   | 50000 |  13.550 ms | 29 | 2146.5 |  271.0 |    1000 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt  | Row   | 50000 | 134.600 ms | 29 |  216.1 | 2692.0 |    4454 B |
