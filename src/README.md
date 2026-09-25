# Código Fuente

## Stack Tecnológico
- **Framework**: ASP.NET Core 8.0
- **Patrón**: Domain-Driven Design (DDD) + Clean Architecture
- **DI Container**: Autofac
- **ORM**: Entity Framework Core 8.0
- **Mapping**: AutoMapper
- **API Docs**: Swagger (Swashbuckle)
- **Base de Datos**: PostgreSQL

## Estándar PascalCase (.NET / C#)
En el ecosistema de .NET es la convención oficial utilizar PascalCase tanto para nombres de proyectos como para las carpetas.

```
src/
├── Api/
├── Application/
├── Domain/
└── Infrastructure/
```

> **Nota**: Las carpetas y todas las referencias usan PascalCase (`src/Api`, `src/Domain`, `src/Application`, `src/Infrastructure`), también en Linux case-sensitive (fix de `NETSDK1004`, PR #19).

## Estructura del Proyecto

```
src/
├── Api/                          # Capa de presentación
│   ├── Controllers/             # Endpoints HTTP (API Controllers)
│   ├── DTOs/                    # Request/Response DTOs
│   ├── Filters/                 # Action filters (validación, logging)
│   ├── Middleware/               # Middleware (error handling, auth)
│   ├── Extensions/              # Extension methods
│   ├── Program.cs               # Entry point (HostBuilder + Autofac, delega a Startup)
│   ├── Startup.cs               # Registro servicios (EF Core, AutoMapper, MediatR, Swagger, HealthChecks) y pipeline
│   └── appsettings.json         # Configuración
│
├── Application/                  # Capa de aplicación (CQRS con MediatR)
│   ├── Clients/
│   │   ├── Commands/            # Peticiones de escritura + handlers
│   │   │   ├── CreateClient/    # CreateClientCommand + Handler
│   │   │   ├── UpdateClient/    # UpdateClientCommand + Handler
│   │   │   └── DeleteClient/    # DeleteClientCommand + Handler
│   │   └── Queries/             # Peticiones de solo lectura + handlers
│   │       ├── GetAllClients/   # GetAllClientsQuery + Handler
│   │       └── GetClientById/   # GetClientByIdQuery + Handler
│   ├── Projects/                 # Feature de proyectos (misma estructura)
│   │   ├── Commands/            # Peticiones de escritura + handlers
│   │   │   ├── CreateProject/   # CreateProjectCommand + Handler (valida que el cliente exista)
│   │   │   ├── UpdateProject/   # UpdateProjectCommand + Handler
│   │   │   └── DeleteProject/   # DeleteProjectCommand + Handler
│   │   └── Queries/             # Peticiones de solo lectura + handlers
│   │       ├── GetAllProjects/  # GetAllProjectsQuery + Handler (filtro opcional por clientId)
│   │       └── GetProjectById/  # GetProjectByIdQuery + Handler
│   ├── Common/                  # Excepciones de aplicación
│   │   ├── ClientNotFoundException.cs
│   │   └── ProjectNotFoundException.cs
│   ├── Interfaces/              # Puertos de entrada (IUseCase) - futuro
│   ├── Mappings/                # Perfiles AutoMapper
│   │   ├── ClientMappingProfile.cs
│   │   └── ProjectMappingProfile.cs
│   └── DTOs/                    # DTOs de aplicación
│       ├── ClientDTOs.cs        # ClientResponse, CreateClientRequest, UpdateClientRequest
│       └── ProjectDTOs.cs       # ProjectResponse, CreateProjectRequest, UpdateProjectRequest
│
├── Domain/                       # Dominio puro (SIN dependencias)
│   ├── Entities/                # Entidades con identidad
│   │   ├── Client.cs            # Cliente (Name, Email, Phone, Logo opcional)
│   │   └── Project.cs           # Proyecto (ClientId, Title, Description, Technologies, StartDate, DurationMonths)
│   ├── ValueObjects/            # Value Objects (inmutables) - futuro Email, PhoneNumber
│   ├── Aggregates/              # Agregados (raíz + entidades) - futuro
│   ├── Events/                  # Domain Events - futuro
│   ├── Services/                # Domain Services (lógica de negocio) - futuro
│   ├── Specifications/          # Specifications (reglas reutilizables) - futuro
│   └── Interfaces/              # Interfaces (puertos de salida)
│       ├── IClientRepository.cs # IClientRepository + IUnitOfWork
│       └── IProjectRepository.cs # GetAllAsync(clientId?), GetByIdAsync, Add/Update/Delete
│
└── Infrastructure/               # Capa de infraestructura
    ├── Data/                    # Configuración EF Core
    │   ├── ApplicationDbContext.cs # Implements IUnitOfWork
    │   ├── Configurations/      # IEntityTypeConfiguration
    │   │   ├── ClientConfiguration.cs
    │   │   └── ProjectConfiguration.cs  # FK a clients, índice, CHECK duration
    │   └── Seeders/             # Datos iniciales
    ├── Repositories/            # Implementación de repositorios
    │   ├── ClientRepository.cs
    │   └── ProjectRepository.cs
    └── Migrations/              # Migraciones EF Core
        └── ... (archivos generados)
```

## Convenciones DDD

### Domain Layer (Puro)
```csharp
// ✅ CORRECTO: Domain puro sin dependencias
public class Client
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Logo { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Factory method
    public static Client Create(string name, string email, string? phone = null, string? logo = null) { ... }

    // Domain logic (valida la URL del logo: absoluta http/https, max 2048)
    public void Update(string name, string email, string? phone = null, string? logo = null) { ... }
}

public class Project
{
    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Technologies { get; private set; }
    public DateTime StartDate { get; private set; }
    public int? DurationMonths { get; private set; }  // null = en curso; si existe, > 0
    public DateTime CreatedAt { get; private set; }

    public static Project Create(Guid clientId, string title, string description,
        string technologies, DateTime startDate, int? durationMonths = null) { ... }

    public void Update(...) { ... }
}
```

```csharp
// ❌ INCORRECTO: Domain no debe depender de EF Core
using Microsoft.EntityFrameworkCore;  // NUNCA en Domain
```

### Application Layer
```csharp
// CQRS: un command (escritura) + su handler
public record CreateClientCommand(string Name, string Email, string? Phone) : IRequest<ClientResponse>;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ClientResponse>
{
    public async Task<ClientResponse> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var client = Client.Create(request.Name, request.Email, request.Phone, request.Logo);
        await _repository.AddAsync(client);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ClientResponse>(client);
    }
}
```

El controller no conoce el handler: solo envía el request al mediator.

```csharp
var client = await _mediator.Send(new CreateClientCommand(request.Name, request.Email, request.Phone));
```

### Infrastructure Layer
```csharp
// Repository Pattern
public class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<Client?> GetByIdAsync(Guid id)
    {
        return await _context.Clients.FindAsync(id);
    }
    
    public async Task<Client> AddAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
        return client;
    }
}
```

## Principios SOLID en DDD

| Principio | Aplicación |
|-----------|------------|
| **S** - Single Responsibility | Cada clase tiene una responsabilidad |
| **O** - Open/Closed | Extensible sin modificar |
| **L** - Liskov Substitution | Interfaces intercambiables |
| **I** - Interface Segregation | Interfaces específicas |
| **D** - Dependency Inversion | Depender de abstracciones |

## Base de Datos y Migraciones

- **Motor**: PostgreSQL 16
- **Local**: DB `postgresql-db-ia-tests`, user `timeforsoftware@gmail.com`, password `postgres` (`src/Api/appsettings.json` / `docker-compose.yml:30`)
- **Prod**: DB `postgresql-db-ia-tests`, user `timeforsoftware@gmail.com`, password desde secreto `DB_PASSWORD` (`ci-cd.yml:110` `host.docker.internal`)
- **Migraciones**: EF Core Code First en `src/Infrastructure/Migrations/` (`20260923102117_InitialCreate.cs`)
- **Auto-creación**: `Startup.cs:54` `db.Database.Migrate()` crea tablas en primera ejecución, no requiere `dotnet ef database update` manual

## Comandos Útiles

```bash
# Crear proyecto
dotnet new webapi -n Api
dotnet new classlib -n Domain
dotnet new classlib -n Application
dotnet new classlib -n Infrastructure

# Agregar paquetes NuGet - Core
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design

# Agregar paquetes NuGet - API
dotnet add package Swashbuckle.AspNetCore
dotnet add package Swashbuckle.AspNetCore.Filters

# Agregar paquetes NuGet - DI
dotnet add package Autofac
dotnet add package Autofac.Extensions.DependencyInjection

# Agregar paquetes NuGet - Mapping
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection

# Ejecutar
dotnet run --project src/Api
```
