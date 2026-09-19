```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9950X3D2 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  5.374 ms |  1.00 | 41 | 7773.1 |  107.5 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  7.950 ms |  1.48 | 41 | 5253.9 |  159.0 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  8.618 ms |  1.60 | 41 | 4847.0 |  172.4 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  9.173 ms |  1.71 | 41 | 4553.6 |  183.5 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 62.806 ms | 11.69 | 41 |  665.1 | 1256.1 | 451.34 KB |      441.00 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 61.481 ms | 11.44 | 41 |  679.4 | 1229.6 | 445.61 KB |      435.40 |
