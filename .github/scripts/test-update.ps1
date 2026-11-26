param(
    [Parameter(Mandatory = $true)][string]$ZipPath,
    [Parameter(Mandatory = $true)][string]$Version
)

$ErrorActionPreference = 'Stop'
$zip = (Resolve-Path -LiteralPath $ZipPath).Path
$repo = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..\..')).Path
$root = Join-Path ([IO.Path]::GetTempPath()) ("IndustrialPark_update_test_" + [guid]::NewGuid().ToString('N'))
$reference = Join-Path $root 'reference'
$app = Join-Path $root 'installation'
$oldBuild = Join-Path $root 'old-build'
$release = Join-Path $root 'release.json'
$job = $null
$bootstrap = $null
$check = $null
$rejectedCheck = $null
$finished = $false

try {
    New-Item -ItemType Directory -Path $root | Out-Null
    Expand-Archive -LiteralPath $zip -DestinationPath $reference
    Copy-Item -LiteralPath $reference -Destination $app -Recurse

    $packageVersion = [Diagnostics.FileVersionInfo]::GetVersionInfo((Join-Path $reference 'IndustrialPark.exe')).ProductVersion
    if ($packageVersion -ne $Version) { throw "Expected package version $Version, found $packageVersion." }
    $tag = 'v' + ($Version -replace '-.*$', '')

    & dotnet build (Join-Path $repo 'IndustrialPark\IndustrialPark.csproj') --configuration Release --no-restore -m:1 -p:IPBuildVersion=1.0.0-beta --output $oldBuild --verbosity quiet
    if ($LASTEXITCODE -ne 0) { throw 'Failed to build the older test installation.' }
    $oldExe = Join-Path $oldBuild 'IndustrialPark.exe'
    if ([Diagnostics.FileVersionInfo]::GetVersionInfo($oldExe).ProductVersion -ne '1.0.0-beta') {
        throw 'The test installation does not have the expected older version.'
    }
    Copy-Item -LiteralPath $oldExe -Destination (Join-Path $app 'IndustrialPark.exe') -Force
    New-Item -ItemType Directory -Path (Join-Path $app 'User') | Out-Null
    Set-Content -LiteralPath (Join-Path $app 'User\settings.txt') -Value 'preserve user data'
    Set-Content -LiteralPath (Join-Path $app 'Library\old-marker.txt') -Value 'preserve backup'

    $updater = Join-Path $app 'Updater.exe'
    $rejectedRelease = Join-Path $root 'rejected.json'
    $rejectedError = Join-Path $root 'rejected-error.txt'
    $rejectedCheck = Start-Process -FilePath $updater -ArgumentList @('--test-check', "`"$rejectedRelease`"", 'https://example.com/') -WorkingDirectory $app -RedirectStandardError $rejectedError -PassThru
    $null = $rejectedCheck.Handle
    if (-not $rejectedCheck.WaitForExit(30000)) { throw 'The non-loopback test check timed out.' }
    if ($rejectedCheck.ExitCode -ne 3 -or (Test-Path -LiteralPath $rejectedRelease) -or
        (Get-Content -LiteralPath $rejectedError -Raw) -notmatch 'loopback HTTP origin') {
        throw 'The test command did not reject a non-loopback release server.'
    }

    $job = Start-Job -ArgumentList $zip, $tag -ScriptBlock {
        param($archive, $releaseTag)
        $ErrorActionPreference = 'Stop'
        $listener = [Net.Sockets.TcpListener]::new([Net.IPAddress]::Loopback, 0)
        try {
            $listener.Start()
            $port = $listener.LocalEndpoint.Port
            Write-Output "PORT:$port"
            $origin = "http://127.0.0.1:$port"
            $seen = @()
            while ($seen.Count -lt 2) {
                $client = $listener.AcceptTcpClient()
                try {
                    $stream = $client.GetStream()
                    $reader = [IO.StreamReader]::new($stream, [Text.Encoding]::ASCII, $false, 1024, $true)
                    $request = $reader.ReadLine()
                    do { $header = $reader.ReadLine() } while ($null -ne $header -and $header.Length -gt 0)
                    $path = ($request -split ' ')[1]
                    if ($path -eq '/api/v3/repos/igorseabra4/IndustrialPark/releases/latest') {
                        $body = @{
                            tag_name = $releaseTag
                            name = "Test $releaseTag"
                            body = 'Local update test'
                            assets = @(
                                @{ name = 'other.zip'; browser_download_url = "$origin/other.zip" },
                                @{ name = "IndustrialPark_$releaseTag.zip"; browser_download_url = "$origin/asset.zip" }
                            )
                        } | ConvertTo-Json -Depth 5 -Compress
                        $bytes = [Text.Encoding]::UTF8.GetBytes($body)
                        $type = 'application/json'
                    } elseif ($path -eq '/asset.zip') {
                        $bytes = [IO.File]::ReadAllBytes($archive)
                        $type = 'application/zip'
                    } else {
                        throw "Unexpected test server request: $request"
                    }
                    $headers = [Text.Encoding]::ASCII.GetBytes("HTTP/1.1 200 OK`r`nContent-Type: $type`r`nContent-Length: $($bytes.Length)`r`nConnection: close`r`n`r`n")
                    $stream.Write($headers, 0, $headers.Length)
                    $stream.Write($bytes, 0, $bytes.Length)
                    $stream.Flush()
                    $seen += $path
                    Write-Output "REQUEST:$path"
                } finally {
                    $client.Dispose()
                }
            }
        } finally {
            $listener.Stop()
        }
    }

    $events = @()
    $deadline = [DateTime]::UtcNow.AddSeconds(30)
    while (-not ($events | Where-Object { $_ -like 'PORT:*' }) -and [DateTime]::UtcNow -lt $deadline) {
        $events += @(Receive-Job -Job $job)
        if ($job.State -eq 'Failed') { throw 'The fake release server failed to start.' }
        Start-Sleep -Milliseconds 100
    }
    $portEvent = $events | Where-Object { $_ -like 'PORT:*' } | Select-Object -First 1
    if (-not $portEvent) { throw 'The fake release server did not start.' }
    $origin = 'http://127.0.0.1:' + $portEvent.Substring(5) + '/'
    $checkError = Join-Path $root 'check-error.txt'
    $check = Start-Process -FilePath $updater -ArgumentList @('--test-check', "`"$release`"", $origin) -WorkingDirectory $app -RedirectStandardError $checkError -PassThru
    $null = $check.Handle
    if (-not $check.WaitForExit(60000)) { throw 'The updater check timed out.' }
    if ($check.ExitCode -ne 0) {
        $detail = Get-Content -LiteralPath $checkError -Raw
        $serverDetail = $job.ChildJobs[0].JobStateInfo.Reason
        throw "The updater check failed with exit code $($check.ExitCode): $detail`nServer: $serverDetail"
    }
    $selected = Get-Content -LiteralPath $release -Raw | ConvertFrom-Json
    if ($selected.TagName -ne $tag -or $selected.DownloadUrl -ne ($origin + 'asset.zip')) {
        throw 'The updater selected the wrong release or asset.'
    }

    $exited = Start-Process -FilePath $env:ComSpec -ArgumentList '/c exit 0' -Wait -PassThru
    $target = Join-Path $root 'restart.cmd'
    Set-Content -LiteralPath $target -Encoding ASCII -Value @('@echo off', '> "%~dp0restarted.txt" echo restarted')
    $bootstrap = Start-Process -FilePath $updater -ArgumentList @('--apply', "`"$target`"", "`"$release`"", $exited.Id.ToString()) -WorkingDirectory $app -PassThru
    $null = $bootstrap.Handle
    if (-not $bootstrap.WaitForExit(30000)) { throw 'The updater bootstrap timed out.' }
    if ($bootstrap.ExitCode -ne 0) { throw "The updater bootstrap failed with exit code $($bootstrap.ExitCode)." }
    $backup = Join-Path $app 'IndustrialPark_old'
    $deadline = [DateTime]::UtcNow.AddSeconds(90)
    do {
        Start-Sleep -Milliseconds 250
        $staging = @(Get-ChildItem -LiteralPath $app -Directory -Filter '.IndustrialPark_update_*')
        $finished = (Test-Path -LiteralPath (Join-Path $backup 'Library\old-marker.txt')) -and
            (Test-Path -LiteralPath (Join-Path $root 'restarted.txt')) -and $staging.Count -eq 0
    } while (-not $finished -and [DateTime]::UtcNow -lt $deadline)
    if (-not $finished) { throw 'The update did not finish.' }

    if (-not (Wait-Job -Job $job -Timeout 15)) { throw 'The fake release server did not finish.' }
    $events += @(Receive-Job -Job $job)
    if ($job.State -ne 'Completed' -or
        $events -notcontains 'REQUEST:/api/v3/repos/igorseabra4/IndustrialPark/releases/latest' -or
        $events -notcontains 'REQUEST:/asset.zip') {
        throw 'The updater did not query and download from the fake release server.'
    }
    foreach ($file in Get-ChildItem -LiteralPath $reference -File -Recurse) {
        $relative = $file.FullName.Substring($reference.Length).TrimStart('\')
        $installed = Join-Path $app $relative
        if (-not (Test-Path -LiteralPath $installed) -or
            (Get-FileHash -LiteralPath $file.FullName).Hash -ne (Get-FileHash -LiteralPath $installed).Hash) {
            throw "Installed file does not match the ZIP: $relative"
        }
    }
    if ((Get-FileHash -LiteralPath $oldExe).Hash -ne (Get-FileHash -LiteralPath (Join-Path $backup 'IndustrialPark.exe')).Hash) {
        throw 'The old executable was not backed up.'
    }
    foreach ($path in @((Join-Path $app 'User\settings.txt'), (Join-Path $backup 'User\settings.txt'))) {
        if ((Get-Content -LiteralPath $path -Raw).Trim() -ne 'preserve user data') { throw "User data was not preserved: $path" }
    }
    Write-Output 'End-to-end update test passed.'
}
finally {
    foreach ($process in @($rejectedCheck, $check, $bootstrap)) {
        if ($null -ne $process -and -not $process.HasExited) { Stop-Process -Id $process.Id -Force }
    }
    if ($null -ne $bootstrap -and -not $finished) {
        $workers = Get-CimInstance Win32_Process -Filter "Name = 'Updater.exe'" | Where-Object {
            $_.CommandLine -and $_.CommandLine.Contains('--install') -and
            $_.CommandLine.Contains($app) -and $_.CommandLine.Contains($release)
        }
        foreach ($worker in $workers) { Stop-Process -Id $worker.ProcessId -Force }
    }
    if ($null -ne $job) {
        if ($job.State -eq 'Running') { Stop-Job -Job $job }
        Remove-Job -Job $job -Force
    }
    if (Test-Path -LiteralPath $root) { Remove-Item -LiteralPath $root -Recurse -Force }
}
