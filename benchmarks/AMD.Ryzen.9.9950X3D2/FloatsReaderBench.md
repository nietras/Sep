```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9950X3D2 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |  1.256 ms |  1.00 | 20 | 16172.4 |   50.3 |     1.26 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.672 ms |  1.33 | 20 | 12154.1 |   66.9 |     12.5 KB |        9.94 |
| ReadLine_ | Row    | 25000 |  6.661 ms |  5.30 | 20 |  3050.5 |  266.4 | 73489.62 KB |   58,426.53 |
| CsvHelper | Row    | 25000 | 15.344 ms | 12.21 | 20 |  1324.3 |  613.8 |    19.95 KB |       15.86 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |  2.013 ms |  1.00 | 20 | 10095.8 |   80.5 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.658 ms |  1.32 | 20 |  7645.1 |  106.3 |     12.5 KB |        9.94 |
| ReadLine_ | Cols   | 25000 |  6.951 ms |  3.45 | 20 |  2923.3 |  278.0 | 73489.62 KB |   58,426.53 |
| CsvHelper | Cols   | 25000 | 16.163 ms |  8.03 | 20 |  1257.1 |  646.5 | 21340.17 KB |   16,966.09 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Floats | 25000 | 16.854 ms |  1.00 | 20 |  1205.6 |  674.2 |     7.91 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.353 ms |  0.14 | 20 |  8637.1 |   94.1 |   179.37 KB |       22.69 |
| Sylvan___ | Floats | 25000 | 36.532 ms |  2.17 | 20 |   556.2 | 1461.3 |     18.6 KB |        2.35 |
| ReadLine_ | Floats | 25000 | 48.886 ms |  2.90 | 20 |   415.7 | 1955.5 | 73492.94 KB |    9,295.55 |
| CsvHelper | Floats | 25000 | 68.591 ms |  4.07 | 20 |   296.2 | 2743.6 | 22060.98 KB |    2,790.32 |
