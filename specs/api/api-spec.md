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
    "phone": "string (opcional)"
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
    "phone": "string (opcional)"
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

## Autenticación
<!-- Pendiente de implementar -->

## Rate Limiting
<!-- Pendiente de implementar -->
