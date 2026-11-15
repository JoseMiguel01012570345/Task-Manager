# Laboratorio de tráfico alto

Este laboratorio no demuestra protección contra una denegación de servicio. El host es una sola máquina, los generadores son pocos y el límite de tasa es un contador en memoria de cada réplica. Un atacante con más origenes, o un corte de red del propio host, queda fuera de lo que se puede afirmar.

Lo que sí se puede observar, cuando k6 está instalado y la API responde:

- El máximo de peticiones que el escenario llegó a enviar, no un máximo universal.
- Cuántas respuestas fueron 2xx, 4xx, 429 y 5xx.
- Si las peticiones de lectura siguieron respondiendo mientras las de escritura recibían 429.
- La capa que respondió: la API (429 y `Retry-After`), Traefik o Coraza en `/waf-probe`.

El escenario `tests/k6/rate-limit.js` está escrito para provocar 429. Sin una ejecución registrada, el resultado es inconcluso. No se publica un número de “tráfico soportado”.
