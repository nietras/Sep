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
| Sep______    | Row   | 50000   |     3.102 ms |  1.00 |  29 | 9375.9 |   62.0 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.212 ms |  1.04 |  29 | 9054.6 |   64.2 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.113 ms |  1.00 |  29 | 9343.6 |   62.3 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    18.845 ms |  6.08 |  29 | 1543.5 |  376.9 |       7516 B |        7.83 |
| ReadLine_    | Row   | 50000   |    17.645 ms |  5.69 |  29 | 1648.4 |  352.9 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    44.373 ms | 14.31 |  29 |  655.5 |  887.5 |      20496 B |       21.35 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     4.291 ms |  1.00 |  29 | 6779.0 |   85.8 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     4.765 ms |  1.11 |  29 | 6103.7 |   95.3 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    22.035 ms |  5.14 |  29 | 1320.0 |  440.7 |       7516 B |        7.83 |
| ReadLine_    | Cols  | 50000   |    18.148 ms |  4.23 |  29 | 1602.7 |  363.0 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    66.704 ms | 15.55 |  29 |  436.0 | 1334.1 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    32.160 ms |  1.00 |  29 |  904.4 |  643.2 |   14133390 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    20.284 ms |  0.63 |  29 | 1433.9 |  405.7 |   14345142 B |        1.01 |
| Sylvan___    | Asset | 50000   |    52.717 ms |  1.64 |  29 |  551.7 | 1054.3 |   14297432 B |        1.01 |
| ReadLine_    | Asset | 50000   |    84.203 ms |  2.62 |  29 |  345.4 | 1684.1 |  104585082 B |        7.40 |
| CsvHelper    | Asset | 50000   |    79.944 ms |  2.49 |  29 |  363.8 | 1598.9 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   601.259 ms |  1.00 | 581 |  967.8 |  601.3 |  273067136 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   422.191 ms |  0.70 | 581 | 1378.2 |  422.2 |  283144376 B |        1.04 |
| Sylvan___    | Asset | 1000000 |   998.207 ms |  1.66 | 581 |  582.9 |  998.2 |  273226784 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,610.962 ms |  2.68 | 581 |  361.2 | 1611.0 | 2087766960 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,523.898 ms |  2.54 | 581 |  381.8 | 1523.9 |  273244592 B |        1.00 |
