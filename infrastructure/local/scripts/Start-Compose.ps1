$ErrorActionPreference = "Stop"
Set-Location (Resolve-Path (Join-Path $PSScriptRoot "../../.."))
docker compose -f infrastructure/edge/docker-compose.yml up -d
docker compose up -d
