```

BenchmarkDotNet v0.15.6, Windows 10 (10.0.19045.6575/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  5.434 ms |  1.00 | 41 | 7687.0 |  108.7 |   1.01 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  7.906 ms |  1.46 | 41 | 5283.2 |  158.1 |   1.01 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  8.541 ms |  1.57 | 41 | 4890.5 |  170.8 |   1.01 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  9.059 ms |  1.67 | 41 | 4610.7 |  181.2 |   1.01 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 61.560 ms | 11.33 | 41 |  678.5 | 1231.2 | 451.27 KB |      447.77 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 60.812 ms | 11.19 | 41 |  686.9 | 1216.2 |  445.6 KB |      442.15 |
