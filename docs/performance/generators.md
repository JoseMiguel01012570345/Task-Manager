# Generadores

k6 puede correr en el host o como Job dentro del namespace. El Job usa la imagen `grafana/k6` y los scripts montados desde un ConfigMap. Así el tráfico nace en el clúster y no en el portátil, que es la forma de no medir solo el enlace local.

```powershell
kubectl create configmap k6-scripts --from-file=tests/k6 -n taskmanagement
kubectl apply -f infrastructure/k8s/k6-job.yaml
```

`Invoke-Tests.ps1` los ejecuta en el host, con `SUITE=1` y `BASE_URL=http://localhost:8088`. `mixed.js` lleva un token de `dev-user`. Si un escenario sale distinto de 0, se reintenta una vez. No se copian percentiles a este documento: el criterio de la suite es el código de salida.
