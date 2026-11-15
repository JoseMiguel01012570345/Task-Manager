# PostgreSQL y migraciones

La cadena local de desarrollo es la de `appsettings.Development.json`. El usuario, la contraseña y la base se llaman `taskmanagement`. Sirven solo para el laboratorio; no son credenciales de un entorno compartido.

## Levantar PostgreSQL

```powershell
docker compose -f infrastructure/database/docker-compose.yml up -d
```

Detenerlo sin borrar el volumen:

```powershell
docker compose -f infrastructure/database/docker-compose.yml stop
```

## Aplicar migraciones

Desde la raíz del repositorio, con el SDK 9:

```powershell
dotnet tool restore
$env:TASKMANAGEMENT_CONNECTION = "Host=localhost;Port=5432;Database=taskmanagement;Username=taskmanagement;Password=taskmanagement"
dotnet ef database update --project src/TaskManagement.Infrastructure --startup-project src/TaskManagement.Infrastructure
```

Ese comando crea el esquema una sola vez por base. Las réplicas de la API no migran al arrancar.

En Development, al iniciar la API se inserta una tarea de ejemplo si la tabla está vacía. Otros entornos no siembran datos.
