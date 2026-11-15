$ErrorActionPreference = "Stop"
& "$PSScriptRoot/New-K3dCluster.ps1"
$root = Resolve-Path (Join-Path $PSScriptRoot "../../..")
$manifests = @(
    "namespace.yaml",
    "postgres.yaml",
    "api.yaml",
    "migration-job.yaml",
    "web.yaml",
    "ingress.yaml",
    "pdb.yaml",
    "hpa.yaml"
)
foreach ($manifest in $manifests) {
    kubectl apply -f (Join-Path $root "infrastructure/k8s/$manifest")
}
