## Test: TASK-066 integridad y restauración local

- Fecha: 2026-11-15
- Commit / versión: ver `git log` de TASK-066
- Entorno: PostgreSQL de Compose `database-postgres-1`
- Nodos K3d: 0
- CPU y memoria del host: no medidas
- Réplicas de API: la API de desarrollo, un proceso
- Réplicas de frontend: no participó
- Duración: un `pg_dump`
- Perfil de carga: ninguno

### Hipótesis

Una copia lógica contiene las tablas de tareas, de auditoría y el historial de migraciones, y puede volver a cargarse con `psql`.

### Procedimiento

`docker exec database-postgres-1 pg_dump -U taskmanagement --no-owner taskmanagement`

La restauración prevista está en `docs/deployment/stateless.md`. No se restauró encima de la base en uso.

### Métricas

- Requests/s: no aplica
- Usuarios virtuales: 0
- P50: no aplica
- P95: no aplica
- P99: no aplica
- HTTP 2xx: no aplica
- HTTP 4xx: no aplica
- HTTP 429: no aplica
- HTTP 5xx: no aplica
- CPU: no medido
- Memoria: no medido
- Reinicios de pods: no aplica
- Tiempo de recuperación: no medido en clúster

### Integridad

El volcado contiene `public.tasks`, `public.audit_records` y `public."__EFMigrationsHistory"`. No se contaron filas duplicadas. La copia no se versiona.

### Resultado

- [x] Aprobado
- [ ] Fallido
- [ ] Inconcluso

### Evidencias

El volcado de comprobación quedó en `infrastructure/local/backups`, ignorado por git.

### Observaciones y limitaciones

La comprobación es de la base de Compose, no de un StatefulSet restaurado dentro de k3d. No cubre un desastre del disco del host.
