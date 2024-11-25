```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  Job-HKRCZO : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

Job=Job-HKRCZO  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    10.03 ms |  1.00 |  33 | 3318.2 |  200.6 |       1018 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.06 ms |  1.00 |  33 | 3309.9 |  201.1 |       1093 B |        1.07 |
| Sylvan___    | Row   | 50000   |    21.10 ms |  2.10 |  33 | 1577.5 |  422.0 |       6958 B |        6.83 |
| ReadLine_    | Row   | 50000   |    20.90 ms |  2.08 |  33 | 1592.2 |  418.1 |  111389487 B |  109,419.93 |
| CsvHelper    | Row   | 50000   |    46.54 ms |  4.64 |  33 |  715.1 |  930.8 |      20764 B |       20.40 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.19 ms |  1.00 |  33 | 2973.6 |  223.8 |       1026 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.09 ms |  1.08 |  33 | 2752.0 |  241.9 |       1127 B |        1.10 |
| Sylvan___    | Cols  | 50000   |    24.91 ms |  2.23 |  33 | 1336.1 |  498.2 |       6958 B |        6.78 |
| ReadLine_    | Cols  | 50000   |    21.52 ms |  1.92 |  33 | 1546.3 |  430.5 |  111389493 B |  108,566.76 |
| CsvHelper    | Cols  | 50000   |    71.19 ms |  6.36 |  33 |  467.5 | 1423.8 |     459732 B |      448.08 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    34.77 ms |  1.00 |  33 |  957.1 |  695.4 |   14134032 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    26.38 ms |  0.76 |  33 | 1261.6 |  527.6 |   14200790 B |        1.00 |
| Sylvan___    | Asset | 50000   |    55.33 ms |  1.59 |  33 |  601.5 | 1106.6 |   14297352 B |        1.01 |
| ReadLine_    | Asset | 50000   |   129.30 ms |  3.72 |  33 |  257.4 | 2586.0 |  125240862 B |        8.86 |
| CsvHelper    | Asset | 50000   |    84.84 ms |  2.44 |  33 |  392.3 | 1696.8 |   14305848 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   812.54 ms |  1.00 | 665 |  819.4 |  812.5 |  273070232 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   517.67 ms |  0.64 | 665 | 1286.2 |  517.7 |  280315208 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,208.02 ms |  1.49 | 665 |  551.2 | 1208.0 |  273235192 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,483.07 ms |  3.06 | 665 |  268.1 | 2483.1 | 2500937776 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,769.26 ms |  2.18 | 665 |  376.3 | 1769.3 |  273246256 B |        1.00 |
