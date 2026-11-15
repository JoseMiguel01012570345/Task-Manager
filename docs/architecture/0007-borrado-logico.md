# ADR 0007 — Borrado lógico de tareas

## Contexto

`DELETE /api/v1/tasks/{id}` tiene que dejar de mostrar la tarea y conservar el historial para auditoría y para comprobar integridad después de un fallo.

## Decisión

La eliminación es lógica: `IsDeleted` pasa a verdadero y `UpdatedAt` se actualiza. El filtro global de EF Core excluye esas filas de las consultas normales, así que una tarea borrada responde `404`. El cliente envía la versión esperada en el query `version`. No hay borrado físico desde la API.

## Consecuencias

Los listados y el detalle no devuelven tareas borradas. La fila sigue en PostgreSQL para auditoría. Un borrado concurrente con versión obsoleta responde `409` y no marca la fila.
