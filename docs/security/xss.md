# XSS

El frontend pinta título, descripción y demás campos de la tarea como texto de React. No usa `dangerouslySetInnerHTML`, `eval` ni HTML enriquecido. Un payload guardado se muestra como caracteres, no como un script.

Cabeceras:

- API: `Content-Security-Policy: default-src 'none'; frame-ancestors 'none'; base-uri 'none'`, `X-Content-Type-Options: nosniff`, `Referrer-Policy: no-referrer`.
- Frontend en Vite: `default-src 'self'`, scripts del mismo origen, estilos en línea que emite el bundler, `connect-src` hacia la propia UI y la API local, `object-src 'none'`.

HSTS se añadirá en el ingress cuando el laboratorio use HTTPS. En HTTP local no se envía.
