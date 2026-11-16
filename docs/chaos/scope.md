# Chaos Mesh

Los experimentos viven en `infrastructure/k8s/chaos` y solo seleccionan pods del namespace `taskmanagement`.

`Deploy-Cluster.ps1` llama a `Install-ChaosMesh.ps1`. Si falta el CRD `podchaos.chaos-mesh.org`, el script descarga el instalador 2.7.2 del espejo, lo renderiza con `--template --k3s` y lo aplica. El controlador queda en el namespace `chaos-mesh`. La prueba espera a que al menos un pod del controlador esté listo. Los CRD de Schedule y Workflow pueden fallar al aplicarlos por el tamaño de la anotación; el `PodChaos` de la prueba no los usa.

`Invoke-Resilience.ps1` aplica `pod-kill.yaml`: mata dos pods de la API (`mode: fixed`, `value: "2"`) y espera a que el Deployment recupere las réplicas. La parada de los nodos no pasa por Chaos Mesh: un nodo k3d es un contenedor Docker, y el script los detiene con `docker stop`.

`postgres-kill.yaml` y `network-delay.yaml` están versionados y la suite no los aplica. Esos dos informes siguen inconclusos. No se publican tiempos de recuperación que no se midieron.
