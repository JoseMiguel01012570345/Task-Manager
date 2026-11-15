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

## Laboratorio en k3d

```powershell
./infrastructure/local/scripts/Deploy-Cluster.ps1
```

Ese script crea el clúster y aplica los manifiestos. `./infrastructure/local/scripts/Remove-K3dCluster.ps1` lo elimina.

## Pruebas

```powershell
./infrastructure/local/scripts/Invoke-Tests.ps1
```

Las pruebas de integración llaman a la API por Traefik, en `http://localhost:8088`. El clúster tiene que estar desplegado. Carga, caos y ZAP se lanzan a mano: están descritos en `docs/performance`, `docs/chaos` y `docs/security`.

Más detalle en `docs/setup.md`.
