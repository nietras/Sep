```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 9950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-RXSQJG : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-RXSQJG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     4.233 ms |  1.00 |  33 | 7884.4 |   84.7 |       1.03 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     4.577 ms |  1.08 |  33 | 7291.9 |   91.5 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.435 ms |  1.05 |  33 | 7526.1 |   88.7 |       1.03 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    11.399 ms |  2.69 |  33 | 2928.1 |  228.0 |       7.67 KB |        7.48 |
| ReadLine_    | Row   | 50000   |     9.827 ms |  2.32 |  33 | 3396.6 |  196.5 |  108778.75 KB |  106,085.18 |
| CsvHelper    | Row   | 50000   |    27.453 ms |  6.48 |  33 | 1215.8 |  549.1 |         20 KB |       19.51 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.995 ms |  1.00 |  33 | 6681.9 |   99.9 |       1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.515 ms |  1.10 |  33 | 6052.4 |  110.3 |       1.03 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    12.760 ms |  2.55 |  33 | 2615.9 |  255.2 |       7.67 KB |        7.46 |
| ReadLine_    | Cols  | 50000   |    10.853 ms |  2.17 |  33 | 3075.3 |  217.1 |  108778.74 KB |  105,782.94 |
| CsvHelper    | Cols  | 50000   |    41.586 ms |  8.33 |  33 |  802.6 |  831.7 |      445.7 KB |      433.42 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    28.426 ms |  1.00 |  33 | 1174.2 |  568.5 |   13802.71 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    20.467 ms |  0.72 |  33 | 1630.8 |  409.3 |   13993.59 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    38.589 ms |  1.36 |  33 |  864.9 |  771.8 |   13962.21 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   100.029 ms |  3.52 |  33 |  333.7 | 2000.6 |  122305.28 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    52.035 ms |  1.83 |  33 |  641.4 | 1040.7 |   13971.41 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   521.217 ms |  1.00 | 667 | 1281.1 |  521.2 |  266668.73 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   270.971 ms |  0.52 | 667 | 2464.1 |  271.0 |   268699.4 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   776.453 ms |  1.49 | 667 |  859.9 |  776.5 |  266824.44 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,850.722 ms |  3.55 | 667 |  360.8 | 1850.7 | 2442318.88 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,117.789 ms |  2.14 | 667 |  597.3 | 1117.8 |  266833.25 KB |        1.00 |
