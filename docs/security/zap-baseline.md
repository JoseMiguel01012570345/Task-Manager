# Línea base de ZAP

El script `infrastructure/local/scripts/Invoke-ZapBaseline.ps1` lanza `ghcr.io/zaproxy/zaproxy:stable` contra el ingreso de Traefik (`http://host.docker.internal:8088`). El código 2 de ZAP significa avisos, no un fallo del escáner. No afirma ausencia de vulnerabilidades: es una pasada baseline del laboratorio.

`Invoke-Tests.ps1` lo lanza al final, contra la API que ya responde detrás de Traefik. En la corrida del 2026-11-16 el baseline terminó con 0 fallos y 2 avisos nuevos: contenido no almacenable y aplicación web moderna. Esos avisos no hacen fallar el script. El informe HTML queda donde lo escribe el contenedor de ZAP.
