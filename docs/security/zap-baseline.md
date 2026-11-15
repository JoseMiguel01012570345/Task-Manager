# Línea base de ZAP

El script `infrastructure/local/scripts/Invoke-ZapBaseline.ps1` lanza `ghcr.io/zaproxy/zaproxy:stable` contra el ingreso de Traefik (`http://host.docker.internal:8088`). El código 2 de ZAP significa avisos, no un fallo del escáner. No afirma ausencia de vulnerabilidades: es una pasada baseline del laboratorio.

En esta máquina la imagen de ZAP no estaba descargada y no se ejecutó el escaneo. El informe HTML aparece cuando el script termina contra una API que ya responde detrás de Traefik.
