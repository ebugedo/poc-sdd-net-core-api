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
| `ArgumentException` | `Client` (Domain) | 400 `{code: "VALIDATION_ERROR"}` |
| `ClientNotFoundException` | Handlers de Application | 404 |
