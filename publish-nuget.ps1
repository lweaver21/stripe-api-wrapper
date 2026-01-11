# StripeApiWrapper NuGet Package Publisher (PowerShell)
# Usage: .\publish-nuget.ps1 -ApiKey <YOUR_API_KEY>
# Or set NUGET_API_KEY environment variable

param(
    [string]$ApiKey = $env:NUGET_API_KEY
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$OutputDir = Join-Path $ScriptDir "nupkg"
$NuGetSource = "https://api.nuget.org/v3/index.json"

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    Write-Error "NuGet API key is required."
    Write-Host "Usage: .\publish-nuget.ps1 -ApiKey <YOUR_API_KEY>"
    Write-Host "   Or: `$env:NUGET_API_KEY = '<your-key>'; .\publish-nuget.ps1"
    exit 1
}

Write-Host "=== Building solution in Release mode ===" -ForegroundColor Cyan
dotnet build $ScriptDir -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ""
Write-Host "=== Running tests ===" -ForegroundColor Cyan
dotnet test $ScriptDir -c Release --no-build
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ""
Write-Host "=== Creating NuGet package ===" -ForegroundColor Cyan
if (Test-Path $OutputDir) {
    Remove-Item $OutputDir -Recurse -Force
}
dotnet pack "$ScriptDir\src\StripeApiWrapper" -c Release --no-build -o $OutputDir
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ""
Write-Host "=== Publishing to NuGet.org ===" -ForegroundColor Cyan
Get-ChildItem "$OutputDir\*.nupkg" | ForEach-Object {
    Write-Host "Publishing: $($_.Name)"
    dotnet nuget push $_.FullName --api-key $ApiKey --source $NuGetSource --skip-duplicate
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

Write-Host ""
Write-Host "=== Done! ===" -ForegroundColor Green
Write-Host "Package published successfully to NuGet.org"
