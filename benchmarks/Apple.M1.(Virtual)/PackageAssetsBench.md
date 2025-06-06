```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-SMYKWG : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-SMYKWG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     3.011 ms |  1.00 |  29 | 9661.0 |   60.2 |        967 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.085 ms |  1.02 |  29 | 9427.7 |   61.7 |        969 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.435 ms |  1.14 |  29 | 8468.1 |   68.7 |       1068 B |        1.10 |
| Sylvan___    | Row   | 50000   |    19.798 ms |  6.58 |  29 | 1469.1 |  396.0 |       6958 B |        7.20 |
| ReadLine_    | Row   | 50000   |    18.895 ms |  6.28 |  29 | 1539.4 |  377.9 |   90734887 B |   93,831.32 |
| CsvHelper    | Row   | 50000   |    43.221 ms | 14.36 |  29 |  673.0 |  864.4 |      20764 B |       21.47 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     4.045 ms |  1.00 |  29 | 7191.1 |   80.9 |       1061 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     4.835 ms |  1.20 |  29 | 6015.1 |   96.7 |        975 B |        0.92 |
| Sylvan___    | Cols  | 50000   |    23.834 ms |  5.89 |  29 | 1220.4 |  476.7 |       6958 B |        6.56 |
| ReadLine_    | Cols  | 50000   |    21.010 ms |  5.19 |  29 | 1384.4 |  420.2 |   90734891 B |   85,518.28 |
| CsvHelper    | Cols  | 50000   |    67.013 ms | 16.57 |  29 |  434.0 | 1340.3 |     456636 B |      430.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    30.372 ms |  1.00 |  29 |  957.7 |  607.4 |   14134038 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    19.616 ms |  0.65 |  29 | 1482.8 |  392.3 |   14278125 B |        1.01 |
| Sylvan___    | Asset | 50000   |    52.767 ms |  1.74 |  29 |  551.2 | 1055.3 |   14297474 B |        1.01 |
| ReadLine_    | Asset | 50000   |    87.433 ms |  2.89 |  29 |  332.7 | 1748.7 |  104585878 B |        7.40 |
| CsvHelper    | Asset | 50000   |    79.676 ms |  2.63 |  29 |  365.1 | 1593.5 |   14306680 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   564.906 ms |  1.00 | 581 | 1030.0 |  564.9 |  273070120 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   434.061 ms |  0.77 | 581 | 1340.5 |  434.1 |  283609224 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,091.097 ms |  1.93 | 581 |  533.3 | 1091.1 |  273228936 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,806.499 ms |  3.20 | 581 |  322.1 | 1806.5 | 2087769408 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,598.667 ms |  2.83 | 581 |  364.0 | 1598.7 |  273245200 B |        1.00 |
