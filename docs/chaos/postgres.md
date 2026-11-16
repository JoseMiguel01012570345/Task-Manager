## Test: TASK-064 caída del pod de PostgreSQL

- Fecha: 2026-11-15
- Commit / versión: ver `git log` de TASK-064
- Entorno: clúster k3d existente; este experimento no forma parte de `Invoke-Resilience.ps1`
- Nodos K3d: 1 servidor y 2 agentes en el laboratorio, no usados por este informe
- CPU y memoria del host: no medidas
- Réplicas de API: 3 en el manifiesto
- Réplicas de frontend: 3 en el manifiesto
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

Chaos Mesh está instalado, pero la suite no aplica `postgres-kill.yaml`. No se afirma el efecto de matar un pod de PostgreSQL. El StatefulSet del laboratorio tiene 3 réplicas y una sola acepta escrituras.
