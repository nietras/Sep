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
| Sep______    | Row   | 50000   |    10.54 ms |  1.00 |  33 | 3157.7 |  210.8 |        968 B |        1.00 |
| Sep_Async    | Row   | 50000   |    10.95 ms |  1.04 |  33 | 3038.6 |  219.1 |        968 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.81 ms |  1.03 |  33 | 3077.7 |  216.3 |        968 B |        1.00 |
| Sylvan___    | Row   | 50000   |    23.05 ms |  2.19 |  33 | 1443.9 |  461.0 |       7478 B |        7.73 |
| ReadLine_    | Row   | 50000   |    27.13 ms |  2.58 |  33 | 1226.6 |  542.7 |  111389420 B |  115,071.71 |
| CsvHelper    | Row   | 50000   |    63.67 ms |  6.04 |  33 |  522.7 | 1273.4 |      20496 B |       21.17 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.33 ms |  1.00 |  33 | 2698.6 |  246.7 |        970 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.46 ms |  1.09 |  33 | 2473.3 |  269.1 |        969 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.46 ms |  2.23 |  33 | 1212.1 |  549.1 |       7481 B |        7.71 |
| ReadLine_    | Cols  | 50000   |    27.59 ms |  2.24 |  33 | 1206.4 |  551.8 |  111389420 B |  114,834.45 |
| CsvHelper    | Cols  | 50000   |    96.77 ms |  7.85 |  33 |  343.9 | 1935.5 |     456368 B |      470.48 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    38.63 ms |  1.00 |  33 |  861.6 |  772.6 |   14133020 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    15.11 ms |  0.39 |  33 | 2202.7 |  302.2 |   14192232 B |        1.00 |
| Sylvan___    | Asset | 50000   |    59.82 ms |  1.55 |  33 |  556.4 | 1196.4 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   119.28 ms |  3.09 |  33 |  279.0 | 2385.6 |  125239048 B |        8.86 |
| CsvHelper    | Asset | 50000   |   107.05 ms |  2.77 |  33 |  310.9 | 2140.9 |   14305310 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   900.75 ms |  1.00 | 665 |  739.2 |  900.7 |  273063640 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   372.76 ms |  0.41 | 665 | 1786.2 |  372.8 |  279234728 B |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,283.37 ms |  1.42 | 665 |  518.8 | 1283.4 |  273225128 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,258.23 ms |  3.62 | 665 |  204.3 | 3258.2 | 2500931656 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,256.72 ms |  2.51 | 665 |  295.0 | 2256.7 |  273234944 B |        1.00 |
