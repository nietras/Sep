```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 11.44 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    10.14 ms |  1.00 |  33 | 3280.8 |  202.9 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |    10.19 ms |  1.00 |  33 | 3265.4 |  203.8 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.52 ms |  1.04 |  33 | 3165.0 |  210.3 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.64 ms |  2.13 |  33 | 1538.1 |  432.8 |       7477 B |        7.79 |
| ReadLine_    | Row   | 50000   |    27.23 ms |  2.68 |  33 | 1222.1 |  544.7 |  111389416 B |  116,030.64 |
| CsvHelper    | Row   | 50000   |    64.74 ms |  6.38 |  33 |  514.1 | 1294.7 |      20424 B |       21.27 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.57 ms |  1.00 |  33 | 2875.5 |  231.5 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.85 ms |  1.11 |  33 | 2589.3 |  257.1 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.68 ms |  2.22 |  33 | 1296.1 |  513.6 |       7479 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    27.44 ms |  2.37 |  33 | 1213.0 |  548.8 |  111389416 B |  116,030.64 |
| CsvHelper    | Cols  | 50000   |    94.24 ms |  8.14 |  33 |  353.2 | 1884.7 |     456296 B |      475.31 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    39.94 ms |  1.00 |  33 |  833.3 |  798.8 |   14132928 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.67 ms |  0.54 |  33 | 1536.0 |  433.4 |   14217741 B |        1.01 |
| Sylvan___    | Asset | 50000   |    58.59 ms |  1.47 |  33 |  568.0 | 1171.8 |   14295507 B |        1.01 |
| ReadLine_    | Asset | 50000   |   128.46 ms |  3.22 |  33 |  259.1 | 2569.2 |  125238840 B |        8.86 |
| CsvHelper    | Asset | 50000   |   106.38 ms |  2.66 |  33 |  312.9 | 2127.6 |   14305232 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   879.89 ms |  1.00 | 665 |  756.7 |  879.9 |  273063736 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   382.07 ms |  0.43 | 665 | 1742.6 |  382.1 |  276224424 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,237.80 ms |  1.41 | 665 |  537.9 | 1237.8 |  273225488 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,343.44 ms |  3.80 | 665 |  199.1 | 3343.4 | 2500933624 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,272.54 ms |  2.58 | 665 |  293.0 | 2272.5 |  273235096 B |        1.00 |
