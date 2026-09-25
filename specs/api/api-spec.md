# Especificación de API

## Información General
- **Versión**: 1.0.0
- **Base URL**: http://localhost:8080/api/v1 (local Docker), https://pocsddnetcoreapi.timeforsoftware.com/api/v1 (prod nginx)
- **Formato**: JSON

## Endpoints - Clientes

### GET /api/v1/clients
- **Descripción**: Obtener lista de todos los clientes
- **Parámetros**: Ninguno
- **Response 200**: 
  ```json
  [
    {
      "id": "uuid",
      "name": "string",
      "email": "string",
      "phone": "string",
      "logo": "string (opcional, URL)",
      "createdAt": "2024-01-01T00:00:00Z"
    }
  ]
  ```
- **Response 500**: Error interno del servidor

### GET /api/v1/clients/{id}
- **Descripción**: Obtener un cliente por su ID
- **Parámetros**: 
  - `id` (path, uuid, requerido): Identificador del cliente
- **Response 200**: 
  ```json
  {
    "id": "uuid",
    "name": "string",
    "email": "string",
    "phone": "string",
    "logo": "string (opcional, URL)",
    "createdAt": "2024-01-01T00:00:00Z"
  }
  ```
- **Response 404**: Cliente no encontrado

### POST /api/v1/clients
- **Descripción**: Crear un nuevo cliente
- **Request Body**: 
  ```json
  {
    "name": "string (requerido)",
    "email": "string (requerido)",
    "phone": "string (opcional)",
    "logo": "string (opcional, URL absoluta http/https)"
  }
  ```
- **Response 201**: Cliente creado exitosamente
- **Response 400**: Datos de entrada inválidos

### PUT /api/v1/clients/{id}
- **Descripción**: Actualizar un cliente existente
- **Parámetros**: 
  - `id` (path, uuid, requerido): Identificador del cliente
- **Request Body**: 
  ```json
  {
    "name": "string (requerido)",
    "email": "string (requerido)",
    "phone": "string (opcional)",
    "logo": "string (opcional, URL absoluta http/https)"
  }
  ```
- **Response 200**: Cliente actualizado exitosamente
- **Response 404**: Cliente no encontrado
- **Response 400**: Datos de entrada inválidos

### DELETE /api/v1/clients/{id}
- **Descripción**: Eliminar un cliente
- **Parámetros**: 
  - `id` (path, uuid, requerido): Identificador del cliente
- **Response 204**: Cliente eliminado exitosamente
- **Response 404**: Cliente no encontrado
- **Nota**: Un cliente con proyectos asociados no se puede eliminar (FK `ON DELETE RESTRICT`, 409 futuro)

## Endpoints - Proyectos

### GET /api/v1/projects
- **Descripción**: Obtener lista de todos los proyectos
- **Parámetros**:
  - `clientId` (query, uuid, opcional): Filtra los proyectos de un cliente
- **Response 200**: 
  ```json
  [
    {
      "id": "uuid",
      "clientId": "uuid",
      "title": "string",
      "description": "string",
      "technologies": "string",
      "startDate": "2024-01-01T00:00:00Z",
      "durationMonths": 6,
      "createdAt": "2024-01-01T00:00:00Z"
    }
  ]
  ```
- **Response 500**: Error interno del servidor

### GET /api/v1/projects/{id}
- **Descripción**: Obtener un proyecto por su ID
- **Parámetros**: 
  - `id` (path, uuid, requerido): Identificador del proyecto
- **Response 200**: 
  ```json
  {
    "id": "uuid",
    "clientId": "uuid",
    "title": "string",
    "description": "string",
    "technologies": "string",
    "startDate": "2024-01-01T00:00:00Z",
    "durationMonths": 6,
    "createdAt": "2024-01-01T00:00:00Z"
  }
  ```
- **Response 404**: Proyecto no encontrado

### POST /api/v1/projects
- **Descripción**: Crear un nuevo proyecto
- **Request Body**: 
  ```json
  {
    "clientId": "uuid (requerido)",
    "title": "string (requerido)",
    "description": "string (requerido)",
    "technologies": "string (requerido)",
    "startDate": "2024-01-01T00:00:00Z (requerido)",
    "durationMonths": 6
  }
  ```
- **Response 201**: Proyecto creado exitosamente
- **Response 400**: Datos de entrada inválidos (incluye `durationMonths <= 0`)
- **Response 404**: El cliente indicado no existe

### PUT /api/v1/projects/{id}
- **Descripción**: Actualizar un proyecto existente
- **Parámetros**: 
  - `id` (path, uuid, requerido): Identificador del proyecto
- **Request Body**: 
  ```json
  {
    "clientId": "uuid (requerido)",
    "title": "string (requerido)",
    "description": "string (requerido)",
    "technologies": "string (requerido)",
    "startDate": "2024-01-01T00:00:00Z (requerido)",
    "durationMonths": 6
  }
  ```
- **Response 200**: Proyecto actualizado exitosamente
- **Response 400**: Datos de entrada inválidos
- **Response 404**: Proyecto no encontrado o cliente indicado no existe

### DELETE /api/v1/projects/{id}
- **Descripción**: Eliminar un proyecto
- **Parámetros**: 
  - `id` (path, uuid, requerido): Identificador del proyecto
- **Response 204**: Proyecto eliminado exitosamente
- **Response 404**: Proyecto no encontrado

## Autenticación
<!-- Pendiente de implementar -->

## Rate Limiting
<!-- Pendiente de implementar -->
