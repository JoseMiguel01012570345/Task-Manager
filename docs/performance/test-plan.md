# Plan de carga

Las pruebas de carga son del laboratorio local. No describen la capacidad de un despliegue de producción y no afirman resistencia a una denegación de servicio.

## Objetivo

Medir, con tráfico controlado, la tasa de respuestas, la latencia y el momento en que aparece HTTP 429 o 5xx. El ingreso es Traefik en `http://localhost:8088` cuando la API está detrás de él. En el clúster el generador llama a `http://api:8080`.

## Entorno

- Un host de desarrollo, no un centro de datos.
- k6 en la máquina o un Job dentro del namespace `taskmanagement`.
- La API en 3 réplicas. El límite de tasa es por proceso, así que el borde reparte el cupo entre ellas.

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

`K6ScenarioTests` lanza los siete archivos con `SUITE=1`. Si k6 no está en PATH, esa prueba no arranca. No se inventan cifras: el informe guarda el código de salida, no los percentiles.
