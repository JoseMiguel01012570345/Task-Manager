# Errores de la API

Las respuestas de error usan `application/problem+json` (`ProblemDetails`) e incluyen `traceId`. Fuera de Development el detalle de un 500 es un mensaje fijo: no se envía el mensaje interno ni el stack trace.

| Situación | Código | Origen |
|---|---|---|
| Cuerpo o parámetros inválidos | 400 | `RequestValidationException` |
| Petición sin autenticación | 401 | `UnauthorizedException` |
| Autenticado sin permiso para la acción | 403 | `ForbiddenException` |
| Tarea inexistente o no visible | 404 | `NotFoundException` |
| Versión obsoleta | 409 | `ConcurrencyException` |
| Transición de estado no permitida | 422 | `DomainException` |
| Límite de solicitudes | 429 | middleware de rate limiting, con `Retry-After` cuando aplica |
| Error no controlado | 500 | cualquier otra excepción |

`traceId` es la cabecera de correlación de la petición. Los logs de la misma petición usan ese identificador.
