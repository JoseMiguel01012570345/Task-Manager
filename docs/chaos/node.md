# Nodo

Chaos Mesh no apaga un nodo de k3d. El nodo es el contenedor Docker del agente, y la prueba lo detiene desde el host.

`Invoke-Resilience.ps1` para los dos agentes, `k3d-taskmanagement-agent-0` y `k3d-taskmanagement-agent-1`, y deja el servidor. El plano de control sigue y es el que debe atender `http://localhost:8088/health/ready`. Al terminar, el script arranca los dos agentes, espera a que los nodos vuelvan a Ready, espera el Deployment de la API y un token de Keycloak.

Keycloak tiene una réplica y no se programa en el plano de control. Al parar los dos agentes se va con ellos. Usa la base embebida, así que al volver importa el realm de nuevo. `Wait-Keycloak.ps1` espera hasta cuatro minutos a que `dev-user` reciba un token en `http://localhost:8088`.
