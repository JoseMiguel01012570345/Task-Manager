$ErrorActionPreference = "Stop"
$config = docker compose -f "$PSScriptRoot/docker-compose.yml" config --format json | ConvertFrom-Json

foreach ($service in $config.services.PSObject.Properties) {
    if ($service.Name -eq "traefik") {
        continue
    }

    $ports = $service.Value.ports
    if ($null -ne $ports -and @($ports).Count -gt 0) {
        throw "$($service.Name) publica puertos y evita el ingreso de Traefik."
    }
}

Write-Output "El unico servicio con puertos publicados es Traefik."
