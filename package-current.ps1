$Version = [DateTime]::UtcNow.ToString('yyyy.MM.dd') + '-beta'
$Zip = Join-Path $PWD "IndustrialPark_v$Version.zip"

dotnet restore .\IndustrialPark.sln
if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }

dotnet build .\IndustrialPark.sln -c Release --no-restore -m:1 "-p:IPBuildVersion=$Version"
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }

.\.github\scripts\package-release.ps1 `
    -Version $Version `
    -OutputDirectory .\IndustrialPark\bin\Release\net10.0-windows `
    -DestinationZip $Zip
