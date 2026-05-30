#Requires -Version 5.1
param(
    [string]$OutputDir = "",
    [string]$PythonVersion = "3.12.10",
    [switch]$SkipPython
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectFile = Join-Path $ScriptDir "ImageCaptureApp.csproj"
if ([string]::IsNullOrWhiteSpace($OutputDir)) {
    $OutputDir = Join-Path $ScriptDir "PublishOutput"
}

function Write-Step([string]$Message) {
    Write-Host ""
    Write-Host "==> $Message" -ForegroundColor Cyan
}

Write-Step "Publishing self-contained win-x64 app"
if (-not (Test-Path $ProjectFile)) {
    throw "Project file not found: $ProjectFile"
}

if (Test-Path $OutputDir) {
    Write-Host "Cleaning output: $OutputDir"
    Remove-Item -LiteralPath $OutputDir -Recurse -Force
}

dotnet publish $ProjectFile `
    -c Release `
    -r win-x64 `
    --self-contained true `
    /p:PublishSingleFile=false `
    /p:IncludeNativeLibrariesForSelfExtract=true `
    -o $OutputDir
if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed"
}

$PythonScriptDir = Join-Path $OutputDir "Python"
New-Item -ItemType Directory -Force -Path $PythonScriptDir | Out-Null
Copy-Item -LiteralPath (Join-Path $ScriptDir "Python\MRC_final.py") -Destination $PythonScriptDir -Force
Copy-Item -LiteralPath (Join-Path $ScriptDir "Python\MappingTable.xlsx") -Destination $PythonScriptDir -Force
Copy-Item -LiteralPath (Join-Path $ScriptDir "PortableReadme.zh-CN.txt") -Destination (Join-Path $OutputDir "使用说明.txt") -Force

Write-Step "Bundling Teledyne DALSA Sapera components"
$RepoRoot = Split-Path -Parent $ScriptDir
$SaperaSourceRoot = Join-Path $RepoRoot "Teledyne DALSA"
$SaperaNetBinSource = Join-Path $SaperaSourceRoot "Sapera\Components\NET\Bin"
$SaperaNativeBinSource = Join-Path $SaperaSourceRoot "Sapera\Bin"
$SaperaNetBinDest = Join-Path $OutputDir "Teledyne DALSA\Sapera\Components\NET\Bin"
$SaperaNativeBinDest = Join-Path $OutputDir "Teledyne DALSA\Sapera\Bin"

if (Test-Path (Join-Path $SaperaNetBinSource "DALSA.SaperaLT.SapClassBasic.dll")) {
    New-Item -ItemType Directory -Force -Path $SaperaNetBinDest | Out-Null
    Copy-Item -LiteralPath (Join-Path $SaperaNetBinSource "DALSA.SaperaLT.SapClassBasic.dll") -Destination $SaperaNetBinDest -Force
    Get-ChildItem -LiteralPath $SaperaNetBinSource -Filter "*.dll" -ErrorAction SilentlyContinue |
        Copy-Item -Destination $SaperaNetBinDest -Force
    Write-Host "Copied Sapera .NET DLLs -> $SaperaNetBinDest"
}
else {
    Write-Warning "Sapera .NET DLL not found at $SaperaNetBinSource"
    Write-Warning "Camera init will fail until DALSA.SaperaLT.SapClassBasic.dll is placed under PublishOutput\Teledyne DALSA\Sapera\Components\NET\Bin\"
}

if (Test-Path $SaperaNativeBinSource) {
    New-Item -ItemType Directory -Force -Path $SaperaNativeBinDest | Out-Null
    Get-ChildItem -LiteralPath $SaperaNativeBinSource -Filter "*.dll" -ErrorAction SilentlyContinue |
        Copy-Item -Destination $SaperaNativeBinDest -Force
    Write-Host "Copied Sapera native Bin DLLs -> $SaperaNativeBinDest"
}

$CcfCandidates = @(
    (Join-Path $RepoRoot "mycamera.ccf"),
    (Join-Path $SaperaSourceRoot "Sapera\CamFiles\User\mycamera.ccf")
)
$CcfCopied = $false
foreach ($ccf in $CcfCandidates) {
    if (Test-Path $ccf) {
        Copy-Item -LiteralPath $ccf -Destination (Join-Path $OutputDir "mycamera.ccf") -Force
        Write-Host "Copied camera config -> $(Join-Path $OutputDir 'mycamera.ccf')"
        $CcfCopied = $true
        break
    }
}
if (-not $CcfCopied) {
    Write-Warning "mycamera.ccf not found; copy it to PublishOutput root manually."
}

