```

BenchmarkDotNet v0.16.0-preview.1, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
Memory: 7 GB Total, 0.09 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     8.844 ms |  1.00 |  33 | 3763.2 |  176.9 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.376 ms |  0.84 |  33 | 4512.1 |  147.5 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     9.351 ms |  1.07 |  33 | 3559.1 |  187.0 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    28.051 ms |  3.21 |  33 | 1186.5 |  561.0 |       7483 B |        7.79 |
| ReadLine_    | Row   | 50000   |    31.502 ms |  3.60 |  33 | 1056.5 |  630.0 |  111389416 B |  116,030.64 |
| CsvHelper    | Row   | 50000   |    54.088 ms |  6.18 |  33 |  615.3 | 1081.8 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     9.632 ms |  1.00 |  33 | 3455.5 |  192.6 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     9.976 ms |  1.04 |  33 | 3336.3 |  199.5 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.266 ms |  2.63 |  33 | 1317.3 |  505.3 |       7480 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    25.489 ms |  2.66 |  33 | 1305.7 |  509.8 |  111389416 B |  116,030.64 |
| CsvHelper    | Cols  | 50000   |    75.225 ms |  7.84 |  33 |  442.4 | 1504.5 |     456296 B |      475.31 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    45.244 ms |  1.00 |  33 |  735.6 |  904.9 |   14133833 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    33.797 ms |  0.75 |  33 |  984.8 |  675.9 |   14247660 B |        1.01 |
| Sylvan___    | Asset | 50000   |    60.534 ms |  1.34 |  33 |  549.8 | 1210.7 |   14296224 B |        1.01 |
| ReadLine_    | Asset | 50000   |   137.611 ms |  3.05 |  33 |  241.9 | 2752.2 |  125241932 B |        8.86 |
| CsvHelper    | Asset | 50000   |    86.078 ms |  1.91 |  33 |  386.6 | 1721.6 |   14305738 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   758.353 ms |  1.00 | 665 |  878.0 |  758.4 |  273070008 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   467.506 ms |  0.62 | 665 | 1424.2 |  467.5 |  283157344 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,161.851 ms |  1.53 | 665 |  573.1 | 1161.9 |  273228864 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,747.637 ms |  3.63 | 665 |  242.3 | 2747.6 | 2500936016 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,180.223 ms |  2.88 | 665 |  305.4 | 2180.2 |  273241968 B |        1.00 |
