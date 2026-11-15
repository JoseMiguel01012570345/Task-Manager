# Preparación del entorno

Un desarrollador puede preparar el equipo con esta guía. Las versiones de abajo son las comprobadas en el laboratorio el 15 de noviembre de 2026.

El plan de trabajo menciona ASP.NET Core 10. Este repositorio usa **.NET 9** (`net9.0`) porque el SDK presente es 9.0.301 y no hay SDK 10 instalado. `global.json` fija esa banda.

## Versiones soportadas

| Herramienta | Versión comprobada | Para qué |
|---|---|---|
| .NET SDK | 9.0.301 | API, dominio y pruebas |
| Node.js | 20.18.1 | Frontend React |
| npm | 10.8.2 | Dependencias del frontend |
| Docker Engine | 28.1.1 | Contenedores locales |
| Git | 2.x o superior | Historial y commits |
| PowerShell | 5.1 o PowerShell 7 | Scripts de Windows |

Comprobar:

```powershell
dotnet --version
node --version
npm --version
docker --version
git --version
```

`dotnet --version` debe resolver a la banda 9.0 que declara `global.json`. Node debe ser 20, alineado con `.nvmrc`.

## Qué hace falta instalar

1. [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0) si `dotnet --list-sdks` no muestra 9.0.301 o una 9.0 posterior (`rollForward` es `latestFeature`).
2. [Node.js 20 LTS](https://nodejs.org/).
3. [Docker Desktop](https://www.docker.com/products/docker-desktop/) en ejecución. PostgreSQL, Keycloak, Redis y el clúster local corren en contenedores; no hace falta instalar PostgreSQL en el host.
4. Git.

Herramientas que entran más adelante, con los scripts del repositorio: k3d, kubectl, k6, Trivy, Playwright y OWASP ZAP. No son necesarias para compilar la API.

## Editor

El repositorio incluye `.editorconfig` (C#, JSON, TypeScript y Markdown) y `nuget.config` (solo nuget.org). El IDE debe respetar EditorConfig. No hay configuración de usuario que haya que copiar fuera del repo.

## Frontend

La aplicación React se crea en `src/TaskManagement.Web`. La URL de la API se configura con `VITE_API_BASE_URL` en un `.env` local que no se versiona. La plantilla commiteada es `.env.example`. El bundle no debe contener secretos.

## Siguiente paso

Con el SDK y Node disponibles, la solución se compila desde la raíz:

```powershell
dotnet build TaskManagement.sln
```

Esa solución aparece a partir de la estructura inicial del repositorio.
