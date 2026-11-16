# De dónde salen los datos de Grafana

Grafana no scrapea la API ni lee sus logs. Solo consulta dos almacenes que ya tiene configurados: Prometheus para métricas y Loki para logs. Las trazas se generan y llegan al collector, pero no hay un backend de trazas: Grafana no las muestra.

Dentro del clúster la API exporta solo si `OpenTelemetry__OtlpEndpoint` apunta a `http://otel-collector.taskmanagement.svc.cluster.local:4317`. Si esa variable está vacía, la instrumentación sigue activa en el proceso y no sale nada hacia Grafana.

```text
API  --OTLP gRPC :4317-->  collector  --métricas :8889-->  Prometheus  -->  Grafana
                          collector  --logs OTLP-->       Loki        -->  Grafana
                          collector  --trazas-->          consola del collector
```

Prometheus raspa el collector cada 15 segundos (`honor_labels: true`) y conserva la etiqueta `job` que el collector publica, `taskmanagement-api`. Loki guarda los logs en disco, con un solo miembro. Grafana entra por `http://localhost:8088/grafana/` con el usuario `admin` y la contraseña `GRAFANA_ADMIN_PASSWORD`.

## Métricas

`AddTaskTelemetry` registra el servicio `taskmanagement-api` y mide ASP.NET Core, `HttpClient` y el runtime de .NET. Esas series salen por OTLP gRPC. El collector las agrupa y las vuelve a exponer en el puerto 8889 con el exportador de Prometheus. También escribe un resumen en su propia consola.

El tablero `API overview` usa la tasa de `http_server_request_duration_seconds`, el percentil 95 de esa histograma y el tamaño del heap (`dotnet_gc_last_collection_heap_size_bytes`). `System Resilience` añade la proporción de respuestas que no son 5xx, la misma latencia sin las rutas `/health` y deja fuera esas sondas de la proporción de éxito. El tablero de resumen no filtra `/health`: las sondas de Kubernetes entran en su tasa y en su p95.

## Logs

Serilog escribe cada evento en la consola de la API, en JSON compacto, y lo reenvía por OTLP gRPC al mismo collector. El recurso lleva `service.name=taskmanagement-api`. El collector lo entrega a Loki en `http://loki:3100/otlp`. El panel de logs de Grafana filtra `{service_name="taskmanagement-api"}`.

Cada petición HTTP queda resumida con método, ruta, código y duración. No se registran el cuerpo, la cabecera `Authorization` ni los tokens. La auditoría de negocio está en la tabla `audit_records` y no pasa por este camino.

## Trazas

La API crea trazas de ASP.NET Core (salvo las rutas `/health`), de `HttpClient` y de Entity Framework Core, y las exporta al collector. El pipeline de trazas del collector solo tiene el exportador de depuración. No hay Tempo ni Jaeger, y Grafana no tiene un origen de datos de trazas.

## Fuera del clúster

En Development el endpoint por defecto es `http://localhost:4317`. El compose de `infrastructure/observability` levanta el mismo collector, Prometheus, Loki y Grafana en la máquina, sin Kubernetes.
