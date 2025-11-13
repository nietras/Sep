```

BenchmarkDotNet v0.15.6, Windows 10 (10.0.19045.6575/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                 | Scope | Rows  | Mean      | MB | MB/s    | ns/row | Allocated |
|------- |--------------------------------------- |------ |------ |----------:|---:|--------:|-------:|----------:|
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt | Row   | 50000 |  1.375 ms | 29 | 21226.0 |   27.5 |    1032 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.405 ms | 29 | 20763.0 |   28.1 |    1114 B |
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.435 ms | 29 | 20340.4 |   28.7 |    1032 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.489 ms | 29 | 19593.9 |   29.8 |    1005 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt | Row   | 50000 |  1.505 ms | 29 | 19386.6 |   30.1 |     952 B |
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt  | Row   | 50000 |  1.516 ms | 29 | 19254.0 |   30.3 |    1192 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.568 ms | 29 | 18612.3 |   31.4 |    1256 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.576 ms | 29 | 18510.7 |   31.5 |     968 B |
| Sep_   | SepParserIndexOfAny                    | Row   | 50000 |  9.077 ms | 29 |  3214.9 |  181.5 |     929 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt     | Row   | 50000 | 50.524 ms | 29 |   577.6 | 1010.5 |     928 B |
