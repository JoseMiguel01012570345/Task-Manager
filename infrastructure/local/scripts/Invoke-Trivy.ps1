$ErrorActionPreference = "Stop"
$root = Resolve-Path (Join-Path $PSScriptRoot "..\..\..")
$log = Join-Path $env:TEMP "taskmanagement-trivy.txt"

docker run --rm `
    -v "${root}:/src" `
    -v "taskmanagement-trivy:/root/.cache" `
    aquasec/trivy:0.58.1 fs `
    --severity CRITICAL `
    --ignore-unfixed `
    --timeout 20m `
    --quiet `
    --exit-code 1 `
    /src *> $log
$code = $LASTEXITCODE
Get-Content $log -Tail 80
if ($code -eq 0) {
    return
}

$downloadFailed = Select-String -Path $log -Pattern "failed to download vulnerability DB" -Quiet
if ($downloadFailed) {
    throw "Trivy no pudo descargar la base de vulnerabilidades."
}

throw "Trivy reported critical vulnerabilities."
