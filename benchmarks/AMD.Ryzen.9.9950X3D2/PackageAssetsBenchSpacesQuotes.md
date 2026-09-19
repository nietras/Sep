```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9950X3D2 4.30GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 52.15 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  5.418 ms |  1.00 | 41 | 7709.2 |  108.4 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  7.983 ms |  1.47 | 41 | 5232.0 |  159.7 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  8.433 ms |  1.56 | 41 | 4952.9 |  168.7 |   1.02 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  8.883 ms |  1.64 | 41 | 4702.2 |  177.7 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 62.540 ms | 11.54 | 41 |  667.9 | 1250.8 | 451.27 KB |      444.32 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 61.773 ms | 11.40 | 41 |  676.2 | 1235.5 |  445.6 KB |      438.75 |
