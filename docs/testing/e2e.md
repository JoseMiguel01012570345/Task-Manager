# Flujo de extremo a extremo

`tests/e2e/frontend.spec.ts` entra por la pantalla de login y crea una tarea en el frontend publicado en `http://localhost:8088`. `Invoke-Tests.ps1` lo deja en marcha mientras corren k6 y la resiliencia. El spec exige al menos un alta en esa ventana y otra cuando la bandera ya no existe, con hasta tres minutos de reintento. El script espera hasta seis minutos a que Playwright termine.

`tests/e2e/tasks.spec.ts` hace el mismo alta y lectura contra la API, sin abrir la interfaz. Pide un token de Keycloak para `dev-user`. Playwright usa Chrome, el mismo runner que las pruebas de texto frente a XSS.

```powershell
npx playwright test tasks.spec.ts
```

El comando se ejecuta en `tests/e2e` con el clúster k3d ya desplegado.
