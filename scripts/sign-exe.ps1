param(
    [string]$ExePath,
    [string]$PfxPath = ".\build\certs\DevToolsCodeSigning.pfx",
    [string]$Password = "DevTools2024!",
    [string]$TimestampServer = "http://timestamp.digicert.com"
)

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = (Get-Item $scriptDir).Parent.FullName

Write-Host "=== DevTools Code Signing ===" -ForegroundColor Cyan

if (-not $ExePath) {
    Write-Host "Error: ExePath parameter is required" -ForegroundColor Red
    Write-Host "Usage: .\sign-exe.ps1 -ExePath <path-to-exe>" -ForegroundColor Yellow
    exit 1
}

if (-not ([System.IO.Path]::IsPathRooted($ExePath))) {
    $ExePath = Join-Path $projectRoot $ExePath
}

if (-not ([System.IO.Path]::IsPathRooted($PfxPath))) {
    $PfxPath = Join-Path $projectRoot $PfxPath
}

if (-not (Test-Path $ExePath)) {
    Write-Host "Error: File not found: $ExePath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $PfxPath)) {
    Write-Host "Error: Certificate not found: $PfxPath" -ForegroundColor Red
    Write-Host "Please run create-cert.ps1 first to create the certificate." -ForegroundColor Yellow
    exit 1
}

Write-Host "Signing: $ExePath" -ForegroundColor Yellow
Write-Host "Certificate: $PfxPath" -ForegroundColor Yellow

$signtool = $null

$sdkPaths = @(
    "${env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.22621.0\x64\signtool.exe",
    "${env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.22000.0\x64\signtool.exe",
    "${env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.19041.0\x64\signtool.exe",
    "${env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.18362.0\x64\signtool.exe"
)

foreach ($path in $sdkPaths) {
    if (Test-Path $path) {
        $signtool = $path
        break
    }
}

if (-not $signtool) {
    $signtool = Get-ChildItem -Path "${env:ProgramFiles(x86)}\Windows Kits\10\bin" -Recurse -Filter "signtool.exe" -ErrorAction SilentlyContinue | 
                Where-Object { $_.FullName -match "\\x64\\" } |
                Sort-Object { $_.FullName } -Descending | 
                Select-Object -ExpandProperty FullName -First 1
}

if (-not $signtool) {
    Write-Host @"

========================================
  Windows SDK Not Found!
========================================

signtool.exe is required for code signing.

Please install Windows SDK:
  1. Download from: https://aka.ms/winsdk
  2. Or run: winget install Microsoft.WindowsSDK.10.0
  3. Select "Windows Software Development Kit" during installation

After installation, run this script again.

"@ -ForegroundColor Red
    exit 1
}

Write-Host "Using signtool: $signtool" -ForegroundColor Gray

$result = & $signtool sign /f $PfxPath /p $Password /tr $TimestampServer /td SHA256 /fd SHA256 $ExePath 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nSigning successful!" -ForegroundColor Green
    
    Write-Host "`nVerifying signature..." -ForegroundColor Cyan
    $verifyResult = & $signtool verify /pa $ExePath 2>&1
    Write-Host $verifyResult
    
    Write-Host @"

========================================
  Signature Applied Successfully!
========================================

The executable is now signed with your certificate.

Note: Since this is a self-signed certificate:
- It will be trusted on THIS computer (certificate installed to Trusted Publishers)
- On other computers, users need to install the .cer file first

Files:
  Signed EXE:  $ExePath
  Public Cert: $($PfxPath -replace '\.pfx$', '.cer')

"@ -ForegroundColor Green
} else {
    Write-Host "`nSigning failed!" -ForegroundColor Red
    Write-Host $result
    exit 1
}
