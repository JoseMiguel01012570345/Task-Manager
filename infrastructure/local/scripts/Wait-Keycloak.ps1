$script:KeycloakEnvFile = Join-Path $PSScriptRoot "..\..\..\.env"

function Wait-KeycloakToken([string]$base) {
    $password = $env:DEV_USER_PASSWORD
    if (-not $password -and (Test-Path $script:KeycloakEnvFile)) {
        foreach ($line in Get-Content $script:KeycloakEnvFile) {
            if ($line -match '^\s*DEV_USER_PASSWORD=(.*)$') {
                $password = $Matches[1].Trim()
            }
        }
    }
    if (-not $password) { $password = "lab-dev-user" }

    $body = "grant_type=password&client_id=taskmanagement-web&username=dev-user&password=$([uri]::EscapeDataString($password))"
    $deadline = (Get-Date).AddMinutes(4)
    while ((Get-Date) -lt $deadline) {
        try {
            $response = Invoke-WebRequest -Uri "$base/realms/taskmanagement/protocol/openid-connect/token" -Method POST -ContentType "application/x-www-form-urlencoded" -Body $body -UseBasicParsing -TimeoutSec 10
            if ($response.StatusCode -eq 200 -and $response.Content -match "access_token") {
                Write-Host "Keycloak entrega tokens en $base"
                return
            }
        }
        catch {
            Write-Host "Keycloak todavia no entrega tokens."
        }
        Start-Sleep -Seconds 3
    }
    throw "Keycloak no entrego un token en $base."
}
