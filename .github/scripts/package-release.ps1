param(
    [Parameter(Mandatory = $true)][string]$Version,
    [Parameter(Mandatory = $true)][string]$OutputDirectory,
    [Parameter(Mandatory = $true)][string]$DestinationZip
)

$ErrorActionPreference = 'Stop'
$output = (Resolve-Path -LiteralPath $OutputDirectory).Path
$destination = [IO.Path]::GetFullPath($DestinationZip)
if (Test-Path -LiteralPath $destination) { throw "Package already exists: $destination" }

$actualVersion = [Diagnostics.FileVersionInfo]::GetVersionInfo((Join-Path $output 'IndustrialPark.exe')).ProductVersion
if ($actualVersion -ne $Version) { throw "Expected version $Version, found $actualVersion." }

$stage = Join-Path ([IO.Path]::GetTempPath()) ("IndustrialPark_package_" + [guid]::NewGuid().ToString('N'))
try {
    New-Item -ItemType Directory -Path $stage | Out-Null
    foreach ($name in @('IndustrialPark', 'Randomizer', 'Updater')) {
        foreach ($suffix in @('.exe', '.dll', '.deps.json', '.runtimeconfig.json')) {
            Copy-Item -LiteralPath (Join-Path $output "$name$suffix") -Destination $stage
        }
    }
    $config = Join-Path $output 'IndustrialPark.dll.config'
    if (Test-Path -LiteralPath $config) { Copy-Item -LiteralPath $config -Destination $stage }
    foreach ($directory in @('Library', 'Resources')) {
        Copy-Item -LiteralPath (Join-Path $output $directory) -Destination $stage -Recurse
    }
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot '..\..\LICENSE.md') -Destination $stage
    $runtimeStage = Join-Path $stage 'runtimes'
    New-Item -ItemType Directory -Path $runtimeStage | Out-Null
    foreach ($rid in @('win-x86', 'win-x64', 'win-arm64')) {
        $runtime = Join-Path (Join-Path $output 'runtimes') $rid
        if (Test-Path -LiteralPath $runtime) { Copy-Item -LiteralPath $runtime -Destination $runtimeStage -Recurse }
    }
    if (-not (Get-ChildItem -LiteralPath $runtimeStage -Directory)) { throw 'No Windows native runtimes were found.' }
    Get-ChildItem -LiteralPath $stage -Filter '*.pdb' -File -Recurse | Remove-Item
    Compress-Archive -Path (Join-Path $stage '*') -DestinationPath $destination
}
finally {
    if (Test-Path -LiteralPath $stage) { Remove-Item -LiteralPath $stage -Recurse -Force }
}
