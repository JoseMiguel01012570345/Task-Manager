# Diagrama de componentes

```mermaid
flowchart TB
  subgraph client [Cliente]
    Browser[Navegador React]
  end

  subgraph edge [Entrada local]
    Traefik[Traefik]
    Waf[Coraza CRS]
  end

  subgraph apiLayer [TaskManagement.API]
    Endpoints[Endpoints HTTP]
    Errors[ProblemDetails]
  end

  subgraph appLayer [TaskManagement.Application]
    UseCases[Casos de uso Tasks]
  end

  subgraph domainLayer [TaskManagement.Domain]
    TaskItem[TaskItem y transiciones]
  end

  subgraph infraLayer [TaskManagement.Infrastructure]
    Ef[EF Core Npgsql]
    AuthAdapter[Usuario actual]
  end

  subgraph data [Datos]
    Postgres[(PostgreSQL)]
    Redis[(Redis)]
  end

  Browser --> Traefik --> Waf --> Endpoints
  Endpoints --> UseCases
  UseCases --> TaskItem
  UseCases --> Ef
  Ef --> Postgres
  Endpoints --> AuthAdapter
  Endpoints --> Errors
  Endpoints --> Redis
```

Una petición de escritura entra por Traefik y el WAF, llega al endpoint, el caso de uso aplica las reglas de `TaskItem` y Infrastructure guarda el resultado en PostgreSQL. Las lecturas siguen el mismo camino y proyectan filas sin duplicar la base.
