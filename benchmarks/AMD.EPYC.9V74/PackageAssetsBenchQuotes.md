```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.174 ms |  1.00 |  33 | 3280.8 |  203.5 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    10.668 ms |  1.05 |  33 | 3128.8 |  213.4 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     9.694 ms |  0.95 |  33 | 3443.2 |  193.9 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    24.856 ms |  2.44 |  33 | 1342.8 |  497.1 |       8.47 KB |        8.27 |
| ReadLine_    | Row   | 50000   |    20.852 ms |  2.05 |  33 | 1600.7 |  417.0 |  108778.73 KB |  106,287.61 |
| CsvHelper    | Row   | 50000   |    72.832 ms |  7.16 |  33 |  458.3 | 1456.6 |      20.02 KB |       19.56 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.462 ms |  1.00 |  33 | 2912.1 |  229.2 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    24.650 ms |  2.15 |  33 | 1354.1 |  493.0 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    36.800 ms |  3.21 |  33 |  907.0 |  736.0 |       8.48 KB |        8.29 |
| ReadLine_    | Cols  | 50000   |    22.585 ms |  1.97 |  33 | 1477.9 |  451.7 |  108778.73 KB |  106,287.61 |
| CsvHelper    | Cols  | 50000   |   105.715 ms |  9.22 |  33 |  315.7 | 2114.3 |      445.6 KB |      435.40 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    53.283 ms |  1.00 |  33 |  626.4 | 1065.7 |   13802.25 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    36.475 ms |  0.69 |  33 |  915.1 |  729.5 |   13860.76 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    75.539 ms |  1.42 |  33 |  441.9 | 1510.8 |   13961.95 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   187.253 ms |  3.53 |  33 |  178.2 | 3745.1 |   122304.4 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   129.524 ms |  2.44 |  33 |  257.7 | 2590.5 |   13971.99 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,121.435 ms |  1.00 | 667 |  595.4 | 1121.4 |  266667.52 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   690.891 ms |  0.62 | 667 |  966.4 |  690.9 |  271683.52 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,585.245 ms |  1.41 | 667 |  421.2 | 1585.2 |  266824.52 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,209.745 ms |  3.75 | 667 |  158.6 | 4209.7 | 2442318.26 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,551.783 ms |  2.28 | 667 |  261.7 | 2551.8 |  266832.38 KB |        1.00 |
