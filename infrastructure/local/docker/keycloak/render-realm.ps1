$ErrorActionPreference = "Stop"
$root = Resolve-Path (Join-Path $PSScriptRoot "..\..\..\..")
$envFile = Join-Path $root ".env"
if (-not (Test-Path $envFile)) {
    throw "Copy .env.example to .env before rendering the Keycloak realm."
}

$values = @{}
Get-Content $envFile | ForEach-Object {
    if ($_ -match '^\s*#' -or $_ -match '^\s*$') { return }
    $pair = $_ -split '=', 2
    if ($pair.Length -eq 2) { $values[$pair[0].Trim()] = $pair[1].Trim() }
}

$required = @("KEYCLOAK_API_CLIENT_SECRET", "DEV_USER_PASSWORD", "ADMIN_USER_PASSWORD")
foreach ($name in $required) {
    if ([string]::IsNullOrWhiteSpace($values[$name])) {
        throw "Missing $name in .env"
    }
}

$template = Get-Content (Join-Path $PSScriptRoot "taskmanagement-realm.template.json") -Raw
$rendered = $template.
    Replace("__API_CLIENT_SECRET__", $values["KEYCLOAK_API_CLIENT_SECRET"]).
    Replace("__DEV_USER_PASSWORD__", $values["DEV_USER_PASSWORD"]).
    Replace("__ADMIN_USER_PASSWORD__", $values["ADMIN_USER_PASSWORD"])

$outDir = Join-Path $PSScriptRoot "generated"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
$outFile = Join-Path $outDir "taskmanagement-realm.json"
Set-Content -Path $outFile -Value $rendered -Encoding utf8
Write-Output $outFile
