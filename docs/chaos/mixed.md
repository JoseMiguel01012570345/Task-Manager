## Test: TASK-065 tráfico mixto durante un fallo

- Fecha: 2026-11-15
- Commit / versión: ver `git log` de TASK-065
- Entorno: laboratorio local, sin clúster k3d y sin k6 en PATH
- Nodos K3d: 0
- CPU y memoria del host: no medidas
- Réplicas de API: 2 en el manifiesto
- Réplicas de frontend: 2 en el manifiesto
- Duración: no aplica
- Perfil de carga: `tests/k6/mixed.js`, no ejecutado

### Hipótesis

Durante la caída de un pod, las lecturas siguen y las escrituras o se confirman o fallan sin duplicar la fila.

### Procedimiento

Lanzar `k6 run tests/k6/mixed.js` y, a la vez, aplicar `infrastructure/k8s/chaos/pod-kill.yaml`.

### Métricas

- Requests/s: no medido
- Usuarios virtuales: 8 previstos, 0 reales
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

No hubo escrituras de este escenario.

### Resultado

- [ ] Aprobado
- [ ] Fallido
- [x] Inconcluso

### Evidencias

El escenario y el experimento están versionados. No hay salida de k6.

### Observaciones y limitaciones

Faltan k6 y el clúster. No se afirma comportamiento bajo fallo con tráfico.
