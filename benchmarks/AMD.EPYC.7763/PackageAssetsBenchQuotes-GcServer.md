```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-YYWBKJ : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-YYWBKJ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.52 ms |  1.00 |  33 |  842.1 |  790.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    21.87 ms |  0.55 |  33 | 1521.7 |  437.4 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    66.35 ms |  1.68 |  33 |  501.6 | 1327.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    64.69 ms |  1.64 |  33 |  514.5 | 1293.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   118.97 ms |  3.01 |  33 |  279.8 | 2379.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   845.73 ms |  1.00 | 665 |  787.3 |  845.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   432.46 ms |  0.51 | 665 | 1539.6 |  432.5 |  262.71 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,343.61 ms |  1.59 | 665 |  495.5 | 1343.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,369.35 ms |  1.62 | 665 |  486.2 | 1369.3 | 2385.08 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,406.94 ms |  2.85 | 665 |  276.6 | 2406.9 |  260.58 MB |        1.00 |
