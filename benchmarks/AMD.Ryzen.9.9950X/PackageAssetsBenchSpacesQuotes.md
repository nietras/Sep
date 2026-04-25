```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7184/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  5.313 ms |  1.00 | 41 | 7861.1 |  106.3 |   1.02 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  8.045 ms |  1.51 | 41 | 5192.2 |  160.9 |   1.02 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  8.821 ms |  1.66 | 41 | 4735.1 |  176.4 |   1.32 KB |        1.30 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  9.443 ms |  1.78 | 41 | 4423.5 |  188.9 |   1.02 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 62.051 ms | 11.68 | 41 |  673.1 | 1241.0 | 451.27 KB |      444.32 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 61.109 ms | 11.50 | 41 |  683.5 | 1222.2 |  445.6 KB |      438.75 |
