```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.56GHz), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                 | Scope | Rows  | Mean       | MB | MB/s   | ns/row | Allocated |
|------- |--------------------------------------- |------ |------ |-----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAvx512PackCmpOrMoveMaskTzcnt  | Row   | 50000 |   4.041 ms | 29 | 7198.0 |   80.8 |    1200 B |
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |   4.530 ms | 29 | 6420.9 |   90.6 |    1040 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt    | Row   | 50000 |   4.644 ms | 29 | 6263.0 |   92.9 |    1072 B |
| Sep_   | SepParserAvx512To256CmpOrMoveMaskTzcnt | Row   | 50000 |   4.743 ms | 29 | 6131.8 |   94.9 |    1040 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt    | Row   | 50000 |   4.845 ms | 29 | 6003.8 |   96.9 |    1264 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt    | Row   | 50000 |   5.302 ms | 29 | 5485.6 |  106.0 |     976 B |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt    | Row   | 50000 |   5.788 ms | 29 | 5025.4 |  115.8 |     960 B |
| Sep_   | SepParserAvx256To128CmpOrMoveMaskTzcnt | Row   | 50000 |   6.149 ms | 29 | 4729.9 |  123.0 |     960 B |
| Sep_   | SepParserIndexOfAny                    | Row   | 50000 |  19.654 ms | 29 | 1479.9 |  393.1 |     936 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt     | Row   | 50000 | 103.063 ms | 29 |  282.2 | 2061.3 |     936 B |
