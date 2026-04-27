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
| Sep_                       | Cols  | 50000 |  14.09 ms |  1.00 | 41 | 2964.3 |  281.8 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.78 ms |  1.33 | 41 | 2224.0 |  375.6 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.39 ms |  1.38 | 41 | 2154.5 |  387.7 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.48 ms |  1.52 | 41 | 1944.5 |  429.6 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 144.29 ms | 10.24 | 41 |  289.5 | 2885.9 | 451.34 KB |      443.54 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 145.05 ms | 10.29 | 41 |  288.0 | 2901.1 | 445.67 KB |      437.97 |
