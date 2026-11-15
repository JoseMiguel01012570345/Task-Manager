# XSS

Las pruebas de `tests/e2e` abren Chrome y comprueban tres casos: un texto guardado, un parámetro reflejado y el fragmento de la URL. En los tres el valor se asigna con `textContent`. El payload no abre un diálogo ni marca `window.__xss`. Otra prueba recorre el código React y falla si aparece `dangerouslySetInnerHTML` o `innerHTML`.

```powershell
npm install
npm test
```

El comando se ejecuta dentro de `tests/e2e` y usa el Chrome instalado en la máquina.
