param(
    [string]$runtime = "win-arm64"
)
dotnet publish src/Sep.Tester/Sep.Tester.csproj -c Release -r "$runtime" -f net9.0 --self-contained true /p:PublishAot=true /p:DebugSymbols=true
dumpbin /DISASM /SYMBOLS "artifacts\publish\Sep.Tester\release_net9.0_$runtime\Sep.Tester.exe" > "artifacts\publish\Sep.Tester\release_net9.0_$runtime\win-arm64-disassembly.asm"
