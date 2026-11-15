# ADR 0002 — CQRS sin bases de datos separadas

## Contexto

Las lecturas (listado filtrado y paginado) y las escrituras (crear, actualizar, cambiar estado, eliminar) tienen contratos distintos, pero el laboratorio no necesita dos almacenes ni sincronización entre ellos.

## Decisión

Los casos de uso se separan en comandos y consultas dentro de `Application/Tasks`. Todos usan el mismo `DbContext` y la misma base PostgreSQL. No hay bus de eventos ni modelo de lectura materializado.

## Consecuencias

El listado puede proyectar columnas sin cargar el agregado completo. No hay consistencia eventual que explicar en la demo. Si más adelante hiciera falta un read model, se añadiría sin cambiar el contrato HTTP.
