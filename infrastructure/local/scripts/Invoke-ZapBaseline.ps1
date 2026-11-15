param(
    [string]$Target = "http://host.docker.internal:8088",
    [string]$Report = "docs/security/zap-baseline-report.html"
)

$ErrorActionPreference = "Stop"
docker run --rm -t ghcr.io/zaproxy/zaproxy:stable zap-baseline.py -t $Target -I
if ($LASTEXITCODE -ne 0 -and $LASTEXITCODE -ne 2) {
    throw "ZAP no pudo completar la linea base. Codigo $LASTEXITCODE."
}
