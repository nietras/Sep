```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     3.217 ms |  1.00 |  29 | 9040.7 |   64.3 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.044 ms |  0.95 |  29 | 9556.4 |   60.9 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     2.965 ms |  0.92 |  29 | 9810.6 |   59.3 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    18.065 ms |  5.62 |  29 | 1610.1 |  361.3 |       7476 B |        7.79 |
| ReadLine_    | Row   | 50000   |    17.054 ms |  5.30 |  29 | 1705.5 |  341.1 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    42.576 ms | 13.23 |  29 |  683.1 |  851.5 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     4.460 ms |  1.00 |  29 | 6521.6 |   89.2 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     4.721 ms |  1.06 |  29 | 6161.2 |   94.4 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    21.046 ms |  4.74 |  29 | 1382.0 |  420.9 |       7516 B |        7.83 |
| ReadLine_    | Cols  | 50000   |    18.299 ms |  4.12 |  29 | 1589.5 |  366.0 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    64.478 ms | 14.53 |  29 |  451.1 | 1289.6 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    31.824 ms |  1.00 |  29 |  914.0 |  636.5 |   14133388 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    19.831 ms |  0.62 |  29 | 1466.7 |  396.6 |   14314534 B |        1.01 |
| Sylvan___    | Asset | 50000   |    50.151 ms |  1.58 |  29 |  580.0 | 1003.0 |   14296064 B |        1.01 |
| ReadLine_    | Asset | 50000   |    82.266 ms |  2.59 |  29 |  353.6 | 1645.3 |  104585062 B |        7.40 |
| CsvHelper    | Asset | 50000   |    75.441 ms |  2.38 |  29 |  385.5 | 1508.8 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   564.547 ms |  1.00 | 581 | 1030.7 |  564.5 |  273067136 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   446.241 ms |  0.79 | 581 | 1303.9 |  446.2 |  284946040 B |        1.04 |
| Sylvan___    | Asset | 1000000 |   996.691 ms |  1.77 | 581 |  583.8 |  996.7 |  273230440 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,657.514 ms |  2.94 | 581 |  351.1 | 1657.5 | 2087766888 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,536.513 ms |  2.72 | 581 |  378.7 | 1536.5 |  273236336 B |        1.00 |
