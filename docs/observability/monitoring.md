# Prometheus, Grafana y Loki

El compose de `infrastructure/observability` levanta el collector, Prometheus, Loki y Grafana. Prometheus scrapea las métricas que el collector expone en el puerto 8889. Loki recibe los logs OTLP. Grafana entra en `http://localhost:3000` con el usuario `admin` y la contraseña de laboratorio `GRAFANA_ADMIN_PASSWORD` (valor de ejemplo en `.env.example`).

El tablero `API overview` muestra tasa de peticiones, latencia p95, tamaño del heap y los logs del servicio `taskmanagement-api`. `System Resilience` añade la proporción de respuestas que no son 5xx, la cola de Kestrel y la pausa del recolector de basura. Las sondas `/health` quedan fuera de la latencia y de la proporción de éxito. Prometheus conserva la etiqueta `job` que publica el collector (`honor_labels`).

```powershell
docker compose -f infrastructure/observability/docker-compose.yml up -d
```
