# Plan de trabajo — Task Management API distribuida, segura y resiliente

## 1. Objetivo

Construir una aplicación de gestión de tareas con **ASP.NET Core 10, C#, React y PostgreSQL**, preparada para ejecutarse como un sistema distribuido y validada íntegramente en un laboratorio local.

El proyecto debe demostrar, mediante pruebas reproducibles y evidencias, que:

- La API REST cumple las operaciones CRUD, filtros y paginación requeridos.
- La aplicación puede ejecutarse con varias réplicas distribuidas entre nodos Kubernetes locales.
- El sistema mantiene las operaciones legítimas dentro de los objetivos establecidos mientras se genera tráfico abusivo HTTP controlado.
- Las instancias y los componentes se recuperan de fallos simulados.
- La aplicación incorpora controles contra XSS, inyección, abuso de API y acceso no autorizado.
- Se pueden observar y correlacionar solicitudes, errores, métricas y eventos de auditoría.
- La instalación, las pruebas y los experimentos pueden repetirse en un equipo local mediante scripts y documentación.

> **Alcance:** todo el desarrollo, despliegue y experimentación se realizará localmente. No se incluyen tareas de AWS, otros proveedores cloud, despliegues externos ni pruebas en infraestructura física o pública real. El laboratorio local demuestra distribución entre contenedores y nodos virtualizados; no demuestra tolerancia a la pérdida física de un equipo o centro de datos.

---

## 2. Arquitectura y tecnologías previstas

| Área | Tecnología propuesta | Uso |
|---|---|---|
| Backend | ASP.NET Core 10 / C# | API REST |
| Arquitectura | Clean Architecture + Vertical Slice | Separación de responsabilidades y organización por casos de uso |
| Persistencia | PostgreSQL + Entity Framework Core | Datos y migraciones |
| Frontend | React + TypeScript | Interfaz de usuario |
| Identidad | Keycloak + OpenID Connect / OAuth 2.0 | Autenticación y emisión de tokens |
| Contenedores | Docker | Empaquetado reproducible |
| Orquestación local | K3d / K3s | Clúster Kubernetes local con varios nodos |
| Entrada y balanceo | Traefik | Ingress y distribución de solicitudes |
| WAF | Coraza + OWASP Core Rule Set | Inspección y bloqueo de solicitudes HTTP maliciosas |
| Rate limiting compartido | Redis | Contadores y límites coordinados entre réplicas |
| Validación | FluentValidation (opcional) | Validación de comandos y DTOs |
| Mediación | MediatR (opcional) | Desacoplamiento de casos de uso |
| Documentación API | OpenAPI + Swagger UI o Scalar | Contrato y exploración de endpoints |
| Logs | Serilog + Loki | Logs estructurados y centralizados |
| Trazas y métricas | OpenTelemetry | Instrumentación y correlación |
| Monitorización | Prometheus + Grafana | Métricas, paneles y alertas |
| Pruebas backend | xUnit | Pruebas unitarias y de integración |
| Pruebas con navegador | Playwright | Flujos reales de frontend y validaciones XSS |
| Carga HTTP | Grafana k6 | Load, stress, spike, soak y mixed traffic |
| Fallos controlados | Chaos Mesh | Experimentos de resiliencia en Kubernetes |
| Seguridad dinámica | OWASP ZAP | Análisis DAST del entorno local |
| Escaneo de imágenes | Trivy | Vulnerabilidades en imágenes y dependencias |
| CI | GitHub Actions | Compilación y pruebas automatizadas |

Las tecnologías opcionales se incorporarán solo si aportan valor concreto. Se evitará añadir componentes que no sean necesarios para demostrar los requisitos.

---

## 3. Estructura objetivo del repositorio

```text
TaskManagement/
├── src/
│   ├── TaskManagement.API/
│   ├── TaskManagement.Application/
│   ├── TaskManagement.Domain/
│   ├── TaskManagement.Infrastructure/
│   └── TaskManagement.Web/
├── tests/
│   ├── Unit/
│   ├── Integration/
│   ├── Architecture/
│   ├── Security/
│   │   ├── XSS/
│   │   ├── Authentication/
│   │   ├── Authorization/
│   │   └── OWASP/
│   ├── Performance/
│   │   ├── k6/
│   │   └── playwright/
│   └── Resilience/
│       ├── PodFailure/
│       ├── NodeFailure/
│       ├── NetworkFailure/
│       └── DependencyFailure/
├── infrastructure/
│   ├── local/
│   │   ├── docker/
│   │   ├── k3d/
│   │   └── scripts/
│   ├── kubernetes/
│   │   ├── base/
│   │   └── overlays/local/
│   ├── traefik/
│   ├── coraza/
│   ├── database/
│   └── monitoring/
├── docs/
│   ├── architecture/
│   ├── api/
│   ├── security/
│   ├── deployment/
│   ├── performance/
│   └── resilience/
├── .github/workflows/
├── docker-compose.yml
├── Makefile                # o scripts equivalentes para Windows
└── README.md
```

La estructura podrá ajustarse durante la implementación, pero las responsabilidades de las capas deben mantenerse separadas.

---

# 4. Backlog de tareas

Las tareas están organizadas por fases y tienen identificadores para poder referenciarlas en issues o en un tablero Kanban.

Prioridades:

- **P0 — Obligatoria:** necesaria para cumplir la tarea o ejecutar las fases posteriores.
- **P1 — Alta:** necesaria para demostrar seguridad, distribución o resiliencia.
- **P2 — Complementaria:** mejora la calidad, automatización o presentación del proyecto.

Estados sugeridos: `Pendiente`, `En curso`, `Bloqueada`, `Hecha`.

## Fase 0 — Definición y preparación del entorno

### TASK-001 — Definir requisitos funcionales y no funcionales
- **Prioridad:** P0
- **Objetivo:** documentar el comportamiento esperado y los límites del sistema.
- **Actividades:**
  - Definir los campos de una tarea: título, descripción, estado, responsable y fecha límite.
  - Definir los estados permitidos: `Pending`, `InProgress` y `Done` (o sus equivalentes en español, de forma consistente).
  - Definir las reglas de propiedad y acceso a tareas.
  - Definir límites iniciales de paginación, longitud de campos y tamaño de solicitudes.
  - Definir objetivos iniciales de rendimiento y disponibilidad para el laboratorio.
  - Documentar qué significa superar cada prueba y qué métricas se recopilarán.
- **Entregables:** `docs/requirements.md` y una matriz de requisitos.
- **Criterios de aceptación:** cada requisito tiene una descripción verificable y un criterio de aceptación. Los objetivos de rendimiento son hipótesis medibles, no garantías universales.

### TASK-002 — Preparar herramientas de desarrollo
- **Prioridad:** P0
- **Dependencias:** TASK-001
- **Actividades:**
  - Preparar .NET SDK 10, Node.js, Docker y Git.
  - Preparar herramientas para PostgreSQL y pruebas.
  - Documentar versiones soportadas.
  - Crear archivos de configuración de editor y formato.
- **Entregables:** `global.json` (si aplica), configuración del frontend y guía de preparación.
- **Criterios de aceptación:** un desarrollador puede preparar el entorno siguiendo exclusivamente la documentación del repositorio.

### TASK-003 — Crear solución y estructura Clean Architecture
- **Prioridad:** P0
- **Dependencias:** TASK-002
- **Actividades:**
  - Crear los proyectos `API`, `Application`, `Domain` e `Infrastructure`.
  - Crear proyectos de pruebas.
  - Configurar referencias permitidas entre capas.
  - Configurar inyección de dependencias.
  - Definir convenciones de nombres y estructura por feature.
