$ErrorActionPreference = "Stop"
$namespace = "taskmanagement"
$base = "http://localhost:8088"

function Write-Step([string]$title) {
    Write-Host ""
    Write-Host "== $title =="
}

function Invoke-Kubectl([string[]]$arguments) {
    Invoke-KubectlOutput $arguments | Out-Host
}

function Invoke-KubectlOutput([string[]]$arguments) {
    for ($attempt = 0; $attempt -lt 5; $attempt++) {
        $previous = $ErrorActionPreference
        $ErrorActionPreference = "Continue"
        $output = & kubectl @arguments 2>&1
        $code = $LASTEXITCODE
        $ErrorActionPreference = $previous
        if ($code -eq 0) {
            $text = $output | Where-Object { $_ -isnot [System.Management.Automation.ErrorRecord] }
            return ($text | Out-String).Trim()
        }
        if ($attempt -eq 1) {
            $ErrorActionPreference = "Continue"
            docker restart k3d-taskmanagement-serverlb | Out-Null
            $ErrorActionPreference = $previous
        }
        Start-Sleep -Seconds 5
    }
    throw "kubectl $($arguments -join ' ') fallo"
}

function Show-Workloads {
    Invoke-Kubectl @("get", "nodes", "-o", "wide", "--request-timeout=20s")
    Invoke-Kubectl @("get", "pods", "-n", $namespace, "-o", "wide", "--request-timeout=20s")
}

function Show-ApiLogs {
    Write-Host "-- logs app=api --"
    $previous = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    kubectl logs -n $namespace -l app=api --tail=30 --prefix=true --all-containers=true
    if ($LASTEXITCODE -ne 0) {
        Write-Host "No se pudieron leer los logs de la API (codigo $LASTEXITCODE)."
    }
    $ErrorActionPreference = $previous
}

function Test-Ingress([string]$path, [int]$attempts = 8) {
    $url = "$base$path"
    for ($attempt = 0; $attempt -lt $attempts; $attempt++) {
        try {
            $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 5
            Write-Host "$url -> $($response.StatusCode)"
            if ($response.StatusCode -eq 200) { return $true }
        }
        catch {
            Write-Host "$url -> $($_.Exception.Message)"
        }
        Start-Sleep -Seconds 2
    }
    return $false
}

Write-Step "Estado antes de los fallos"
Show-Workloads

Write-Step "Caida de dos pods de la API"
& (Join-Path $PSScriptRoot "Install-ChaosMesh.ps1")
$podKill = Join-Path $PSScriptRoot "..\..\k8s\chaos\pod-kill.yaml"
Invoke-Kubectl @("delete", "-f", $podKill, "--ignore-not-found")
try {
    Invoke-Kubectl @("apply", "-f", $podKill)
    $injected = $false
    for ($attempt = 0; $attempt -lt 30; $attempt++) {
        $status = Invoke-KubectlOutput @("get", "podchaos", "api-pod-kill", "-n", $namespace, "--request-timeout=20s", "-o", "jsonpath={.status.conditions[?(@.type=='AllInjected')].status}")
        if ($status -eq "True") { $injected = $true; break }
        Start-Sleep -Seconds 2
    }
    if (-not $injected) { throw "Chaos Mesh no confirmo la caida de los dos pods." }
    Show-Workloads
    Invoke-Kubectl @("rollout", "status", "deployment/api", "-n", $namespace, "--timeout=180s")
}
finally {
    Invoke-Kubectl @("delete", "-f", $podKill, "--ignore-not-found")
}
if (-not (Test-Ingress "/health/live")) {
    throw "El ingreso no respondio despues de recrear los dos pods."
}
Show-ApiLogs

Write-Step "Parada de dos nodos"
$agents = @("k3d-taskmanagement-agent-0", "k3d-taskmanagement-agent-1")
Write-Host "Se detienen los dos agentes. El plano de control sigue y es el nodo que debe atender."
$stopped = @()
try {
    foreach ($agent in $agents) {
        docker stop $agent | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "docker stop $agent fallo con $LASTEXITCODE" }
        $stopped += $agent
    }
    Start-Sleep -Seconds 5
    Show-Workloads
    if (-not (Test-Ingress "/health/ready" 20)) {
        throw "El nodo que quedaba no respondio en $base/health/ready."
    }
    Show-ApiLogs
}
finally {
    foreach ($agent in $stopped) {
        Write-Host "Arrancando $agent"
        docker start $agent | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "docker start $agent fallo con $LASTEXITCODE" }
    }
}

foreach ($agent in $agents) {
    Invoke-Kubectl @("wait", "--for=condition=Ready", "node/$agent", "--timeout=180s")
}
Invoke-Kubectl @("rollout", "status", "deployment/api", "-n", $namespace, "--timeout=180s")
if (-not (Test-Ingress "/health/ready" 20)) {
    throw "La API no quedo lista en $base/health/ready."
}
. (Join-Path $PSScriptRoot "Wait-Keycloak.ps1")
Wait-KeycloakToken $base

Write-Step "Estado despues de recuperar los nodos"
Show-Workloads
Show-ApiLogs
Write-Host "Resiliencia comprobada: los dos agentes cayeron y el nodo que quedo siguio respondiendo."
