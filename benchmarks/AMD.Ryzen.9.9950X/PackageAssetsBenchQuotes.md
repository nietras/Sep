```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7184/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     4.186 ms |  1.00 |  33 | 7973.2 |   83.7 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     4.303 ms |  1.03 |  33 | 7756.4 |   86.1 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.170 ms |  1.00 |  33 | 8004.6 |   83.4 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    10.639 ms |  2.54 |  33 | 3137.2 |  212.8 |       8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |     9.464 ms |  2.26 |  33 | 3526.7 |  189.3 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    27.532 ms |  6.58 |  33 | 1212.3 |  550.6 |      19.95 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.125 ms |  1.00 |  33 | 6512.0 |  102.5 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.701 ms |  1.11 |  33 | 5854.7 |  114.0 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    11.930 ms |  2.33 |  33 | 2797.9 |  238.6 |       8.46 KB |        8.33 |
| ReadLine_    | Cols  | 50000   |    10.103 ms |  1.97 |  33 | 3303.7 |  202.1 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |    40.502 ms |  7.90 |  33 |  824.1 |  810.0 |     445.61 KB |      438.75 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    28.119 ms |  1.00 |  33 | 1187.0 |  562.4 |   13802.38 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    16.974 ms |  0.60 |  33 | 1966.4 |  339.5 |   13995.39 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    33.211 ms |  1.18 |  33 | 1005.0 |  664.2 |   13961.94 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    94.445 ms |  3.36 |  33 |  353.4 | 1888.9 |  122304.82 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    51.841 ms |  1.84 |  33 |  643.8 | 1036.8 |   13970.67 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   520.110 ms |  1.00 | 667 | 1283.8 |  520.1 |  266667.34 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   228.902 ms |  0.44 | 667 | 2917.0 |  228.9 |  269331.98 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   715.715 ms |  1.38 | 667 |  932.9 |  715.7 |  266826.29 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,134.339 ms |  4.10 | 667 |  312.8 | 2134.3 | 2442318.89 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,114.437 ms |  2.14 | 667 |  599.1 | 1114.4 |  266832.37 KB |        1.00 |
