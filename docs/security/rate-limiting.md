# Límites de solicitudes

Los contadores de una sola réplica viven en memoria. Cuando Redis esté en el entorno, el mismo contrato se puede respaldar con un contador compartido. Las particiones son por operación y por usuario (`preferred_username`) o, si no hay usuario, por IP.

| Política | Límite | Ventana |
|---|---|---|
| `reads` | 100 | 10 s |
| `writes` | 30 | 10 s |
| `login` | 10 | 60 s |

Al superar el límite la API responde `429` y `Retry-After: 10`. Un usuario limitado no comparte contador con otro usuario.
