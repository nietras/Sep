```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26100.6899/24H2/2024Update/HudsonValley)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics 3.30GHz, 1 CPU, 16 logical and 8 physical cores
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
| Sep_                       | Cols  | 50000 |  13.76 ms |  1.00 | 41 | 3036.5 |  275.1 |   1.01 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.25 ms |  1.33 | 41 | 2289.2 |  364.9 |   1.01 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.43 ms |  1.34 | 41 | 2266.6 |  368.6 |   1.01 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.16 ms |  1.54 | 41 | 1974.2 |  423.2 |   1.01 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 127.73 ms |  9.29 | 41 |  327.0 | 2554.6 | 451.34 KB |      447.84 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 126.60 ms |  9.21 | 41 |  329.9 | 2532.1 | 445.68 KB |      442.22 |
