# ADR 0006 — .NET 9 en lugar de .NET 10

## Contexto

El plan de trabajo nombra ASP.NET Core 10. En el equipo de laboratorio solo está instalado el SDK 9.0.301.

## Decisión

La solución apunta a `net9.0`. `global.json` pide el SDK 9.0.301 con `rollForward` `latestFeature`. No se instala un SDK 10 en este ciclo.

## Consecuencias

El comportamiento de la API se demuestra sobre la línea 9. Subir a `net10.0` es un cambio de `TargetFramework` y de paquetes cuando el SDK esté disponible; no cambia las reglas de dominio ni el contrato HTTP.
