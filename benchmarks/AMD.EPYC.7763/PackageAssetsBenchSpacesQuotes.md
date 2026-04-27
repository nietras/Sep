```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  14.07 ms |  1.00 | 41 | 2969.5 |  281.3 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.39 ms |  1.31 | 41 | 2271.1 |  367.8 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.18 ms |  1.36 | 41 | 2177.6 |  383.6 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.28 ms |  1.51 | 41 | 1962.7 |  425.6 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 147.39 ms | 10.48 | 41 |  283.4 | 2947.7 | 451.34 KB |      443.54 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 143.44 ms | 10.20 | 41 |  291.2 | 2868.9 | 445.68 KB |      437.98 |
