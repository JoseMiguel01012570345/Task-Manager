# Flujo de extremo a extremo

`tests/e2e/tasks.spec.ts` crea una tarea y la vuelve a leer contra la API por Traefik, en `http://localhost:8088`. Pide un token de Keycloak para `dev-user`. Playwright usa Chrome, el mismo runner que las pruebas de texto frente a XSS.

```powershell
npx playwright test tasks.spec.ts
```

El comando se ejecuta en `tests/e2e` con el clúster k3d ya desplegado.
