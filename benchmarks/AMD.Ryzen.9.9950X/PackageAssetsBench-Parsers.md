```

BenchmarkDotNet v0.15.2, Windows 10 (10.0.19045.6456/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.100-rc.2.25502.107
  [Host]     : .NET 10.0.0 (10.0.25.50307), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-VUTQUY : .NET 9.0.10 (9.0.1025.47515), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-BKCCMB : .NET 10.0.0 (10.0.25.50307), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Runtime   | Parser                                 | Scope | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|------- |---------- |--------------------------------------- |------ |------ |----------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep_   | .NET 9.0  | SepParserAvx256To128CmpOrMoveMaskTzcnt | Row   | 50000 |  1.573 ms |  1.00 | 29 | 18556.7 |   31.5 |     952 B |        1.00 |
| Sep_   | .NET 10.0 | SepParserAvx256To128CmpOrMoveMaskTzcnt | Row   | 50000 |  1.612 ms |  1.02 | 29 | 18107.6 |   32.2 |     952 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 9.0  | SepParserAvx2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.496 ms |  1.00 | 29 | 19501.7 |   29.9 |    1032 B |        1.00 |
| Sep_   | .NET 10.0 | SepParserAvx2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.502 ms |  1.00 | 29 | 19424.3 |   30.0 |    1032 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 10.0 | SepParserAvx512PackCmpOrMoveMaskTzcnt  | Row   | 50000 |  1.536 ms |  1.00 | 29 | 19002.3 |   30.7 |    1192 B |        1.00 |
| Sep_   | .NET 9.0  | SepParserAvx512PackCmpOrMoveMaskTzcnt  | Row   | 50000 |  1.537 ms |  1.00 | 29 | 18990.4 |   30.7 |    1192 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 9.0  | SepParserAvx512To256CmpOrMoveMaskTzcnt | Row   | 50000 |  1.446 ms |  1.00 | 29 | 20179.9 |   28.9 |    1032 B |        1.00 |
| Sep_   | .NET 10.0 | SepParserAvx512To256CmpOrMoveMaskTzcnt | Row   | 50000 |  1.462 ms |  1.01 | 29 | 19959.4 |   29.2 |    1032 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 10.0 | SepParserIndexOfAny                    | Row   | 50000 |  9.405 ms |  0.87 | 29 |  3102.9 |  188.1 |     929 B |        1.00 |
| Sep_   | .NET 9.0  | SepParserIndexOfAny                    | Row   | 50000 | 10.778 ms |  1.00 | 29 |  2707.6 |  215.6 |     930 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 10.0 | SepParserSse2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.538 ms |  0.97 | 29 | 18979.0 |   30.8 |     952 B |        1.00 |
| Sep_   | .NET 9.0  | SepParserSse2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.589 ms |  1.00 | 29 | 18368.6 |   31.8 |     952 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 10.0 | SepParserVector128NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.626 ms |  0.98 | 29 | 17949.8 |   32.5 |     968 B |        1.00 |
| Sep_   | .NET 9.0  | SepParserVector128NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.665 ms |  1.00 | 29 | 17530.4 |   33.3 |     968 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 10.0 | SepParserVector256NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.476 ms |  1.00 | 29 | 19766.7 |   29.5 |    1064 B |        1.00 |
| Sep_   | .NET 9.0  | SepParserVector256NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.479 ms |  1.00 | 29 | 19727.7 |   29.6 |    1064 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 10.0 | SepParserVector512NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.622 ms |  0.95 | 29 | 17988.2 |   32.4 |    1256 B |        1.00 |
| Sep_   | .NET 9.0  | SepParserVector512NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.705 ms |  1.00 | 29 | 17118.4 |   34.1 |    1256 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 10.0 | SepParserVector64NrwCmpExtMsbTzcnt     | Row   | 50000 | 51.275 ms |  0.80 | 29 |   569.1 | 1025.5 |     928 B |        1.00 |
| Sep_   | .NET 9.0  | SepParserVector64NrwCmpExtMsbTzcnt     | Row   | 50000 | 64.169 ms |  1.00 | 29 |   454.8 | 1283.4 |     928 B |        1.00 |
