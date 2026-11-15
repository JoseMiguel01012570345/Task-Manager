# Cortafuegos de aplicación

Traefik carga Coraza con el conjunto de reglas OWASP CRS. El ingreso general queda en detección: la petición sigue y la coincidencia se registra. La ruta de prueba `/waf-probe` usa el mismo conjunto en bloqueo y responde 403 cuando una regla niega la petición.

El destino no publica puertos. La única forma de llegar a él es atravesar Traefik.
