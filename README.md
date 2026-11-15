# Task Manager

API en .NET 9 y frontend en React para gestionar tareas. El ingreso del laboratorio es Traefik. La autenticación de los entornos desplegados es JWT de Keycloak.

## Requisitos

- .NET SDK 9.0.301 o una 9.0 posterior (`global.json`)
- Node 20 para el frontend en local
- Docker, para PostgreSQL, Keycloak, Traefik y las pruebas de integración

## API y frontend en local

```powershell
docker compose -f infrastructure/database/docker-compose.yml up -d
dotnet run --project src/TaskManagement.API --launch-profile http
```

La API queda en `http://localhost:5080`. En Development, una petición sin `Authorization` usa el usuario `dev-user`.

```powershell
cd src/TaskManagement.Web
npm install
npm run dev
```

El frontend queda en `http://localhost:5173` y proxya `/api` hacia la API.

## Laboratorio con Compose

```powershell
./infrastructure/local/docker/keycloak/render-realm.ps1
./infrastructure/local/scripts/Start-Compose.ps1
```

La aplicación entra por `http://localhost:8088`. Keycloak publica `8080` para emitir tokens. `./infrastructure/local/scripts/Stop-Compose.ps1` apaga ese conjunto.

## Pruebas

```powershell
./infrastructure/local/scripts/Invoke-Tests.ps1
```

Las pruebas de integración levantan PostgreSQL con Testcontainers. Carga, caos y ZAP se lanzan a mano: están descritos en `docs/performance`, `docs/chaos` y `docs/security`.

Más detalle en `docs/setup.md`.
