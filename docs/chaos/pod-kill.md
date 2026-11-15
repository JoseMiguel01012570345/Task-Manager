## Test: TASK-061 caída de un pod de la API

- Fecha: 2026-11-15
- Commit / versión: ver `git log` de TASK-061
- Entorno: laboratorio local, sin clúster k3d
- Nodos K3d: 0
- CPU y memoria del host: no medidas para este experimento
- Réplicas de API: 2 en el manifiesto, 0 en ejecución
- Réplicas de frontend: 2 en el manifiesto, 0 en ejecución
- Duración: no aplica
- Perfil de carga: ninguno

### Hipótesis

Matar un pod de la API deja el otro sirviendo. El presupuesto de disrupción impide quedar en cero durante un desalojo voluntario.

### Procedimiento

Aplicar `infrastructure/k8s/chaos/pod-kill.yaml` con Chaos Mesh instalado en `taskmanagement`.

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

El manifiesto está en el repositorio. No hay logs del controlador.

### Observaciones y limitaciones

k3d no está instalado en esta máquina. No se afirma recuperación.
