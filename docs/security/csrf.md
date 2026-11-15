# CSRF

La API no autentica con cookies. El cliente guarda el access token en memoria y lo envía en `Authorization: Bearer`. Un sitio de otro origen no puede leer ese token ni hacer que el navegador lo adjunte solo: no hay cookie de sesión, así que no aplica el antiforgery de cookie ni hace falta un token CSRF adicional.

Las respuestas de tareas no envían `Set-Cookie`. Las cookies de terceros no participan en el flujo. Si en el futuro la sesión pasara a una cookie, habría que añadir antiforgery y `SameSite`, `Secure` y `HttpOnly`. Hoy ese no es el mecanismo.
