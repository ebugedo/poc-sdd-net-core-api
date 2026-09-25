# Arquitectura del Sistema

## Visión General

```
┌─────────────────────────────────────────────────────────┐
│                    ASP.NET Core API                      │
├─────────────────────────────────────────────────────────┤
│                    Presentation Layer                    │
│         Controllers │ DTOs │ Middleware │ Filters        │
├─────────────────────────────────────────────────────────┤
│                    Application Layer                     │
│   Commands │ Queries │ Handlers │ Mediator │ DTOs        │
├─────────────────────────────────────────────────────────┤
│                      Domain Layer                        │
│   Entities │ Value Objects │ Aggregates │ Domain Events  │
│                 Domain Services │ Specifications         │
├─────────────────────────────────────────────────────────┤
│                  Infrastructure Layer                    │
│     DbContext │ Repositories │ Migrations │ External     │
├─────────────────────────────────────────────────────────┤
│              PostgreSQL + Entity Framework Core          │
└─────────────────────────────────────────────────────────┘
```

> Requests HTTP (comandos) y respuestas (queries) se despachan vía **MediatR**: el controller no conoce el caso de uso, solo envía `IRequest` y MediatR resuelve el `IRequestHandler` registrado por Autofac. Ver `/decisions/ADR-003-cqrs-mediatr.md`.

## Stack Tecnológico

| Capa | Tecnología | Propósito |
|------|------------|-----------|
| **API** | ASP.NET Core 8.0 | Framework web, routing, serialización |
| **API Docs** | Swagger / Swashbuckle | Documentación OpenAPI de la API |
| **DDD** | .NET 8.0 | Domain-Driven Design, patrones de dominio |
| **CQRS** | MediatR 12.5.0 | Despacho de Commands/Queries, desacopla controller y handlers |
| **DI Container** | Autofac | Dependency Injection avanzado, módulos |
| **ORM** | Entity Framework Core 8.0 | Mapeo objeto-relacional, migraciones |
| **Mapping** | AutoMapper | Mapeo de objetos (Entity ↔ DTO) |
| **Base de Datos** | PostgreSQL 16 | Base de datos relacional |
| **Pruebas** | xUnit + Bogus + FluentAssertions | Testing con datos fake y assertions fluent |

## Capas (DDD + Clean Architecture)

### API Layer (Presentation)
- **Responsabilidad**: Recibir peticiones HTTP, retornar respuestas
- **Tecnología**: ASP.NET Core 8.0 (patrón clásico Program + Startup)
- **Componentes**: Controllers, DTOs, Middleware, Filters, Program.cs, Startup.cs
  - `Program.cs`: HostBuilder, `UseServiceProviderFactory(AutofacServiceProviderFactory)`, `UseStartup<Startup>()`
  - `Startup.cs`: `ConfigureContainer` (Autofac), `ConfigureServices` (EF Core, AutoMapper, MediatR, Swagger), `Configure` (Swagger, Routing, `Database.Migrate()`)
- **Controllers**: delgados, solo traducen HTTP ↔ `IRequest`; la lógica está en los handlers de Application
- **API Docs**: Swagger (Swashbuckle) para documentación OpenAPI
- **DI Container**: Autofac para dependency injection

### Application Layer (CQRS)
- **Responsabilidad**: Orquestar operaciones, coordinar entre capas
- **Tecnología**: .NET 8.0 + MediatR 12.5.0
- **Componentes**:
  - `<Feature>/Commands/<UseCase>/<UseCase>Command.cs` + `<UseCase>CommandHandler.cs` (escritura: crean/mueven/borran, usan `IUnitOfWork`)
  - `<Feature>/Queries/<UseCase>/<UseCase>Query.cs` + `<UseCase>QueryHandler.cs` (lectura: solo mapean a DTO, nunca guardan)
  - Features activas: `Clients/` (5 casos de uso) y `Projects/` (5 casos de uso)
  - `Common/ClientNotFoundException.cs`, `Common/ProjectNotFoundException.cs` (excepciones de aplicación → 404 en el controller)
  - `DTOs/`, `Mappings/` (AutoMapper)
- **Contrato con la API**: los handlers implementan `IRequestHandler<TRequest, TResponse>`; el controller envía con `IMediator.Send(...)`
- **Registro**: `services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(...))` (`Startup.cs:43`) → handlers transitorios resueltos por Autofac
- **Mapping**: AutoMapper para Entity ↔ DTO (`ClientResponse`, `ProjectResponse`)

