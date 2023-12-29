```

BenchmarkDotNet v0.13.11, Ubuntu 22.04.2 LTS (Jammy Jellyfish)
Unknown processor
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  Job-EEMDRF : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  Toolchain=net80  InvocationCount=Default  
IterationTime=350.0000 ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   9.537 ms |  1.00 | 20 | 2125.7 |  381.5 |     1.21 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  30.832 ms |  3.23 | 20 |  657.5 | 1233.3 |     9.74 KB |        8.02 |
| ReadLine_ | Row    | 25000 |  33.938 ms |  3.56 | 20 |  597.3 | 1357.5 | 73489.73 KB |   60,493.16 |
| CsvHelper | Row    | 25000 |  59.210 ms |  6.21 | 20 |  342.4 | 2368.4 |    20.71 KB |       17.04 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |  11.460 ms |  1.00 | 20 | 1768.9 |  458.4 |     1.23 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  34.857 ms |  3.04 | 20 |  581.6 | 1394.3 |     9.75 KB |        7.91 |
| ReadLine_ | Cols   | 25000 |  35.410 ms |  3.09 | 20 |  572.5 | 1416.4 | 73489.72 KB |   59,630.33 |
| CsvHelper | Cols   | 25000 |  64.828 ms |  5.65 | 20 |  312.7 | 2593.1 | 21341.07 KB |   17,316.37 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  52.297 ms |  1.00 | 20 |  387.6 | 2091.9 |     8.12 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  15.271 ms |  0.29 | 20 | 1327.5 |  610.9 |    65.28 KB |        8.04 |
| Sylvan___ | Floats | 25000 | 168.664 ms |  3.23 | 20 |  120.2 | 6746.6 |     18.2 KB |        2.24 |
| ReadLine_ | Floats | 25000 | 166.356 ms |  3.18 | 20 |  121.9 | 6654.3 | 73493.46 KB |    9,052.97 |
| CsvHelper | Floats | 25000 | 218.131 ms |  4.17 | 20 |   92.9 | 8725.2 | 22063.34 KB |    2,717.78 |
