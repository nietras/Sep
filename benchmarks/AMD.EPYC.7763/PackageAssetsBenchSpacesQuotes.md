```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.59 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.36 ms |  1.00 | 41 | 3379.3 |  247.2 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  19.11 ms |  1.55 | 41 | 2185.5 |  382.3 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.26 ms |  1.48 | 41 | 2287.1 |  365.3 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.04 ms |  1.70 | 41 | 1985.1 |  420.8 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 149.21 ms | 12.07 | 41 |  279.9 | 2984.2 | 451.27 KB |      444.32 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 142.75 ms | 11.55 | 41 |  292.6 | 2855.0 |  445.6 KB |      438.75 |
