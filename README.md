# POC SDD - .NET Core API

## Propósito
Proof of Concept (PoC) para implementar **Spec-Driven Development (SDD)** en una API .NET Core.

## ¿Qué es SDD?
Spec-Driven Development es una metodología donde las **especificaciones** guían todo el proceso de desarrollo:
1. Primero defines **qué** debe hacer el sistema (specs)
2. Luego defines **cómo** se diseña (design)
3. Después defines **los contratos** (contracts)
4. Finalmente **implementas** (src) y **verificas** (tests)

## Estructura del Proyecto

```
├── specs/                  # Especificaciones del sistema
│   ├── domain/            # Modelo de dominio y reglas de negocio
│   ├── api/               # Especificación de la API
│   ├── features/          # Historias de usuario
│   └── glossary/          # Términos del dominio
│
├── contracts/             # Contratos formales
│   ├── api/               # OpenAPI/Swagger
│   ├── events/            # Contratos de eventos
│   └── schemas/           # JSON Schemas
│
├── design/                # Diseño del sistema
│   ├── architecture/      # Arquitectura general
│   ├── flows/             # Flujos y secuencias
│   └── data-model/        # Modelo de datos
│
├── decisions/             # Architecture Decision Records (ADR)
│
├── src/                   # Código fuente
│   ├── Api/               # Controllers, middleware (Program + Startup)
│   ├── Application/        # Lógica de aplicación
│   ├── Domain/            # Entidades, lógica de negocio
│   └── Infrastructure/    # Acceso a datos, servicios externos
│
└── tests/                 # Pruebas
    ├── unit/              # Pruebas unitarias
    ├── integration/       # Pruebas de integración
    └── acceptance/        # Pruebas de aceptación
```

## Flujo de Trabajo

```
Especificar → Diseñar → Definir Contratos → Implementar → Verificar
   (specs)   (design)    (contracts)          (src)        (tests)
```

## Guía Completa
Ver [SDD-GUIDE.md](SDD-GUIDE.md) para una guía detallada de SDD.

## Inicio Rápido

1. Revisar las especificaciones en `specs/`
2. Revisar el diseño en `design/`
3. Revisar los contratos en `contracts/`
4. Implementar en `src/`
5. Verificar con `tests/`

## Stack Tecnológico

| Componente | Tecnología |
|------------|------------|
| Framework | ASP.NET Core 8.0 |
| API Docs | Swagger (Swashbuckle) |
| Patrón | DDD + Clean Architecture |
| DI Container | Autofac |
| ORM | Entity Framework Core 8.0 |
| Mapping | AutoMapper |
| Base de Datos | PostgreSQL 16 |
| Testing | xUnit + Bogus + FluentAssertions |
| Container | Docker |
| CI/CD | GitHub Actions + GHCR |
| Server | VPS Debian (Docker + Docker Compose + nginx + Portainer) |

## Despliegue

### Desarrollo Local
```bash
# Ejecutar con docker-compose
docker-compose up -d

# Ver logs
docker-compose logs -f api
```

### Producción (Automático - Debian VPS)
El despliegue es automático al hacer push a `main`:
1. GitHub Actions ejecuta tests
2. Build de Docker image
3. Push a GitHub Container Registry (ghcr.io)
4. Deploy via SSH al VPS Debian: `docker pull ghcr.io/...` (no se construye en VPS, solo se extrae) en red `nginx-net` tras nginx + Portainer

Ver [docs/deployment.md](docs/deployment.md) para guía completa.

## Comandos Útiles

```bash
# Ejecutar pruebas
dotnet test

# Ejecutar pruebas con cobertura
dotnet test /p:CollectCoverage=true

# Docker build local
docker build -t poc-sdd-api .

# Docker run local
docker run -p 8080:8080 poc-sdd-api
```
