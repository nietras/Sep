```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.326 ms |  1.00 | 20 | 6094.8 |  133.0 |     1.18 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.792 ms |  7.15 | 20 |  852.1 |  951.7 |    12.12 KB |       10.28 |
| ReadLine_ | Row    | 25000 |  19.417 ms |  5.84 | 20 | 1044.0 |  776.7 | 73489.62 KB |   62,295.83 |
| CsvHelper | Row    | 25000 |  33.342 ms | 10.02 | 20 |  608.0 | 1333.7 |    19.95 KB |       16.91 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.616 ms |  1.00 | 20 | 4392.1 |  184.6 |     1.18 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.758 ms |  5.80 | 20 |  757.6 | 1070.3 |    12.13 KB |       10.28 |
| ReadLine_ | Cols   | 25000 |  20.007 ms |  4.34 | 20 | 1013.3 |  800.3 | 73489.62 KB |   62,295.83 |
| CsvHelper | Cols   | 25000 |  37.139 ms |  8.05 | 20 |  545.8 | 1485.6 | 21340.16 KB |   18,089.68 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  32.194 ms |  1.00 | 20 |  629.7 | 1287.8 |     7.83 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.471 ms |  0.29 | 20 | 2140.5 |  378.8 |    69.78 KB |        8.91 |
| Sylvan___ | Floats | 25000 |  87.168 ms |  2.71 | 20 |  232.6 | 3486.7 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 | 100.471 ms |  3.12 | 20 |  201.8 | 4018.8 | 73492.94 KB |    9,388.32 |
| CsvHelper | Floats | 25000 | 136.114 ms |  4.23 | 20 |  148.9 | 5444.6 | 22061.27 KB |    2,818.21 |
