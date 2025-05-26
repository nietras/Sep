#!/usr/bin/env pwsh
Try {
    $parsers = @(
        "SepParserAdvSimdX8NrwCmpOrMoveMaskTzcnt",
        "SepParserAvx512To256CmpOrMoveMaskTzcnt",
        "SepParserAvx512PackCmpOrMoveMaskTzcnt",
        "SepParserAvx2PackCmpOrMoveMaskTzcnt",
        "SepParserAvx256To128CmpOrMoveMaskTzcnt",
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
