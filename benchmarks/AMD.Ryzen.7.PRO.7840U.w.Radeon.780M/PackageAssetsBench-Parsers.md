```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-DEDWQO : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-DEDWQO  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                 | Scope | Rows  | Mean       | MB | MB/s   | ns/row | Allocated |
|------- |--------------------------------------- |------ |------ |-----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |   3.385 ms | 29 | 8619.9 |   67.7 |    1046 B |
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt | Row   | 50000 |   3.419 ms | 29 | 8535.4 |   68.4 |    1047 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt    | Row   | 50000 |   3.424 ms | 29 | 8521.7 |   68.5 |    1078 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |   3.797 ms | 29 | 7685.3 |   75.9 |     968 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt | Row   | 50000 |   3.815 ms | 29 | 7649.7 |   76.3 |     968 B |
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt  | Row   | 50000 |   3.828 ms | 29 | 7623.3 |   76.6 |    1207 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt    | Row   | 50000 |   3.852 ms | 29 | 7576.6 |   77.0 |     983 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt    | Row   | 50000 |   4.201 ms | 29 | 6946.3 |   84.0 |    1273 B |
| Sep_   | SepParserIndexOfAny                    | Row   | 50000 |  19.134 ms | 29 | 1525.1 |  382.7 |    1002 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt     | Row   | 50000 | 143.313 ms | 29 |  203.6 | 2866.3 |    1656 B |
