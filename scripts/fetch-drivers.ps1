# Reconstruct the drivers/ folder, which is deliberately not tracked in this repository.
#
# There is no longer a standalone MCHOSE V9 PRO driver download. MCHOSE's own support pages
# state the previous V9/V9 Pro driver is no longer offered, the model has been dropped from
# their live update manifest (cdn.mchose.com.cn/OTA/PC/prod/otaFile/otaConfigApp_prod.json
# lists V9 TURBO, V9 TURBO+ and V9 PRO 2 but no V9 PRO), and the only driver installers hosted
# there are THX APO packages for other models. The driver therefore only ships inside the
# MCHOSE HUB installer, under libs/<model>.
#
# Every file this script copies is verified against tools/driver-manifest.json, so you can
# confirm you obtained the same payload this project was developed against.

[CmdletBinding()]
param(
    # Where to take the files from: an installed MCHOSE HUB, or a downloaded HUB installer.
    [ValidateSet('Hub', 'Installer')]
    [string]$From = 'Hub',

    # Path to the MCHOSE HUB installer, required when -From Installer.
    [string]$InstallerPath,

    # Only check what is already in drivers/, copy nothing.
    [switch]$Verify,

    [string]$HubPath = 'C:\Program Files\MCHOSE HUB',
    [string]$Destination,
    [string]$ManifestPath
)

$ErrorActionPreference = 'Stop'

# $PSScriptRoot is not populated in the param block on Windows PowerShell 5.1.
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
if (-not $Destination) { $Destination = Join-Path (Split-Path $scriptDir -Parent) 'drivers' }
if (-not $ManifestPath) { $ManifestPath = Join-Path $scriptDir 'driver-manifest.json' }

if (-not (Test-Path $ManifestPath)) { throw "manifest not found: $ManifestPath" }
$manifest = Get-Content $ManifestPath -Raw | ConvertFrom-Json
$models = $manifest.models.PSObject.Properties.Name

function Test-Payload {
    param([string]$Root, [string]$Label)

    $ok = 0; $bad = 0; $absent = 0
    foreach ($model in $models) {
        foreach ($entry in $manifest.models.$model) {
            $file = Join-Path $Root (Join-Path $model ($entry.path -replace '/', '\'))
            if (-not (Test-Path -LiteralPath $file)) { $absent++; continue }
            if ((Get-FileHash -LiteralPath $file -Algorithm SHA256).Hash -eq $entry.sha256) { $ok++ } else { $bad++; Write-Warning "checksum mismatch: $model/$($entry.path)" }
        }
    }

    Write-Output ("{0}: {1} verified, {2} mismatched, {3} missing" -f $Label, $ok, $bad, $absent)
    return ($bad -eq 0 -and $absent -eq 0)
}

if ($Verify) {
    if (Test-Payload -Root $Destination -Label 'drivers') { Write-Output 'All driver files match the manifest.'; exit 0 }
    Write-Output 'Run this script without -Verify to restore the missing or altered files.'
    exit 1
}

# Resolve the libs/ folder holding the per-model driver payload.
if ($From -eq 'Installer') {
    if (-not $InstallerPath) { throw 'give -InstallerPath when using -From Installer' }
    if (-not (Test-Path $InstallerPath)) { throw "installer not found: $InstallerPath" }

    $sevenZip = @("$env:ProgramFiles\7-Zip\7z.exe", "${env:ProgramFiles(x86)}\7-Zip\7z.exe") | Where-Object { Test-Path $_ } | Select-Object -First 1
    if (-not $sevenZip) { throw '7-Zip is required to unpack the installer; install it or use -From Hub against an installed MCHOSE HUB' }

    $staging = Join-Path ([IO.Path]::GetTempPath()) ("mchose-hub-" + [Guid]::NewGuid().ToString('N').Substring(0, 8))
    Write-Output "unpacking installer into $staging"
    & $sevenZip x $InstallerPath "-o$staging" -y | Out-Null

    $libs = Get-ChildItem $staging -Recurse -Directory -Filter 'libs' | Select-Object -First 1
    if (-not $libs) { throw 'no libs/ folder inside the installer; the layout may have changed' }
    $libsPath = $libs.FullName
} else {
    $libsPath = Join-Path $HubPath 'libs'
    if (-not (Test-Path $libsPath)) { throw "MCHOSE HUB not found at $HubPath. Install it, or use -From Installer -InstallerPath <setup.exe>" }
}

Write-Output "source: $libsPath"
Write-Output "target: $Destination"

$copied = 0
foreach ($model in $models) {
    foreach ($entry in $manifest.models.$model) {
        $relative = $entry.path -replace '/', '\'
        $source = Join-Path $libsPath (Join-Path $model $relative)
        if (-not (Test-Path -LiteralPath $source)) { Write-Warning "not in this HUB build: $model/$($entry.path)"; continue }

        $target = Join-Path $Destination (Join-Path $model $relative)
        $parent = Split-Path $target -Parent
        if (-not (Test-Path $parent)) { New-Item -ItemType Directory -Path $parent -Force | Out-Null }
        Copy-Item -LiteralPath $source -Destination $target -Force
        $copied++
    }
}

Write-Output "copied $copied files"
if (Test-Payload -Root $Destination -Label 'drivers') { Write-Output 'All driver files match the manifest.' } else { Write-Output 'Some files did not match; this HUB build may ship a different driver version.' }

if ($From -eq 'Installer' -and $staging -and (Test-Path $staging)) { Remove-Item $staging -Recurse -Force }
