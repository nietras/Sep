```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-WVXDVE : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-WVXDVE  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
| Method          | Scope | Rows    | Mean         | Ratio | RatioSD | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|---------------- |------ |-------- |-------------:|------:|--------:|----:|-------:|-------:|-------------:|------------:|
| Sep______       | Row   | 50000   |     6.776 ms |  1.00 |    0.00 |  33 | 4926.1 |  135.5 |      1.03 KB |        1.00 |
| Sep_Unescape    | Row   | 50000   |     7.253 ms |  1.08 |    0.02 |  33 | 4602.2 |  145.1 |      1.03 KB |        1.00 |
|                 |       |         |              |       |         |     |        |        |              |             |
| Sep______       | Cols  | 50000   |     7.627 ms |  1.00 |    0.00 |  33 | 4376.4 |  152.5 |      1.03 KB |        1.00 |
| Sep_Unescape    | Cols  | 50000   |     9.420 ms |  1.24 |    0.01 |  33 | 3543.3 |  188.4 |      1.04 KB |        1.00 |
|                 |       |         |              |       |         |     |        |        |              |             |
| Sep______       | Asset | 50000   |    42.165 ms |  1.00 |    0.00 |  33 |  791.6 |  843.3 |  13803.04 KB |        1.00 |
| Sep_MT___       | Asset | 50000   |    27.159 ms |  0.64 |    0.03 |  33 | 1229.0 |  543.2 |  13986.16 KB |        1.01 |
| RecordParser_MT | Asset | 50000   |    78.349 ms |  1.86 |    0.02 |  33 |  426.0 | 1567.0 |  28498.49 KB |        2.06 |
|                 |       |         |              |       |         |     |        |        |              |             |
| Sep______       | Asset | 1000000 |   764.302 ms |  1.00 |    0.00 | 667 |  873.6 |  764.3 |  266671.9 KB |        1.00 |
| Sep_MT___       | Asset | 1000000 |   414.657 ms |  0.55 |    0.01 | 667 | 1610.3 |  414.7 | 267882.55 KB |        1.00 |
| RecordParser_MT | Asset | 1000000 | 1,377.553 ms |  1.80 |    0.01 | 667 |  484.7 | 1377.6 | 380729.86 KB |        1.43 |
