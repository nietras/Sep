```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-KCKHSP : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-NSODLD : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Runtime  | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |--------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | .NET 8.0 | Asset | 50000   |    43.03 ms |  1.00 |  33 |  775.7 |  860.6 |   13.48 MB |        1.00 |
| Sep_MT___ | .NET 8.0 | Asset | 50000   |    21.83 ms |  0.51 |  33 | 1529.2 |  436.5 |   13.59 MB |        1.01 |
| Sylvan___ | .NET 8.0 | Asset | 50000   |    73.06 ms |  1.70 |  33 |  456.8 | 1461.2 |   13.63 MB |        1.01 |
| ReadLine_ | .NET 8.0 | Asset | 50000   |    65.21 ms |  1.52 |  33 |  511.9 | 1304.2 |  119.44 MB |        8.86 |
| CsvHelper | .NET 8.0 | Asset | 50000   |   136.28 ms |  3.17 |  33 |  244.9 | 2725.5 |   13.64 MB |        1.01 |
| Sep______ | .NET 9.0 | Asset | 50000   |    39.89 ms |  0.93 |  33 |  836.8 |  797.7 |   13.48 MB |        1.00 |
| Sep_MT___ | .NET 9.0 | Asset | 50000   |    21.11 ms |  0.49 |  33 | 1581.5 |  422.1 |   13.59 MB |        1.01 |
| Sylvan___ | .NET 9.0 | Asset | 50000   |    66.27 ms |  1.54 |  33 |  503.6 | 1325.4 |   13.63 MB |        1.01 |
| ReadLine_ | .NET 9.0 | Asset | 50000   |    62.62 ms |  1.46 |  33 |  533.0 | 1252.5 |  119.44 MB |        8.86 |
| CsvHelper | .NET 9.0 | Asset | 50000   |   113.23 ms |  2.63 |  33 |  294.8 | 2264.6 |   13.64 MB |        1.01 |
|           |          |       |         |             |       |     |        |        |            |             |
| Sep______ | .NET 8.0 | Asset | 1000000 |   905.17 ms |  1.00 | 667 |  737.7 |  905.2 |  260.41 MB |        1.00 |
| Sep_MT___ | .NET 8.0 | Asset | 1000000 |   402.94 ms |  0.45 | 667 | 1657.1 |  402.9 |  261.91 MB |        1.01 |
| Sylvan___ | .NET 8.0 | Asset | 1000000 | 1,519.41 ms |  1.68 | 667 |  439.5 | 1519.4 |  260.57 MB |        1.00 |
| ReadLine_ | .NET 8.0 | Asset | 1000000 | 1,176.28 ms |  1.30 | 667 |  567.6 | 1176.3 | 2385.07 MB |        9.16 |
| CsvHelper | .NET 8.0 | Asset | 1000000 | 2,767.08 ms |  3.06 | 667 |  241.3 | 2767.1 |  260.58 MB |        1.00 |
| Sep______ | .NET 9.0 | Asset | 1000000 |   837.23 ms |  0.93 | 667 |  797.5 |  837.2 |  260.41 MB |        1.00 |
| Sep_MT___ | .NET 9.0 | Asset | 1000000 |   392.25 ms |  0.43 | 667 | 1702.3 |  392.2 |  261.36 MB |        1.00 |
| Sylvan___ | .NET 9.0 | Asset | 1000000 | 1,352.10 ms |  1.50 | 667 |  493.8 | 1352.1 |  260.57 MB |        1.00 |
| ReadLine_ | .NET 9.0 | Asset | 1000000 | 1,160.25 ms |  1.28 | 667 |  575.5 | 1160.2 | 2385.07 MB |        9.16 |
| CsvHelper | .NET 9.0 | Asset | 1000000 | 2,336.73 ms |  2.58 | 667 |  285.7 | 2336.7 |  260.58 MB |        1.00 |
