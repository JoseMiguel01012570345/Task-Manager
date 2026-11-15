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
dotnet test TaskManagement.sln --filter "FullyQualifiedName!~K6ScenarioTests"
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

dotnet test tests/Integration/TaskManagement.IntegrationTests.csproj --filter "FullyQualifiedName~K6ScenarioTests"
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}
