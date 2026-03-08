param(
    [string]$CertificatePassword = "DevTools2024!",
    [string]$CertName = "DevTools Code Signing",
    [string]$OutputPath = ".\build\certs"
)

$ErrorActionPreference = "Stop"

Write-Host "=== DevTools Self-Signed Certificate Creator ===" -ForegroundColor Cyan

$pfxPath = Join-Path $OutputPath "DevToolsCodeSigning.pfx"
$cerPath = Join-Path $OutputPath "DevToolsCodeSigning.cer"

if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    Write-Host "Created output directory: $OutputPath" -ForegroundColor Green
}

Write-Host "`nStep 1: Creating self-signed certificate..." -ForegroundColor Yellow

$cert = New-SelfSignedCertificate `
    -Subject "CN=$CertName" `
    -Type CodeSigningCert `
    -KeyUsage DigitalSignature `
    -FriendlyName "$CertName" `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3,1.3.6.1.4.1.311.10.3.13") `
    -KeyExportPolicy Exportable `
    -KeyLength 2048 `
    -KeyAlgorithm RSA `
    -HashAlgorithm SHA256 `
    -NotAfter (Get-Date).AddYears(10)

Write-Host "Certificate created with thumbprint: $($cert.Thumbprint)" -ForegroundColor Green

Write-Host "`nStep 2: Exporting certificate to PFX file..." -ForegroundColor Yellow

$securePassword = ConvertTo-SecureString -String $CertificatePassword -Force -AsPlainText
Export-PfxCertificate -Cert $cert -FilePath $pfxPath -Password $securePassword | Out-Null
Write-Host "PFX exported to: $pfxPath" -ForegroundColor Green

Write-Host "`nStep 3: Exporting public certificate..." -ForegroundColor Yellow
Export-Certificate -Cert $cert -FilePath $cerPath -Type CERT | Out-Null
Write-Host "Public cert exported to: $cerPath" -ForegroundColor Green

Write-Host "`nStep 4: Adding certificate to Trusted Publishers..." -ForegroundColor Yellow

$trustedPublishers = Get-ChildItem -Path "Cert:\CurrentUser\TrustedPublisher" -ErrorAction SilentlyContinue
$alreadyTrusted = $trustedPublishers | Where-Object { $_.Thumbprint -eq $cert.Thumbprint }

if (-not $alreadyTrusted) {
    $cert | Export-Certificate -FilePath (Join-Path $OutputPath "temp.cer") -Type CERT -Force | Out-Null
    Import-Certificate -FilePath (Join-Path $OutputPath "temp.cer") -CertStoreLocation "Cert:\CurrentUser\TrustedPublisher" | Out-Null
    Remove-Item (Join-Path $OutputPath "temp.cer") -Force
    Write-Host "Certificate added to Trusted Publishers" -ForegroundColor Green
} else {
    Write-Host "Certificate already in Trusted Publishers" -ForegroundColor Yellow
}

Write-Host "`nStep 5: Adding certificate to Root CA (for local trust)..." -ForegroundColor Yellow

$rootCerts = Get-ChildItem -Path "Cert:\CurrentUser\Root" -ErrorAction SilentlyContinue
$alreadyInRoot = $rootCerts | Where-Object { $_.Thumbprint -eq $cert.Thumbprint }

if (-not $alreadyInRoot) {
    $cert | Export-Certificate -FilePath (Join-Path $OutputPath "temp.cer") -Type CERT -Force | Out-Null
    Import-Certificate -FilePath (Join-Path $OutputPath "temp.cer") -CertStoreLocation "Cert:\CurrentUser\Root" | Out-Null
    Remove-Item (Join-Path $OutputPath "temp.cer") -Force
    Write-Host "Certificate added to Trusted Root Certification Authorities" -ForegroundColor Green
} else {
    Write-Host "Certificate already in Root CA store" -ForegroundColor Yellow
}

Write-Host @"

=== Certificate Creation Complete ===

Certificate Details:
  Subject:         $($cert.Subject)
  Thumbprint:      $($cert.Thumbprint)
  Valid From:      $($cert.NotBefore)
  Valid To:        $($cert.NotAfter)
  
Files Created:
  PFX (Private):   $pfxPath
  CER (Public):    $cerPath
  Password:        $CertificatePassword

IMPORTANT:
  - Keep the PFX file and password secure!
  - The certificate is now trusted on THIS computer only
  - On other computers, users need to install the CER file to:
    1. Trusted Publishers
    2. Trusted Root Certification Authorities

"@ -ForegroundColor Cyan

return @{
    PfxPath = $pfxPath
    CerPath = $cerPath
    Thumbprint = $cert.Thumbprint
    Password = $CertificatePassword
}
