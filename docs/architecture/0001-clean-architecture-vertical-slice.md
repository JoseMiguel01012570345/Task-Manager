# ADR 0001 — Clean Architecture y vertical slice

## Contexto

La API tiene que separar reglas de negocio, casos de uso, persistencia y HTTP, y a la vez mantener cada operación de tareas junta para que se pueda seguir sin saltar entre carpetas genéricas.

## Decisión

Cuatro proyectos: `Domain`, `Application`, `Infrastructure` y `API`. Las dependencias apuntan hacia el dominio. Dentro de Application e Infrastructure, el código de tareas vive bajo la feature `Tasks`, no bajo carpetas `Services` o `Repositories` transversales.

## Consecuencias

Domain no referencia EF Core ni ASP.NET Core. Cambiar PostgreSQL o el estilo de los endpoints no obliga a reescribir las reglas de estado. Una feature nueva añade archivos en las capas que le correspondan, sin un proyecto por caso de uso.
