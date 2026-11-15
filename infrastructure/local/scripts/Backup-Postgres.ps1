param(
    [string]$Container = "database-postgres-1",
    [switch]$Cluster
)

$ErrorActionPreference = "Stop"
$root = Resolve-Path (Join-Path $PSScriptRoot "../../..")
$destDir = Join-Path $root "infrastructure/local/backups"
New-Item -ItemType Directory -Force -Path $destDir | Out-Null
$file = Join-Path $destDir ("taskmanagement-" + (Get-Date -Format "yyyyMMdd-HHmmss") + ".sql")

if ($Cluster) {
    kubectl exec -n taskmanagement postgres-0 -- pg_dump -U taskmanagement --no-owner taskmanagement | Set-Content -Path $file -Encoding utf8
}
else {
    docker exec $Container pg_dump -U taskmanagement --no-owner taskmanagement | Set-Content -Path $file -Encoding utf8
}

Write-Output $file
