```

BenchmarkDotNet v0.16.0-preview.1, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
Memory: 7 GB Total, 0.09 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  12.80 ms |  1.00 | 41 | 3256.0 |  256.0 |     960 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  16.10 ms |  1.29 | 41 | 2588.2 |  322.0 |     960 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  15.39 ms |  1.23 | 41 | 2708.5 |  307.7 |     960 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.88 ms |  1.76 | 41 | 1904.3 |  437.7 |     960 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 151.22 ms | 12.13 | 41 |  275.6 | 3024.3 |  462096 B |      481.35 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 142.67 ms | 11.44 | 41 |  292.1 | 2853.4 |  456296 B |      475.31 |
