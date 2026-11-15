# Generadores

k6 puede correr en el host o como Job dentro del namespace. El Job usa la imagen `grafana/k6` y los scripts montados desde un ConfigMap. Así el tráfico nace en el clúster y no en el portátil, que es la forma de no medir solo el enlace local.

```powershell
kubectl create configmap k6-scripts --from-file=tests/k6 -n taskmanagement
kubectl apply -f infrastructure/k8s/k6-job.yaml
```

En esta máquina `k6` no formaba parte del PATH y el clúster k3d no estaba creado, así que los escenarios no se ejecutaron. No hay cifras de rendimiento que reportar.
