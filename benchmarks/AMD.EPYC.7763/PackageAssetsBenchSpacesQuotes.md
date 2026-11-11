```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26100.33674/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.55 GB Available
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
| Sep_                       | Cols  | 50000 | 10.45 ms |  1.00 | 41 | 3996.8 |  209.0 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 16.18 ms |  1.55 | 41 | 2581.7 |  323.6 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 17.52 ms |  1.68 | 41 | 2383.6 |  350.5 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 19.54 ms |  1.87 | 41 | 2137.4 |  390.8 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 79.65 ms |  7.62 | 41 |  524.4 | 1593.0 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 86.87 ms |  8.31 | 41 |  480.8 | 1737.4 |    1264 B |        4.39 |
