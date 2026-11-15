$ErrorActionPreference = "Stop"
Set-Location (Resolve-Path (Join-Path $PSScriptRoot "../../.."))
docker compose down
docker compose -f infrastructure/edge/docker-compose.yml down