- **Entregables:** solución .NET compilable y estructura inicial.
- **Criterios de aceptación:** Domain no depende de Infrastructure ni de ASP.NET Core; Application no depende de API; la solución compila.

### TASK-004 — Documentar decisiones arquitectónicas
- **Prioridad:** P1
- **Dependencias:** TASK-003
- **Actividades:**
  - Documentar Clean Architecture y Vertical Slice Architecture.
  - Explicar el uso de CQRS para separar lecturas y escrituras sin exigir bases de datos separadas.
  - Justificar cuándo se utiliza Mediator.
  - Documentar Repository/Unit of Work solo donde aporten abstracción; evitar duplicar innecesariamente las capacidades de EF Core.
  - Documentar la estrategia stateless de las instancias API.
- **Entregables:** ADRs en `docs/architecture/` y diagrama de componentes.
- **Criterios de aceptación:** las principales decisiones tienen contexto, decisión y consecuencias documentadas.

---

## Fase 1 — Dominio, persistencia y API REST

### TASK-005 — Modelar entidad Task y reglas de dominio
- **Prioridad:** P0
- **Dependencias:** TASK-003
- **Actividades:**
  - Crear la entidad `Task`.
  - Definir identificador, título, descripción, estado, responsable, fecha límite y fechas de creación/actualización.
  - Definir reglas para cambios de estado y validaciones de dominio.
  - Considerar una propiedad de versión o token de concurrencia.
  - Evitar que la entidad dependa de EF Core o de HTTP.
- **Entregables:** modelo de dominio y pruebas unitarias de reglas.
- **Criterios de aceptación:** los estados no admitidos y las transiciones inválidas se rechazan en el dominio.

### TASK-006 — Configurar PostgreSQL y Entity Framework Core
- **Prioridad:** P0
- **Dependencias:** TASK-005
- **Actividades:**
  - Configurar el proveedor PostgreSQL de EF Core.
  - Crear `DbContext` y configuraciones de entidades.
  - Definir índices para estado, responsable y fecha límite cuando estén justificados por las consultas.
  - Crear la migración inicial.
  - Configurar datos semilla exclusivamente para desarrollo y pruebas.
- **Entregables:** configuración de persistencia y migración inicial.
- **Criterios de aceptación:** la aplicación crea y consulta datos en PostgreSQL; las migraciones pueden aplicarse desde una instrucción documentada.

### TASK-007 — Implementar creación de tareas
- **Prioridad:** P0
- **Dependencias:** TASK-005, TASK-006
- **Actividades:**
  - Crear el comando/caso de uso `CreateTask`.
  - Definir DTO de entrada y respuesta.
  - Validar título, descripción, responsable, estado inicial y fecha límite.
  - Persistir la tarea.
  - Devolver `201 Created` y la referencia al recurso.
- **Entregables:** endpoint `POST /api/v1/tasks`.
- **Criterios de aceptación:** una solicitud válida crea una tarea; los datos inválidos devuelven errores estructurados y no insertan registros.

### TASK-008 — Implementar listado, filtros y paginación
- **Prioridad:** P0
- **Dependencias:** TASK-006
- **Actividades:**
  - Implementar `GET /api/v1/tasks`.
  - Filtrar por estado y responsable.
  - Incorporar paginación mediante `page` y `pageSize` o un contrato equivalente.
  - Aplicar ordenamiento estable.
  - Limitar el máximo de elementos por página.
  - Devolver metadatos de paginación.
- **Entregables:** endpoint de listado y consultas.
- **Criterios de aceptación:** los filtros pueden combinarse; no se devuelven registros de otras páginas; los parámetros fuera de rango se gestionan de forma consistente.

### TASK-009 — Implementar consulta individual
- **Prioridad:** P0
- **Dependencias:** TASK-006
- **Actividades:**
  - Implementar `GET /api/v1/tasks/{id}`.
  - Comprobar autorización sobre la tarea.
  - Devolver `404 Not Found` cuando no exista o no sea visible según la política de acceso.
- **Entregables:** endpoint de consulta individual.
- **Criterios de aceptación:** devuelve la tarea correcta o el error HTTP correspondiente sin filtrar información no autorizada.

### TASK-010 — Implementar actualización de tareas
- **Prioridad:** P0
- **Dependencias:** TASK-006, TASK-009
- **Actividades:**
  - Implementar `PUT /api/v1/tasks/{id}`.
  - Validar los datos modificables.
  - Definir campos que no pueden cambiarse directamente.
  - Añadir control de concurrencia optimista.
  - Devolver `409 Conflict` o `412 Precondition Failed` cuando exista conflicto, según el contrato elegido.
- **Entregables:** endpoint de actualización.
- **Criterios de aceptación:** una actualización válida persiste; una versión obsoleta no sobrescribe silenciosamente cambios concurrentes.

### TASK-011 — Implementar cambio de estado
- **Prioridad:** P1
- **Dependencias:** TASK-005, TASK-006
- **Actividades:**
  - Implementar `PATCH /api/v1/tasks/{id}/status`.
  - Validar los estados y las transiciones permitidas.
  - Registrar la modificación para auditoría.
- **Entregables:** endpoint específico de cambio de estado.
- **Criterios de aceptación:** los estados permitidos se actualizan; valores inválidos producen `400 Bad Request` o `422 Unprocessable Entity`, según el estándar adoptado.

### TASK-012 — Implementar eliminación de tareas
- **Prioridad:** P0
- **Dependencias:** TASK-006
- **Actividades:**
  - Implementar `DELETE /api/v1/tasks/{id}`.
  - Definir si la eliminación será física o lógica y documentar la decisión.
  - Comprobar autorización.
  - Registrar la operación en auditoría.
- **Entregables:** endpoint de eliminación.
- **Criterios de aceptación:** la eliminación autorizada produce el código HTTP definido; una tarea inexistente devuelve `404`.

### TASK-013 — Estandarizar validaciones y respuestas de error
- **Prioridad:** P0
- **Dependencias:** TASK-007 a TASK-012
- **Actividades:**
  - Definir un contrato común de errores con `ProblemDetails`.
  - Centralizar excepciones mediante middleware o exception handler.
  - Asociar `traceId` a errores.
  - Diferenciar validación, autenticación, autorización, conflicto y error interno.
  - No exponer stack traces ni detalles internos en producción.
- **Entregables:** middleware/handler y documentación de errores.
- **Criterios de aceptación:** los errores son consistentes y utilizan códigos HTTP apropiados: `400`, `401`, `403`, `404`, `409`, `429` y `500` cuando corresponda.

### TASK-014 — Configurar concurrencia optimista
- **Prioridad:** P1
- **Dependencias:** TASK-010
- **Actividades:**
  - Añadir token de concurrencia o versión.
  - Verificar la versión durante actualizaciones.
  - Definir el comportamiento de conflictos en API.
  - Crear pruebas de dos actualizaciones concurrentes.
- **Entregables:** control de concurrencia documentado.
- **Criterios de aceptación:** una actualización concurrente incompatible se detecta y no produce pérdida silenciosa de cambios.

---

## Fase 2 — Frontend y experiencia de usuario

### TASK-015 — Crear frontend React
- **Prioridad:** P0
- **Dependencias:** TASK-001
- **Actividades:**
  - Crear aplicación React con TypeScript.
  - Configurar rutas, estructura de componentes y manejo de configuración por entorno.
  - Crear cliente HTTP centralizado.
  - Evitar almacenar secretos en el bundle del navegador.
