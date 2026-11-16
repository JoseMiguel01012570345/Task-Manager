$ErrorActionPreference = "Stop"

$crd = kubectl get crd podchaos.chaos-mesh.org --ignore-not-found -o name
if ($LASTEXITCODE -ne 0) { throw "No se pudo consultar el CRD de Chaos Mesh." }
if (-not $crd) {
    $installer = Join-Path $env:TEMP "chaos-mesh-install.sh"
    $manifest = Join-Path $env:TEMP "chaos-mesh.yaml"
    curl.exe -fsSL -o $installer "https://mirrors.chaos-mesh.org/v2.7.2/install.sh"
    if ($LASTEXITCODE -ne 0) { throw "No se pudo descargar el instalador de Chaos Mesh." }
    $drive = $installer.Substring(0, 1).ToLower()
    $wslInstaller = "/mnt/$drive" + ($installer.Substring(2) -replace '\\', '/')
    $wslManifest = "/mnt/$drive" + ($manifest.Substring(2) -replace '\\', '/')
    wsl -e bash -lc "bash '$wslInstaller' --template --k3s > '$wslManifest'"
    if ($LASTEXITCODE -ne 0) { throw "No se pudieron generar los manifiestos de Chaos Mesh." }
    kubectl apply -f $manifest
    if ($LASTEXITCODE -ne 0) { throw "kubectl apply de Chaos Mesh fallo con $LASTEXITCODE" }
}

$deadline = (Get-Date).AddSeconds(180)
$ready = $false
while ((Get-Date) -lt $deadline) {
    $states = kubectl get pods -n chaos-mesh -l app.kubernetes.io/component=controller-manager -o jsonpath="{range .items[*]}{.status.containerStatuses[0].ready}{'\n'}{end}"
    if ($LASTEXITCODE -ne 0) { throw "No se pudo consultar el controlador de Chaos Mesh." }
    if ($states -match "true") { $ready = $true; break }
    Start-Sleep -Seconds 3
}
if (-not $ready) { throw "El controlador de Chaos Mesh no quedo listo." }
