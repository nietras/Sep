param(
    [string]$filter = "*"
)
dotnet run -c Release -f net9.0 --project src/Sep.ComparisonBenchmarks/Sep.ComparisonBenchmarks.csproj -- -m --envVars DOTNET_GCDynamicAdaptationMode:0 --warmupCount 6 --minIterationCount 5 --maxIterationCount 15 --runtimes net90 --iterationTime 350 --hide Type Quotes Reader RatioSD Gen0 Gen1 Gen2 Error Median StdDev --filter "$filter"
