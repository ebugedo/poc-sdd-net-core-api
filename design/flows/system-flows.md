# Flujos del Sistema

## Flujo: Crear Cliente (Command)

```mermaid
sequenceDiagram
    participant Client
    participant API as ClientsController
    participant Mediator as MediatR (IMediator)
    participant Handler as CreateClientCommandHandler
    participant Domain as Client (Domain)
    participant Repo as IClientRepository / IUnitOfWork
    participant DB

    Client->>API: POST /api/v1/clients
    API->>Mediator: Send(CreateClientCommand)
    Mediator->>Handler: Handle(command)
    Handler->>Domain: Client.Create(name, email, phone)
    Domain-->>Handler: Client (invariantes validadas)
    Handler->>Repo: AddAsync(client) + SaveChangesAsync()
    Repo->>DB: INSERT clients
    DB-->>Repo: OK
    Handler-->>API: ClientResponse
    API-->>Client: 201 Created
```

## Flujo: Obtener Cliente (Query)

```mermaid
sequenceDiagram
    participant Client
    participant API as ClientsController
    participant Mediator as MediatR (IMediator)
    participant Handler as GetAllClientsQueryHandler
    participant Repo as IClientRepository
    participant DB

    Client->>API: GET /api/v1/clients
    API->>Mediator: Send(GetAllClientsQuery)
    Mediator->>Handler: Handle(query)
    Handler->>Repo: GetAllAsync()
    Repo->>DB: SELECT * FROM clients
    DB-->>Repo: filas
    Handler-->>API: IReadOnlyList<ClientResponse> (AutoMapper)
    API-->>Client: 200 OK
```

## Flujo: Actualizar / Eliminar Cliente (Command)

```mermaid
sequenceDiagram
    participant Client
    participant API as ClientsController
    participant Mediator as MediatR (IMediator)
    participant Handler as UpdateClientCommandHandler
    participant Repo as IClientRepository
    participant DB

    Client->>API: PUT /api/v1/clients/{id}
    API->>Mediator: Send(UpdateClientCommand)
    Mediator->>Handler: Handle(command)
    Handler->>Repo: GetByIdAsync(id)
    alt Cliente no existe
        Handler-->>API: null
        API-->>Client: 404 Not Found
    else Cliente existe
        Handler->>Repo: Client.Update(...) + SaveChangesAsync()
        Repo->>DB: UPDATE clients
        Handler-->>API: ClientResponse
        API-->>Client: 200 OK
    end
```

`DELETE` sigue el mismo flujo, pero el handler lanza `ClientNotFoundException` cuando no existe y el controller devuelve 404; en caso contrario 204 No Content.

## Flujo de Errores

```mermaid
sequenceDiagram
    participant Client
    participant API as ClientsController
    participant Mediator as MediatR (IMediator)
    participant Handler as CreateClientCommandHandler
    participant Domain as Client (Domain)

    Client->>API: POST /api/v1/clients
    API->>Mediator: Send(CreateClientCommand)
    Mediator->>Handler: Handle(command)
    Handler->>Domain: Client.Create(name, email, phone)
    Domain-->>Handler: ArgumentException
    Handler-->>API: excepción
    API-->>Client: 400 Bad Request {code: VALIDATION_ERROR}
```

| Excepción | Origen | Respuesta HTTP |
|-----------|--------|----------------|
| `ArgumentException` | `Client`, `Project` (Domain) | 400 `{code: "VALIDATION_ERROR"}` |
| `ClientNotFoundException` | Handlers de Application | 404 |
| `ProjectNotFoundException` | Handlers de Application | 404 |

## Flujo: Crear Proyecto (Command)

```mermaid
sequenceDiagram
    participant Client
    participant API as ProjectsController
    participant Mediator as MediatR (IMediator)
    participant Handler as CreateProjectCommandHandler
    participant Clients as IClientRepository
    participant Repo as IProjectRepository
    participant DB

    Client->>API: POST /api/v1/projects {clientId, title, ...}
    API->>Mediator: Send(CreateProjectCommand)
    Mediator->>Handler: Handle(command)
    Handler->>Clients: GetByIdAsync(clientId)
    alt Cliente no existe
        Handler-->>API: ClientNotFoundException
        API-->>Client: 404 Not Found
    else Cliente existe
        Handler->>Repo: Project.Create(...) + AddAsync() + SaveChangesAsync()
        Repo->>DB: INSERT INTO projects
        Handler-->>API: ProjectResponse (AutoMapper)
        API-->>Client: 201 Created
    end
```

## Flujo: Listar Proyectos con Filtro (Query)

```mermaid
sequenceDiagram
    participant Client
    participant API as ProjectsController
    participant Mediator as MediatR (IMediator)
    participant Handler as GetAllProjectsQueryHandler
    participant Repo as IProjectRepository
    participant DB

    Client->>API: GET /api/v1/projects?clientId={id}
    API->>Mediator: Send(GetAllProjectsQuery(clientId?))
    Mediator->>Handler: Handle(query)
    Handler->>Repo: GetAllAsync(clientId?)
    alt Con filtro
        Repo->>DB: SELECT * FROM projects WHERE client_id = @id
    else Sin filtro
        Repo->>DB: SELECT * FROM projects ORDER BY start_date
    end
    Handler-->>API: IReadOnlyList<ProjectResponse>
    API-->>Client: 200 OK
```

Los handlers de lectura nunca llaman a `SaveChangesAsync`. `GetProjectById` y `DeleteProject` lanzan `ProjectNotFoundException` → 404.

## Flujo: Logo del Cliente

El logo viaja como string (URL) en el mismo command/query de cliente; no hay flujo de upload.

```mermaid
sequenceDiagram
    participant Client
    participant API as ClientsController
    participant Handler as UpdateClientCommandHandler
    participant Domain as Client (Domain)

    Client->>API: PUT /api/v1/clients/{id} {logo: "https://.../acme.png"}
    API->>Handler: Send(UpdateClientCommand)
    Handler->>Domain: client.Update(name, email, phone, logo)
    alt URL no absoluta o > 2048 chars
        Domain-->>Handler: ArgumentException
        Handler-->>API: excepción
        API-->>Client: 400 Bad Request
    else Logo válido
        Domain-->>Handler: cliente actualizado
        API-->>Client: 200 OK
    end
```

`logo: null` o ausente borra el logo del cliente.
