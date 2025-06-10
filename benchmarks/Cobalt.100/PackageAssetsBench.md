```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-ZAPULK : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-ZAPULK  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     4.876 ms |  1.00 |  29 | 5984.3 |   97.5 |        972 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.015 ms |  1.03 |  29 | 5819.2 |  100.3 |        967 B |        0.99 |
| Sep_Unescape | Row   | 50000   |     4.906 ms |  1.01 |  29 | 5948.6 |   98.1 |        954 B |        0.98 |
| Sylvan___    | Row   | 50000   |    19.995 ms |  4.10 |  29 | 1459.4 |  399.9 |       6675 B |        6.87 |
| ReadLine_    | Row   | 50000   |    24.854 ms |  5.10 |  29 | 1174.1 |  497.1 |   90734833 B |   93,348.59 |
| CsvHelper    | Row   | 50000   |    52.474 ms | 10.76 |  29 |  556.1 | 1049.5 |      20435 B |       21.02 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     6.104 ms |  1.00 |  29 | 4780.6 |  122.1 |        953 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.819 ms |  1.12 |  29 | 4279.5 |  136.4 |        977 B |        1.03 |
| Sylvan___    | Cols  | 50000   |    24.057 ms |  3.94 |  29 | 1213.0 |  481.1 |       6659 B |        6.99 |
| ReadLine_    | Cols  | 50000   |    25.439 ms |  4.17 |  29 | 1147.1 |  508.8 |   90734855 B |   95,209.71 |
| CsvHelper    | Cols  | 50000   |    89.199 ms | 14.62 |  29 |  327.1 | 1784.0 |     456480 B |      478.99 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    45.363 ms |  1.00 |  29 |  643.3 |  907.3 |   14134318 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.591 ms |  0.63 |  29 | 1020.6 |  571.8 |   14206591 B |        1.01 |
| Sylvan___    | Asset | 50000   |    67.094 ms |  1.48 |  29 |  434.9 | 1341.9 |   14296766 B |        1.01 |
| ReadLine_    | Asset | 50000   |   123.098 ms |  2.72 |  29 |  237.1 | 2462.0 |  104585284 B |        7.40 |
| CsvHelper    | Asset | 50000   |   110.509 ms |  2.44 |  29 |  264.1 | 2210.2 |   14307050 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   898.580 ms |  1.00 | 583 |  649.7 |  898.6 |  273069616 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   431.867 ms |  0.48 | 583 | 1351.8 |  431.9 |  281670432 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,389.197 ms |  1.55 | 583 |  420.2 | 1389.2 |  273227800 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,288.710 ms |  2.55 | 583 |  255.1 | 2288.7 | 2087767112 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,268.402 ms |  2.52 | 583 |  257.4 | 2268.4 |  273237304 B |        1.00 |
