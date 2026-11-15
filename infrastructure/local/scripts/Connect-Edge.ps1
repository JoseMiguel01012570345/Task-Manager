$ErrorActionPreference = "Stop"

$lbName = "k3d-taskmanagement-serverlb"
$edgeName = "taskmanagement-edge-traefik"
$network = "k3d-taskmanagement"
$edgeFile = Resolve-Path (Join-Path $PSScriptRoot "..\..\edge\docker-compose.yml")

docker inspect $lbName 1>$null 2>$null
if ($LASTEXITCODE -ne 0) {
    throw "No está el balanceador $lbName. Ejecuta Deploy-Cluster.ps1."
}

$published = @(docker port $lbName)
if ($LASTEXITCODE -ne 0) {
    throw "No se pudo leer los puertos de $lbName."
}
$appPort = @($published | Where-Object { $_ -match '^80/tcp\b' })
if ($appPort.Count -gt 0) {
    throw "El balanceador de k3d publica el ingreso en el host ($($appPort -join ', ')). Elimina el clúster con Remove-K3dCluster.ps1 y vuelve a crearlo: la aplicación solo entra por Traefik."
}

docker compose -f $edgeFile up -d
if ($LASTEXITCODE -ne 0) {
    throw "No se pudo levantar Traefik."
}

$format = '{{range $n, $net := .NetworkSettings.Networks}}{{println $n}}{{end}}'
$joined = @(docker inspect -f $format $edgeName) -join "`n"
if ($LASTEXITCODE -ne 0) {
    throw "No está el contenedor $edgeName."
}
if ($joined -notmatch "(?m)^$network\s*$") {
    docker network connect $network $edgeName
    if ($LASTEXITCODE -ne 0) {
        throw "Traefik no pudo unirse a la red $network."
    }
    docker restart $edgeName | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "No se pudo reiniciar Traefik tras unirlo al clúster."
    }
}

$ready = $false
for ($attempt = 0; $attempt -lt 40; $attempt++) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:8088/health/ready" -UseBasicParsing -TimeoutSec 5
        if ($response.StatusCode -eq 200) {
            $ready = $true
            break
        }
    }
    catch {
        Start-Sleep -Seconds 3
    }
}

if (-not $ready) {
    throw "Traefik no responde en http://localhost:8088/health/ready."
}
