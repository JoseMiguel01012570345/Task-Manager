$ErrorActionPreference = "Stop"
$root = Resolve-Path (Join-Path $PSScriptRoot "..\..\..")
docker run --rm -v "${root}:/src" aquasec/trivy:0.58.1 fs --severity CRITICAL --ignore-unfixed --exit-code 1 /src
if ($LASTEXITCODE -ne 0) {
    throw "Trivy reported critical vulnerabilities."
}
