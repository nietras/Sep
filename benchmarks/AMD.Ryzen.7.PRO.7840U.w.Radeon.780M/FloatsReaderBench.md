```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4602/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-VCJIGY : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.415 ms |  1.00 | 20 | 5949.8 |  136.6 |     1.41 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.803 ms |  1.11 | 20 | 5343.6 |  152.1 |    10.71 KB |        7.59 |
| ReadLine_ | Row    | 25000 |  15.853 ms |  4.64 | 20 | 1281.8 |  634.1 | 73489.64 KB |   52,078.47 |
| CsvHelper | Row    | 25000 |  39.778 ms | 11.65 | 20 |  510.8 | 1591.1 |    20.03 KB |       14.19 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.470 ms |  1.00 | 20 | 4546.3 |  178.8 |     1.42 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   5.999 ms |  1.34 | 20 | 3387.4 |  239.9 |    10.71 KB |        7.54 |
| ReadLine_ | Cols   | 25000 |  17.779 ms |  3.98 | 20 | 1142.9 |  711.2 | 73489.66 KB |   51,756.13 |
| CsvHelper | Cols   | 25000 |  43.374 ms |  9.70 | 20 |  468.5 | 1735.0 | 21340.41 KB |   15,029.29 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  32.146 ms |  1.00 | 20 |  632.1 | 1285.8 |      8.2 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   6.082 ms |  0.19 | 20 | 3340.7 |  243.3 |   115.72 KB |       14.11 |
| Sylvan___ | Floats | 25000 |  81.398 ms |  2.53 | 20 |  249.6 | 3255.9 |    18.88 KB |        2.30 |
| ReadLine_ | Floats | 25000 | 107.332 ms |  3.34 | 20 |  189.3 | 4293.3 | 73493.12 KB |    8,960.23 |
| CsvHelper | Floats | 25000 | 157.689 ms |  4.91 | 20 |  128.9 | 6307.6 | 22062.72 KB |    2,689.87 |
