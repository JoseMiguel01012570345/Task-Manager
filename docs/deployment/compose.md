# Compose de la aplicación

El `docker-compose.yml` de la raíz ya no está. Levantaba PostgreSQL, Redis, Keycloak, la API y el frontend como contenedores `taskmanagement-*`, y esos nombres se confundían con los nodos del clúster k3d.

El laboratorio de la aplicación es el clúster de `docs/deployment/k3d.md`. PostgreSQL, Keycloak y la observabilidad siguen teniendo un compose propio para el desarrollo local.
