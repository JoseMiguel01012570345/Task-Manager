## Test: parada de los dos agentes

- Fecha: 2026-11-16
- Entorno: clúster k3d `taskmanagement`
- Nodos K3d: 1 servidor y 2 agentes
- Réplicas de API: 3
- Réplicas de frontend: 3
- Perfil de carga: el frontend sigue creando tareas. k6 no corre durante esta parada.

### Hipótesis

Con los dos agentes detenidos, el servidor sigue respondiendo en `http://localhost:8088/health/ready`. Al arrancarlos, los nodos vuelven a Ready y la API queda lista.

### Procedimiento

Después del `PodChaos`, `Invoke-Resilience.ps1` ejecuta `docker stop` de `k3d-taskmanagement-agent-0` y `k3d-taskmanagement-agent-1`. Comprueba `/health/ready` hasta 20 veces. En el `finally` hace `docker start` de los agentes que llegó a parar, espera cada nodo Ready (180 s), el rollout de la API, otra vez `/health/ready` y un token de Keycloak.

### Métricas

No se anotaron percentiles. El criterio es el 200 de `/health/ready` con los agentes parados y, al volver, nodos Ready, rollout y token.

### Integridad

La suite pide otra alta del frontend cuando la bandera de la ventana ya no existe, con hasta tres minutos de reintento.

### Resultado

- [x] Aprobado
- [ ] Fallido
- [ ] Inconcluso

La misma corrida del 2026-11-16 terminó con código 0.

### Observaciones y limitaciones

No se apaga el servidor. Keycloak no está en ese nodo: al perder los dos agentes hay que esperar a que vuelva a importar el realm. Si `kubectl` pierde el enlace con el balanceador de k3d, el script reintenta y, en el segundo intento, reinicia `k3d-taskmanagement-serverlb`.
