# Auditoría

Cada alta, modificación, cambio de estado y baja escribe una fila en `audit_records` en la misma transacción que el cambio. La fila guarda actor, acción, recurso, fecha UTC, correlación de la traza y un resumen corto. No guarda secretos ni el cuerpo completo de la petición.

La aplicación solo inserta. No hay endpoint para editar o borrar el historial. Los logs técnicos de Serilog van por otro canal.
