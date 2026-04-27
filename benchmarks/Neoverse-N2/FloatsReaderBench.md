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
| Sep______ | Row    | 25000 |   3.673 ms |  1.00 | 20 | 5519.7 |  146.9 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.786 ms |  6.48 | 20 |  852.3 |  951.5 |    12.12 KB |       10.35 |
| ReadLine_ | Row    | 25000 |  19.595 ms |  5.34 | 20 | 1034.5 |  783.8 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  33.364 ms |  9.09 | 20 |  607.6 | 1334.5 |    19.95 KB |       17.02 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.871 ms |  1.00 | 20 | 4162.0 |  194.8 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.873 ms |  5.52 | 20 |  754.4 | 1074.9 |    12.13 KB |       10.35 |
| ReadLine_ | Cols   | 25000 |  20.082 ms |  4.12 | 20 | 1009.4 |  803.3 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  37.032 ms |  7.60 | 20 |  547.4 | 1481.3 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  32.010 ms |  1.00 | 20 |  633.3 | 1280.4 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.458 ms |  0.30 | 20 | 2143.5 |  378.3 |    69.62 KB |        8.90 |
| Sylvan___ | Floats | 25000 |  88.491 ms |  2.76 | 20 |  229.1 | 3539.6 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 | 101.168 ms |  3.16 | 20 |  200.4 | 4046.7 | 73492.94 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 137.347 ms |  4.29 | 20 |  147.6 | 5493.9 | 22061.31 KB |    2,821.03 |
