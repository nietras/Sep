```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
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
| Sep______ | Row    | 25000 |   2.659 ms |  1.00 | 20 | 7623.5 |  106.4 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  19.837 ms |  7.48 | 20 | 1021.9 |  793.5 |    12.16 KB |       10.38 |
| ReadLine_ | Row    | 25000 |  15.153 ms |  5.71 | 20 | 1337.8 |  606.1 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  28.121 ms | 10.60 | 20 |  720.9 | 1124.9 |    20.02 KB |       17.08 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.274 ms |  1.00 | 20 | 6190.9 |  131.0 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  20.481 ms |  6.25 | 20 |  989.8 |  819.2 |    12.12 KB |       10.34 |
| ReadLine_ | Cols   | 25000 |  14.380 ms |  4.39 | 20 | 1409.7 |  575.2 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  29.956 ms |  9.15 | 20 |  676.7 | 1198.2 | 21340.23 KB |   18,210.33 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  23.899 ms |  1.00 | 20 |  848.2 |  956.0 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.051 ms |  0.38 | 20 | 2239.8 |  362.0 |    81.34 KB |       10.40 |
| Sylvan___ | Floats | 25000 |  66.316 ms |  2.77 | 20 |  305.7 | 2652.6 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  75.462 ms |  3.16 | 20 |  268.6 | 3018.5 | 73492.94 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 101.102 ms |  4.23 | 20 |  200.5 | 4044.1 | 22061.63 KB |    2,821.07 |
