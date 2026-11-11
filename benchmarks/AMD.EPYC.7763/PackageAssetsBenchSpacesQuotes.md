```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26100.33674/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.57 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 10.33 ms |  1.00 | 41 | 4042.1 |  206.7 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 15.73 ms |  1.52 | 41 | 2655.3 |  314.6 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 17.08 ms |  1.65 | 41 | 2445.3 |  341.6 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 20.43 ms |  1.98 | 41 | 2044.2 |  408.7 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 81.99 ms |  7.93 | 41 |  509.4 | 1639.8 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 88.14 ms |  8.53 | 41 |  473.9 | 1762.8 |    1264 B |        4.39 |
