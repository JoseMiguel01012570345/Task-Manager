# Concurrencia optimista

Cada tarea expone `version`, el `xmin` de PostgreSQL. El cliente lo recibe en las respuestas y lo reenvía:

- `PUT /api/v1/tasks/{id}` en el campo `version` del cuerpo.
- `PATCH /api/v1/tasks/{id}/status` en el campo `version` del cuerpo.
- `DELETE /api/v1/tasks/{id}?version=`.

Si la fila cambió desde la lectura, la API responde `409 Conflict` y no guarda el cuerpo obsoleto. El cliente debe volver a leer la tarea y reintentar con la versión nueva. No hay fusión automática de campos.