- **Entregables:** aplicación frontend ejecutable localmente.
- **Criterios de aceptación:** el frontend arranca y puede comunicarse con la API usando una URL configurable.

### TASK-016 — Implementar pantallas de gestión de tareas
- **Prioridad:** P0
- **Dependencias:** TASK-007 a TASK-012, TASK-015
- **Actividades:**
  - Crear listado con paginación.
  - Añadir filtros por estado y responsable.
  - Crear formulario de alta y edición.
  - Crear vista de detalle.
  - Añadir cambio de estado y eliminación con confirmación.
  - Mostrar estados de carga, vacío y error.
- **Entregables:** flujo funcional de gestión de tareas.
- **Criterios de aceptación:** un usuario puede completar las operaciones CRUD desde el navegador.

### TASK-017 — Manejar errores y expiración de sesión en frontend
- **Prioridad:** P1
- **Dependencias:** TASK-016, TASK-020
- **Actividades:**
  - Centralizar el tratamiento de `401`, `403`, `409`, `429` y `5xx`.
  - Evitar reintentos ilimitados.
  - Mostrar mensajes que no revelen detalles internos.
  - Gestionar la sesión expirada.
- **Entregables:** manejo común de errores y autenticación.
- **Criterios de aceptación:** la interfaz responde de forma consistente a los principales errores HTTP.

---

## Fase 3 — Identidad, autorización y seguridad de aplicación

### TASK-018 — Desplegar Keycloak localmente
- **Prioridad:** P1
- **Dependencias:** TASK-002
- **Actividades:**
  - Incorporar Keycloak al entorno Docker local.
  - Crear realm, cliente y usuarios de prueba.
  - Configurar flujos compatibles con una aplicación web y API.
  - Mantener credenciales fuera del repositorio.
- **Entregables:** configuración local reproducible de identidad.
- **Criterios de aceptación:** un usuario de prueba puede autenticarse y obtener un token válido.

### TASK-019 — Implementar autenticación JWT en la API
- **Prioridad:** P1
- **Dependencias:** TASK-018
- **Actividades:**
  - Configurar validación de firma, emisor, audiencia y expiración.
  - Validar los tokens mediante los mecanismos estándar de ASP.NET Core.
  - Proteger los endpoints que requieran autenticación.
  - Definir el comportamiento de endpoints públicos, si existen.
- **Entregables:** autenticación en API.
- **Criterios de aceptación:** tokens inválidos, expirados o emitidos por otro emisor no permiten acceso.

### TASK-020 — Implementar autorización por roles y propiedad del recurso
- **Prioridad:** P1
- **Dependencias:** TASK-019
- **Actividades:**
  - Definir roles iniciales, por ejemplo `Administrator` y `User`.
  - Aplicar políticas de autorización.
  - Verificar que un usuario solo pueda operar sobre tareas autorizadas.
  - Evitar confiar en el `assigneeId` enviado por el cliente sin validación.
- **Entregables:** políticas y pruebas de autorización.
- **Criterios de aceptación:** un usuario no puede acceder ni modificar tareas fuera de sus permisos; el administrador dispone de los permisos documentados.

### TASK-021 — Aplicar validación de entrada y límites de recursos
- **Prioridad:** P1
- **Dependencias:** TASK-013
- **Actividades:**
  - Validar longitudes, formatos, fechas e identificadores.
  - Establecer límites de tamaño de cuerpo y parámetros.
  - Rechazar contenido y formatos no admitidos.
  - Evitar que los mensajes de error reflejen contenido sin codificar.
  - Usar consultas parametrizadas a través de EF Core.
- **Entregables:** reglas de validación y límites documentados.
- **Criterios de aceptación:** las entradas inválidas no llegan a la persistencia y no provocan excepciones no controladas.

### TASK-022 — Proteger el frontend contra XSS
- **Prioridad:** P1
- **Dependencias:** TASK-015, TASK-016
- **Actividades:**
  - Utilizar renderizado de texto normal de React para contenido proporcionado por usuarios.
  - Evitar `dangerouslySetInnerHTML`, `eval` y ejecución dinámica de contenido.
  - Si se requiere HTML enriquecido, aplicar DOMPurify con una allowlist restrictiva.
  - Añadir una Content Security Policy ajustada a los recursos reales de la aplicación.
  - Configurar cabeceras como `X-Content-Type-Options` y `Referrer-Policy`; habilitar HSTS cuando se use HTTPS.
- **Entregables:** políticas CSP y guía de codificación segura.
- **Criterios de aceptación:** el contenido de títulos y descripciones se muestra como texto y no ejecuta scripts; la CSP no bloquea los recursos legítimos.

### TASK-023 — Añadir protección contra CSRF cuando corresponda
- **Prioridad:** P1
- **Dependencias:** TASK-018, TASK-019, TASK-015
- **Actividades:**
  - Revisar el mecanismo de almacenamiento y envío de credenciales.
  - Si se utilizan cookies para autenticación, implementar protección antiforgery y atributos `SameSite`, `Secure` y `HttpOnly` adecuados.
  - Documentar por qué el flujo utilizado mitiga o no requiere el mismo mecanismo de CSRF.
- **Entregables:** decisión de seguridad y pruebas.
- **Criterios de aceptación:** el modelo de autenticación y protección CSRF está documentado y probado.

### TASK-024 — Configurar CORS y cabeceras de seguridad
- **Prioridad:** P1
- **Dependencias:** TASK-015, TASK-019
- **Actividades:**
  - Permitir únicamente los orígenes locales previstos.
  - Restringir métodos y cabeceras según necesidad.
  - No combinar credenciales con orígenes comodín.
  - Verificar cabeceras de seguridad en las respuestas.
- **Entregables:** política CORS y pruebas.
- **Criterios de aceptación:** orígenes no permitidos no pueden utilizar la API desde navegadores.

### TASK-025 — Implementar protección contra abuso de solicitudes
- **Prioridad:** P1
- **Dependencias:** TASK-021, TASK-029
- **Actividades:**
  - Configurar límites iniciales por IP, identidad y endpoint.
  - Diferenciar operaciones de lectura, escritura y autenticación.
  - Devolver `429 Too Many Requests` y, cuando aplique, `Retry-After`.
  - Añadir límites de concurrencia y timeouts.
  - Establecer límites máximos de paginación y tamaño de solicitudes.
- **Entregables:** políticas de limitación y pruebas.
- **Criterios de aceptación:** el abuso controlado queda limitado sin aplicar restricciones indiscriminadas a todas las operaciones legítimas.

### TASK-026 — Añadir escaneo de vulnerabilidades
- **Prioridad:** P1
- **Dependencias:** TASK-003, TASK-015
- **Actividades:**
  - Configurar Trivy para analizar imágenes y dependencias.
  - Configurar análisis de código disponible en el pipeline.
  - Definir severidades que bloquean una compilación.
  - Documentar excepciones justificadas y temporales.
- **Entregables:** configuración de escaneo y reporte.
- **Criterios de aceptación:** los análisis se ejecutan de forma reproducible y las vulnerabilidades críticas no se ignoran silenciosamente.

---

## Fase 4 — Auditoría, logs, métricas y trazabilidad

### TASK-027 — Implementar auditoría de operaciones
- **Prioridad:** P1
- **Dependencias:** TASK-007 a TASK-012, TASK-020
- **Actividades:**
  - Crear registros de auditoría para creación, modificación, cambio de estado y eliminación.
  - Registrar actor, acción, recurso, fecha UTC y correlación.
  - Registrar cambios relevantes antes/después sin almacenar secretos ni datos innecesarios.
  - Diseñar los registros como históricos append-only desde la aplicación.
  - Separar auditoría funcional de logs técnicos.
