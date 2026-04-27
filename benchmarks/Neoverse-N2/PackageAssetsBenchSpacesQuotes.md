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
| Sep_                       | Cols  | 50000 |  14.19 ms |  1.00 | 41 | 2936.1 |  283.9 |     962 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.66 ms |  1.31 | 41 | 2233.4 |  373.2 |     960 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.39 ms |  1.37 | 41 | 2148.9 |  387.9 |    1648 B |        1.71 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  21.07 ms |  1.48 | 41 | 1978.2 |  421.3 |     963 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 119.58 ms |  8.42 | 41 |  348.5 | 2391.6 |  462168 B |      480.42 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 116.68 ms |  8.22 | 41 |  357.2 | 2333.6 |  456374 B |      474.40 |
