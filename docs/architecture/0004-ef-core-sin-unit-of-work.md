# ADR 0004 — EF Core sin Repository ni Unit of Work propios

## Contexto

Un repositorio genérico y un unit of work propios repetirían `DbSet` y `SaveChanges`, que ya son esa abstracción en EF Core.

## Decisión

Los casos de uso dependen de interfaces pequeñas definidas en Application (`ITaskRepository` para las consultas y comandos que el dominio no puede expresar con una sola llamada). Infrastructure las implementa con EF Core. No hay un `IUnitOfWork`: quien persiste llama a `SaveChanges` en el mismo método que la operación. Las consultas de listado se proyectan en Infrastructure.

## Consecuencias

Las pruebas de dominio no necesitan un repositorio. Las pruebas de integración usan PostgreSQL real. No se puede cambiar el ORM sin tocar Infrastructure, y eso es aceptable porque el laboratorio estandariza EF Core y Npgsql.
