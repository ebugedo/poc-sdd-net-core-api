# ADR-001: Stack Tecnológico

## Estado
[x] Aceptado

## Fecha
2024-01-XX

## Contexto
Se necesita definir el stack tecnológico para el proyecto POC que implementa SDD (Spec-Driven Development) con una API RESTful que cumpla con:
- Arquitectura limpia y mantenible
- Domain-Driven Design (DDD)
- Base de datos relacional robusta
- Soporte para migraciones Code First
- Buena documentación y comunidad

## Decisión
Se selecciona el siguiente stack tecnológico:

| Componente | Tecnología | Versión | Justificación |
|------------|------------|---------|---------------|
| **Framework Web** | ASP.NET Core | 8.0 | Rendimiento, cross-platform, ecosistema maduro |
| **API Docs** | Swagger (Swashbuckle) | 6.x | Documentación OpenAPI, UI interactiva |
| **Patrón Arquitectónico** | DDD + Clean Architecture | - | Separación de capas, lógica de negocio pura |
| **DI Container** | Autofac | 8.x | Módulos, registration assembly, lifetime scopes |
| **ORM** | Entity Framework Core | 8.0 | Code First, migraciones, LINQ |
| **Mapping** | AutoMapper | 13.x | Mapeo Entity ↔ DTO, profiles |
| **Base de Datos** | PostgreSQL | 16 | Robusto, JSON support, extensions |
| **Driver PostgreSQL** | Npgsql | - | Driver oficial .NET para PostgreSQL |
| **Testing** | xUnit + Bogus + FluentAssertions | - | Testing moderno con datos fake |
| **Containerización** | Docker | - | Consistencia dev/prod, portabilidad |
| **Registry** | GitHub Container Registry | - | Integrado con repo, gratuito para públicos |
| **CI/CD** | GitHub Actions | - | Automatización,免费 para repos públicos |
| **Server** | VPS (Docker) | - | Control total, costo fijo |

## Consecuencias

### Positivas
- **ASP.NET Core 8.0**: Alto rendimiento, soporte LTS, minimal APIs
- **Swagger**: Documentación automática de la API, UI interactiva
- **DDD**: Lógica de negocio separada y testable
- **Autofac**: DI container robusto, módulos organizados, registration assembly
- **EF Core Code First**: Migraciones automáticas, schema versionado
- **AutoMapper**: Mapeo limpio entre entidades y DTOs
- **PostgreSQL**: ACID, escalabilidad, extensiones (JSON, PostGIS, etc.)
- **Clean Architecture**: Fácil de mantener y testear
- **Bogus**: Datos fake realistas para testing
- **FluentAssertions**: Assertions legibles y expresivas

### Negativas
- **Curva de aprendizaje DDD**: Requiere entender conceptos como Agregados, Value Objects
- **Autofac complejidad**: Más configuración que DI nativo de ASP.NET Core
- **AutoMapper overhead**: Ligero costo de rendimiento (aceptable para la mayoría de casos)
- **EF Core overhead**: Rendimiento menor que ADO.NET puro (aceptable para la mayoría de casos)
- **PostgreSQL**: Configuración inicial más compleja que SQLite

## Alternativas Consideradas

### Alternativa 1: SQL Server + EF Core
**Pros**: 
- Integración nativa con .NET
- Mejor soporte Visual Studio

**Contras**: 
- Licencia requerida para producción
- Menos flexible que PostgreSQL

### Alternativa 2: MongoDB (NoSQL)
**Pros**: 
- Schema flexible
- Escalado horizontal

**Contras**: 
- No relacional (el modelo es relacional)
- Menos consistencia transaccional

### Alternativa 3: Dapper (micro-ORM)
**Pros**: 
- Más rápido que EF Core
- Control total sobre SQL

**Contras**: 
- Más código boilerplate
- Sin migraciones automáticas

## Notas Adicionales
- Se utiliza **Code First** para mantener el modelo como fuente de verdad
- Las migraciones se generan desde el proyecto Infrastructure
- Se recomienda usar **Repository Pattern** con interfaces en Domain
