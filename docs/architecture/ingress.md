# Ingreso

El tráfico de la aplicación entra solo por Traefik, en `http://localhost:8088`. `/api` y `/health` van a la API. El resto va al frontend. Ninguno de los dos publica puertos en el host.

```powershell
docker compose -f infrastructure/edge/docker-compose.yml up -d
```
