```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26100.33674/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.57 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  
Runtime=.NET 11.0  Toolchain=CsProjCore  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean      | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  34.16 ms |  1.00 |  29 |  854.2 |  683.3 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |  16.98 ms |  0.50 |  29 | 1718.6 |  339.6 |  13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |  42.33 ms |  1.24 |  29 |  689.3 |  846.7 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  41.52 ms |  1.22 |  29 |  702.7 |  830.5 |  14.41 MB |        1.07 |
|           |       |         |           |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 672.42 ms |  1.00 | 583 |  868.2 |  672.4 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 344.74 ms |  0.51 | 583 | 1693.4 |  344.7 | 269.58 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 860.44 ms |  1.28 | 583 |  678.5 |  860.4 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 803.56 ms |  1.20 | 583 |  726.5 |  803.6 | 261.35 MB |        1.00 |
