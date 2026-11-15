$ErrorActionPreference = "Stop"
$root = Resolve-Path (Join-Path $PSScriptRoot "../../..")
$cluster = & k3d cluster list -o json | ConvertFrom-Json
$existing = @($cluster) | Where-Object { $_.name -eq "taskmanagement" }
if (-not $existing) {
    if (-not (Get-Command k3d -ErrorAction SilentlyContinue)) {
        throw "k3d no esta en PATH. Instala k3d 5.8 y vuelve a ejecutar este script."
    }
    & k3d cluster create --config "$PSScriptRoot/../k3d/cluster.yaml"
    $kubeconfig = Join-Path $env:USERPROFILE ".kube\config"
    if (Test-Path $kubeconfig) {
        $text = [IO.File]::ReadAllText($kubeconfig)
        $text = $text -replace 'https://host\.docker\.internal:', 'https://127.0.0.1:'
        $utf8 = New-Object System.Text.UTF8Encoding $false
        [IO.File]::WriteAllText($kubeconfig, $text, $utf8)
    }
}
kubectl config use-context k3d-taskmanagement | Out-Null

$images = @(
    "taskmanagement-api:local",
    "taskmanagement-web:local",
    "postgres:16-alpine",
    "quay.io/keycloak/keycloak:26.0",
    "traefik:v3.2.0"
)
foreach ($image in $images) {
    & k3d image import $image -c taskmanagement
}
kubectl apply -f (Join-Path $root "infrastructure/k8s/ingress-controller.yaml")
kubectl wait --for=condition=ready pod -l app=traefik-ingress -n kube-system --timeout=180s

$beforeMigrate = @(
    "namespace.yaml",
    "postgres.yaml",
    "api.yaml",
    "keycloak.yaml"
)
$afterMigrate = @(
    "web.yaml",
    "ingress.yaml",
    "pdb.yaml",
    "hpa.yaml",
    "backup.yaml"
)

$envFile = Join-Path $root ".env"
$adminPassword = "lab-keycloak-admin"
if (Test-Path $envFile) {
    foreach ($line in Get-Content $envFile) {
        if ($line -match '^\s*KEYCLOAK_ADMIN_PASSWORD=(.*)$') {
            $adminPassword = $Matches[1].Trim()
        }
    }
}

kubectl apply -f (Join-Path $root "infrastructure/k8s/namespace.yaml")
$realmFile = Join-Path $root "infrastructure/local/docker/keycloak/generated/taskmanagement-realm.json"
if (-not (Test-Path $realmFile)) {
    & (Join-Path $root "infrastructure/local/docker/keycloak/render-realm.ps1")
}
kubectl create configmap keycloak-realm -n taskmanagement --from-file="taskmanagement-realm.json=$realmFile" --dry-run=client -o yaml | kubectl apply -f -
kubectl create secret generic keycloak-admin -n taskmanagement --from-literal="password=$adminPassword" --dry-run=client -o yaml | kubectl apply -f -

foreach ($manifest in $beforeMigrate) {
    kubectl apply -f (Join-Path $root "infrastructure/k8s/$manifest")
}

kubectl wait --for=condition=ready pod -l app=postgres -n taskmanagement --timeout=180s
kubectl apply -f (Join-Path $root "infrastructure/k8s/migration-job.yaml")
kubectl wait --for=condition=complete job/migrate -n taskmanagement --timeout=180s

foreach ($manifest in $afterMigrate) {
    kubectl apply -f (Join-Path $root "infrastructure/k8s/$manifest")
}
kubectl delete job postgres-backup-now -n taskmanagement --ignore-not-found
kubectl create job postgres-backup-now -n taskmanagement --from=cronjob/postgres-backup
kubectl wait --for=condition=ready pod -l app=api -n taskmanagement --timeout=180s
kubectl wait --for=condition=ready pod -l app=web -n taskmanagement --timeout=180s
kubectl wait --for=condition=complete job/postgres-backup-now -n taskmanagement --timeout=180s
kubectl get pods -n taskmanagement -o wide
