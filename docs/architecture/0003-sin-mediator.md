# ADR 0003 — Sin Mediator

## Contexto

MediatR desacopla el endpoint del manejador, pero añade una indirección y un paquete para un conjunto pequeño de casos de uso que ya viven en clases propias.

## Decisión

No se usa MediatR ni otro mediator. Cada caso de uso es una clase registrada en `AddApplication()` y el endpoint la recibe por inyección de dependencias.

## Consecuencias

El grafo de dependencias se lee en el contenedor y en el endpoint. No hay pipeline de behaviors genérico: la validación la ejecuta el caso de uso con FluentValidation y los errores HTTP los centraliza el manejador de excepciones. Si el número de cross-cutting concerns crece, se puede introducir un mediator sin cambiar las reglas de dominio.
