```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-BFPPER : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-BFPPER  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    10.57 ms |  1.00 |  33 | 3147.7 |  211.5 |       1.06 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    10.56 ms |  1.00 |  33 | 3152.0 |  211.2 |       1.06 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.44 ms |  0.99 |  33 | 3188.5 |  208.8 |       1.06 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    23.18 ms |  2.19 |  33 | 1435.6 |  463.7 |       7.73 KB |        7.29 |
| ReadLine_    | Row   | 50000   |    26.19 ms |  2.48 |  33 | 1270.8 |  523.8 |   108778.8 KB |  102,568.59 |
| CsvHelper    | Row   | 50000   |    77.54 ms |  7.33 |  33 |  429.2 | 1550.8 |      20.28 KB |       19.12 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    11.85 ms |  1.00 |  33 | 2808.1 |  237.0 |       1.07 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.74 ms |  1.16 |  33 | 2422.9 |  274.7 |       1.08 KB |        1.01 |
| Sylvan___    | Cols  | 50000   |    27.56 ms |  2.33 |  33 | 1207.5 |  551.2 |       7.75 KB |        7.26 |
| ReadLine_    | Cols  | 50000   |    28.25 ms |  2.38 |  33 | 1178.1 |  565.0 |  108778.81 KB |  101,818.56 |
| CsvHelper    | Cols  | 50000   |   106.81 ms |  9.01 |  33 |  311.6 | 2136.1 |     445.94 KB |      417.41 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    47.05 ms |  1.00 |  33 |  707.4 |  940.9 |   13802.84 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    30.50 ms |  0.65 |  33 | 1091.2 |  610.0 |   13860.99 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    69.36 ms |  1.47 |  33 |  479.9 | 1387.2 |   13962.38 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   148.40 ms |  3.16 |  33 |  224.3 | 2968.0 |  122305.51 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   126.52 ms |  2.69 |  33 |  263.1 | 2530.3 |    13971.3 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   990.05 ms |  1.00 | 665 |  672.5 |  990.0 |  266670.43 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   573.36 ms |  0.58 | 665 | 1161.2 |  573.4 |  271926.59 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,426.57 ms |  1.44 | 665 |  466.7 | 1426.6 |  266828.59 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,090.02 ms |  3.12 | 665 |  215.5 | 3090.0 | 2442321.23 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,609.20 ms |  2.64 | 665 |  255.2 | 2609.2 |  266834.35 KB |        1.00 |
