# ============================================================================
# NillyCalc Pro — Clean Uninstaller (Zero Admin)
# created by Nilly graphics (Sotiris Christos) — https://nillygraphics.com/
# ============================================================================
param([switch]$Silent)

$ErrorActionPreference = 'SilentlyContinue'
Write-Host ''
Write-Host '  Αφαίρεση συντομεύσεων του NillyCalc Pro...' -ForegroundColor Yellow

Stop-Process -Name 'NillyCalcPro' -Force -ErrorAction SilentlyContinue

$desktopLnk = Join-Path ([Environment]::GetFolderPath('Desktop')) 'NillyCalc Pro.lnk'
$startLnk = Join-Path ([Environment]::GetFolderPath('Programs')) 'Nilly graphics\NillyCalc Pro.lnk'
if (Test-Path $desktopLnk) { Remove-Item $desktopLnk -Force }
if (Test-Path $startLnk) { Remove-Item $startLnk -Force }

Write-Host '  OK -> Αφαιρέθηκαν καθαρά οι συντομεύσεις του NillyCalc Pro.' -ForegroundColor Green
Write-Host ''
if (-not $Silent) { Read-Host '  Πάτησε Enter για κλείσιμο' }
