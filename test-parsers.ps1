#!/usr/bin/env pwsh
Try {
    $parsers = @(
        "SepParserAvx512PackCmpOrMoveMaskTzcnt",
        "SepParserAvx2PackCmpOrMoveMaskTzcnt",
        "SepParserSse2PackCmpOrMoveMaskTzcnt",
        "SepParserVector512NrwCmpExtMsbTzcnt",
        "SepParserVector256NrwCmpExtMsbTzcnt",
        "SepParserVector128NrwCmpExtMsbTzcnt",
        "SepParserVector64NrwCmpExtMsbTzcnt",
        "SepParserIndexOfAny"
    )

    foreach ($parser in $parsers) {
        $env:SEPFORCEPARSER=$parser
        Write-Output "Testing $parser Debug"
        dotnet test .\src\Sep.Test\Sep.Test.csproj --nologo -c Debug -- /Parallel
        Write-Output "Testing $parser Release"
        dotnet test .\src\Sep.Test\Sep.Test.csproj --nologo -c Release -- /Parallel
    }
} Finally {
    Remove-Item env:SEPFORCEPARSER
}