- **Entregables:** entidad/tabla de auditoría y documentación.
- **Criterios de aceptación:** cada mutación autorizada deja un registro atribuible; el usuario común no puede alterar el historial mediante la API.

### TASK-028 — Configurar logs estructurados
- **Prioridad:** P1
- **Dependencias:** TASK-013
- **Actividades:**
  - Integrar Serilog.
  - Añadir niveles y propiedades estructuradas.
  - Incorporar identificadores de solicitud y usuario cuando sea seguro.
  - Evitar contraseñas, tokens, secretos y datos sensibles en logs.
  - Enviar logs al colector local previsto.
- **Entregables:** configuración de logging y convenciones.
- **Criterios de aceptación:** los logs permiten investigar errores sin exponer credenciales ni información sensible.

### TASK-029 — Instrumentar OpenTelemetry
- **Prioridad:** P1
- **Dependencias:** TASK-003, TASK-006
- **Actividades:**
  - Instrumentar ASP.NET Core, HttpClient y acceso a base de datos.
  - Propagar contexto de trazas entre componentes.
  - Exportar telemetría al OpenTelemetry Collector local.
  - Definir nombres y atributos de spans evitando datos sensibles.
- **Entregables:** instrumentación y configuración del collector.
- **Criterios de aceptación:** una operación HTTP puede seguirse desde la entrada hasta la persistencia mediante una traza correlacionada.

### TASK-030 — Desplegar Prometheus, Grafana y Loki
- **Prioridad:** P1
- **Dependencias:** TASK-028, TASK-029
- **Actividades:**
  - Desplegar Prometheus para recopilar métricas.
  - Desplegar Grafana con fuentes de datos configuradas.
  - Desplegar Loki para logs.
  - Crear dashboards de API, infraestructura, base de datos y seguridad.
- **Entregables:** stack local de observabilidad y dashboards versionados.
- **Criterios de aceptación:** las métricas, logs y trazas se visualizan y correlacionan durante una ejecución de prueba.

### TASK-031 — Configurar health checks
- **Prioridad:** P1
- **Dependencias:** TASK-006, TASK-029
- **Actividades:**
  - Implementar endpoints de liveness y readiness.
  - Comprobar dependencias necesarias sin exponer detalles internos.
  - Evitar que una dependencia opcional convierta innecesariamente la instancia en no saludable.
  - Configurar probes Kubernetes con umbrales apropiados.
- **Entregables:** health checks y configuración de probes.
- **Criterios de aceptación:** Kubernetes distingue una instancia viva de una instancia preparada para recibir tráfico.

### TASK-032 — Crear dashboard de resiliencia
- **Prioridad:** P1
- **Dependencias:** TASK-029, TASK-030, TASK-031
- **Actividades:**
  - Mostrar réplicas disponibles y no disponibles.
  - Mostrar nodos saludables, reinicios y escalado.
  - Mostrar solicitudes por segundo, latencias P50/P95/P99 y errores.
  - Mostrar respuestas `429`, solicitudes bloqueadas por WAF y códigos `5xx`.
  - Mostrar CPU, memoria, conexiones y latencia de PostgreSQL.
  - Mostrar eventos y duración de experimentos de resiliencia.
- **Entregables:** dashboard `System Resilience`.
- **Criterios de aceptación:** durante una prueba pueden observarse simultáneamente el tráfico, el estado de los servicios y los efectos de un fallo.

---

## Fase 5 — WAF y protección local de la capa HTTP

### TASK-033 — Desplegar Traefik como punto de entrada
- **Prioridad:** P1
- **Dependencias:** TASK-003
- **Actividades:**
  - Configurar Traefik como Ingress Controller local.
  - Definir rutas para frontend, API y servicios internos que deban exponerse.
  - Configurar health checks y timeouts.
  - Configurar TLS local para el laboratorio cuando corresponda.
  - Evitar exponer directamente los pods de API al host.
- **Entregables:** manifiestos de Traefik y rutas.
- **Criterios de aceptación:** el tráfico externo del laboratorio entra por el punto de entrada definido y se distribuye entre réplicas saludables.

### TASK-034 — Desplegar Coraza WAF con OWASP CRS
- **Prioridad:** P1
- **Dependencias:** TASK-033
- **Actividades:**
  - Incorporar Coraza delante de la API o como middleware/proxy compatible con la arquitectura elegida.
  - Configurar OWASP Core Rule Set.
  - Empezar en modo detección para identificar falsos positivos.
  - Ajustar excepciones específicas y justificadas.
  - Pasar gradualmente a modo bloqueo en los escenarios previstos.
  - Centralizar logs de reglas activadas.
- **Entregables:** configuración WAF, reglas y guía operativa.
- **Criterios de aceptación:** las solicitudes de prueba maliciosas definidas se detectan o bloquean; las operaciones legítimas del conjunto de pruebas siguen funcionando.

### TASK-035 — Verificar que no se pueda eludir el WAF
- **Prioridad:** P1
- **Dependencias:** TASK-033, TASK-034
- **Actividades:**
  - Mantener los servicios API en redes internas del clúster.
  - No publicar puertos de pods directamente al host.
  - Comprobar que solo Traefik/WAF puede alcanzar los servicios expuestos.
  - Documentar puertos y flujos permitidos.
- **Entregables:** reglas de red y diagrama de exposición.
- **Criterios de aceptación:** las rutas de acceso documentadas atraviesan el WAF; no existe una ruta pública alternativa directa hacia la API.

---

## Fase 6 — Contenedores y clúster Kubernetes local

### TASK-036 — Dockerizar API y frontend
- **Prioridad:** P0
- **Dependencias:** TASK-007 a TASK-016
- **Actividades:**
  - Crear Dockerfiles multi-stage.
  - Ejecutar los procesos con usuarios no privilegiados.
  - Reducir el tamaño de las imágenes.
  - Excluir secretos y archivos innecesarios del contexto de build.
  - Configurar health checks cuando proceda.
- **Entregables:** imágenes Docker de API y frontend.
- **Criterios de aceptación:** ambas imágenes se construyen y ejecutan localmente; no contienen credenciales ni dependen del entorno del desarrollador.

### TASK-037 — Crear entorno Docker Compose de desarrollo
- **Prioridad:** P0
- **Dependencias:** TASK-036, TASK-006, TASK-018
- **Actividades:**
  - Configurar API, frontend, PostgreSQL, Redis y Keycloak.
  - Añadir volúmenes persistentes para los servicios que lo requieran.
  - Usar health checks y dependencias de arranque razonables.
  - Incorporar perfiles opcionales para observabilidad y pruebas.
- **Entregables:** `docker-compose.yml` y archivos de entorno de ejemplo.
- **Criterios de aceptación:** un comando documentado levanta el entorno local funcional y otro lo detiene sin eliminar datos persistentes por accidente.

### TASK-038 — Crear clúster K3d de múltiples nodos
- **Prioridad:** P1
- **Dependencias:** TASK-002, TASK-036
- **Actividades:**
  - Crear configuración reproducible de K3d con un nodo server y varios agentes.
  - Reservar recursos suficientes para API, frontend y herramientas auxiliares.
  - Configurar acceso local al clúster.
  - Documentar límites de CPU/RAM del equipo anfitrión.
- **Entregables:** configuración K3d y scripts de creación/eliminación.
- **Criterios de aceptación:** el clúster se crea desde cero y muestra varios nodos `Ready`.

