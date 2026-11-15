# Ingreso

El tráfico de la aplicación entra solo por Traefik, en `http://localhost:8088`. El servicio de detrás no publica puertos en el host: vive en la red `taskmanagement-edge` y Traefik lo alcanza por el nombre `whoami`. Cuando existan las imágenes de la API y del frontend, ese destino pasa a ser la aplicación y este puerto sigue siendo la única entrada.

```powershell
docker compose -f infrastructure/edge/docker-compose.yml up -d
```
