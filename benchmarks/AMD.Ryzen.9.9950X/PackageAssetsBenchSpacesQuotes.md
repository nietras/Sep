```

BenchmarkDotNet v0.15.6, Windows 10 (10.0.19045.6575/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  5.418 ms |  1.00 | 41 | 7710.0 |  108.4 |   1.01 KB |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  7.904 ms |  1.46 | 41 | 5284.9 |  158.1 |   1.01 KB |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  8.553 ms |  1.58 | 41 | 4883.5 |  171.1 |   1.01 KB |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |  9.166 ms |  1.69 | 41 | 4557.1 |  183.3 |   1.01 KB |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 61.144 ms | 11.29 | 41 |  683.1 | 1222.9 | 451.27 KB |      447.33 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 60.851 ms | 11.23 | 41 |  686.4 | 1217.0 | 447.61 KB |      443.71 |
