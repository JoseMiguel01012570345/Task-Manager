# Demostración

Esta guía usa lo que ya puede arrancar en el portátil. No depende del clúster k3d.

## 1. Base y API

```powershell
docker compose -f infrastructure/database/docker-compose.yml up -d
dotnet run --project src/TaskManagement.API --launch-profile http
```

En otra terminal:

```powershell
curl http://localhost:5080/health/live
curl http://localhost:5080/health/ready
curl http://localhost:5080/openapi/v1.json
```

Scalar está en `http://localhost:5080/scalar`.

## 2. Una tarea

Sin cabecera `Authorization`, Development usa `dev-user`.

```powershell
curl -H "Content-Type: application/json" -d "{\"title\":\"Demo\",\"assigneeId\":\"dev-user\"}" http://localhost:5080/api/v1/tasks
```

La respuesta 201 trae el `id`. `GET /api/v1/tasks/{id}` devuelve la misma tarea. Un `id` que no te pertenece responde 404.

## 3. Frontend

```powershell
cd src/TaskManagement.Web
npm install
npm run dev
```

Abre `http://localhost:5173`, crea una tarea, cámbiale el estado y bórrala. El borrado pide confirmación en la página.

## 4. Ingreso y cortafuegos

```powershell
docker compose -f infrastructure/edge/docker-compose.yml up -d
curl http://localhost:8088/waf-probe
curl "http://localhost:8088/waf-probe?id=1%27%20OR%20%271%27%3D%271"
```

La primera responde la aplicación (o 502 si la API aún no está en esa red). La segunda responde 403.

## 5. Observabilidad

Grafana queda dentro del clúster, en `http://localhost:8088/grafana/`, con el usuario `admin` y la contraseña de `.env.example`. Los tableros están en la carpeta Task Management.

## 6. Lo que esta guía no ejecuta

Esta guía no lanza k6, ZAP ni Chaos Mesh. Eso lo hace `infrastructure/local/scripts/Invoke-Tests.ps1` con el clúster ya desplegado. El detalle está en `docs/performance`, `docs/security/zap-baseline.md`, `docs/chaos` y `docs/reports/summary.md`.
