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
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     9.857 ms |  1.00 |  33 | 3376.6 |  197.1 |        961 B |        1.00 |
| Sep_Async    | Row   | 50000   |    10.169 ms |  1.03 |  33 | 3272.9 |  203.4 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     9.927 ms |  1.01 |  33 | 3352.7 |  198.5 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    22.369 ms |  2.27 |  33 | 1487.9 |  447.4 |       7478 B |        7.78 |
| ReadLine_    | Row   | 50000   |    26.946 ms |  2.73 |  33 | 1235.1 |  538.9 |  111389420 B |  115,909.91 |
| CsvHelper    | Row   | 50000   |    63.902 ms |  6.48 |  33 |  520.8 | 1278.0 |      20424 B |       21.25 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.551 ms |  1.00 |  33 | 2881.4 |  231.0 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.086 ms |  1.13 |  33 | 2543.3 |  261.7 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.220 ms |  2.36 |  33 | 1222.7 |  544.4 |       7481 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    27.442 ms |  2.38 |  33 | 1212.8 |  548.8 |  111389420 B |  116,030.65 |
| CsvHelper    | Cols  | 50000   |    96.352 ms |  8.34 |  33 |  345.4 | 1927.0 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    40.159 ms |  1.00 |  33 |  828.8 |  803.2 |   14132936 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    15.246 ms |  0.38 |  33 | 2182.9 |  304.9 |   14191678 B |        1.00 |
| Sylvan___    | Asset | 50000   |    59.348 ms |  1.48 |  33 |  560.8 | 1187.0 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   115.805 ms |  2.88 |  33 |  287.4 | 2316.1 |  125239048 B |        8.86 |
| CsvHelper    | Asset | 50000   |   107.592 ms |  2.68 |  33 |  309.3 | 2151.8 |   14305310 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   899.778 ms |  1.00 | 665 |  740.0 |  899.8 |  273063464 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   368.766 ms |  0.41 | 665 | 1805.5 |  368.8 |  278161952 B |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,280.963 ms |  1.42 | 665 |  519.8 | 1281.0 |  273225352 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,154.204 ms |  3.51 | 665 |  211.1 | 3154.2 | 2500931832 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,388.303 ms |  2.65 | 665 |  278.8 | 2388.3 |  273234896 B |        1.00 |
