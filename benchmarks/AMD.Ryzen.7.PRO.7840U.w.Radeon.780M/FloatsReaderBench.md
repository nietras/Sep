```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-IHPSBG : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-FNCVNM : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Runtime  | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |--------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | .NET 8.0 | Row    | 25000 |   3.378 ms |  1.00 | 20 | 6014.8 |  135.1 |     1.41 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Row    | 25000 |   3.857 ms |  1.14 | 20 | 5268.1 |  154.3 |    10.71 KB |        7.59 |
| ReadLine_ | .NET 8.0 | Row    | 25000 |  17.420 ms |  5.16 | 20 | 1166.4 |  696.8 | 73489.66 KB |   52,078.49 |
| CsvHelper | .NET 8.0 | Row    | 25000 |  40.731 ms | 12.06 | 20 |  498.9 | 1629.2 |    20.19 KB |       14.31 |
| Sep______ | .NET 9.0 | Row    | 25000 |   3.343 ms |  0.99 | 20 | 6078.1 |  133.7 |     1.41 KB |        1.00 |
| Sylvan___ | .NET 9.0 | Row    | 25000 |   3.792 ms |  1.12 | 20 | 5358.1 |  151.7 |    10.71 KB |        7.59 |
| ReadLine_ | .NET 9.0 | Row    | 25000 |  17.225 ms |  5.10 | 20 | 1179.7 |  689.0 | 73489.64 KB |   52,078.47 |
| CsvHelper | .NET 9.0 | Row    | 25000 |  40.113 ms | 11.87 | 20 |  506.6 | 1604.5 |     20.2 KB |       14.31 |
|           |          |        |       |            |       |    |        |        |             |             |
| Sep______ | .NET 8.0 | Cols   | 25000 |   5.091 ms |  1.00 | 20 | 3991.0 |  203.7 |     1.42 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Cols   | 25000 |   6.405 ms |  1.26 | 20 | 3172.5 |  256.2 |    10.71 KB |        7.56 |
| ReadLine_ | .NET 8.0 | Cols   | 25000 |  18.035 ms |  3.54 | 20 | 1126.7 |  721.4 | 73489.68 KB |   51,898.92 |
| CsvHelper | .NET 8.0 | Cols   | 25000 |  44.565 ms |  8.75 | 20 |  456.0 | 1782.6 | 21340.41 KB |   15,070.74 |
| Sep______ | .NET 9.0 | Cols   | 25000 |   4.469 ms |  0.88 | 20 | 4547.1 |  178.7 |     1.42 KB |        1.00 |
| Sylvan___ | .NET 9.0 | Cols   | 25000 |   6.009 ms |  1.18 | 20 | 3381.7 |  240.3 |    10.71 KB |        7.56 |
| ReadLine_ | .NET 9.0 | Cols   | 25000 |  18.784 ms |  3.69 | 20 | 1081.7 |  751.4 | 73489.66 KB |   51,898.91 |
| CsvHelper | .NET 9.0 | Cols   | 25000 |  42.852 ms |  8.42 | 20 |  474.2 | 1714.1 | 21340.25 KB |   15,070.63 |
|           |          |        |       |            |       |    |        |        |             |             |
| Sep______ | .NET 8.0 | Floats | 25000 |  37.354 ms |  1.00 | 20 |  544.0 | 1494.2 |     8.24 KB |        1.00 |
| Sep_MT___ | .NET 8.0 | Floats | 25000 |   9.056 ms |  0.24 | 20 | 2243.7 |  362.2 |   116.31 KB |       14.12 |
| Sylvan___ | .NET 8.0 | Floats | 25000 |  90.382 ms |  2.42 | 20 |  224.8 | 3615.3 |    18.88 KB |        2.29 |
| ReadLine_ | .NET 8.0 | Floats | 25000 | 114.180 ms |  3.06 | 20 |  178.0 | 4567.2 |  73493.3 KB |    8,923.07 |
| CsvHelper | .NET 8.0 | Floats | 25000 | 163.201 ms |  4.37 | 20 |  124.5 | 6528.0 | 22062.31 KB |    2,678.66 |
| Sep______ | .NET 9.0 | Floats | 25000 |  32.165 ms |  0.86 | 20 |  631.7 | 1286.6 |      8.2 KB |        1.00 |
| Sep_MT___ | .NET 9.0 | Floats | 25000 |   8.586 ms |  0.23 | 20 | 2366.6 |  343.4 |   115.19 KB |       13.99 |
| Sylvan___ | .NET 9.0 | Floats | 25000 |  81.121 ms |  2.17 | 20 |  250.5 | 3244.8 |    21.89 KB |        2.66 |
| ReadLine_ | .NET 9.0 | Floats | 25000 | 106.479 ms |  2.85 | 20 |  190.8 | 4259.1 | 73493.12 KB |    8,923.04 |
| CsvHelper | .NET 9.0 | Floats | 25000 | 156.763 ms |  4.20 | 20 |  129.6 | 6270.5 | 22062.08 KB |    2,678.63 |
