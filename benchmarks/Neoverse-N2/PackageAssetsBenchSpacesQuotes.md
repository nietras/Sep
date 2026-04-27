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
| Sep_                       | Cols  | 50000 |  14.16 ms |  1.00 | 41 | 2943.6 |  283.1 |     962 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.72 ms |  1.32 | 41 | 2226.4 |  374.4 |     960 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.77 ms |  1.40 | 41 | 2108.5 |  395.3 |     963 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.97 ms |  1.48 | 41 | 1987.0 |  419.5 |     963 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 119.40 ms |  8.43 | 41 |  349.0 | 2388.0 |  462168 B |      480.42 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 115.79 ms |  8.18 | 41 |  359.9 | 2315.9 |  456374 B |      474.40 |
