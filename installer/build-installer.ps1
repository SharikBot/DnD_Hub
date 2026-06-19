#Requires -Version 5.1
$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $PSScriptRoot
$PublishRoot = Join-Path $PSScriptRoot "output\publish"
$Rid = "win-x64"
$Config = "Release"

Write-Host "==> Clean publish folder"
if (Test-Path $PublishRoot) {
    Remove-Item $PublishRoot -Recurse -Force
}
New-Item -ItemType Directory -Path $PublishRoot -Force | Out-Null

$PublishArgs = @(
    "-c", $Config,
    "-r", $Rid,
    "--self-contained", "true",
    "-p:PublishReadyToRun=true",
    "-p:DebugType=none",
    "-p:DebugSymbols=false"
)

Write-Host "==> Publish API"
dotnet publish (Join-Path $Root "src\DnDCharacterManager.Api\DnDCharacterManager.Api.csproj") `
    @PublishArgs `
    -o (Join-Path $PublishRoot "Api")

Write-Host "==> Publish Desktop"
dotnet publish (Join-Path $Root "src\DnDCharacterManager.Desktop\DnDCharacterManager.Desktop.csproj") `
    @PublishArgs `
    -o (Join-Path $PublishRoot "Desktop")

Write-Host "==> Publish Launcher"
dotnet publish (Join-Path $Root "src\DnDCharacterManager.Launcher\DnDCharacterManager.Launcher.csproj") `
    @PublishArgs `
    -p:PublishSingleFile=true `
    -o $PublishRoot

$prodSettings = Join-Path $PSScriptRoot "templates\appsettings.Production.json"
Copy-Item $prodSettings (Join-Path $PublishRoot "Api\appsettings.Production.json") -Force

Write-Host "==> Publish complete: $PublishRoot"

$IsccCandidates = @(
    "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
    "$env:ProgramFiles\Inno Setup 6\ISCC.exe",
    "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe"
)
$Iscc = $IsccCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1

if ($Iscc) {
    Write-Host "==> Build installer with Inno Setup"
    & $Iscc (Join-Path $PSScriptRoot "setup.iss")
    $SetupExe = Get-ChildItem (Join-Path $PSScriptRoot "output\installer") -Filter "*.exe" | Select-Object -First 1
    if ($SetupExe) {
        Write-Host "Installer: $($SetupExe.FullName)"
    }
}
else {
    Write-Host "Inno Setup not found. Install from https://jrsoftware.org/isinfo.php"
    Write-Host "Then run: `"${IsccCandidates[0]}`" `"$PSScriptRoot\setup.iss`""
    Write-Host "Portable publish ready in: $PublishRoot"
}
