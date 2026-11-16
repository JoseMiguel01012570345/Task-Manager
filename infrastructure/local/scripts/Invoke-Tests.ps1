$ErrorActionPreference = "Stop"
$root = Resolve-Path (Join-Path $PSScriptRoot "../../..")
Set-Location $root

if (-not (Get-Command kubectl -ErrorAction SilentlyContinue)) {
    throw "kubectl no esta en PATH."
}

kubectl config use-context k3d-taskmanagement | Out-Null
kubectl get nodes --request-timeout=10s | Out-Null

& (Join-Path $PSScriptRoot "Connect-Edge.ps1")
$base = "http://localhost:8088"

$envFile = Join-Path $root ".env"
if (Test-Path $envFile) {
    foreach ($line in Get-Content $envFile) {
        if ($line -match '^\s*(DEV_USER_PASSWORD|ADMIN_USER_PASSWORD)=(.*)$') {
            Set-Item -Path "Env:$($Matches[1])" -Value $Matches[2].Trim()
        }
    }
}

$env:TASKMANAGEMENT_BASE_URL = $base
$env:BASE_URL = $base

function Invoke-DotNet([string[]]$arguments) {
    dotnet @arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet fallo con codigo $LASTEXITCODE."
    }
}
Write-Host ""
Write-Host "== Infraestructura =="
Write-Host "El frontend crea tareas mientras corren k6 y, despues, la resiliencia."

$e2eDir = Join-Path $root "tests/e2e"
$npm = (Get-Command npm.cmd -ErrorAction SilentlyContinue).Source
if (-not $npm) {
    throw "npm no esta en PATH."
}
Push-Location $e2eDir
if (-not (Test-Path "node_modules/@playwright/test")) {
    npm ci
    if ($LASTEXITCODE -ne 0) {
        Pop-Location
        throw "npm ci fallo con codigo $LASTEXITCODE."
    }
}
Pop-Location

$stamp = [guid]::NewGuid().ToString("n")
$flag = Join-Path $env:TEMP "taskmanagement-ops-$stamp"
$ready = "$flag.ready"
$e2eOut = Join-Path $env:TEMP "taskmanagement-e2e-$stamp.out"
$e2eErr = Join-Path $env:TEMP "taskmanagement-e2e-$stamp.err"
New-Item -Path $flag -ItemType File -Force | Out-Null
$env:OPS_FLAG = $flag
$env:OPS_READY = $ready

$infraFailed = $null
$e2e = $null
try {
    $e2e = Start-Process -FilePath "cmd.exe" -ArgumentList @("/c", "npm exec -- playwright test frontend.spec.ts --reporter=line") -WorkingDirectory $e2eDir -PassThru -RedirectStandardOutput $e2eOut -RedirectStandardError $e2eErr -WindowStyle Hidden
    $deadline = (Get-Date).AddMinutes(3)
    while (-not (Test-Path $ready)) {
        if ($e2e.HasExited) {
            throw "Playwright termino antes de operar el frontend. Revisa $e2eOut y $e2eErr."
        }
        if ((Get-Date) -gt $deadline) {
            throw "El frontend no llego a abrir. Revisa $e2eOut y $e2eErr."
        }
        Start-Sleep -Seconds 1
    }

    Invoke-DotNet @(
        "test", "tests/Integration/TaskManagement.IntegrationTests.csproj", "--nologo",
        "--filter", "FullyQualifiedName~K6ScenarioTests"
    )
    & (Join-Path $PSScriptRoot "Invoke-Resilience.ps1")
}
catch {
    $infraFailed = $_
}
finally {
    if (Test-Path $flag) {
        Remove-Item $flag -Force
    }
    if ($e2e -and -not $e2e.HasExited) {
        if (-not $e2e.WaitForExit(180000)) {
            Stop-Process -Id $e2e.Id -Force -ErrorAction SilentlyContinue
            $infraFailed = "El frontend no termino despues de k6 y la resiliencia."
        }
    }
    if ($e2e) {
        $e2e.WaitForExit() | Out-Null
        $e2e.Refresh()
    }
    Remove-Item $ready -Force -ErrorAction SilentlyContinue
}

if ($infraFailed) {
    Write-Host "Salida del frontend:"
    if (Test-Path $e2eOut) { Get-Content $e2eOut -Tail 40 }
    if (Test-Path $e2eErr) { Get-Content $e2eErr -Tail 40 }
    throw $infraFailed
}
$frontendCode = 0
if ($e2e) {
    $frontendCode = $e2e.ExitCode
}
if ($null -eq $frontendCode) {
    $frontendLog = ""
    if (Test-Path $e2eOut) { $frontendLog += Get-Content $e2eOut -Raw }
    if (Test-Path $e2eErr) { $frontendLog += Get-Content $e2eErr -Raw }
    if ($frontendLog -match "(?m)^\s*\d+ failed") {
        $frontendCode = 1
    } else {
        $frontendCode = 0
    }
}
if ($frontendCode -ne 0) {
    Write-Host "Salida del frontend:"
    if (Test-Path $e2eOut) { Get-Content $e2eOut -Tail 40 }
    if (Test-Path $e2eErr) { Get-Content $e2eErr -Tail 40 }
    exit $frontendCode
}

Write-Host ""
Write-Host "== Seguridad =="
Invoke-DotNet @("test", "tests/Architecture/TaskManagement.ArchitectureTests.csproj", "--nologo")
Invoke-DotNet @("test", "tests/Unit/TaskManagement.UnitTests.csproj", "--nologo")
Invoke-DotNet @(
    "test", "tests/Integration/TaskManagement.IntegrationTests.csproj", "--nologo",
    "--filter", "FullyQualifiedName~TaskApiTests|FullyQualifiedName~ConcurrentUpdateTests|FullyQualifiedName~ErrorContractTests|FullyQualifiedName~SolutionSmokeTests"
)
& (Join-Path $PSScriptRoot "Invoke-Trivy.ps1")
Invoke-DotNet @(
    "test", "tests/Integration/TaskManagement.IntegrationTests.csproj", "--nologo",
    "--filter", "FullyQualifiedName~SecurityHeaderTests|FullyQualifiedName~RateLimitTests|FullyQualifiedName~CorsTests|FullyQualifiedName~AccessTests|FullyQualifiedName~KeycloakRoleTests|FullyQualifiedName~JsonRequestTests"
)
& (Join-Path $PSScriptRoot "Invoke-ZapBaseline.ps1")

