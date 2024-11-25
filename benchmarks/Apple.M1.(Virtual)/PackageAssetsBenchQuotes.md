```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  Job-PJJVEM : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

Job=Job-PJJVEM  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.47 ms |  1.00 |  33 | 3178.1 |  209.4 |          1 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.45 ms |  1.00 |  33 | 3184.3 |  209.0 |       1.08 KB |        1.08 |
| Sylvan___    | Row   | 50000   |    22.02 ms |  2.10 |  33 | 1511.4 |  440.4 |       6.79 KB |        6.79 |
| ReadLine_    | Row   | 50000   |    21.93 ms |  2.09 |  33 | 1517.8 |  438.6 |   108778.8 KB |  108,778.80 |
| CsvHelper    | Row   | 50000   |    48.46 ms |  4.63 |  33 |  686.9 |  969.1 |      20.28 KB |       20.28 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.87 ms |  1.00 |  33 | 2805.0 |  237.3 |          1 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.50 ms |  1.05 |  33 | 2663.6 |  249.9 |       1.09 KB |        1.08 |
| Sylvan___    | Cols  | 50000   |    26.11 ms |  2.20 |  33 | 1274.5 |  522.3 |       6.79 KB |        6.77 |
| ReadLine_    | Cols  | 50000   |    22.97 ms |  1.94 |  33 | 1448.7 |  459.5 |  108778.81 KB |  108,355.54 |
| CsvHelper    | Cols  | 50000   |    73.75 ms |  6.22 |  33 |  451.3 | 1475.1 |     446.72 KB |      444.98 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    37.25 ms |  1.00 |  33 |  893.5 |  745.0 |   13802.77 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.24 ms |  0.76 |  33 | 1178.5 |  564.8 |   13887.87 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    56.35 ms |  1.52 |  33 |  590.7 | 1126.9 |   13961.24 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   128.72 ms |  3.46 |  33 |  258.6 | 2574.5 |  122305.87 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    79.64 ms |  2.14 |  33 |  417.9 | 1592.8 |   13971.07 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   790.01 ms |  1.00 | 665 |  842.8 |  790.0 |  266670.24 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   529.49 ms |  0.67 | 665 | 1257.4 |  529.5 |  272111.11 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,202.12 ms |  1.52 | 665 |  553.9 | 1202.1 |  266825.15 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,524.72 ms |  3.20 | 665 |  263.7 | 2524.7 | 2442322.15 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,726.39 ms |  2.19 | 665 |  385.7 | 1726.4 |  266840.23 KB |        1.00 |
