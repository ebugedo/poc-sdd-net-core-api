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
│        Use Cases │ Services │ Interfaces (Ports)         │
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

## Stack Tecnológico

| Capa | Tecnología | Propósito |
|------|------------|-----------|
| **API** | ASP.NET Core 8.0 | Framework web, routing, serialización |
| **DDD** | .NET 8.0 | Domain-Driven Design, patrones de dominio |
| **ORM** | Entity Framework Core 8.0 | Mapeo objeto-relacional, migraciones |
| **Base de Datos** | PostgreSQL 16 | Base de datos relacional |
| **Pruebas** | xUnit + Moq | Testing unitario y de integración |

## Capas (DDD + Clean Architecture)

### API Layer (Presentation)
- **Responsabilidad**: Recibir peticiones HTTP, retornar respuestas
- **Tecnología**: ASP.NET Core 8.0
- **Componentes**: Controllers, DTOs, Middleware, Filters, Program.cs

### Application Layer
- **Responsabilidad**: Orquestar operaciones, coordinar entre capas
- **Tecnología**: .NET 8.0
- **Componentes**: Use Cases, Application Services, Interfaces (Ports), DTOs

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
| **Repository Pattern** | Acceso a datos abstracto |
| **Aggregate Pattern** | Consistencia transaccional |
| **Value Object Pattern** | Inmutabilidad |
| **Domain Event Pattern** | Comunicación entre agregados |
| **Specification Pattern** | Reglas de negocio reutilizables |
| **Unit of Work Pattern** | Transacciones (EF Core lo implementa) |

## Estructura del Proyecto

```
src/
├── Api/                          # ASP.NET Core Web API
│   ├── Controllers/             # Endpoints HTTP
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Filters/                 # Action filters
│   ├── Middleware/               # Middleware custom
│   └── Program.cs               # Configuración
│
├── Application/                  # Capa de aplicación
│   ├── Interfaces/              # Puertos de entrada/salida
│   ├── Services/                # Application services
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

## Decisiones Técnicas
Ver `/decisions/` para decisiones de arquitectura documentadas.
