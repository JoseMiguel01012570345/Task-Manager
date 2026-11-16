## Test: caída de dos pods de la API

- Fecha: 2026-11-16
- Entorno: clúster k3d `taskmanagement`, contexto `k3d-taskmanagement`
- Nodos K3d: 1 servidor y 2 agentes
- Réplicas de API: 3
- Réplicas de frontend: 3
- Perfil de carga: el frontend de `Invoke-Tests.ps1` crea tareas mientras corre la prueba. k6 ya terminó antes de este paso.

### Hipótesis

Matar dos pods de la API deja una réplica sirviendo. El Deployment vuelve a tres réplicas y `/health/live` responde 200 por Traefik.

### Procedimiento

`Invoke-Resilience.ps1` instala Chaos Mesh si falta, borra un `PodChaos` anterior y aplica `infrastructure/k8s/chaos/pod-kill.yaml` (`action: pod-kill`, `mode: fixed`, `value: "2"`, etiqueta `app: api`). Espera la condición `AllInjected`, el `rollout` del Deployment `api` y retira el experimento. Después pide `http://localhost:8088/health/live`.

### Métricas

No se anotaron percentiles ni una tasa de peticiones. El criterio de la suite es la condición de Chaos Mesh, el rollout y el 200 de `/health/live`.

### Integridad

El frontend crea tareas por la API durante la ventana. La suite exige al menos un alta en esa ventana y otra cuando los nodos ya volvieron.

### Resultado

- [x] Aprobado
- [ ] Fallido
- [ ] Inconcluso

La corrida de `Invoke-Tests.ps1` del 2026-11-16 terminó con código 0 e imprimió `Resiliencia comprobada`.

### Observaciones y limitaciones

El experimento mata dos pods, no uno. No mide cuántas peticiones fallaron durante el reemplazo.
