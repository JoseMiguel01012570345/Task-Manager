# Autenticación JWT

La API valida access tokens de Keycloak con la autoridad `Authentication:Authority`, la audiencia `taskmanagement-api`, la firma publicada por el realm y la expiración. Un token inválido, expirado o de otro emisor no autentica la petición.

En Development, una petición sin cabecera `Authorization` usa el usuario de `DevUser` para poder trabajar el frontend sin un token. Si la cabecera `Bearer` está presente, se valida como JWT y un token malo responde `401`. Fuera de Development no existe ese usuario de desarrollo: toda ruta de tareas exige un token válido.

Los roles de realm `User` y `Administrator` se copian a claims de rol. El identificador del actor es `preferred_username`.