### TASK-039 — Desplegar API y frontend en Kubernetes
- **Prioridad:** P1
- **Dependencias:** TASK-038, TASK-033, TASK-036
- **Actividades:**
  - Crear Deployments y Services.
  - Ejecutar varias réplicas de la API y del frontend.
  - Configurar requests y limits de CPU/memoria.
  - Configurar ConfigMaps y Secrets para configuración sensible.
  - Aplicar probes de salud.
- **Entregables:** manifiestos Kubernetes base.
- **Criterios de aceptación:** frontend y API están accesibles a través del Ingress; las réplicas funcionan sin estado local persistente.

### TASK-040 — Distribuir réplicas entre nodos
- **Prioridad:** P1
- **Dependencias:** TASK-039
- **Actividades:**
  - Configurar `topologySpreadConstraints` o anti-affinity.
  - Evitar que todas las réplicas críticas queden concentradas en un único nodo cuando la capacidad lo permita.
  - Configurar Pod Disruption Budgets.
  - Verificar la distribución real con `kubectl get pods -o wide`.
- **Entregables:** políticas de distribución.
- **Criterios de aceptación:** las réplicas se distribuyen entre los nodos disponibles según las restricciones configuradas. Documentar las limitaciones cuando el número de nodos o recursos no permita una distribución completa.

### TASK-041 — Configurar escalabilidad horizontal
- **Prioridad:** P1
- **Dependencias:** TASK-039, TASK-030
- **Actividades:**
  - Configurar Horizontal Pod Autoscaler.
  - Definir métricas y umbrales iniciales.
  - Establecer mínimo y máximo de réplicas.
  - Probar aumento y reducción de réplicas bajo carga.
  - Evaluar si se necesita escalado por métricas adicionales; no asumir que CPU por sí sola representa toda la saturación.
- **Entregables:** configuración HPA y reporte de escalado.
- **Criterios de aceptación:** el número de réplicas cambia conforme a la política y vuelve al mínimo cuando la carga baja, sin perder la disponibilidad dentro de los límites del laboratorio.

### TASK-042 — Gestionar migraciones de base de datos en despliegues
- **Prioridad:** P1
- **Dependencias:** TASK-006, TASK-039
- **Actividades:**
  - Ejecutar migraciones mediante un Job o paso controlado separado del arranque de cada réplica.
  - Evitar que todas las instancias intenten migrar al iniciar.
  - Documentar estrategia de compatibilidad entre versiones de aplicación y esquema.
  - Probar despliegue y rollback compatible.
- **Entregables:** Job o procedimiento de migración.
- **Criterios de aceptación:** el despliegue aplica migraciones una sola vez de forma controlada y las réplicas pueden iniciar correctamente.

### TASK-043 — Configurar persistencia y recuperación local de PostgreSQL
- **Prioridad:** P1
- **Dependencias:** TASK-039
- **Actividades:**
  - Configurar almacenamiento persistente local para PostgreSQL.
  - Automatizar backups locales.
  - Documentar y probar restauración.
  - Definir claramente que el almacenamiento del laboratorio no equivale a alta disponibilidad física.
- **Entregables:** configuración de volumen, backup y restore.
- **Criterios de aceptación:** los datos sobreviven al reinicio de contenedores y se puede restaurar un backup siguiendo la guía.

### TASK-044 — Hacer stateless las instancias de aplicación
- **Prioridad:** P1
- **Dependencias:** TASK-019, TASK-029, TASK-039
- **Actividades:**
  - No guardar sesiones ni archivos necesarios para la operación exclusivamente en memoria o disco local de una réplica.
  - Compartir el estado requerido mediante servicios adecuados.
  - Verificar propagación de trazas entre réplicas.
  - Gestionar correctamente cierre y terminación de procesos.
- **Entregables:** guía de estado de aplicación y pruebas.
- **Criterios de aceptación:** una solicitud posterior puede ser atendida por otra réplica sin perder el contexto necesario para operar.

---

## Fase 7 — Suite de pruebas funcionales y de seguridad

### TASK-045 — Crear pruebas unitarias con xUnit
- **Prioridad:** P0
- **Dependencias:** TASK-005, TASK-007 a TASK-012
- **Actividades:**
  - Probar reglas de dominio.
  - Probar validadores.
  - Probar casos de uso principales.
  - Cubrir creación, cambios de estado, fechas y permisos de negocio.
- **Entregables:** proyecto de pruebas unitarias.
- **Criterios de aceptación:** existen al menos tres pruebas automatizadas significativas; se recomienda una cobertura mayor que el mínimo exigido y se documentan las áreas cubiertas.

### TASK-046 — Crear pruebas de integración con PostgreSQL real
- **Prioridad:** P1
- **Dependencias:** TASK-006, TASK-013
- **Actividades:**
  - Utilizar Testcontainers para levantar PostgreSQL en pruebas.
  - Probar endpoints HTTP mediante `WebApplicationFactory`.
  - Verificar persistencia, migraciones y filtros.
  - Verificar códigos HTTP y contratos de error.
- **Entregables:** suite de integración.
- **Criterios de aceptación:** las pruebas se ejecutan automáticamente y no dependen de una base de datos compartida manualmente.

### TASK-047 — Crear pruebas de arquitectura
- **Prioridad:** P2
- **Dependencias:** TASK-003
- **Actividades:**
  - Verificar que Domain no depende de Infrastructure ni API.
  - Verificar reglas de dependencias entre capas.
  - Detectar referencias circulares no permitidas.
- **Entregables:** pruebas de arquitectura.
- **Criterios de aceptación:** el pipeline falla cuando se incumplen las reglas arquitectónicas.

### TASK-048 — Crear pruebas XSS de API y frontend
- **Prioridad:** P1
- **Dependencias:** TASK-022, TASK-024, TASK-039
- **Actividades:**
  - Preparar casos de XSS almacenado: guardar payloads de prueba en título y descripción.
  - Preparar casos de XSS reflejado: valores especiales en parámetros y respuestas de error.
  - Preparar casos de XSS basado en DOM en búsquedas y componentes dinámicos.
  - Usar Playwright para escuchar eventos de ejecución y verificar que no se ejecuta código de prueba.
  - Confirmar que los datos se presentan como texto o se sanitizan bajo la política definida.
  - Verificar Content-Type y CSP.
- **Entregables:** casos Playwright y reporte XSS.
- **Criterios de aceptación:** los payloads de prueba no ejecutan código en el navegador; el contenido legítimo sigue mostrándose correctamente.

### TASK-049 — Ejecutar análisis DAST con OWASP ZAP
- **Prioridad:** P1
- **Dependencias:** TASK-039, TASK-048
- **Actividades:**
  - Ejecutar baseline scan contra el entorno local.
  - Ejecutar pruebas activas únicamente contra el laboratorio aislado y datos desechables.
  - Clasificar falsos positivos y vulnerabilidades reales.
  - Guardar reportes como artefactos de la ejecución.
- **Entregables:** configuración ZAP y reportes.
- **Criterios de aceptación:** las vulnerabilidades relevantes se corrigen o quedan registradas con justificación y plan de resolución.

### TASK-050 — Probar autenticación y autorización
- **Prioridad:** P1
- **Dependencias:** TASK-019, TASK-020
- **Actividades:**
  - Probar ausencia de token, token inválido y token expirado.
  - Probar permisos por rol.
  - Probar acceso a tareas de otros usuarios.
  - Probar asignación de responsable no autorizada.
