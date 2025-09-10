```

BenchmarkDotNet v0.15.2, Windows 10 (10.0.19045.6216/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.100-rc.1.25451.107
  [Host]     : .NET 10.0.0 (10.0.25.45207), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-BKCCMB : .NET 10.0.0 (10.0.25.45207), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-VUTQUY : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Runtime   | Parser                                 | Scope | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|------- |---------- |--------------------------------------- |------ |------ |----------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep_   | .NET 10.0 | SepParserAvx256To128CmpOrMoveMaskTzcnt | Row   | 50000 |  1.548 ms |  1.00 | 29 | 18856.5 |   31.0 |     952 B |        1.00 |
| Sep_   | .NET 9.0  | SepParserAvx256To128CmpOrMoveMaskTzcnt | Row   | 50000 |  1.554 ms |  1.00 | 29 | 18783.4 |   31.1 |     952 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 9.0  | SepParserAvx2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.446 ms |  1.00 | 29 | 20178.3 |   28.9 |    1032 B |        1.00 |
| Sep_   | .NET 10.0 | SepParserAvx2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.453 ms |  1.00 | 29 | 20090.2 |   29.1 |    1032 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 9.0  | SepParserAvx512PackCmpOrMoveMaskTzcnt  | Row   | 50000 |  1.484 ms |  1.00 | 29 | 19670.4 |   29.7 |    1192 B |        1.00 |
| Sep_   | .NET 10.0 | SepParserAvx512PackCmpOrMoveMaskTzcnt  | Row   | 50000 |  1.490 ms |  1.00 | 29 | 19586.1 |   29.8 |    1192 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 10.0 | SepParserAvx512To256CmpOrMoveMaskTzcnt | Row   | 50000 |  1.396 ms |  1.00 | 29 | 20897.6 |   27.9 |    1032 B |        1.00 |
| Sep_   | .NET 9.0  | SepParserAvx512To256CmpOrMoveMaskTzcnt | Row   | 50000 |  1.397 ms |  1.00 | 29 | 20891.0 |   27.9 |    1032 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 10.0 | SepParserIndexOfAny                    | Row   | 50000 |  9.254 ms |  0.87 | 29 |  3153.2 |  185.1 |     929 B |        1.00 |
| Sep_   | .NET 9.0  | SepParserIndexOfAny                    | Row   | 50000 | 10.678 ms |  1.00 | 29 |  2732.8 |  213.6 |     929 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 10.0 | SepParserSse2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.512 ms |  0.97 | 29 | 19298.1 |   30.2 |     952 B |        1.00 |
| Sep_   | .NET 9.0  | SepParserSse2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |  1.559 ms |  1.00 | 29 | 18722.1 |   31.2 |     952 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 10.0 | SepParserVector128NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.615 ms |  0.99 | 29 | 18072.0 |   32.3 |     968 B |        1.00 |
| Sep_   | .NET 9.0  | SepParserVector128NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.634 ms |  1.00 | 29 | 17858.5 |   32.7 |     968 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 10.0 | SepParserVector256NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.461 ms |  1.00 | 29 | 19976.5 |   29.2 |    1064 B |        1.00 |
| Sep_   | .NET 9.0  | SepParserVector256NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.466 ms |  1.00 | 29 | 19905.1 |   29.3 |    1064 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 10.0 | SepParserVector512NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.582 ms |  0.96 | 29 | 18442.0 |   31.6 |    1256 B |        1.00 |
| Sep_   | .NET 9.0  | SepParserVector512NrwCmpExtMsbTzcnt    | Row   | 50000 |  1.648 ms |  1.00 | 29 | 17704.8 |   33.0 |    1256 B |        1.00 |
|        |           |                                        |       |       |           |       |    |         |        |           |             |
| Sep_   | .NET 9.0  | SepParserVector64NrwCmpExtMsbTzcnt     | Row   | 50000 | 63.922 ms |  1.00 | 29 |   456.5 | 1278.4 |     928 B |        1.00 |
| Sep_   | .NET 10.0 | SepParserVector64NrwCmpExtMsbTzcnt     | Row   | 50000 | 65.089 ms |  1.02 | 29 |   448.3 | 1301.8 |     928 B |        1.00 |
