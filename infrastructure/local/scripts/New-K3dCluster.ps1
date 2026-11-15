$ErrorActionPreference = "Stop"
$k3d = Get-Command k3d -ErrorAction SilentlyContinue
if (-not $k3d) {
    throw "k3d no esta en PATH. Instala k3d 5.8 y vuelve a ejecutar este script."
}

& k3d cluster create --config "$PSScriptRoot/../k3d/cluster.yaml"
kubectl config use-context k3d-taskmanagement
kubectl get nodes
