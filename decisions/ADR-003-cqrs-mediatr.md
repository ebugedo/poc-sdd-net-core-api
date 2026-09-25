# ADR-003: CQRS con MediatR

## Estado
[x] Aceptado

## Fecha
2026-09-25

## Contexto
La capa de Application usaba un `ClientService` por entidad, con métodos tipo `GetAllAsync` / `CreateAsync` / `UpdateAsync` / `DeleteAsync` que mezclaban lectura y escritura en la misma clase. Problemas:
- El controller depende de un service concreto (`Application.Services.ClientService`), no de un contrato
- No se distingue el contrato de lectura del de escritura (mismo DTO de entrada/salida)
- Añadir un caso de uso obliga a ampliar el service (clases que crecen sin límite)
- Cada método mezcla la invocación con la lógica; difícil de testear de forma aislada y de componer

## Decisión
Adoptar **CQRS** en la capa Application usando **MediatR 12.5.0** como mediator, con **un único modelo de datos** (tabla `clients`).

- Cada caso de uso es un `record` que implementa `IRequest<TResponse>`:
  - `Clients/Commands/CreateClient/CreateClientCommand` (escritura)
  - `Clients/Commands/UpdateClient/UpdateClientCommand`
  - `Clients/Commands/DeleteClient/DeleteClientCommand` (devuelve `Unit`)
  - `Clients/Queries/GetAllClients/GetAllClientsQuery` (lectura, devuelve `IReadOnlyList<ClientResponse>`)
  - `Clients/Queries/GetClientById/GetClientByIdQuery`
- Cada request tiene su `IRequestHandler<TRequest, TResponse>` en la misma carpeta
- El controller solo hace `_mediator.Send(new XxxQuery(...))`; no conoce el handler ni el repositorio
- Los handlers de lectura mapean a DTO con AutoMapper y **nunca** llaman a `IUnitOfWork.SaveChangesAsync`
- Los handlers de escritura mutan la entidad de dominio y persisten vía `IUnitOfWork`
- Entidad inexistente en Delete/Update-by-id → `Application.Common.ClientNotFoundException` → el controller devuelve 404
- Registro: `services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(...))` en `Startup.cs`; los handlers transient quedan disponibles en Autofac
- Se elimina `Application/Services/ClientService.cs`

El contrato HTTP **no cambia**: mismos endpoints, mismos cuerpos JSON y mismos códigos de estado. `specs/api/api-spec.md` y `contracts/api/openapi.yaml` no se modifican.

## Consecuencias
### Positivas
- [x] Controller delgado: sin lógica de negocio ni conocimiento de persistencia
- [x] Un archivo (o carpeta) por caso de uso: añadir features no inflata clases existentes
- [x] Tests por handler, sin depender de un service "todo en uno"
- [x] Base para añadir comportamientos asíncronos (notificaciones, proyección) por `INotificationHandler`
- [x] Contrato de lectura (DTO) separado del de escritura (command)

### Negativas
- [x] Dependencia externa nueva (MediatR 12.5.0) y versión fijada por licencia (v13+ es de pago)
- [x] Más archivos por caso de uso (record + handler)
- [x] `ClientNotFoundException` para "no existe" en lugar de devolver `null`, porque `Unit` no transporta ese estado
- [x] No se separan modelos de lectura/escritura: sigue habiendo una sola tabla

## Alternativas Consideradas
### Alternativa 1: Dispatcher propio (~40 líneas)
**Descripción**: interfaz `IMediator` mínima implementada en el proyecto, con escaneo de `IRequestHandler<,>`.
**Pros**: 0 dependencias, 0 licencias, control total.
**Contras**: código propio que mantener; no estándar, nadie más lo reconocerá.

### Alternativa 2: Mediator (martinothamar)
**Descripción**: mediator gratuito con source generators.
**Pros**: sin coste, buen rendimiento.
**Contras**: menos adotado; API distinta a la que exige el estándar de la industria.

### Alternativa 3: CQRS completo con read model separado
**Descripción**: tabla `clients_read` + proyección actualizada en los command handlers + nueva migración.
**Pros**: lectura optimizada, sidecar reutilizable.
**Contras**: complejidad y duplicación de datos innecesaria para el alcance del POC.

## Notas Adicionales
- Modo CQRS sin sidecar: se mantiene una única tabla `clients`, por lo que **no hay migración nueva**.
- Las lecturas no usan `AsNoTracking()`: `Application` no referencia EF Core (dependencia prohibida por Clean Architecture). Optimización pendiente cuando el volumen lo justifique.
- Pendiente a futuro: `INotificationHandler` para eventos de dominio y `IPipelineBehavior` para logging/validación.
