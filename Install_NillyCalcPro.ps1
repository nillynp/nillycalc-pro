# ============================================================================
# NillyCalc Pro — Universal Windows 10/11 Installer (Zero Admin · Portable)
# created by Nilly graphics (Sotiris Christos) — https://nillygraphics.com/
# Ultra-Light Windows 11 & Graphic Designer Calculator (DPI, CMYK, Color Picker)
# ============================================================================
param([switch]$Silent)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$exePath = Join-Path $root 'NillyCalcPro.exe'
$icoPath = Join-Path $root 'NillyCalcPro.ico'
$srcPath = Join-Path $root 'desktop\NillyCalcPro.cs'

Write-Host ''
Write-Host '  ============================================================' -ForegroundColor Cyan
Write-Host '   NillyCalc Pro — Windows 11 & Graphic Designer Calculator' -ForegroundColor White
Write-Host '   created by Nilly graphics (Sotiris Christos)' -ForegroundColor DarkGray
Write-Host '   https://nillygraphics.com/' -ForegroundColor DarkGray
Write-Host '  ============================================================' -ForegroundColor Cyan
Write-Host ''

# 1. Unblock downloaded files if extracted from GitHub Portable ZIP
Get-ChildItem -Path $root -Recurse -File -ErrorAction SilentlyContinue | Unblock-File -ErrorAction SilentlyContinue

# 2. Auto-compile NillyCalcPro.exe on any Windows 10/11 PC if missing or older than source
$needBuild = (-not (Test-Path $exePath)) -or ((Test-Path $srcPath) -and ((Get-Item $srcPath).LastWriteTimeUtc -gt (Get-Item $exePath).LastWriteTimeUtc))
if ($needBuild) {
    $fxDir = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319'
    if (-not (Test-Path "$fxDir\csc.exe")) { $fxDir = 'C:\Windows\Microsoft.NET\Framework\v4.0.30319' }
    $csc = Join-Path $fxDir 'csc.exe'
    if (-not (Test-Path $csc)) { throw 'Δεν βρέθηκε ο .NET Framework 4.x C# compiler (csc.exe).' }
    Write-Host '  [1/2] Μεταγλώττιση NillyCalcPro.exe (100% Native Windows 11 Studio Host)...' -ForegroundColor Yellow
    $cscArgs = @(
        '/nologo', '/target:winexe', '/optimize+', '/utf8output', "/out:$exePath",
        '/r:System.dll', '/r:System.Drawing.dll', '/r:System.Windows.Forms.dll'
    )
    if (Test-Path $icoPath) { $cscArgs += "/win32icon:$icoPath" }
    $cscArgs += $srcPath
    & $csc @cscArgs
    if ($LASTEXITCODE -ne 0 -or -not (Test-Path $exePath)) {
        throw "Αποτυχία μεταγλώττισης NillyCalcPro.exe (exit $LASTEXITCODE)"
    }
    Write-Host ("        OK -> " + [math]::Round((Get-Item $exePath).Length / 1KB, 1) + " KB") -ForegroundColor Green
} else {
    Write-Host '  [1/2] NillyCalcPro.exe έτοιμο.' -ForegroundColor Green
}

# 3. Desktop & Start Menu Shortcuts (Zero Admin!)
Write-Host '  [2/2] Δημιουργία συντομεύσεων (Επιφάνεια Εργασίας & Start Menu)...' -ForegroundColor Yellow
$wsh = New-Object -ComObject WScript.Shell
$desktopDir = [Environment]::GetFolderPath('Desktop')
$startMenuDir = Join-Path ([Environment]::GetFolderPath('Programs')) 'Nilly graphics'
New-Item -ItemType Directory -Force -Path $startMenuDir | Out-Null

foreach ($lnkPath in @(
    (Join-Path $desktopDir 'NillyCalc Pro.lnk'),
    (Join-Path $startMenuDir 'NillyCalc Pro.lnk')
)) {
    $sc = $wsh.CreateShortcut($lnkPath)
    $sc.TargetPath = $exePath
    $sc.WorkingDirectory = $root
    $sc.Description = 'NillyCalc Pro — Windows 11 & Graphic Designer Calculator (DPI, CMYK, Color Picker, VAT & m2) | created by Nilly graphics'
    if (Test-Path $icoPath) { $sc.IconLocation = "$icoPath,0" }
    $sc.Save()
}
Write-Host '        OK -> Έτοιμες οι συντομεύσεις!' -ForegroundColor Green
Write-Host ''
Write-Host '  ΟΛΟΚΛΗΡΩΘΗΚΕ! Άνοιξε το NillyCalc Pro από την Επιφάνεια Εργασίας ή το Start Menu.' -ForegroundColor Cyan
Write-Host '  Συντομεύσεις: Ctrl+1..4 (Tabs) · Ctrl+Shift+C (Πιπέτα Χρώματος) · Ctrl+T (Pin επάνω)' -ForegroundColor White
Write-Host ''

if (-not $Silent) {
    Start-Process -FilePath $exePath
}
