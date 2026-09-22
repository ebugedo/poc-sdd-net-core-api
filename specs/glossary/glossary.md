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

## Conceptos Técnicos

| Concepto | Descripción en el contexto del proyecto |
|----------|----------------------------------------|
| Entity | Objeto con identidad (Client) - `src/domain/Entities/Client.cs:3` |
| Repository | Abstracción de persistencia `IClientRepository` |
| Unit of Work | Patrón para transacciones `IUnitOfWork` |
| AutoMapper | Librería para mapeo Entity <-> DTO |
| Autofac | Contenedor DI en `Program.cs` / `Startup.cs` |
