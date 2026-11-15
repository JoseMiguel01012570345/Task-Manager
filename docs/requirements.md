# Requisitos — Task Management

Este documento define el comportamiento esperado y los límites del laboratorio local. Los objetivos de rendimiento son hipótesis medibles para este equipo, no garantías del producto.

Desviación conocida: el plan original cita ASP.NET Core 10. Este repositorio se implementa en **.NET 9** porque el SDK instalado es 9.0.301. El detalle de entorno está en `docs/setup.md` y la decisión en `docs/architecture/`.

## 1. Actores

| Actor | Descripción |
|---|---|
| Usuario (`User`) | Crea tareas y opera solo sobre las que creó o de las que es responsable. |
| Administrador (`Administrator`) | Opera sobre cualquier tarea. |
| Sistema | Registra auditoría, aplica límites y rechaza transiciones inválidas. |

Hasta que la autenticación JWT esté activa, la API usa un usuario de desarrollo inyectado por configuración. A partir de ese punto no hay acceso anónimo a los endpoints de tareas.

## 2. Entidad tarea

La entidad de dominio se llama `TaskItem` para no chocar con `System.Threading.Tasks.Task`. Representa una tarea de trabajo.

| Campo | Regla |
|---|---|
| `Id` | `Guid` generado por el dominio al crear. |
| `Title` | Obligatorio. Longitud 1–200. Se recortan espacios externos. |
| `Description` | Opcional. Longitud 0–4000. |
| `Status` | `Pending`, `InProgress` o `Done`. Estado inicial: `Pending`. |
| `AssigneeId` | Identificador del responsable. Obligatorio. Máximo 128 caracteres. |
| `CreatedById` | Identificador de quien crea la tarea. Lo fija el servidor, no el cliente. |
| `DueDate` | Opcional, UTC. En la creación no puede ser anterior al momento actual. |
| `CreatedAt` / `UpdatedAt` | UTC, fijadas por el servidor. |
| `IsDeleted` | Borrado lógico. Las consultas normales no devuelven tareas borradas. |
| `Version` | Token de concurrencia optimista (xmin de PostgreSQL). |

## 3. Transiciones de estado

| Desde | Hacia permitido |
|---|---|
| `Pending` | `InProgress`, `Done` |
| `InProgress` | `Done` |
| `Done` | `InProgress` |

Cualquier otra transición se rechaza en el dominio y la API responde `422 Unprocessable Entity`. El estado no cambia por el `PUT` general: solo por `PATCH /api/v1/tasks/{id}/status`.

## 4. Propiedad y acceso

- Un `User` puede leer, actualizar, cambiar estado y eliminar una tarea si `CreatedById` o `AssigneeId` coincide con su identificador.
- Un `User` puede crear una tarea asignada a sí mismo. No puede asignarla a otra persona.
- Un `Administrator` puede crear, leer, actualizar, cambiar estado y eliminar cualquier tarea, y puede asignar cualquier responsable.
- Si la tarea no existe o no es visible para el actor, la API responde `404 Not Found` y no indica si el recurso existe.
- El cliente no puede cambiar `Id`, `CreatedById`, `CreatedAt` ni `IsDeleted`.

## 5. API

Base: `/api/v1/tasks`.

| Operación | Método y ruta | Éxito |
|---|---|---|
| Crear | `POST /api/v1/tasks` | `201 Created` y cabecera `Location` |
| Listar | `GET /api/v1/tasks` | `200` con página y metadatos |
| Consultar | `GET /api/v1/tasks/{id}` | `200` o `404` |
| Actualizar | `PUT /api/v1/tasks/{id}` | `200` o `409` si la versión no coincide |
| Cambiar estado | `PATCH /api/v1/tasks/{id}/status` | `200` o `422` |
| Eliminar | `DELETE /api/v1/tasks/{id}` | `204 No Content` o `404` |

Filtros de listado combinables: `status`, `assigneeId`. Paginación: `page` (mínimo 1, por defecto 1) y `pageSize` (por defecto 20, máximo 100). Orden estable: `CreatedAt` descendente y después `Id` descendente.

La actualización envía la versión esperada. Si otra escritura cambió la fila, la API responde `409 Conflict` y no sobrescribe.

La eliminación es lógica. La tarea deja de aparecer en listados y consultas.

### Contrato de error

Las respuestas de error usan `ProblemDetails` (`application/problem+json`) e incluyen `traceId`.

| Situación | Código |
|---|---|
| Validación de entrada | `400` |
| Sin autenticación | `401` |
| Autenticado sin permiso explícito de rol | `403` |
| Recurso inexistente o no visible | `404` |
| Conflicto de versión | `409` |
| Transición de estado inválida | `422` |
| Límite de solicitudes | `429` con `Retry-After` cuando aplica |
| Error interno | `500` sin stack trace fuera de Development |

## 6. Límites

