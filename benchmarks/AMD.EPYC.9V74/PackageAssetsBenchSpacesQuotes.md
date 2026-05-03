```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep_                       | Cols  | 50000 |  12.75 ms |  1.00 | 41 | 3275.8 |  255.0 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  19.05 ms |  1.49 | 41 | 2192.1 |  381.1 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.87 ms |  1.56 | 41 | 2101.8 |  397.5 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.73 ms |  1.70 | 41 | 1921.9 |  434.7 |   1.03 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 131.89 ms | 10.34 | 41 |  316.7 | 2637.7 | 451.34 KB |      441.01 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 131.37 ms | 10.30 | 41 |  318.0 | 2627.4 | 445.68 KB |      435.47 |
