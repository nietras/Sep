```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.6899) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  14.19 ms |  1.00 | 41 | 2942.7 |  283.9 |     954 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  18.47 ms |  1.30 | 41 | 2261.5 |  369.4 |     955 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  19.41 ms |  1.37 | 41 | 2152.0 |  388.2 |     955 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  20.91 ms |  1.47 | 41 | 1997.4 |  418.3 |     952 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 120.00 ms |  8.45 | 41 |  348.1 | 2400.0 |  462174 B |      484.46 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 115.80 ms |  8.16 | 41 |  360.7 | 2316.1 |  459458 B |      481.61 |