| Límite | Valor |
|---|---|
| Título | 200 caracteres |
| Descripción | 4000 caracteres |
| Identificadores de usuario | 128 caracteres |
| Cuerpo HTTP | 64 KB |
| `pageSize` máximo | 100 |
| Lecturas por ventana | 100 / 10 s por usuario o IP |
| Escrituras por ventana | 30 / 10 s por usuario o IP |
| Intentos de autenticación | 10 / 60 s por IP |

## 7. Objetivos del laboratorio

Estos umbrales se miden en el equipo anfitrión y se ajustan si el hardware no da para sostenerlos. No describen un entorno de producción.

| Métrica | Objetivo inicial |
|---|---|
| Lecturas P95 | < 500 ms |
| Escrituras P95 | < 800 ms |
| HTTP 5xx bajo la carga objetivo | < 0,1 % |
| Disponibilidad en el escenario de carga acordado | ≥ 99,9 % |
| Integridad de operaciones confirmadas | 100 % en las pruebas de aceptación |
| Recuperación tras perder una réplica de API | Medida y reportada; objetivo inicial < 30 s |

## 8. Qué significa superar cada prueba

| Prueba | Se supera cuando |
|---|---|
| CRUD y filtros | Cada operación persiste o rechaza según este documento y devuelve el código HTTP indicado. |
| Paginación | La página pedida no incluye filas de otras páginas y un `pageSize` mayor que 100 se rechaza. |
| Concurrencia | Dos actualizaciones con la misma versión producen un solo éxito y un `409`. |
| Autorización | Un usuario no lee ni modifica tareas ajenas. Un token inválido, expirado o de otro emisor recibe `401`. |
| XSS | Un payload guardado en título o descripción se muestra como texto y no ejecuta script. |
| Rate limiting | El exceso recibe `429` y otro usuario dentro de su cupo sigue operando. |
| WAF | Las peticiones de prueba definidas se detectan o bloquean y el conjunto legítimo sigue funcionando. |
| Distribución | Hay más de una réplica y el tráfico entra por Traefik hacia réplicas sanas. |
| Carga | Smoke, load, stress, spike y soak terminan con reporte de latencia, errores y throughput. |
| Tráfico mixto | El tráfico HTTP abusivo del laboratorio queda limitado y las operaciones legítimas cumplen el SLO de ese escenario. |
| Caos | Caída de pod, nodo, red y PostgreSQL quedan medidas con el tiempo de recuperación y sin éxitos falsos. |
| Integridad | Toda operación confirmada está en la base; las no confirmadas se identifican. |

## 9. Métricas que se recopilan

- Solicitudes por segundo, latencia P50/P95/P99 y códigos HTTP (`2xx`, `4xx`, `429`, `5xx`), separando tráfico legítimo y tráfico bloqueado.
- Réplicas disponibles, reinicios, CPU y memoria de API, frontend, PostgreSQL y generadores de carga.
- Trazas correlacionadas desde la entrada HTTP hasta la base de datos.
- Registros de auditoría de altas, cambios, transiciones y bajas.
- Reglas del WAF que se activaron.

## 10. Matriz de requisitos

| ID | Requisito | Criterio de aceptación |
|---|---|---|
| RF-01 | Crear una tarea válida | `POST` persiste y responde `201` con `Location`. |
| RF-02 | Rechazar datos inválidos | Título vacío, texto largo o fecha límite pasada responden `400` y no insertan. |
| RF-03 | Listar con filtros y página | `status` y `assigneeId` se combinan; los metadatos indican página, tamaño y total. |
| RF-04 | Consultar una tarea | Devuelve la tarea visible o `404`. |
| RF-05 | Actualizar campos editables | Título, descripción, responsable y fecha límite cambian; la versión obsoleta responde `409`. |
| RF-06 | Cambiar estado | Solo las transiciones de la sección 3; el resto responde `422`. |
| RF-07 | Eliminar | Borrado lógico autorizado responde `204`; una tarea ausente responde `404`. |
| RF-08 | Autorización | Un `User` no opera tareas ajenas; un `Administrator` sí. |
| RNF-01 | Errores uniformes | Los fallos usan `ProblemDetails` y `traceId`. |
| RNF-02 | Sin secretos en el cliente | El bundle del navegador no contiene contraseñas ni client secrets. |
| RNF-03 | Stateless | Una petición posterior puede atenderla otra réplica. |
| RNF-04 | Observabilidad | Logs, métricas y trazas se correlacionan en el laboratorio. |
| RNF-05 | Rendimiento de laboratorio | Se miden los umbrales de la sección 7 y se documenta el hardware. |
| RNF-06 | Seguridad de entrada | XSS de prueba no se ejecuta; orígenes no listados no usan la API desde el navegador. |
| RNF-07 | Reproducibilidad | El entorno local se levanta con los scripts y el README del repositorio. |
