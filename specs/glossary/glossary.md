# Glosario del Dominio

## Términos del Negocio

| Término | Definición | Alias |
|---------|------------|-------|
| Client | Persona o empresa registrada como cliente del sistema. Entidad principal con Id, Name, Email, Phone, Logo (URL opcional), CreatedAt | Cliente |
| Project | Proyecto de un cliente: ClientId, Title, Description, Technologies, StartDate, DurationMonths (meses, opcional), CreatedAt | - |
| Name | Nombre completo del cliente, obligatorio | - |
| Email | Correo electrónico del cliente, obligatorio y único en BD | - |
| Phone | Teléfono del cliente, opcional | - |
| Logo | URL absoluta http/https del logo del cliente, opcional (`clients.logo`) | - |
| CreatedAt | Fecha UTC de creación, inmutable | - |
| ClientId | Cliente propietario del proyecto (FK → `clients.id`) | - |
| Title | Título del proyecto, obligatorio | - |
| Description | Descripción del proyecto, obligatoria | - |
| Technologies | Texto libre con las tecnologías del proyecto, obligatorio | - |
| StartDate | Fecha de inicio del proyecto, obligatoria | - |
| DurationMonths | Duración del proyecto en meses; `null` = en curso | - |

## Abreviaciones

| Abreviación | Significado |
|-------------|-------------|
| API | Application Programming Interface |
| DTO | Data Transfer Object |
| ORM | Object-Relational Mapping |
| CRUD | Create, Read, Update, Delete |
| DDD | Domain-Driven Design |
| CQRS | Command Query Responsibility Segregation |

## Conceptos Técnicos

| Concepto | Descripción en el contexto del proyecto |
|----------|----------------------------------------|
| Entity | Objeto con identidad (Client, Project) - `src/Domain/Entities/` |
| Repository | Abstracción de persistencia `IClientRepository` |
| Unit of Work | Patrón para transacciones `IUnitOfWork` |
| AutoMapper | Librería para mapeo Entity <-> DTO |
| Autofac | Contenedor DI en `Program.cs` / `Startup.cs` |
| Command | Petición de escritura (`IRequest<TResponse>`) que cambia estado: `CreateClientCommand`, `UpdateClientCommand`, `DeleteClientCommand` |
| Query | Petición de solo lectura que devuelve DTO: `GetAllClientsQuery`, `GetClientByIdQuery`, `GetAllProjectsQuery`, `GetProjectByIdQuery` |
| Handler | `IRequestHandler<TRequest, TResponse>` que ejecuta un Command o Query - `src/Application/Clients/`, `src/Application/Projects/` |
| Mediator | MediatR 12.5.0: despacha el request al handler correcto; el controller usa `IMediator.Send(...)` |
