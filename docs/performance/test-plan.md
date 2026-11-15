# Plan de carga

Las pruebas de carga son del laboratorio local. No describen la capacidad de un despliegue de producción y no afirman resistencia a una denegación de servicio.

## Objetivo

Medir, con tráfico controlado, la tasa de respuestas, la latencia y el momento en que aparece HTTP 429 o 5xx. El ingreso es Traefik en `http://localhost:8088` cuando la API está detrás de él. En el clúster el generador llama a `http://api:8080`.

## Entorno

- Un host de desarrollo, no un centro de datos.
- k6 en la máquina o un Job dentro del namespace `taskmanagement`.
- La API en las réplicas que tenga el manifiesto (mínimo 2 cuando el clúster existe).
- PostgreSQL del laboratorio. Redis está en Compose, pero el límite de tasa todavía es por proceso.

## Escenarios

| Escenario | Archivo | Intención |
| --- | --- | --- |
| Humo | `tests/k6/smoke.js` | Un usuario confirma que `/health/live` responde |
| Carga | `tests/k6/load.js` | Tráfico sostenido y moderado de lectura |
| Estrés | `tests/k6/stress.js` | Sube hasta encontrar degradación |
| Pico | `tests/k6/spike.js` | Un salto breve y la recuperación |
| Resistencia | `tests/k6/soak.js` | Poca carga durante más tiempo |
| Mixto | `tests/k6/mixed.js` | Lecturas y escrituras |
| Límite | `tests/k6/rate-limit.js` | Provocar 429 |

Cada ejecución se anota con la plantilla de la sección 8 del plan. Si k6 no está instalado, el resultado es inconcluso: los escenarios quedan versionados y no se inventan cifras.
