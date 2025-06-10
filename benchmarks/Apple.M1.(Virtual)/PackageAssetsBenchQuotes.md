```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-DUYDYG : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-DUYDYG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     7.713 ms |  1.00 |  33 | 4315.0 |  154.3 |       1170 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.659 ms |  0.99 |  33 | 4345.7 |  153.2 |        991 B |        0.85 |
| Sep_Unescape | Row   | 50000   |     8.234 ms |  1.07 |  33 | 4042.2 |  164.7 |       1170 B |        1.00 |
| Sylvan___    | Row   | 50000   |    19.799 ms |  2.57 |  33 | 1681.0 |  396.0 |       6958 B |        5.95 |
| ReadLine_    | Row   | 50000   |    20.916 ms |  2.71 |  33 | 1591.2 |  418.3 |  111389487 B |   95,204.69 |
| CsvHelper    | Row   | 50000   |    46.911 ms |  6.08 |  33 |  709.5 |  938.2 |      20764 B |       17.75 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     8.754 ms |  1.00 |  33 | 3802.0 |  175.1 |       1170 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     9.653 ms |  1.10 |  33 | 3447.9 |  193.1 |       1170 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.279 ms |  2.66 |  33 | 1429.7 |  465.6 |       6958 B |        5.95 |
| ReadLine_    | Cols  | 50000   |    22.264 ms |  2.54 |  33 | 1494.9 |  445.3 |  111389493 B |   95,204.69 |
| CsvHelper    | Cols  | 50000   |    72.093 ms |  8.24 |  33 |  461.7 | 1441.9 |     456636 B |      390.29 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    35.684 ms |  1.00 |  33 |  932.7 |  713.7 |   14134022 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.646 ms |  0.61 |  33 | 1537.5 |  432.9 |   14243836 B |        1.01 |
| Sylvan___    | Asset | 50000   |    53.985 ms |  1.52 |  33 |  616.5 | 1079.7 |   14296340 B |        1.01 |
| ReadLine_    | Asset | 50000   |   124.514 ms |  3.51 |  33 |  267.3 | 2490.3 |  125240818 B |        8.86 |
| CsvHelper    | Asset | 50000   |    83.992 ms |  2.36 |  33 |  396.3 | 1679.8 |   14308942 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   744.707 ms |  1.00 | 665 |  894.0 |  744.7 |  273070152 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   522.435 ms |  0.70 | 665 | 1274.4 |  522.4 |  284369952 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,304.594 ms |  1.75 | 665 |  510.3 | 1304.6 |  273229072 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,756.012 ms |  3.70 | 665 |  241.6 | 2756.0 | 2500938080 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,745.951 ms |  2.34 | 665 |  381.3 | 1746.0 |  273241360 B |        1.00 |
