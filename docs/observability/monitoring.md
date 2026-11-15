# Prometheus, Grafana y Loki

El compose de `infrastructure/observability` levanta el collector, Prometheus, Loki y Grafana. Prometheus scrapea las métricas que el collector expone en el puerto 8889. Loki recibe los logs OTLP. Grafana entra en `http://localhost:3000` con el usuario `admin` y la contraseña de laboratorio `GRAFANA_ADMIN_PASSWORD` (valor de ejemplo en `.env.example`).

El tablero versionado `API overview` muestra tasa de peticiones, latencia p95, tamaño del heap y los logs del servicio `taskmanagement-api`.

```powershell
docker compose -f infrastructure/observability/docker-compose.yml up -d
```
