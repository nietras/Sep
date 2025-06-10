```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-ZAPULK : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-ZAPULK  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     9.787 ms |  1.00 |  33 | 3410.2 |  195.7 |        954 B |        1.00 |
| Sep_Async    | Row   | 50000   |    10.038 ms |  1.03 |  33 | 3325.0 |  200.8 |        985 B |        1.03 |
| Sep_Unescape | Row   | 50000   |    10.089 ms |  1.03 |  33 | 3308.3 |  201.8 |        956 B |        1.00 |
| Sylvan___    | Row   | 50000   |    23.339 ms |  2.38 |  33 | 1430.1 |  466.8 |       6658 B |        6.98 |
| ReadLine_    | Row   | 50000   |    30.144 ms |  3.08 |  33 | 1107.3 |  602.9 |  111389422 B |  116,760.40 |
| CsvHelper    | Row   | 50000   |    63.437 ms |  6.48 |  33 |  526.1 | 1268.7 |      20435 B |       21.42 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    10.978 ms |  1.00 |  33 | 3040.3 |  219.6 |        954 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.733 ms |  1.16 |  33 | 2621.3 |  254.7 |        955 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.362 ms |  2.49 |  33 | 1219.9 |  547.2 |       6694 B |        7.02 |
| ReadLine_    | Cols  | 50000   |    31.368 ms |  2.86 |  33 | 1064.1 |  627.4 |  111389422 B |  116,760.40 |
| CsvHelper    | Cols  | 50000   |    97.797 ms |  8.91 |  33 |  341.3 | 1955.9 |     456396 B |      478.40 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    49.459 ms |  1.00 |  33 |  674.9 |  989.2 |   14133872 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.680 ms |  0.58 |  33 | 1163.8 |  573.6 |   14194524 B |        1.00 |
| Sylvan___    | Asset | 50000   |    70.217 ms |  1.42 |  33 |  475.3 | 1404.3 |   14296230 B |        1.01 |
| ReadLine_    | Asset | 50000   |   168.258 ms |  3.41 |  33 |  198.4 | 3365.2 |  125240128 B |        8.86 |
| CsvHelper    | Asset | 50000   |   116.827 ms |  2.37 |  33 |  285.7 | 2336.5 |   14305678 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 | 1,025.138 ms |  1.00 | 667 |  651.3 | 1025.1 |  273070392 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   469.321 ms |  0.46 | 667 | 1422.7 |  469.3 |  278808056 B |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,447.437 ms |  1.41 | 667 |  461.3 | 1447.4 |  273242192 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,930.860 ms |  2.86 | 667 |  227.8 | 2930.9 | 2500934248 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,362.454 ms |  2.30 | 667 |  282.6 | 2362.5 |  273240528 B |        1.00 |
