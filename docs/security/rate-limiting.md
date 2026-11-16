# Límites de solicitudes

Los contadores viven en la memoria de cada réplica. No hay un contador compartido: el borde reparte las peticiones y cada pod cuenta las suyas. La partición es la operación más el usuario (`preferred_username`) o, si no hay usuario, la IP. `RateLimitTests` abre un `kubectl port-forward` a un solo pod de la API para llenar ese cupo, y exige `429` con `Retry-After: 10`. Después de 11 segundos la misma lectura tiene que volver a 200.

| Política | Límite | Ventana |
|---|---|---|
| `reads` | 100 | 10 s |
| `writes` | 30 | 10 s |
| `login` | 10 | 60 s |

Al superar el límite la API responde `429` y `Retry-After: 10`. Un usuario limitado no comparte contador con otro usuario.
