# Registro técnico

La API escribe en la consola con Serilog en JSON compacto. Cada línea incluye el nivel, el mensaje, la aplicación y las propiedades de contexto. La petición HTTP se resume con método, ruta, código de estado y duración. El cuerpo de la petición, las cabeceras `Authorization` y los tokens no se registran.

El identificador de correlación de la traza de OpenTelemetry viaja en el contexto de diagnóstico. La auditoría de negocio vive en `audit_records` y no se mezcla con estos logs.
