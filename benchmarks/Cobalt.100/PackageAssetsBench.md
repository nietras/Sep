```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-FXNRMG : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-FXNRMG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     4.770 ms |  1.00 |  29 | 6117.7 |   95.4 |        972 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.006 ms |  1.05 |  29 | 5829.1 |  100.1 |        972 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.789 ms |  1.00 |  29 | 6092.9 |   95.8 |        970 B |        1.00 |
| Sylvan___    | Row   | 50000   |    19.965 ms |  4.19 |  29 | 1461.6 |  399.3 |       6656 B |        6.85 |
| ReadLine_    | Row   | 50000   |    24.603 ms |  5.16 |  29 | 1186.1 |  492.1 |   90734858 B |   93,348.62 |
| CsvHelper    | Row   | 50000   |    52.364 ms | 10.98 |  29 |  557.3 | 1047.3 |      20435 B |       21.02 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     6.103 ms |  1.00 |  29 | 4781.5 |  122.1 |        955 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.852 ms |  1.12 |  29 | 4258.9 |  137.0 |        953 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    24.055 ms |  3.94 |  29 | 1213.1 |  481.1 |       6686 B |        7.00 |
| ReadLine_    | Cols  | 50000   |    25.907 ms |  4.25 |  29 | 1126.4 |  518.1 |   90734829 B |   95,010.29 |
| CsvHelper    | Cols  | 50000   |    89.741 ms | 14.70 |  29 |  325.2 | 1794.8 |     456396 B |      477.90 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    45.045 ms |  1.00 |  29 |  647.8 |  900.9 |   14134146 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.571 ms |  0.64 |  29 | 1021.4 |  571.4 |   14198453 B |        1.00 |
| Sylvan___    | Asset | 50000   |    66.218 ms |  1.47 |  29 |  440.7 | 1324.4 |   14296192 B |        1.01 |
| ReadLine_    | Asset | 50000   |   131.307 ms |  2.92 |  29 |  222.2 | 2626.1 |  104585118 B |        7.40 |
| CsvHelper    | Asset | 50000   |   110.400 ms |  2.46 |  29 |  264.3 | 2208.0 |   14305660 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   907.416 ms |  1.00 | 583 |  643.3 |  907.4 |  273069856 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   425.985 ms |  0.47 | 583 | 1370.4 |  426.0 |  281372840 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,374.743 ms |  1.52 | 583 |  424.6 | 1374.7 |  273228480 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,282.225 ms |  2.52 | 583 |  255.8 | 2282.2 | 2087766512 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,247.721 ms |  2.48 | 583 |  259.7 | 2247.7 |  273249784 B |        1.00 |
