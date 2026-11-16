## Test: TASK-063 retardo de red hacia la API

- Fecha: 2026-11-15
- Commit / versión: ver `git log` de TASK-063
- Entorno: clúster k3d existente; este experimento no forma parte de `Invoke-Resilience.ps1`
- Nodos K3d: 1 servidor y 2 agentes en el laboratorio, no usados por este informe
- CPU y memoria del host: no medidas
- Réplicas de API: 3 en el manifiesto
- Réplicas de frontend: 3 en el manifiesto
- Duración: 45s previstas en el manifiesto
- Perfil de carga: ninguno

### Hipótesis

Un retardo de 200 ms aumenta la latencia y no pierde las filas ya confirmadas.

### Procedimiento

Aplicar `infrastructure/k8s/chaos/network-delay.yaml`.

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

El manifiesto de NetworkChaos está en el repositorio.

### Observaciones y limitaciones

Chaos Mesh está instalado, pero la suite no aplica `network-delay.yaml`. No se afirma el efecto del retardo.
