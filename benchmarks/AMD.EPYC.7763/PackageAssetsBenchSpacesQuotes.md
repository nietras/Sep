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
| Sep_                       | Cols  | 50000 |  13.74 ms |  1.00 | 41 | 3039.3 |  274.9 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.31 ms |  1.33 | 41 | 2280.8 |  366.3 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.10 ms |  1.39 | 41 | 2186.8 |  382.0 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.02 ms |  1.53 | 41 | 1986.8 |  420.5 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 144.01 ms | 10.48 | 41 |  290.0 | 2880.3 | 451.35 KB |      444.40 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 142.79 ms | 10.39 | 41 |  292.5 | 2855.8 | 445.68 KB |      438.83 |