- **Entregables:** suite de pruebas de acceso.
- **Criterios de aceptación:** no se puede saltar la autorización manipulando IDs o parámetros del cliente.

---

## Fase 8 — Rendimiento y pruebas de carga distribuidas

### TASK-051 — Definir perfiles de carga y SLO del laboratorio
- **Prioridad:** P1
- **Dependencias:** TASK-001, TASK-039
- **Actividades:**
  - Definir usuarios virtuales iniciales y duración de cada escenario.
  - Definir mezcla de operaciones de lectura y escritura.
  - Definir objetivos iniciales de latencia P50/P95/P99, tasa de errores y throughput.
  - Registrar hardware y límites del equipo anfitrión.
  - Separar resultados de carga funcional, tráfico bloqueado y errores reales.
- **Entregables:** `docs/performance/test-plan.md`.
- **Criterios de aceptación:** cada escenario tiene carga, duración, umbrales y criterios de aprobación definidos antes de ejecutarse.

### TASK-052 — Crear escenario k6 Smoke Test
- **Prioridad:** P1
- **Dependencias:** TASK-051
- **Actividades:**
  - Verificar disponibilidad de endpoints esenciales.
  - Ejecutar un volumen mínimo de usuarios.
  - Validar respuestas y tiempos básicos.
- **Entregables:** `smoke.js`.
- **Criterios de aceptación:** detecta errores funcionales o de configuración rápidamente y puede ejecutarse antes de pruebas más costosas.

### TASK-053 — Crear escenario k6 Load Test
- **Prioridad:** P1
- **Dependencias:** TASK-052
- **Actividades:**
  - Simular usuarios autenticados.
  - Ejecutar listado, filtros, detalle, creación, actualización y eliminación.
  - Usar identificadores y datos únicos para evitar colisiones accidentales.
  - Validar el resultado de las operaciones, no solo el código HTTP.
- **Entregables:** `load.js` y reporte.
- **Criterios de aceptación:** el escenario completa el perfil de carga acordado y registra métricas y errores por operación.

### TASK-054 — Crear escenarios Stress, Spike y Soak
- **Prioridad:** P1
- **Dependencias:** TASK-053
- **Actividades:**
  - Stress: aumentar carga progresivamente para encontrar límites.
  - Spike: elevar tráfico rápidamente y observar respuesta/recuperación.
  - Soak: mantener carga moderada durante un período prolongado.
  - Vigilar memoria, conexiones, latencia, errores y acumulación de recursos.
- **Entregables:** `stress.js`, `spike.js`, `soak.js` y reportes.
- **Criterios de aceptación:** se identifica el punto de saturación observado y se documenta la recuperación tras reducir la carga.

### TASK-055 — Distribuir generadores k6 en el clúster local
- **Prioridad:** P1
- **Dependencias:** TASK-038, TASK-053
- **Actividades:**
  - Configurar k6 Operator o una estrategia equivalente para ejecutar varios generadores.
  - Distribuir workers entre nodos disponibles.
  - Recopilar resultados en una salida común.
  - Medir CPU y memoria de los generadores para detectar cuándo ellos mismos limitan la prueba.
  - Mantener los generadores separados de las réplicas de la aplicación cuando los recursos lo permitan.
- **Entregables:** manifiestos/scripts y guía de ejecución distribuida.
- **Criterios de aceptación:** una misma prueba puede ejecutarse desde múltiples workers y produce un reporte consolidado; se documenta que los nodos comparten el hardware anfitrión.

### TASK-056 — Crear pruebas de frontend con Playwright
- **Prioridad:** P1
- **Dependencias:** TASK-016, TASK-039
- **Actividades:**
  - Automatizar login, consulta, filtro, creación, modificación y eliminación.
  - Ejecutar pruebas en paralelo de forma controlada.
  - Capturar errores de consola y fallos de red.
  - Comprobar que el usuario recibe resultados coherentes durante carga moderada.
- **Entregables:** suite E2E Playwright.
- **Criterios de aceptación:** los flujos críticos pueden ejecutarse repetidamente en el navegador contra el entorno local.

### TASK-057 — Crear escenario de tráfico mixto legítimo y abusivo
- **Prioridad:** P1
- **Dependencias:** TASK-025, TASK-034, TASK-053, TASK-056
- **Actividades:**
  - Ejecutar usuarios legítimos que realizan operaciones normales.
  - Ejecutar simultáneamente generadores k6 con patrones HTTP abusivos controlados y acotados.
  - Medir por separado éxito, latencia y errores de usuarios legítimos.
  - Medir solicitudes detectadas, bloqueadas o limitadas por WAF y rate limiting.
  - Verificar salud de API, base de datos y Redis.
  - No utilizar botnets, IP spoofing, amplificación, paquetes de inundación ni destinos fuera del laboratorio.
- **Entregables:** `mixed-traffic.js`, configuración del escenario y reporte.
- **Criterios de aceptación:** el tráfico abusivo de laboratorio se limita según las políticas y las operaciones legítimas cumplen los SLO establecidos para esa carga. No se considera aprobado si el sistema simplemente rechaza a todos los usuarios.

### TASK-058 — Crear pruebas de límites y recuperación
- **Prioridad:** P1
- **Dependencias:** TASK-025, TASK-057
- **Actividades:**
  - Verificar respuestas `429` y cabecera `Retry-After` cuando corresponda.
  - Comprobar que el límite se aplica coherentemente entre las réplicas cuando se configure como global.
  - Verificar que un usuario limitado no provoca el bloqueo indiscriminado de otros usuarios.
  - Observar el comportamiento después de finalizar la ráfaga.
- **Entregables:** pruebas de rate limiting y análisis.
- **Criterios de aceptación:** los límites funcionan de forma consistente y el servicio vuelve a su comportamiento normal después del escenario.

### TASK-059 — Documentar límites de las pruebas DDoS locales
- **Prioridad:** P1
- **Dependencias:** TASK-057
- **Actividades:**
  - Explicar que k6 valida carga y patrones HTTP controlados, no resistencia ilimitada a ataques volumétricos de red.
  - Documentar límites de red, CPU y memoria del equipo anfitrión.
  - Diferenciar mitigación en WAF/aplicación de saturación de enlace de red.
  - Definir límites máximos seguros para las pruebas.
- **Entregables:** `docs/security/local-ddos-lab.md`.
- **Criterios de aceptación:** los reportes no afirman protección absoluta contra DDoS; indican con precisión el alcance probado y las limitaciones del laboratorio.

---

## Fase 9 — Chaos Engineering y resiliencia local

### TASK-060 — Desplegar Chaos Mesh
- **Prioridad:** P1
- **Dependencias:** TASK-038, TASK-039
- **Actividades:**
  - Instalar Chaos Mesh en el clúster local.
  - Restringir permisos y namespaces de experimentación.
  - Crear un namespace/laboratorio aislado.
  - Documentar cómo detener experimentos y recuperar el entorno.
- **Entregables:** configuración de Chaos Mesh y guía segura.
- **Criterios de aceptación:** los experimentos solo afectan recursos del laboratorio definidos.

### TASK-061 — Simular caída de una instancia API
- **Prioridad:** P1
- **Dependencias:** TASK-040, TASK-060
- **Actividades:**
  - Ejecutar carga normal.
  - Terminar una réplica API.
  - Observar redistribución del tráfico y recreación del pod.
  - Registrar errores, latencia y tiempo de recuperación.
- **Entregables:** experimento y reporte `API Instance Failure`.
- **Criterios de aceptación:** las réplicas restantes atienden las solicitudes dentro de los SLO acordados o se documenta el período de degradación observado.

