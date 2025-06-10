```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-DUYDYG : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-DUYDYG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     3.560 ms |  1.00 |  29 | 8169.3 |   71.2 |        970 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.435 ms |  0.97 |  29 | 8466.6 |   68.7 |       1068 B |        1.10 |
| Sep_Unescape | Row   | 50000   |     3.915 ms |  1.10 |  29 | 7429.4 |   78.3 |        969 B |        1.00 |
| Sylvan___    | Row   | 50000   |    26.841 ms |  7.55 |  29 | 1083.6 |  536.8 |       6738 B |        6.95 |
| ReadLine_    | Row   | 50000   |    28.801 ms |  8.11 |  29 | 1009.9 |  576.0 |   90734906 B |   93,541.14 |
| CsvHelper    | Row   | 50000   |    52.790 ms | 14.86 |  29 |  551.0 | 1055.8 |      20603 B |       21.24 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     5.831 ms |  1.04 |  29 | 4988.4 |  116.6 |       1068 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.140 ms |  1.10 |  29 | 4736.8 |  122.8 |       1097 B |        1.03 |
| Sylvan___    | Cols  | 50000   |    24.513 ms |  4.37 |  29 | 1186.5 |  490.3 |       6958 B |        6.51 |
| ReadLine_    | Cols  | 50000   |    21.727 ms |  3.88 |  29 | 1338.7 |  434.5 |   90734895 B |   84,957.77 |
| CsvHelper    | Cols  | 50000   |    74.460 ms | 13.29 |  29 |  390.6 | 1489.2 |     456564 B |      427.49 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    32.555 ms |  1.00 |  29 |  893.4 |  651.1 |   14135890 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    23.243 ms |  0.72 |  29 | 1251.4 |  464.9 |   14238974 B |        1.01 |
| Sylvan___    | Asset | 50000   |    60.865 ms |  1.88 |  29 |  477.9 | 1217.3 |   14297225 B |        1.01 |
| ReadLine_    | Asset | 50000   |   105.409 ms |  3.25 |  29 |  275.9 | 2108.2 |  104586054 B |        7.40 |
| CsvHelper    | Asset | 50000   |    85.077 ms |  2.62 |  29 |  341.9 | 1701.5 |   14306376 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   613.141 ms |  1.00 | 581 |  949.0 |  613.1 |  273070216 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   496.778 ms |  0.81 | 581 | 1171.3 |  496.8 |  283152440 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,139.885 ms |  1.86 | 581 |  510.5 | 1139.9 |  273230288 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,838.002 ms |  3.00 | 581 |  316.6 | 1838.0 | 2087769408 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,558.047 ms |  2.54 | 581 |  373.5 | 1558.0 |  273242008 B |        1.00 |
