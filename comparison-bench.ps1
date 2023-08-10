dotnet run -c Release -f net7.0 --project src\Sep.ComparisonBenchmarks\Sep.ComparisonBenchmarks.csproj -- `
    -m --warmupCount 6 --minIterationCount 5 --runtimes net70 `
    --iterationTime 200 --hide Type Quotes Reader RatioSD Gen0 Gen1 Gen2 Error Median StdDev --filter *
