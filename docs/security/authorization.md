# Autorización

Hay dos roles de realm: `User` y `Administrator`.

| Acción | User | Administrator |
|---|---|---|
| Crear una tarea asignada a sí mismo | Sí | Sí |
| Asignar la tarea a otra persona | No | Sí |
| Leer o cambiar una tarea propia (creador o responsable) | Sí | Sí |
| Leer o cambiar una tarea ajena | No. La API responde `404` | Sí |

La política `TaskAccess` exige un usuario autenticado en `/api/v1/tasks`. La política `Administrator` exige el rol. La propiedad del recurso se comprueba en los casos de uso con `TaskAccess` del proyecto Application: el `assigneeId` enviado por el cliente no se acepta si el actor no puede asignarlo.

Un usuario de desarrollo no administrador no puede leer una tarea cuyo responsable y creador son otra persona. Esa regla está cubierta por `GetTaskHandlerTests`.
