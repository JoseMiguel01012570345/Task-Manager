# Convenciones

## Capas

| Proyecto | Puede referenciar | No puede referenciar |
|---|---|---|
| `TaskManagement.Domain` | BCL | Application, Infrastructure, API, EF Core, ASP.NET Core |
| `TaskManagement.Application` | Domain | Infrastructure, API, EF Core, ASP.NET Core |
| `TaskManagement.Infrastructure` | Application y Domain | API |
| `TaskManagement.API` | Infrastructure y, a través de ella, Application | — |

La composición de dependencias vive en `TaskManagement.API/Program.cs`. Cada capa expone `AddApplication()` o `AddInfrastructure(...)`.

## Nombres

- Casos de uso: verbo + sustantivo (`CreateTask`, `ListTasks`). Un archivo por caso de uso dentro de `Application/Tasks/`.
- Entidad de tarea: `TaskItem` (evita el conflicto con `System.Threading.Tasks.Task`).
- DTOs de entrada: sufijo `Request`. DTOs de salida: sufijo `Response`.
- Endpoints HTTP: `API/Endpoints/TasksEndpoints.cs`, prefijo `/api/v1`.
- Persistencia: `Infrastructure/Persistence`.
- Pruebas: `Método_Condición_Resultado`.

## Estructura por feature

El agregado `Tasks` agrupa entidad, reglas, casos de uso y endpoints. No se crean carpetas genéricas `Controllers`, `Repositories` y `Services` para repartir el mismo caso de uso.

## Usuario de desarrollo

Hasta activar JWT, `ICurrentUser` lo resuelve Infrastructure con el valor de configuración `DevUser`. El identificador por defecto es `dev-user`.
