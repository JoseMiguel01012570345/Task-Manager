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

## Inconcluso

- k3d no está instalado. No hay clúster, así que no se aplicaron los manifiestos ni Chaos Mesh. Los informes de `docs/chaos` lo dicen.
- k6 no está en PATH. Los escenarios están versionados y no tienen cifras.
- La imagen de ZAP no estaba en Docker. El script queda en `infrastructure/local/scripts/Invoke-ZapBaseline.ps1`.
- Trivy está en el workflow y no se lanzó el escaneo en esta sesión.
- El límite de tasa sigue en memoria por réplica. Redis está en Compose y todavía no es el almacén del contador.

## Desviaciones

- El plan habla de .NET 10. El SDK instalado es 9.0.301, así que la solución es `net9.0`. Está anotado en `docs/setup.md`.
- OpenTelemetry se fijó en 1.20.1 porque 1.21 arrastra bibliotecas de .NET 10.
- El frontend de la imagen se compiló con `node:22-alpine`, que es la imagen presente en Docker. El desarrollo local sigue en Node 20.
