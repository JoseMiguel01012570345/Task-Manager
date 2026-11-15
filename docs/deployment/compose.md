# Compose

`docker-compose.yml` levanta PostgreSQL, Redis, Keycloak, la API y el frontend. La API y el frontend no publican puertos: entran por Traefik en `http://localhost:8088`. PostgreSQL queda en `5432` y Keycloak en `8080` para operar el laboratorio. Redis no sale al host.

Antes de arrancar, la red interna de Traefik tiene que existir y el realm tiene que estar generado:

```powershell
docker compose -f infrastructure/edge/docker-compose.yml up -d
./infrastructure/local/docker/keycloak/render-realm.ps1
docker compose up -d
```

El perfil `observability` añade el collector, Prometheus, Loki y Grafana. Para usarlo, `OTEL_EXPORTER_OTLP_ENDPOINT` vale `http://otel-collector:4317`. No lo combines con el compose suelto de `infrastructure/observability`, porque comparten los mismos puertos.

Las migraciones no se aplican al arrancar cada réplica. Se aplican con el job descrito en el despliegue.
