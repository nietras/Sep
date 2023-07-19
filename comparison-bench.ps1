dotnet run -c Release -f net7.0 --project src\Sep.ComparisonBenchmarks\Sep.ComparisonBenchmarks.csproj -- `
    -m --warmupCount 6 --minIterationCount 5 --maxIterationCount 13 --runtimes net70 `
    --hide Type Quotes Reader RatioSD Gen0 Gen1 Gen2 Error StdDev --filter *