### Domain Layer
- **Responsabilidad**: Lógica de negocio, reglas, invariantes
- **Tecnología**: .NET 8.0 (puro, sin dependencias externas)
- **Componentes**: 
  - **Entities**: Objetos con identidad
  - **Value Objects**: Objetos sin identidad
  - **Aggregates**: Conjuntos de entidades con raíz
  - **Domain Events**: Eventos de dominio
  - **Domain Services**: Lógica que no pertenece a una entidad
  - **Specifications**: Reglas de negocio reutilizables

### Infrastructure Layer
- **Responsabilidad**: Acceso a datos, servicios externos
- **Tecnología**: Entity Framework Core 8.0 + Npgsql
- **Componentes**: 
  - **DbContext**: Contexto de EF Core
  - **Repositories**: Implementación de interfaces del dominio
  - **Migrations**: Migraciones de EF Core Code First
  - **Configurations**: Configuración de entidades (IEntityTypeConfiguration)

## Patrones de Diseño (DDD)

| Patrone | Uso |
|---------|-----|
| **CQRS** | Separar Commands (escritura) y Queries (lectura) con MediatR |
| **Mediator** | Desacoplar controller de los handlers de Application |
| **Repository Pattern** | Acceso a datos abstracto |
| **Aggregate Pattern** | Consistencia transaccional |
| **Value Object Pattern** | Inmutabilidad |
| **Domain Event Pattern** | Comunicación entre agregados |
| **Specification Pattern** | Reglas de negocio reutilizables |
| **Unit of Work Pattern** | Transacciones (EF Core lo implementa) |

## Estándar de Nomenclatura

**PascalCase (.NET / C#)**: Convención oficial del ecosistema .NET para proyectos y carpetas:

```
src/
├── Api/
├── Application/
├── Domain/
└── Infrastructure/
```

Desde el PR #19 las carpetas físicas y todas las referencias del repo usan PascalCase (`src/Api`, `src/Domain`, `src/Application`, `src/Infrastructure`), también en Linux case-sensitive.

## Estructura del Proyecto

```
src/
├── Api/                          # ASP.NET Core Web API (Program + Startup)
│   ├── Controllers/             # Endpoints HTTP
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Filters/                 # Action filters
│   ├── Middleware/               # Middleware custom
│   ├── Program.cs               # HostBuilder + Autofac factory
│   └── Startup.cs               # ConfigureServices / ConfigureContainer / Configure
│
├── Application/                  # Capa de aplicación (CQRS)
│   ├── Clients/
│   │   ├── Commands/             # CreateClient, UpdateClient, DeleteClient (+ handlers)
│   │   └── Queries/              # GetAllClients, GetClientById (+ handlers)
│   ├── Projects/
│   │   ├── Commands/             # CreateProject, UpdateProject, DeleteProject (+ handlers)
│   │   └── Queries/              # GetAllProjects, GetProjectById (+ handlers)
│   ├── Common/                   # Excepciones de aplicación
│   ├── Interfaces/              # Puertos de entrada/salida
│   ├── Mappings/                # Perfiles AutoMapper
│   └── DTOs/                    # DTOs de aplicación
│
├── Domain/                       # Dominio puro (sin dependencias)
│   ├── Entities/                # Entidades
│   ├── ValueObjects/            # Value Objects
│   ├── Aggregates/              # Agregados
│   ├── Events/                  # Domain Events
│   ├── Services/                # Domain Services
│   ├── Specifications/          # Specifications
│   └── Interfaces/              # Interfaces (Ports)
│
└── Infrastructure/               # Infraestructura
    ├── Data/                    # DbContext, Configurations
    ├── Repositories/            # Implementación repositorios
    └── Migrations/              # Migraciones EF Core
```

## Despliegue

- **Container**: Docker, `Dockerfile:36` expone 8080
- **Red**: `nginx-net` (externa) para que `poc-sdd-api` sea alcanzable por nginx reverse proxy
- **Proxy**: nginx en `nginx-net` hace `proxy_pass http://poc-sdd-api:8080` (`docs/deployment.md:122`)
- **DB**: PostgreSQL externo en host, API conecta vía `host.docker.internal:5432` con `--add-host`

## Decisiones Técnicas
Ver `/decisions/` para decisiones de arquitectura documentadas.
