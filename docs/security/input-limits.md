# Límites de entrada

| Límite | Valor |
|---|---|
| Cuerpo HTTP | 64 KB |
| Título | 200 |
| Descripción | 4000 |
| Identificadores | 128 |
| `pageSize` | 1–100 |

`POST`, `PUT` y `PATCH` solo aceptan `application/json`. Otro `Content-Type` responde `415` y no llega a la base de datos. Las consultas usan EF Core, así que los filtros van parametrizados. Los mensajes de validación son textos fijos del validador; no se reflejan el cuerpo ni el stack.
