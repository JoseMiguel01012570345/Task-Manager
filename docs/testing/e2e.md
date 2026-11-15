# Flujo de extremo a extremo

`tests/e2e/tasks.spec.ts` crea una tarea y la vuelve a leer contra la API en `http://localhost:5080`, con el usuario de desarrollo. Playwright usa Chrome, el mismo runner que las pruebas de texto frente a XSS.

```powershell
npx playwright test tasks.spec.ts
```

El comando se ejecuta en `tests/e2e` con la API ya en marcha.
