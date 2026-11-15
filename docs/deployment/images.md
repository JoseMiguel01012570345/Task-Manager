# Imágenes

Las dos imágenes se construyen en varias etapas y el proceso final no es root. La API usa el usuario `app` de la imagen `aspnet:9.0` y escucha en el puerto 8080. El frontend se compila con la imagen local `node:22-alpine` y se sirve con `nginx:1.27-alpine` bajo el usuario `nginx`, también en el puerto 8080. El desarrollo local sigue usando Node 20, fijado en `.nvmrc`.

```powershell
docker build -f src/TaskManagement.API/Dockerfile -t taskmanagement-api:local .
docker build -f src/TaskManagement.Web/Dockerfile -t taskmanagement-web:local src/TaskManagement.Web
```

`VITE_API_BASE_URL` vacío hace que el navegador llame a `/api` en el mismo origen. Traefik reparte esa ruta hacia la API.
