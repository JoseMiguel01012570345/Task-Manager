## Test: tráfico durante k6 y la resiliencia

- Fecha: 2026-11-16
- Entorno: clúster k3d `taskmanagement`, ingreso `http://localhost:8088`
- Nodos K3d: 1 servidor y 2 agentes
- Réplicas de API: 3
- Réplicas de frontend: 3
- Perfil de carga: los siete escenarios de `tests/k6` con `SUITE=1`, y después la resiliencia

### Hipótesis

El frontend consigue crear una tarea mientras corren k6 y la resiliencia, y otra cuando esa ventana ya cerró.

### Procedimiento

`Invoke-Tests.ps1` arranca `tests/e2e/frontend.spec.ts` y, cuando el spec señala que ya abrió, lanza `K6ScenarioTests` y luego `Invoke-Resilience.ps1`. k6 y el `PodChaos` no van a la vez: k6 termina antes de matar pods. El frontend sí sigue durante los dos tramos. Si un escenario de k6 sale distinto de 0, se reintenta una vez.

### Métricas

No se copiaron percentiles de k6 a este informe. El criterio de la suite es que los siete escenarios terminen en 0 y que el frontend complete las dos altas.

### Integridad

Cada alta del frontend exige un `POST /api/v1/tasks` con 201.

### Resultado

- [x] Aprobado
- [ ] Fallido
- [ ] Inconcluso

En la corrida del 2026-11-16 los siete escenarios pasaron y el spec de Playwright terminó en 0.

### Observaciones y limitaciones

Este informe no sustituye un k6 mixto lanzado en el mismo instante que el `PodChaos`. Ese solape no forma parte del script.
