```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 9950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.103
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-WEBEIB : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-WEBEIB  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0,SEPFORCEPARSER=SepParserAvx2PackCmpOrMoveMaskTzcnt  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  Error=0.0132 ms  
StdDev=0.0034 ms  

```
| Method    | Scope | Rows  | Mean     | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|---------- |------ |------ |---------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep______ | Row   | 50000 | 1.396 ms |  1.00 | 29 | 20908.8 |   27.9 |   1.01 KB |        1.00 |
