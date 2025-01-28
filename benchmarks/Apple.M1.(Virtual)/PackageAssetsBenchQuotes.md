```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
  Job-KNSVKR : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD

Job=Job-KNSVKR  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    12.18 ms |  1.01 |  33 | 2732.2 |  243.6 |      1.09 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    11.74 ms |  0.97 |  33 | 2835.4 |  234.8 |         1 KB |        0.92 |
| Sep_Unescape | Row   | 50000   |    11.10 ms |  0.92 |  33 | 2997.8 |  222.0 |         1 KB |        0.92 |
| Sylvan___    | Row   | 50000   |    24.97 ms |  2.06 |  33 | 1332.7 |  499.5 |      6.79 KB |        6.25 |
| ReadLine_    | Row   | 50000   |    26.23 ms |  2.17 |  33 | 1269.0 |  524.5 | 108778.81 KB |  100,080.42 |
| CsvHelper    | Row   | 50000   |    49.35 ms |  4.08 |  33 |  674.4 |  986.9 |     20.09 KB |       18.49 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.41 ms |  1.00 |  33 | 2681.1 |  248.3 |      1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    14.75 ms |  1.19 |  33 | 2256.8 |  295.0 |      1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    26.17 ms |  2.11 |  33 | 1271.6 |  523.5 |      6.79 KB |        6.72 |
| ReadLine_    | Cols  | 50000   |    25.07 ms |  2.02 |  33 | 1327.5 |  501.4 |  108778.8 KB |  107,622.70 |
| CsvHelper    | Cols  | 50000   |    78.74 ms |  6.35 |  33 |  422.7 | 1574.8 |    446.72 KB |      441.97 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    39.11 ms |  1.00 |  33 |  851.0 |  782.1 |  13802.77 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    30.33 ms |  0.78 |  33 | 1097.4 |  606.5 |  13876.85 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    56.15 ms |  1.44 |  33 |  592.7 | 1123.1 |  13961.25 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   127.77 ms |  3.28 |  33 |  260.5 | 2555.5 |  122305.8 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    80.19 ms |  2.06 |  33 |  415.1 | 1603.7 |  13971.07 KB |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   794.37 ms |  1.00 | 665 |  838.1 |  794.4 | 266670.09 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   623.62 ms |  0.79 | 665 | 1067.6 |  623.6 | 275579.94 KB |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,218.66 ms |  1.54 | 665 |  546.3 | 1218.7 | 266825.24 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,426.00 ms |  3.06 | 665 |  274.4 | 2426.0 |   2442322 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,183.43 ms |  2.75 | 665 |  304.9 | 2183.4 | 266837.34 KB |        1.00 |
