$ErrorActionPreference = "SilentlyContinue"

$root = $PSScriptRoot
$searchDirs = @(
  $root,
  "$root\Teledyne DALSA",
  "$root\Teledyne DALSA\Sapera",
  "$root\Teledyne DALSA\Sapera\Demos\NET",
  "$root\Teledyne DALSA\Sapera\Examples\NET",
  "$env:ProgramFiles\Teledyne DALSA",
  "$env:ProgramFiles\Teledyne",
  "${env:ProgramFiles(x86)}\Teledyne DALSA",
  "${env:ProgramFiles(x86)}\Teledyne",
  "$env:ProgramFiles\DALSA",
  "${env:ProgramFiles(x86)}\DALSA",
  "$env:ProgramFiles\Sapera",
  "${env:ProgramFiles(x86)}\Sapera"
)

$keywords = @("Sapera","DALSA","Sap","Cor","GigE","CameraLink")

Write-Host "=== SAPERA FILE FINDER ===" -ForegroundColor Cyan
Write-Host "Root: $root" -ForegroundColor Gray
Write-Host ""

$dlls = @()
$libs = @()

foreach ($dir in $searchDirs) {
  if (-not (Test-Path -LiteralPath $dir)) { continue }
  Write-Host "[SCAN] $dir" -ForegroundColor DarkGray

  $dlls += Get-ChildItem -Path $dir -Recurse -File -Include *.dll,*.DLL |
    Where-Object {
      $name = $_.Name
      $matched = $false
      foreach ($k in $keywords) { if ($name -match $k) { $matched = $true; break } }
      $matched
    }

  $libs += Get-ChildItem -Path $dir -Recurse -File -Include *.lib,*.LIB |
    Where-Object {
      $name = $_.Name
      $matched = $false
      foreach ($k in $keywords) { if ($name -match $k) { $matched = $true; break } }
      $matched
    }
}

$dlls = $dlls | Sort-Object FullName -Unique
$libs = $libs | Sort-Object FullName -Unique

$managed = @()
$native = @()

foreach ($d in $dlls) {
  try {
    [void][System.Reflection.AssemblyName]::GetAssemblyName($d.FullName)
    $managed += $d
  } catch {
    $native += $d
  }
}

Write-Host ""
Write-Host "--- Managed .NET DLL (C# direct reference) ---" -ForegroundColor Green
if ($managed.Count -eq 0) {
  Write-Host "(none)" -ForegroundColor Gray
} else {
  $managed | ForEach-Object { Write-Host $_.FullName }
}

Write-Host ""
Write-Host "--- Native DLL (usually C/C++) ---" -ForegroundColor Yellow
if ($native.Count -eq 0) {
  Write-Host "(none)" -ForegroundColor Gray
} else {
  $native | ForEach-Object { Write-Host $_.FullName }
}

Write-Host ""
Write-Host "--- LIB files (C/C++ link libs) ---" -ForegroundColor Yellow
if ($libs.Count -eq 0) {
  Write-Host "(none)" -ForegroundColor Gray
} else {
  $libs | ForEach-Object { Write-Host $_.FullName }
}

Write-Host ""
Write-Host "Done. If you find DALSA.SaperaLT.SapClassBasic.dll, send me the full path." -ForegroundColor Cyan
