```

BenchmarkDotNet v0.15.6, Windows 10 (10.0.19045.6575/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |  1.265 ms |  1.00 | 20 | 16063.2 |   50.6 |     1.24 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.624 ms |  1.28 | 20 | 12511.4 |   65.0 |    10.75 KB |        8.66 |
| ReadLine_ | Row    | 25000 |  6.545 ms |  5.17 | 20 |  3104.8 |  261.8 | 73489.62 KB |   59,161.45 |
| CsvHelper | Row    | 25000 | 14.820 ms | 11.72 | 20 |  1371.1 |  592.8 |    19.95 KB |       16.06 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |  1.929 ms |  1.00 | 20 | 10533.1 |   77.2 |     1.24 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.589 ms |  1.34 | 20 |  7850.0 |  103.5 |     10.7 KB |        8.61 |
| ReadLine_ | Cols   | 25000 |  6.814 ms |  3.53 | 20 |  2982.2 |  272.5 | 73489.62 KB |   59,161.45 |
| CsvHelper | Cols   | 25000 | 15.995 ms |  8.29 | 20 |  1270.4 |  639.8 | 21340.17 KB |   17,179.50 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Floats | 25000 | 13.965 ms |  1.00 | 20 |  1455.0 |  558.6 |     7.89 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.049 ms |  0.15 | 20 |  9918.5 |   81.9 |   178.98 KB |       22.68 |
| Sylvan___ | Floats | 25000 | 35.057 ms |  2.51 | 20 |   579.6 | 1402.3 |    18.67 KB |        2.37 |
| ReadLine_ | Floats | 25000 | 47.821 ms |  3.42 | 20 |   424.9 | 1912.8 | 73492.95 KB |    9,311.65 |
| CsvHelper | Floats | 25000 | 66.495 ms |  4.76 | 20 |   305.6 | 2659.8 | 22061.22 KB |    2,795.19 |
