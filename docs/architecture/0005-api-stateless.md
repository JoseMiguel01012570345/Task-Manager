# ADR 0005 — Instancias de API sin estado local

## Contexto

Varias réplicas atienden tráfico detrás de Traefik. Una petición y la siguiente pueden caer en procesos distintos.

## Decisión

La API no guarda sesión ni archivos de negocio en memoria o disco local. La identidad viaja en el JWT de cada petición. Los datos de tareas y la auditoría están en PostgreSQL. Los contadores de rate limiting compartidos entre réplicas están en Redis. Las trazas se exportan al collector; no se conservan solo en el proceso.

## Consecuencias

Cualquier réplica puede responder una petición autenticada. Perder un pod no pierde tareas confirmadas. Hay que externalizar cada estado nuevo (caché, límites, sesiones) en lugar de dejarlo en un campo estático.