if ($SkipPython) {
    Write-Host "Skipped embedded Python packaging (-SkipPython)"
}
else {
    Write-Step "Bundling embedded Python $PythonVersion"
    $RuntimePythonDir = Join-Path $OutputDir "Runtime\Python"
    $TempDir = Join-Path ([System.IO.Path]::GetTempPath()) ("imagecapture-python-" + [guid]::NewGuid().ToString("N"))
    New-Item -ItemType Directory -Force -Path $TempDir | Out-Null
    try {
        $EmbedZipName = "python-$PythonVersion-embed-amd64.zip"
        $EmbedUrl = "https://www.python.org/ftp/python/$PythonVersion/$EmbedZipName"
        $EmbedZipPath = Join-Path $TempDir $EmbedZipName
        Write-Host "Downloading $EmbedUrl"
        Invoke-WebRequest -Uri $EmbedUrl -OutFile $EmbedZipPath
        Expand-Archive -LiteralPath $EmbedZipPath -DestinationPath $RuntimePythonDir -Force

        $MajorMinor = ($PythonVersion.Split(".")[0..1] -join "")
        $PthFile = Join-Path $RuntimePythonDir "python$MajorMinor._pth"
        if (-not (Test-Path $PthFile)) {
            throw "Embedded python path file not found: $PthFile"
        }

        @(
            "python$MajorMinor.zip"
            "."
            "Lib\site-packages"
            "import site"
        ) | Set-Content -LiteralPath $PthFile -Encoding ASCII

        $SitePackagesDir = Join-Path $RuntimePythonDir "Lib\site-packages"
        New-Item -ItemType Directory -Force -Path $SitePackagesDir | Out-Null

        $GetPipPath = Join-Path $TempDir "get-pip.py"
        Invoke-WebRequest -Uri "https://bootstrap.pypa.io/get-pip.py" -OutFile $GetPipPath
        $PythonExe = Join-Path $RuntimePythonDir "python.exe"
        & $PythonExe $GetPipPath --no-warn-script-location
        if ($LASTEXITCODE -ne 0) {
            throw "get-pip failed"
        }

        $RequirementsFile = Join-Path $ScriptDir "Python\requirements-mrc.txt"
        & $PythonExe -m pip install --no-cache-dir -r $RequirementsFile --no-warn-script-location
        if ($LASTEXITCODE -ne 0) {
            throw "pip install failed for MRC requirements"
        }

        Write-Host "Validating embedded Python imports..."
        & $PythonExe -c "import cv2, numpy, matplotlib, openpyxl, scipy; print('python-ok', cv2.__version__)"
        if ($LASTEXITCODE -ne 0) {
            throw "embedded Python validation failed"
        }
    }
    finally {
        if (Test-Path $TempDir) {
            Remove-Item -LiteralPath $TempDir -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
}

$ReadmeLines = @(
    "ImageCaptureApp portable release",
    "",
    "1. Copy the entire PublishOutput folder to the target PC",
    "2. Run ImageCaptureApp.exe",
    "3. No .NET / Python / PATH setup required",
    "",
    "Folders:",
    "  ImageCaptureApp.exe  - main app",
    "  Runtime\Python\      - bundled Python for MRC",
    "  Python\              - MRC_final.py and MappingTable.xlsx",
    "  Config\              - capture device config",
    "",
    "Camera:",
    "  Teledyne DALSA / Sapera LT still needs vendor SDK on target PC",
    "",
    "OS: Windows 10/11 x64"
)
$ReadmePath = Join-Path $OutputDir "README-portable.txt"
$ReadmeLines | Set-Content -LiteralPath $ReadmePath -Encoding UTF8

Write-Step "Done"
Write-Host "Output: $OutputDir"
Write-Host "Exe:    $(Join-Path $OutputDir 'ImageCaptureApp.exe')"
if (-not $SkipPython) {
    Write-Host "Python: $(Join-Path $OutputDir 'Runtime\Python\python.exe')"
}
