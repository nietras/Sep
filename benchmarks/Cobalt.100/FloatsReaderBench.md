```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3)
Cobalt 100, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-MYYDFG : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.373 ms |  1.00 | 20 | 6023.5 |  134.9 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  24.433 ms |  7.24 | 20 |  831.7 |  977.3 |    10.35 KB |        8.81 |
| ReadLine_ | Row    | 25000 |  22.373 ms |  6.63 | 20 |  908.2 |  894.9 | 73489.65 KB |   62,554.78 |
| CsvHelper | Row    | 25000 |  32.657 ms |  9.68 | 20 |  622.2 | 1306.3 |    19.99 KB |       17.01 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.322 ms |  1.00 | 20 | 4701.2 |  172.9 |     1.18 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  27.118 ms |  6.28 | 20 |  749.3 | 1084.7 |    10.36 KB |        8.79 |
| ReadLine_ | Cols   | 25000 |  22.700 ms |  5.25 | 20 |  895.1 |  908.0 | 73489.62 KB |   62,399.15 |
| CsvHelper | Cols   | 25000 |  35.617 ms |  8.24 | 20 |  570.5 | 1424.7 | 21340.21 KB |   18,119.72 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  30.520 ms |  1.00 | 20 |  665.8 | 1220.8 |     7.94 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   8.394 ms |  0.28 | 20 | 2420.6 |  335.8 |    66.64 KB |        8.39 |
| Sylvan___ | Floats | 25000 |  96.604 ms |  3.17 | 20 |  210.3 | 3864.2 |    18.42 KB |        2.32 |
| ReadLine_ | Floats | 25000 | 104.030 ms |  3.41 | 20 |  195.3 | 4161.2 | 73492.95 KB |    9,254.40 |
| CsvHelper | Floats | 25000 | 146.469 ms |  4.80 | 20 |  138.7 | 5858.7 | 22061.94 KB |    2,778.09 |
