[CmdletBinding()]
param(
    [switch]$SkipBuild,
    [switch]$SkipInstall,
    [int]$BackendPort = 5201,
    [int]$FrontendPort = 4200
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSCommandPath
$frontendRoot = Join-Path $repoRoot 'Frontend'
$backendProject = Join-Path $repoRoot 'Server\VideoGameCatalogue.Api\VideoGameCatalogue.Api.csproj'
$solutionPath = Join-Path $repoRoot 'VideoGameCatalogue.slnx'
$runRoot = Join-Path $repoRoot '.run'
$backendLog = Join-Path $runRoot 'backend.log'
$frontendLog = Join-Path $runRoot 'frontend.log'
$backendErrorLog = Join-Path $runRoot 'backend-error.log'
$frontendErrorLog = Join-Path $runRoot 'frontend-error.log'
$installStampFile = Join-Path $runRoot 'frontend-install.stamp'
$packageLockPath = Join-Path $frontendRoot 'package-lock.json'

function Test-PortAvailable {
    param([int]$Port)

    $listeners = Get-NetTCPConnection -State Listen -LocalPort $Port -ErrorAction SilentlyContinue
    return $null -eq $listeners
}

function Wait-ForHttpReady {
    param(
        [string]$Url,
        [int]$TimeoutSeconds = 90
    )

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)

    while ((Get-Date) -lt $deadline) {
        try {
            Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 5 | Out-Null
            return
        }
        catch {
            Start-Sleep -Milliseconds 500
        }
    }

    throw "Timed out waiting for $Url"
}

function Get-FileHashValue {
    param([string]$Path)

    if (-not (Test-Path $Path)) {
        return $null
    }

    return (Get-FileHash -Path $Path -Algorithm SHA256).Hash
}

if (-not (Test-Path $runRoot)) {
    New-Item -ItemType Directory -Path $runRoot | Out-Null
}

if (-not (Test-PortAvailable -Port $BackendPort)) {
    throw "Backend port $BackendPort is already in use. Stop the existing process and try again."
}

if (-not (Test-PortAvailable -Port $FrontendPort)) {
    throw "Frontend port $FrontendPort is already in use. Stop the existing process and try again."
}

if (-not $SkipInstall) {
    $currentLockHash = Get-FileHashValue -Path $packageLockPath
    $previousLockHash = if (Test-Path $installStampFile) { Get-Content -Path $installStampFile -Raw } else { $null }
    $requiresInstall =
        (-not (Test-Path (Join-Path $frontendRoot 'node_modules'))) -or
        [string]::IsNullOrWhiteSpace($currentLockHash) -or
        ($currentLockHash -ne $previousLockHash)

    if ($requiresInstall) {
        Push-Location $frontendRoot
        try {
            npm ci
        }
        finally {
            Pop-Location
        }

        if ($null -ne $currentLockHash) {
            Set-Content -Path $installStampFile -Value $currentLockHash
        }
    }
}

if (-not $SkipBuild) {
    Push-Location $repoRoot
    try {
        dotnet build $solutionPath
    }
    finally {
        Pop-Location
    }

    Push-Location $frontendRoot
    try {
        npm run build
    }
    finally {
        Pop-Location
    }
}

Remove-Item $backendLog, $frontendLog, $backendErrorLog, $frontendErrorLog -ErrorAction SilentlyContinue

$backendProcess = Start-Process `
    -FilePath 'dotnet' `
    -ArgumentList @('run', '--project', $backendProject, '--launch-profile', 'http', '--no-build') `
    -WorkingDirectory $repoRoot `
    -RedirectStandardOutput $backendLog `
    -RedirectStandardError $backendErrorLog `
    -PassThru

$frontendProcess = Start-Process `
    -FilePath 'npm.cmd' `
    -ArgumentList @('start', '--', '--host', '127.0.0.1', '--port', $FrontendPort.ToString()) `
    -WorkingDirectory $frontendRoot `
    -RedirectStandardOutput $frontendLog `
    -RedirectStandardError $frontendErrorLog `
    -PassThru

try {
    Wait-ForHttpReady -Url "http://127.0.0.1:$BackendPort/swagger/index.html"
    Wait-ForHttpReady -Url "http://127.0.0.1:$FrontendPort"
}
catch {
    foreach ($process in @($backendProcess, $frontendProcess)) {
        if ($null -ne $process -and -not $process.HasExited) {
            Stop-Process -Id $process.Id -Force
        }
    }

    throw
}

Write-Host ''
Write-Host 'Application is running.' -ForegroundColor Green
Write-Host "Frontend: http://127.0.0.1:$FrontendPort"
Write-Host "Backend:  http://127.0.0.1:$BackendPort/swagger"
Write-Host "Backend PID:  $($backendProcess.Id)"
Write-Host "Frontend PID: $($frontendProcess.Id)"
Write-Host "Backend logs:  $backendLog"
Write-Host "Frontend logs: $frontendLog"
Write-Host ''
Write-Host "To stop the app, run: Stop-Process -Id $($backendProcess.Id), $($frontendProcess.Id)"