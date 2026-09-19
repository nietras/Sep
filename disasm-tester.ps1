param(
    [string]$runtime = "win-arm64",
    [string]$tfm = "net11.0"
)
dotnet publish src/Sep.Tester/Sep.Tester.csproj -c Release -r "$runtime" -f $tfm --self-contained true /p:PublishAot=true /p:DebugSymbols=true
dumpbin /DISASM /SYMBOLS "artifacts\publish\Sep.Tester\release_${tfm}_${runtime}\Sep.Tester.exe" > "artifacts\publish\Sep.Tester\release_${tfm}_${runtime}\disassembly.asm"
