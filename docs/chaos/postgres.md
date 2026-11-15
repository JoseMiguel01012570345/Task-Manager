## Test: TASK-064 caída del pod de PostgreSQL

- Fecha: 2026-11-15
- Commit / versión: ver `git log` de TASK-064
- Entorno: laboratorio local, sin clúster k3d
- Nodos K3d: 0
- CPU y memoria del host: no medidas
- Réplicas de API: 2 en el manifiesto
- Réplicas de frontend: 2 en el manifiesto
- Duración: 20s previstas en el manifiesto
- Perfil de carga: ninguno

### Hipótesis

Mientras PostgreSQL no está listo, `/health/ready` responde 503 y no se confirman escrituras. Al volver el pod, el volumen conserva las filas.

### Procedimiento

Aplicar `infrastructure/k8s/chaos/postgres-kill.yaml` sobre el StatefulSet `postgres`.

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

El manifiesto está en el repositorio. El volumen se define en `infrastructure/k8s/postgres.yaml`.

### Observaciones y limitaciones

Sin clúster no se mató el pod de la base. La base de Compose siguió en marcha para el resto del laboratorio.
