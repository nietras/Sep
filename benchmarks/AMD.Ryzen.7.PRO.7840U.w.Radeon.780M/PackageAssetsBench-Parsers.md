```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26100.6899/24H2/2024Update/HudsonValley)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics 3.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                 | Scope | Rows  | Mean       | MB | MB/s   | ns/row | Allocated |
|------- |--------------------------------------- |------ |------ |-----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |   3.364 ms | 29 | 8674.1 |   67.3 |    1032 B |
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt | Row   | 50000 |   3.465 ms | 29 | 8421.2 |   69.3 |    1032 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt    | Row   | 50000 |   3.530 ms | 29 | 8267.7 |   70.6 |    1064 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |   3.759 ms | 29 | 7762.3 |   75.2 |     952 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt    | Row   | 50000 |   3.813 ms | 29 | 7653.9 |   76.3 |     968 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt | Row   | 50000 |   3.880 ms | 29 | 7520.4 |   77.6 |     953 B |
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt  | Row   | 50000 |   3.958 ms | 29 | 7372.4 |   79.2 |    1192 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt    | Row   | 50000 |   4.224 ms | 29 | 6908.8 |   84.5 |    1256 B |
| Sep_   | SepParserIndexOfAny                    | Row   | 50000 |  19.074 ms | 29 | 1529.9 |  381.5 |     928 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt     | Row   | 50000 | 112.859 ms | 29 |  258.6 | 2257.2 |     928 B |
