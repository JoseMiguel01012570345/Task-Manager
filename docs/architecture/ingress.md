# Ingreso

El tráfico de la aplicación entra solo por el Traefik del borde, en `http://localhost:8088`. Ese proceso reparte hacia el DaemonSet `traefik-ingress` de los tres nodos. El Ingress de dentro del clúster envía `/realms` a Keycloak, `/api` y `/health` a la API, `/grafana` a Grafana y `/` al frontend. El borde no tiene un router propio de `/realms`: esa ruta la resuelve el Ingress. Si Keycloak no tiene endpoints, un `POST` de login puede caer en el frontend y nginx responde 405.

```powershell
docker compose -f infrastructure/edge/docker-compose.yml up -d
```
