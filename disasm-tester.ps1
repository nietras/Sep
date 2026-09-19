param(
    [string]$runtime = "win-arm64"
)
dotnet publish src/Sep.Tester/Sep.Tester.csproj -c Release -r "$runtime" -f net11.0 --self-contained true /p:PublishAot=true /p:DebugSymbols=true
dumpbin /DISASM /SYMBOLS "artifacts\publish\Sep.Tester\release_net11.0_$runtime\Sep.Tester.exe" > "artifacts\publish\Sep.Tester\release_net11.0_$runtime\disassembly.asm"