### TASK-062 — Simular pérdida de un nodo Kubernetes
- **Prioridad:** P1
- **Dependencias:** TASK-040, TASK-060
- **Actividades:**
  - Ejecutar tráfico normal y observar distribución.
  - Simular la indisponibilidad de un nodo del clúster.
  - Verificar reprogramación de pods cuando existan nodos y recursos suficientes.
  - Medir el tiempo de recuperación.
- **Entregables:** experimento y reporte `Node Failure`.
- **Criterios de aceptación:** se documenta qué réplicas se recuperan, cuáles quedan pendientes por falta de capacidad y cuánto tarda el servicio en estabilizarse.

### TASK-063 — Simular latencia y pérdida de red entre componentes
- **Prioridad:** P1
- **Dependencias:** TASK-060
- **Actividades:**
  - Introducir latencia controlada entre API y PostgreSQL o Redis.
  - Simular pérdida de paquetes dentro de los límites del laboratorio.
  - Verificar timeouts, cancelación y reintentos acotados.
  - Comprobar que no se duplican operaciones por reintentos.
- **Entregables:** experimentos de red y reporte.
- **Criterios de aceptación:** los errores se manejan de manera controlada; no hay reintentos infinitos ni duplicación silenciosa de escrituras.

### TASK-064 — Simular indisponibilidad temporal de PostgreSQL
- **Prioridad:** P1
- **Dependencias:** TASK-043, TASK-060
- **Actividades:**
  - Interrumpir temporalmente la conectividad o detener la instancia de PostgreSQL del laboratorio.
  - Verificar respuestas controladas de la API.
  - Restaurar PostgreSQL y comprobar recuperación de conexiones.
  - Verificar integridad de operaciones confirmadas.
- **Entregables:** experimento y reporte `Database Dependency Failure`.
- **Criterios de aceptación:** la API no devuelve falsos éxitos cuando la persistencia no está disponible; tras restaurar la base de datos, vuelve a atender solicitudes.

### TASK-065 — Ejecutar carga mixta durante fallos de infraestructura
- **Prioridad:** P1
- **Dependencias:** TASK-057, TASK-061, TASK-062
- **Actividades:**
  - Mantener usuarios legítimos ejecutando operaciones.
  - Ejecutar tráfico abusivo HTTP controlado.
  - Eliminar una réplica API durante el escenario.
  - En una ejecución separada, simular pérdida de nodo.
  - Registrar resultados de usuarios legítimos, tráfico bloqueado, salud y recuperación.
- **Entregables:** reporte `Mixed Traffic + Failure`.
- **Criterios de aceptación:** se demuestra con métricas qué operaciones continuaron, cuáles se degradaron y cuánto tardó la recuperación. No se ocultan fallos parciales.

### TASK-066 — Verificar integridad y recuperación de datos
- **Prioridad:** P1
- **Dependencias:** TASK-014, TASK-027, TASK-043, TASK-064
- **Actividades:**
  - Comparar operaciones confirmadas con datos persistidos.
  - Detectar escrituras duplicadas o pérdidas.
  - Verificar auditoría después de recuperar el servicio.
  - Probar restauración desde backup en un entorno desechable.
- **Entregables:** scripts de verificación y reporte de integridad.
- **Criterios de aceptación:** todas las operaciones confirmadas según el contrato del sistema están presentes; las operaciones no confirmadas se identifican claramente.

---

## Fase 10 — Automatización, documentación y entrega

### TASK-067 — Crear comandos de automatización local
- **Prioridad:** P1
- **Dependencias:** TASK-037, TASK-038, TASK-039
- **Actividades:**
  - Crear scripts para levantar y detener Docker Compose.
  - Crear scripts para crear y eliminar el clúster K3d.
  - Crear scripts para desplegar y retirar manifiestos.
  - Crear comandos para ejecutar pruebas unitarias, integración, seguridad, carga y resiliencia.
  - Considerar compatibilidad con PowerShell y documentar alternativas.
- **Entregables:** scripts o `Makefile` y guía de comandos.
- **Criterios de aceptación:** las operaciones habituales no requieren recordar secuencias manuales largas.

### TASK-068 — Configurar pipeline CI local/remoto de repositorio
- **Prioridad:** P1
- **Dependencias:** TASK-045, TASK-046, TASK-026
- **Actividades:**
  - Compilar backend y frontend.
  - Ejecutar unit tests, integration tests y architecture tests.
  - Ejecutar escaneos de dependencias e imágenes.
  - Construir imágenes Docker.
  - Publicar reportes como artefactos del pipeline.
  - Mantener las pruebas intensivas de carga y Chaos Engineering como ejecuciones manuales o programadas, no para cada commit.
- **Entregables:** workflows CI.
- **Criterios de aceptación:** un cambio que rompe compilación, pruebas o reglas de seguridad bloquea el pipeline según las políticas definidas.

### TASK-069 — Documentar instalación y ejecución local
- **Prioridad:** P0
- **Dependencias:** TASK-037, TASK-038, TASK-039
- **Actividades:**
  - Documentar requisitos de hardware y software.
  - Documentar configuración de variables de entorno.
  - Explicar ejecución con Docker Compose.
  - Explicar creación del clúster K3d y despliegue Kubernetes.
  - Documentar cómo acceder a frontend, API, Swagger, Keycloak y Grafana.
  - Incluir limpieza segura del entorno.
- **Entregables:** README principal y guía de despliegue.
- **Criterios de aceptación:** una persona nueva puede levantar el entorno siguiendo el README sin instrucciones externas.

### TASK-070 — Documentar API con OpenAPI y Swagger UI/Scalar
- **Prioridad:** P0
- **Dependencias:** TASK-007 a TASK-013
- **Actividades:**
  - Generar documento OpenAPI.
  - Incorporar interfaz Swagger UI o Scalar.
  - Documentar autenticación, parámetros, ejemplos y errores.
  - Documentar filtros y paginación.
  - Separar configuración de desarrollo y entornos no destinados a exposición pública.
- **Entregables:** documentación interactiva de API.
- **Criterios de aceptación:** todos los endpoints públicos están documentados y pueden probarse desde la interfaz en el entorno local.

### TASK-071 — Documentar arquitectura y patrones
- **Prioridad:** P1
- **Dependencias:** TASK-004, TASK-039
- **Actividades:**
  - Crear diagramas de contexto, contenedores y despliegue local.
  - Documentar responsabilidades de cada proyecto.
  - Explicar los patrones utilizados y los problemas que resuelven.
  - Documentar flujos de autenticación, autorización, auditoría y trazabilidad.
- **Entregables:** documentación en `docs/architecture/`.
- **Criterios de aceptación:** un lector puede comprender cómo fluye una petición desde el navegador hasta la base de datos.

### TASK-072 — Consolidar reportes de pruebas y resiliencia
- **Prioridad:** P1
- **Dependencias:** TASK-048, TASK-049, TASK-053 a TASK-066
- **Actividades:**
  - Guardar configuración y fecha de cada ejecución.
  - Registrar hardware, versión de imágenes y configuración del clúster.
  - Consolidar métricas k6, resultados de Playwright, ZAP y experimentos Chaos Mesh.
  - Incluir resultados fallidos y limitaciones, no solo ejecuciones exitosas.
  - Crear una tabla de cumplimiento de objetivos.
- **Entregables:** `docs/performance/` y `docs/resilience/` con reportes reproducibles.
- **Criterios de aceptación:** cada afirmación de rendimiento o resiliencia está respaldada por una ejecución identificable y sus artefactos.

