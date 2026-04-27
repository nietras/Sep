```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  13.17 ms |  1.00 | 41 | 3164.7 |  263.4 |     960 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.04 ms |  1.37 | 41 | 2309.5 |  360.9 |     960 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  18.34 ms |  1.39 | 41 | 2272.0 |  366.8 |     960 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.31 ms |  1.54 | 41 | 2051.7 |  406.2 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 119.22 ms |  9.06 | 41 |  349.6 | 2384.4 |  462174 B |      481.43 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 115.59 ms |  8.78 | 41 |  360.5 | 2311.9 |  456374 B |      475.39 |
