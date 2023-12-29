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
        Write-Host "Testing $parser Debug"
        dotnet test --nologo -c Debug -- /Parallel
        Write-Host "Testing $parser Release"
        dotnet test --nologo -c Release -- /Parallel 
    }
} Finally {
    Remove-Item env:SEPFORCEPARSER
}
