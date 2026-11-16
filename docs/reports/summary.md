# Informes

Lo que se ejecutó en esta máquina y lo que quedó pendiente. No se rellenan métricas que no se midieron.

## Hecho

- La API compila en `net9.0` y las pruebas de unidad, de arquitectura y de integración con Testcontainers pasan.
- Playwright creó y leyó una tarea contra `http://localhost:5080`, y comprobó que un payload queda como texto.
- Coraza en `/waf-probe` respondió 403 a una inyección SQL y a un script. La misma inyección en `/` quedó en detección y respondió 200.
- `/health/live` y `/health/ready` respondieron 200 con PostgreSQL en marcha.
- Prometheus devolvió el tamaño del heap de `taskmanagement-api`. Grafana cargó los tableros `API overview` y `System Resilience`.
- El collector recibió trazas y logs.
- `pg_dump` de la base de Compose incluye `tasks`, `audit_records` y el historial de migraciones.
- Las imágenes `taskmanagement-api:local` y `taskmanagement-web:local` se construyeron. La API corre como `app` y el frontend como `nginx`.
- El clúster k3d `taskmanagement` está en uso: 1 servidor y 2 agentes, API y frontend a 3 réplicas, PostgreSQL a 3 con una sola primaria. El ingreso es `http://localhost:8088`.
- Chaos Mesh 2.7.2 está en el namespace `chaos-mesh`. La resiliencia mata dos pods de la API con un `PodChaos` y para los dos agentes con `docker stop`. El servidor sigue respondiendo `/health/ready`.
- `Invoke-Tests.ps1` del 2026-11-16 terminó con código 0: los siete escenarios k6 con `SUITE=1`, el frontend durante y después de esa ventana, la resiliencia, Trivy, las pruebas de seguridad y el baseline de ZAP (0 fallos, 2 avisos).
- El límite de tasa se comprobó contra un solo pod, con `429` y `Retry-After: 10`.

## Inconcluso

- `postgres-kill.yaml` y `network-delay.yaml` no los aplica la suite. No hay resultado de esos dos experimentos.
- No se guardaron percentiles de k6 ni un tiempo de recuperación en segundos.
- El límite de tasa sigue en memoria por réplica. No hay un contador compartido.
- El workflow de GitHub no ejecuta `Invoke-Tests.ps1`.

## Desviaciones

- El plan habla de .NET 10. El SDK instalado es 9.0.301, así que la solución es `net9.0`. Está anotado en `docs/setup.md`.
- OpenTelemetry se fijó en 1.20.1 porque 1.21 arrastra bibliotecas de .NET 10.
- El frontend de la imagen se compiló con `node:22-alpine`, que es la imagen presente en Docker. El desarrollo local sigue en Node 20.
- El experimento de nodo para los dos agentes, no uno. Chaos Mesh no puede apagar un nodo k3d.
- El `PodChaos` mata dos pods de la API, no uno.
