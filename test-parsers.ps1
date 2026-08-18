#!/usr/bin/env pwsh
Try {
    dotnet build .\src\Sep.Test\Sep.Test.csproj --nologo -c Debug --no-restore
    dotnet build .\src\Sep.Test\Sep.Test.csproj --nologo -c Release --no-restore

    $parsers = @(
        "SepParserAdvSimdLoad4xNrwCmpOrBulkMoveMaskTzcnt",
        "SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt",
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
        dotnet test --project .\src\Sep.Test\Sep.Test.csproj -c Debug --no-build --no-restore -p:TestTfmsInParallel=true --report-github
        Write-Output "Testing $parser Release"
        dotnet test --project .\src\Sep.Test\Sep.Test.csproj -c Release --no-build --no-restore -p:TestTfmsInParallel=true --report-github
    }
} Finally {
    Remove-Item env:SEPFORCEPARSER
}
