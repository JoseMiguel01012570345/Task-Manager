## Test: TASK-062 parada de un nodo

- Fecha: 2026-11-15
- Commit / versión: ver `git log` de TASK-062
- Entorno: laboratorio local, sin clúster k3d
- Nodos K3d: 0
- CPU y memoria del host: no medidas
- Réplicas de API: 2 en el manifiesto
- Réplicas de frontend: 2 en el manifiesto
- Duración: no aplica
- Perfil de carga: ninguno

### Hipótesis

Al parar un agente, las réplicas del otro nodo siguen respondiendo y el pod desplazado vuelve a programarse.

### Procedimiento

Seguir `docs/chaos/node.md`: `docker stop` de un agente y después `docker start`.

### Métricas

- Requests/s: no medido
- Usuarios virtuales: 0
- P50: no medido
- P95: no medido
- P99: no medido
- HTTP 2xx: no medido
- HTTP 4xx: no medido
- HTTP 429: no medido
- HTTP 5xx: no medido
- CPU: no medido
- Memoria: no medido
- Reinicios de pods: no medido
- Tiempo de recuperación: no medido

### Integridad

No hubo escrituras durante el experimento.

### Resultado

- [ ] Aprobado
- [ ] Fallido
- [x] Inconcluso

### Evidencias

El procedimiento está escrito. No existe el contenedor `k3d-taskmanagement-agent-0`.

### Observaciones y limitaciones

Sin clúster no hay nodo que parar. No se afirma tolerancia a la pérdida de un nodo.
