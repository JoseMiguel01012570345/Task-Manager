$ErrorActionPreference = "Stop"
$namespace = "taskmanagement"
$base = "http://localhost:8088"

function Write-Step([string]$title) {
    Write-Host ""
    Write-Host "== $title =="
}

function Show-Workloads {
    kubectl get nodes -o wide
    if ($LASTEXITCODE -ne 0) { throw "kubectl get nodes fallo con $LASTEXITCODE" }
    kubectl get pods -n $namespace -o wide
    if ($LASTEXITCODE -ne 0) { throw "kubectl get pods fallo con $LASTEXITCODE" }
}

function Show-ApiLogs {
    Write-Host "-- logs app=api --"
    kubectl logs -n $namespace -l app=api --tail=30 --prefix=true --all-containers=true
    if ($LASTEXITCODE -ne 0) {
        Write-Host "No se pudieron leer los logs de la API (codigo $LASTEXITCODE)."
    }
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
$listed = kubectl get pods -n $namespace -l app=api --field-selector=status.phase=Running -o jsonpath="{.items[*].metadata.name}"
if ($LASTEXITCODE -ne 0) { throw "No se pudieron listar los pods de la API." }
$apiPods = @($listed -split '\s+' | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
if ($apiPods.Count -lt 2) {
    throw "Hacen falta dos pods de la API en ejecucion y hay $($apiPods.Count)."
}
$targets = @($apiPods | Select-Object -First 2)
Write-Host "Eliminando $($targets -join ', ')"
kubectl delete pod -n $namespace @targets --wait=false
if ($LASTEXITCODE -ne 0) { throw "No se pudieron eliminar los pods de la API." }
Show-Workloads
kubectl rollout status deployment/api -n $namespace --timeout=180s
if ($LASTEXITCODE -ne 0) { throw "La API no recupero sus dos replicas." }
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
    kubectl wait --for=condition=Ready "node/$agent" --timeout=180s
    if ($LASTEXITCODE -ne 0) { throw "El nodo $agent no volvio a Ready." }
}
kubectl rollout status deployment/api -n $namespace --timeout=180s
if ($LASTEXITCODE -ne 0) { throw "La API no quedo lista despues de recuperar los agentes." }
if (-not (Test-Ingress "/health/ready" 20)) {
    throw "La API no quedo lista en $base/health/ready."
}

Write-Step "Estado despues de recuperar los nodos"
Show-Workloads
Show-ApiLogs
Write-Host "Resiliencia comprobada: los dos agentes cayeron y el nodo que quedo siguio respondiendo."
