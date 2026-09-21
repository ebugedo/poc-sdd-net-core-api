# Código Fuente

## Stack Tecnológico
- **Framework**: ASP.NET Core 8.0
- **Patrón**: Domain-Driven Design (DDD) + Clean Architecture
- **ORM**: Entity Framework Core 8.0
- **Base de Datos**: PostgreSQL

## Estructura del Proyecto

```
src/
├── Api/                          # Capa de presentación
│   ├── Controllers/             # Endpoints HTTP (API Controllers)
│   ├── DTOs/                    # Request/Response DTOs
│   ├── Filters/                 # Action filters (validación, logging)
│   ├── Middleware/               # Middleware (error handling, auth)
│   ├── Extensions/              # Extension methods
│   ├── Program.cs               # Entry point, DI container
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
│   │   └── Resource.cs
│   ├── ValueObjects/            # Value Objects (inmutables)
│   │   ├── ResourceName.cs
│   │   └── AuditInfo.cs
│   ├── Aggregates/              # Agregados (raíz + entidades)
│   │   └── ResourceAggregate.cs
│   ├── Events/                  # Domain Events
│   │   ├── ResourceCreatedEvent.cs
│   │   └── ResourceDeletedEvent.cs
│   ├── Services/                # Domain Services (lógica de negocio)
│   │   └── ResourceDomainService.cs
│   ├── Specifications/          # Specifications (reglas reutilizables)
│   │   ├── ResourceNameSpecification.cs
│   │   └── ValidResourceSpecification.cs
│   └── Interfaces/              # Interfaces (puertos de salida)
│       ├── IRepository.cs
│       └── IUnitOfWork.cs
│
└── Infrastructure/               # Capa de infraestructura
    ├── Data/                    # Configuración EF Core
    │   ├── ApplicationDbContext.cs
    │   ├── Configurations/      # IEntityTypeConfiguration
    │   │   └── ResourceConfiguration.cs
    │   └── Seeders/             # Datos iniciales
    ├── Repositories/            # Implementación de repositorios
    │   └── ResourceRepository.cs
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

# Agregar paquetes NuGet
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design

# Ejecutar
dotnet run --project src/Api
```
