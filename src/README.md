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

> **Nota**: El estándar es PascalCase. Históricamente en este repo las carpetas se crearon en minúsculas (`src/api`, `src/domain`, `src/infrastructure`) por compatibilidad Linux case-sensitive; el código y la solución ya usan referencias en minúsculas para evitar `NETSDK1004`.

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
│   ├── Startup.cs               # Registro servicios (EF Core, AutoMapper, Swagger, HealthChecks) y pipeline
│   └── appsettings.json         # Configuración
│
├── Application/                  # Capa de aplicación
│   ├── Interfaces/              # Puertos de entrada (IUseCase)
│   │   ├── ICreateResourceUseCase.cs
│   │   ├── IGetResourceUseCase.cs
│   │   └── IDeleteResourceUseCase.cs
│   ├── Services/                # Implementación de use cases
│   │   └── ResourceService.cs
│   └── DTOs/                    # DTOs de aplicación
│       ├── CreateResourceRequest.cs
│       ├── ResourceResponse.cs
│       └── ErrorResponse.cs
│
├── Domain/                       # Dominio puro (SIN dependencias)
│   ├── Entities/                # Entidades con identidad
│   │   └── Client.cs            # Entidad actual (Resource ejemplo removido)
│   ├── ValueObjects/            # Value Objects (inmutables) - futuro Email, PhoneNumber
│   ├── Aggregates/              # Agregados (raíz + entidades) - futuro
│   ├── Events/                  # Domain Events - futuro
│   ├── Services/                # Domain Services (lógica de negocio) - futuro
│   ├── Specifications/          # Specifications (reglas reutilizables) - futuro
│   └── Interfaces/              # Interfaces (puertos de salida)
│       └── IClientRepository.cs # IClientRepository + IUnitOfWork
│
└── Infrastructure/               # Capa de infraestructura
    ├── Data/                    # Configuración EF Core
    │   ├── ApplicationDbContext.cs # Implements IUnitOfWork
    │   ├── Configurations/      # IEntityTypeConfiguration
    │   │   └── ClientConfiguration.cs
    │   └── Seeders/             # Datos iniciales
    ├── Repositories/            # Implementación de repositorios
    │   └── ClientRepository.cs
    └── Migrations/              # Migraciones EF Core
        └── ... (archivos generados)
```

## Convenciones DDD

### Domain Layer (Puro)
```csharp
// ✅ CORRECTO: Domain puro sin dependencias
public class Resource
{
    public Guid Id { get; private set; }
    public ResourceName Name { get; private set; }  // Value Object
    public DateTime CreatedAt { get; private set; }
    
    // Factory method
    public static Resource Create(ResourceName name) { ... }
    
    // Domain logic
    public void Update(ResourceName newName) { ... }
}
```

```csharp
// ❌ INCORRECTO: Domain no debe depender de EF Core
using Microsoft.EntityFrameworkCore;  // NUNCA en Domain
```

### Application Layer
```csharp
// Use Case Pattern
public interface ICreateResourceUseCase
{
    Task<ResourceResponse> ExecuteAsync(CreateResourceRequest request);
}

public class CreateResourceUseCase : ICreateResourceUseCase
{
    private readonly IResourceRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<ResourceResponse> ExecuteAsync(CreateResourceRequest request)
    {
        var resource = Resource.Create(new ResourceName(request.Name));
        await _repository.AddAsync(resource);
        await _unitOfWork.SaveChangesAsync();
        return MapToResponse(resource);
    }
}
```

### Infrastructure Layer
```csharp
// Repository Pattern
public class ResourceRepository : IResourceRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<Resource> GetByIdAsync(Guid id)
    {
        return await _context.Resources.FindAsync(id);
    }
    
    public async Task AddAsync(Resource resource)
    {
        await _context.Resources.AddAsync(resource);
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
