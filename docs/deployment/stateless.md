# Estado

La API no guarda la sesión en disco ni en memoria compartida. El token viaja en `Authorization` y cada réplica puede atender cualquier petición. Los contadores de límite de tasa viven en la memoria de la réplica hasta que Redis quede cableado; perder una réplica no pierde tareas.

Lo único durable es PostgreSQL. En el clúster es un StatefulSet con un volumen de 1 Gi. El Service es headless y no se publica en el nodo. La copia la hace el CronJob `postgres-backup` y queda en el volumen `postgres-backups`.

Comprobación de que la API no necesita disco local:

- No hay volúmenes en el Deployment de la API.
- Las migraciones corren en un Job, no al arrancar cada réplica.
- `/health/ready` falla si la base no responde, así que una réplica sin base no recibe tráfico.
