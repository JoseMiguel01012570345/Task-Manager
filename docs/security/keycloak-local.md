# Keycloak local

El realm `taskmanagement` tiene los roles `User` y `Administrator`, el cliente público `taskmanagement-web` (authorization code y password grant para el laboratorio) y el cliente `taskmanagement-api`.

Las contraseñas no están en el realm versionado. Copia `.env.example` a `.env` y genera el import:

```powershell
powershell -File infrastructure/local/docker/keycloak/render-realm.ps1
docker compose --env-file .env -f infrastructure/local/docker/keycloak/docker-compose.yml up -d
```

Token de un usuario de prueba:

```powershell
$body = @{
  client_id = "taskmanagement-web"
  grant_type = "password"
  username = "dev-user"
  password = $env:DEV_USER_PASSWORD
}
Invoke-RestMethod -Method Post -Uri http://localhost:8080/realms/taskmanagement/protocol/openid-connect/token -Body $body
```

El emisor que valida la API de Compose es `http://localhost:8080/realms/taskmanagement` y la audiencia es `taskmanagement-api`.

## En el clúster

Keycloak es una réplica, con `start-dev --import-realm` y la base embebida. No se programa en el plano de control. El emisor que usa el laboratorio es `http://localhost:8088/realms/taskmanagement`. `Wait-Keycloak.ps1` pide un token de `dev-user` con el cliente `taskmanagement-web` y espera hasta cuatro minutos. Cada rearranque vuelve a importar el realm.
