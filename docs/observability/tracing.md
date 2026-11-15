# Trazas y métricas

La API instrumenta ASP.NET Core, `HttpClient`, Entity Framework Core y el runtime con OpenTelemetry 1.20.1. Esa banda es la última que declara soporte de `net9.0` sin arrastrar `Microsoft.Extensions` 11, pensado para .NET 10. El servicio se llama `taskmanagement-api`.

En Development el exportador OTLP apunta a `http://localhost:4317`. Si `OpenTelemetry:OtlpEndpoint` está vacío, la instrumentación sigue activa y no intenta exportar. Las rutas `/health` no generan trazas.

El collector local está en `infrastructure/observability`. Recibe OTLP, escribe un resumen en su consola, expone métricas en el puerto 8889 para Prometheus y reenvía logs a Loki.

```powershell
docker compose -f infrastructure/observability/docker-compose.yml up -d
```
