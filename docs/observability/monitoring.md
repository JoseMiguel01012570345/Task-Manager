# Prometheus, Grafana y Loki

Dentro del clúster, `Deploy-Cluster.ps1` levanta el collector, Prometheus, Loki y Grafana. Prometheus scrapea las métricas que el collector expone en el puerto 8889. Loki recibe los logs OTLP. La API exporta a `http://otel-collector.taskmanagement.svc.cluster.local:4317`. Grafana entra por `http://localhost:8088/grafana/` con el usuario `admin` y la contraseña de laboratorio `GRAFANA_ADMIN_PASSWORD` (valor de ejemplo en `.env.example`).

El tablero `API overview` muestra tasa de peticiones, latencia p95, tamaño del heap y los logs del servicio `taskmanagement-api`. `System Resilience` añade la proporción de respuestas que no son 5xx, la cola de Kestrel y la pausa del recolector de basura. Las sondas `/health` quedan fuera de la latencia y de la proporción de éxito. Prometheus conserva la etiqueta `job` que publica el collector (`honor_labels`).

```powershell
./infrastructure/local/scripts/Deploy-Cluster.ps1
```