### TASK-073 — Preparar demostración final reproducible
- **Prioridad:** P1
- **Dependencias:** TASK-067 a TASK-072
- **Actividades:**
  - Preparar una demostración de operaciones normales desde React.
  - Mostrar Swagger y los contratos de API.
  - Mostrar varias réplicas distribuidas en nodos K3d.
  - Ejecutar una prueba de carga.
  - Ejecutar el escenario de tráfico mixto controlado.
  - Eliminar una réplica y demostrar recuperación.
  - Mostrar dashboards, logs, trazas y auditoría.
  - Presentar limitaciones del laboratorio de forma explícita.
- **Entregables:** guion de demostración y evidencias.
- **Criterios de aceptación:** otra persona puede repetir la demostración siguiendo el guion y obtener resultados comparables dentro de la variabilidad del entorno local.

---

# 5. Criterios globales de aceptación

El proyecto se considerará terminado cuando se cumplan y documenten los siguientes puntos:

| Área | Criterio de aceptación |
|---|---|
| CRUD | Crear, listar, consultar, actualizar y eliminar tareas |
| Filtros | Filtrar por estado y responsable |
| Paginación | Paginación estable con límites máximos |
| Validación | Datos incorrectos rechazados con errores uniformes |
| Persistencia | PostgreSQL y migraciones EF Core funcionales |
| Pruebas | Al menos tres pruebas automatizadas; suite ampliada de unitarias e integración |
| Documentación | OpenAPI y Swagger UI/Scalar disponibles localmente |
| Frontend | Operaciones habituales utilizables desde React |
| XSS | Casos almacenados, reflejados y DOM no ejecutan payloads de prueba |
| Autorización | No se accede a recursos sin permisos |
| WAF | Coraza/OWASP CRS detecta o bloquea las solicitudes de prueba configuradas |
| Rate limiting | Los límites se aplican coherentemente entre réplicas cuando se configure alcance global |
| Distribución | Varias réplicas frontend/API ejecutándose en distintos nodos K3d cuando los recursos lo permitan |
| Balanceo | El tráfico entra por Traefik y se dirige a instancias saludables |
| Observabilidad | Logs, métricas y trazas disponibles y correlacionables |
| Carga | Smoke, load, stress, spike y soak ejecutables y reportados |
| Tráfico mixto | Operaciones legítimas evaluadas simultáneamente con tráfico HTTP abusivo controlado |
| Fallos | Experimentos de caída de pod, nodo, red y dependencia documentados |
| Integridad | Operaciones confirmadas verificadas después de escenarios de fallo |
| Reproducibilidad | Entorno y pruebas levantables con scripts y documentación |

## Objetivos iniciales de rendimiento

Los siguientes valores son **umbrales de partida para el laboratorio**, no garantías del producto. Deben validarse y ajustarse según los recursos de la máquina anfitriona y el perfil de carga acordado.

| Métrica | Objetivo inicial propuesto |
|---|---:|
| Lecturas P95 | < 500 ms |
| Escrituras P95 | < 800 ms |
| Errores HTTP 5xx bajo carga objetivo | < 0,1 % |
| Disponibilidad durante escenario de carga acordado | ≥ 99,9 % |
| Integridad de operaciones confirmadas | 100 % en las pruebas de aceptación |
| Recuperación ante pérdida de una réplica API | Medida y reportada; objetivo inicial < 30 s |

No debe declararse que el sistema es resistente a cualquier ataque DDoS. El informe debe indicar el máximo tráfico controlado probado, la duración, el hardware, los generadores utilizados, qué capa mitigó cada solicitud y qué ocurrió con los usuarios legítimos.

---

# 6. Orden recomendado de ejecución

Para evitar construir infraestructura compleja antes de tener una aplicación funcional, se recomienda este orden:

1. **Fundación:** TASK-001 a TASK-006.
2. **API funcional:** TASK-007 a TASK-014.
3. **Frontend:** TASK-015 a TASK-017.
4. **Seguridad de aplicación:** TASK-018 a TASK-026.
5. **Auditoría y observabilidad:** TASK-027 a TASK-032.
6. **Entrada protegida:** TASK-033 a TASK-035.
7. **Contenedores y Kubernetes local:** TASK-036 a TASK-044.
8. **Pruebas funcionales y de seguridad:** TASK-045 a TASK-050.
9. **Rendimiento y tráfico mixto:** TASK-051 a TASK-059.
10. **Chaos Engineering:** TASK-060 a TASK-066.
11. **Automatización y entrega:** TASK-067 a TASK-073.

Algunas tareas pueden desarrollarse en paralelo cuando sus dependencias estén satisfechas.

---

# 7. Límites y condiciones del laboratorio

- Todas las pruebas de carga, tráfico abusivo y fallos se ejecutarán exclusivamente contra el laboratorio local y recursos bajo control del equipo.
- Los escenarios de tráfico abusivo serán simulaciones HTTP controladas, con límites de intensidad y duración.
- No se utilizarán botnets, IP spoofing, técnicas de amplificación ni ataques contra redes o servicios de terceros.
- Una prueba local en K3d comparte CPU, memoria, almacenamiento y red con el equipo anfitrión. Los resultados deben incluir esas condiciones.
- La caída de un nodo K3d representa la pérdida de un nodo virtualizado, no la pérdida física independiente de un servidor.
- El laboratorio puede demostrar recuperación de pods, distribución entre nodos virtuales, balanceo, límites de aplicación y comportamiento ante fallos simulados.
- El laboratorio no demuestra por sí solo protección frente a saturación de un enlace de Internet ni tolerancia a fallos físicos o regionales.
- Ningún resultado se considerará aprobado sin conservar la configuración del experimento, métricas, logs y reporte.

---

# 8. Plantilla para registrar cada ejecución de prueba

Usar esta plantilla para documentar los experimentos importantes:

```markdown
## Test: [ID y nombre]

- Fecha:
- Commit / versión:
- Entorno:
- Nodos K3d:
- CPU y memoria del host:
- Réplicas de API:
- Réplicas de frontend:
- Duración:
- Perfil de carga:

### Hipótesis
[Qué se espera que suceda]

### Procedimiento
[Pasos exactos para repetir el experimento]

### Métricas
- Requests/s:
- Usuarios virtuales:
- P50:
- P95:
- P99:
- HTTP 2xx:
- HTTP 4xx:
- HTTP 429:
- HTTP 5xx:
- CPU:
- Memoria:
- Reinicios de pods:
- Tiempo de recuperación:

### Integridad
[Operaciones confirmadas, duplicadas, perdidas o pendientes]

### Resultado
- [ ] Aprobado
- [ ] Fallido
- [ ] Inconcluso

### Evidencias
[Enlaces a dashboards, logs, trazas, capturas y reportes]

### Observaciones y limitaciones
[Problemas detectados y condiciones que afectan la interpretación]
```

---

## 9. Definición de terminado (Definition of Done)

Una tarea individual solo se marcará como `Hecha` cuando:

1. El cambio esté implementado y revisado.
2. Existan pruebas apropiadas para el comportamiento añadido.
3. La documentación relacionada esté actualizada.
4. La configuración necesaria esté versionada, sin secretos.
5. La ejecución local esté verificada.
6. Los errores y limitaciones conocidos estén registrados.

El proyecto completo solo se marcará como terminado cuando se hayan satisfecho los criterios globales de aceptación y exista una demostración reproducible de funcionamiento, seguridad, carga y resiliencia en el laboratorio local.
