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
| Sep______    | Row   | 50000   |    10.98 ms |  1.00 |  33 | 3031.8 |  219.6 |        962 B |        1.00 |
| Sep_Async    | Row   | 50000   |    11.27 ms |  1.03 |  33 | 2951.9 |  225.5 |        962 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    11.06 ms |  1.01 |  33 | 3008.4 |  221.3 |        962 B |        1.00 |
| Sylvan___    | Row   | 50000   |    23.06 ms |  2.10 |  33 | 1443.1 |  461.2 |       7478 B |        7.77 |
| ReadLine_    | Row   | 50000   |    27.38 ms |  2.49 |  33 | 1215.4 |  547.7 |  111389420 B |  115,789.42 |
| CsvHelper    | Row   | 50000   |    63.44 ms |  5.78 |  33 |  524.6 | 1268.8 |      20502 B |       21.31 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.48 ms |  1.00 |  33 | 2666.8 |  249.6 |        962 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.71 ms |  1.10 |  33 | 2427.5 |  274.2 |        962 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.34 ms |  2.19 |  33 | 1217.3 |  546.8 |       7481 B |        7.78 |
| ReadLine_    | Cols  | 50000   |    28.03 ms |  2.25 |  33 | 1187.2 |  560.7 |  111389420 B |  115,789.42 |
| CsvHelper    | Cols  | 50000   |    95.45 ms |  7.65 |  33 |  348.7 | 1909.0 |     456374 B |      474.40 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    40.63 ms |  1.00 |  33 |  819.1 |  812.7 |   14132936 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    15.97 ms |  0.39 |  33 | 2084.4 |  319.3 |   14191158 B |        1.00 |
| Sylvan___    | Asset | 50000   |    59.52 ms |  1.46 |  33 |  559.2 | 1190.3 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   131.93 ms |  3.25 |  33 |  252.3 | 2638.5 |  125239048 B |        8.86 |
| CsvHelper    | Asset | 50000   |   107.05 ms |  2.63 |  33 |  310.9 | 2140.9 |   14305310 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   906.66 ms |  1.00 | 665 |  734.3 |  906.7 |  273063480 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   385.54 ms |  0.43 | 665 | 1726.9 |  385.5 |  275346288 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,286.77 ms |  1.42 | 665 |  517.4 | 1286.8 |  273225288 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,452.59 ms |  3.81 | 665 |  192.8 | 3452.6 | 2500939752 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,272.76 ms |  2.51 | 665 |  292.9 | 2272.8 |  273234968 B |        1.00 |
