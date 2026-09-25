# Glosario del Dominio

## Términos del Negocio

| Término | Definición | Alias |
|---------|------------|-------|
| Client | Persona o empresa registrada como cliente del sistema. Entidad principal con Id, Name, Email, Phone, CreatedAt | Cliente |
| Name | Nombre completo del cliente, obligatorio | - |
| Email | Correo electrónico del cliente, obligatorio y único en BD | - |
| Phone | Teléfono del cliente, opcional | - |
| CreatedAt | Fecha UTC de creación del cliente, inmutable | - |

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
| Entity | Objeto con identidad (Client) - `src/Domain/Entities/Client.cs:3` |
| Repository | Abstracción de persistencia `IClientRepository` |
| Unit of Work | Patrón para transacciones `IUnitOfWork` |
| AutoMapper | Librería para mapeo Entity <-> DTO |
| Autofac | Contenedor DI en `Program.cs` / `Startup.cs` |
| Command | Petición de escritura (`IRequest<TResponse>`) que cambia estado: `CreateClientCommand`, `UpdateClientCommand`, `DeleteClientCommand` |
| Query | Petición de solo lectura que devuelve DTO: `GetAllClientsQuery`, `GetClientByIdQuery` |
| Handler | `IRequestHandler<TRequest, TResponse>` que ejecuta un Command o Query - `src/Application/Clients/` |
| Mediator | MediatR 12.5.0: despacha el request al handler correcto; el controller usa `IMediator.Send(...)` |
