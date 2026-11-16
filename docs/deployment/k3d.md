# Clúster local

El laboratorio de Kubernetes es un clúster k3d con un servidor y dos agentes. k3s no instala su Traefik: el ingreso lo define el manifiesto del proyecto. El balanceador no publica ese ingreso en el host. La aplicación se abre solo por Traefik, en `http://localhost:8088`.

```powershell
./infrastructure/local/scripts/Deploy-Cluster.ps1
./infrastructure/local/scripts/Remove-K3dCluster.ps1
```

Hace falta k3d 5.8 en PATH y un Docker que pueda crear los nodos. `kubectl` del cliente habla con el contexto `k3d-taskmanagement`. El balanceador del clúster publica el API de Kubernetes, no el puerto 80 del host.

El script importa las imágenes locales, aplica los manifiestos, espera a PostgreSQL, la migración, la API y el frontend, instala Chaos Mesh y conecta el Traefik del borde. La API y el frontend piden 3 réplicas. PostgreSQL también: una acepta escrituras y las otras replican. Keycloak es una réplica, fuera del plano de control.

Si `kubectl` no completa el saludo TLS contra `127.0.0.1`, los scripts de prueba reintentan y pueden reiniciar el contenedor `k3d-taskmanagement-serverlb`. Eso no arregla un motor de Docker caído.
