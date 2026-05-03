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
| Sep_                       | Cols  | 50000 |  14.11 ms |  1.00 | 41 | 2953.3 |  282.2 |     970 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.50 ms |  1.31 | 41 | 2252.1 |  370.1 |     971 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.33 ms |  1.37 | 41 | 2155.8 |  386.6 |     971 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.81 ms |  1.47 | 41 | 2002.4 |  416.2 |     971 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 119.12 ms |  8.44 | 41 |  349.8 | 2382.5 |  462168 B |      476.46 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 115.59 ms |  8.19 | 41 |  360.5 | 2311.9 |  456368 B |      470.48 |
