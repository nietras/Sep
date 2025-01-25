```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
  Job-ILBOFO : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD

Job=Job-ILBOFO  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    10.37 ms |  1.00 |  33 | 3210.5 |  207.3 |       1021 B |        1.00 |
| Sep_Async    | Row   | 50000   |    10.41 ms |  1.00 |  33 | 3196.0 |  208.3 |       1019 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.05 ms |  0.97 |  33 | 3311.1 |  201.0 |       1093 B |        1.07 |
| Sylvan___    | Row   | 50000   |    21.19 ms |  2.04 |  33 | 1570.5 |  423.8 |       6958 B |        6.81 |
| ReadLine_    | Row   | 50000   |    21.15 ms |  2.04 |  33 | 1573.8 |  423.0 |  111389487 B |  109,098.42 |
| CsvHelper    | Row   | 50000   |    46.45 ms |  4.48 |  33 |  716.5 |  929.0 |      20764 B |       20.34 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.48 ms |  1.00 |  33 | 2898.0 |  229.7 |       1093 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.12 ms |  1.06 |  33 | 2745.7 |  242.4 |       1102 B |        1.01 |
| Sylvan___    | Cols  | 50000   |    24.83 ms |  2.16 |  33 | 1340.2 |  496.7 |       6958 B |        6.37 |
| ReadLine_    | Cols  | 50000   |    22.39 ms |  1.95 |  33 | 1486.4 |  447.8 |  111389493 B |  101,911.70 |
| CsvHelper    | Cols  | 50000   |    72.54 ms |  6.32 |  33 |  458.8 | 1450.8 |     457440 B |      418.52 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    34.39 ms |  1.00 |  33 |  967.7 |  687.9 |   14135314 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    26.44 ms |  0.77 |  33 | 1258.7 |  528.9 |   14208702 B |        1.01 |
| Sylvan___    | Asset | 50000   |    54.30 ms |  1.58 |  33 |  613.0 | 1086.0 |   14296311 B |        1.01 |
| ReadLine_    | Asset | 50000   |   123.85 ms |  3.60 |  33 |  268.7 | 2477.0 |  125240888 B |        8.86 |
| CsvHelper    | Asset | 50000   |    79.62 ms |  2.32 |  33 |  418.0 | 1592.5 |   14306376 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   780.57 ms |  1.00 | 665 |  853.0 |  780.6 |  273070232 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   497.68 ms |  0.64 | 665 | 1337.8 |  497.7 |  280410952 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,197.89 ms |  1.54 | 665 |  555.8 | 1197.9 |  273228952 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,440.06 ms |  3.13 | 665 |  272.9 | 2440.1 | 2500937688 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,717.85 ms |  2.20 | 665 |  387.6 | 1717.8 |  273241416 B |        1.00 |
