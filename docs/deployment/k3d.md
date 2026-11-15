# Clúster local

El laboratorio de Kubernetes es un clúster k3d con un servidor y dos agentes. k3s no instala su Traefik: el ingreso lo define el manifiesto del proyecto. El balanceador publica el puerto 8088, el mismo ingreso que usa Compose.

```powershell
./infrastructure/local/scripts/Deploy-Cluster.ps1
./infrastructure/local/scripts/Remove-K3dCluster.ps1
```

Hace falta k3d 5.8 en PATH y un Docker que pueda crear los nodos. `kubectl` del cliente basta para hablar con el contexto `k3d-taskmanagement`.
