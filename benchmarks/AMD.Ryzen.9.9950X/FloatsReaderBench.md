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
| Sep______ | Row    | 25000 |  1.287 ms |  1.00 | 20 | 15793.8 |   51.5 |     1.24 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  1.646 ms |  1.28 | 20 | 12342.6 |   65.9 |     10.7 KB |        8.61 |
| ReadLine_ | Row    | 25000 |  6.762 ms |  5.26 | 20 |  3004.8 |  270.5 | 73489.62 KB |   59,161.45 |
| CsvHelper | Row    | 25000 | 14.950 ms | 11.62 | 20 |  1359.1 |  598.0 |    19.95 KB |       16.06 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |  1.949 ms |  1.00 | 20 | 10426.9 |   78.0 |     1.24 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  2.642 ms |  1.36 | 20 |  7692.0 |  105.7 |     10.7 KB |        8.61 |
| ReadLine_ | Cols   | 25000 |  7.037 ms |  3.61 | 20 |  2887.5 |  281.5 | 73489.62 KB |   59,161.45 |
| CsvHelper | Cols   | 25000 | 16.180 ms |  8.30 | 20 |  1255.9 |  647.2 | 21340.17 KB |   17,179.50 |
|           |        |       |           |       |    |         |        |             |             |
| Sep______ | Floats | 25000 | 14.225 ms |  1.00 | 20 |  1428.5 |  569.0 |     7.89 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  2.034 ms |  0.14 | 20 |  9989.8 |   81.4 |   178.66 KB |       22.64 |
| Sylvan___ | Floats | 25000 | 35.214 ms |  2.48 | 20 |   577.0 | 1408.5 |    18.67 KB |        2.37 |
| ReadLine_ | Floats | 25000 | 48.283 ms |  3.39 | 20 |   420.8 | 1931.3 | 73492.94 KB |    9,312.80 |
| CsvHelper | Floats | 25000 | 66.484 ms |  4.67 | 20 |   305.6 | 2659.3 | 22061.22 KB |    2,795.53 |
