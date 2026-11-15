# Salud

`GET /health/live` responde 200 cuando el proceso atiende peticiones. No consulta PostgreSQL.

`GET /health/ready` consulta el `DbContext`. Responde 200 si la base acepta la comprobación y 503 si no. Los dos endpoints son anónimos y no entran en las trazas de negocio.
