```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    11.09 ms |  1.00 |  33 | 3002.2 |  221.7 |        962 B |        1.00 |
| Sep_Async    | Row   | 50000   |    11.23 ms |  1.01 |  33 | 2963.4 |  224.6 |        962 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    11.08 ms |  1.00 |  33 | 3004.5 |  221.6 |        962 B |        1.00 |
| Sylvan___    | Row   | 50000   |    23.09 ms |  2.08 |  33 | 1441.4 |  461.8 |       7478 B |        7.77 |
| ReadLine_    | Row   | 50000   |    27.46 ms |  2.48 |  33 | 1212.0 |  549.2 |  111389420 B |  115,789.42 |
| CsvHelper    | Row   | 50000   |    62.62 ms |  5.65 |  33 |  531.5 | 1252.5 |      20424 B |       21.23 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.48 ms |  1.00 |  33 | 2667.1 |  249.6 |        962 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.83 ms |  1.11 |  33 | 2406.4 |  276.6 |        962 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.39 ms |  2.20 |  33 | 1215.0 |  547.8 |       7481 B |        7.78 |
| ReadLine_    | Cols  | 50000   |    28.01 ms |  2.24 |  33 | 1188.3 |  560.2 |  111389420 B |  115,789.42 |
| CsvHelper    | Cols  | 50000   |    97.24 ms |  7.79 |  33 |  342.3 | 1944.7 |     456368 B |      474.40 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    40.16 ms |  1.00 |  33 |  828.7 |  803.3 |   14132936 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    15.49 ms |  0.39 |  33 | 2148.9 |  309.8 |   14189860 B |        1.00 |
| Sylvan___    | Asset | 50000   |    59.45 ms |  1.48 |  33 |  559.8 | 1189.0 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   131.84 ms |  3.28 |  33 |  252.4 | 2636.8 |  125239048 B |        8.86 |
| CsvHelper    | Asset | 50000   |   107.60 ms |  2.68 |  33 |  309.3 | 2152.0 |   14305310 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   911.65 ms |  1.00 | 665 |  730.3 |  911.6 |  273063512 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   383.17 ms |  0.42 | 665 | 1737.6 |  383.2 |  278421752 B |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,284.25 ms |  1.41 | 665 |  518.4 | 1284.3 |  273225280 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,462.38 ms |  3.80 | 665 |  192.3 | 3462.4 | 2500939688 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,289.20 ms |  2.51 | 665 |  290.8 | 2289.2 |  273234952 B |        1.00 |
