```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.3695) (Hyper-V)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.204
  [Host]     : .NET 9.0.5 (9.0.525.21509), X64 RyuJIT AVX2
  Job-OEZKPT : .NET 9.0.5 (9.0.525.21509), X64 RyuJIT AVX2

Job=Job-OEZKPT  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                              | Scope | Rows  | Mean       | MB | MB/s   | ns/row | Allocated |
|------- |------------------------------------ |------ |------ |-----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.552 ms | 29 | 8215.0 |   71.0 |    1047 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt | Row   | 50000 |   3.767 ms | 29 | 7746.2 |   75.3 |    1214 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   3.929 ms | 29 | 7426.7 |   78.6 |     968 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt | Row   | 50000 |   3.965 ms | 29 | 7358.9 |   79.3 |    1271 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt | Row   | 50000 |   4.139 ms | 29 | 7049.7 |   82.8 |     985 B |
| Sep_   | SepParserIndexOfAny                 | Row   | 50000 |  13.683 ms | 29 | 2132.7 |  273.7 |     986 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt  | Row   | 50000 | 128.331 ms | 29 |  227.4 | 2566.6 |    1280 B |
