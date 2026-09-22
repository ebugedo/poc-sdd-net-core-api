# Historias de Usuario

## Formato
```
### [ID] Título
**Como** [rol]
**Quiero** [objetivo]
**Para** [beneficio]

**Criterios de Aceptación:**
- [ ] Criterio 1
- [ ] Criterio 2

**Escenario:** [nombre del escenario]
- Dado [precondición]
- Cuando [acción]
- Entonces [resultado]
```

## Historias

### US-001: Gestión de Clientes (CRUD)
**Como** usuario del sistema
**Quiero** crear, consultar, actualizar y eliminar clientes
**Para** gestionar la base de clientes

**Criterios de Aceptación:**
- [ ] Puedo listar todos los clientes `GET /api/v1/clients` -> 200
- [ ] Puedo obtener un cliente por Id `GET /api/v1/clients/{id}` -> 200 / 404
- [ ] Puedo crear un cliente con `name` y `email` requeridos `POST /api/v1/clients` -> 201 / 400
- [ ] Puedo actualizar un cliente `PUT /api/v1/clients/{id}` -> 200 / 404 / 400
- [ ] Puedo eliminar un cliente `DELETE /api/v1/clients/{id}` -> 204 / 404
- [ ] El teléfono es opcional
- [ ] Validación `Name is required` / `Email is required` devuelve 400 `VALIDATION_ERROR`

**Escenario: Crear cliente válido**
- Dado que no existe un cliente con email "john@example.com"
- Cuando POST /api/v1/clients con `{name: "John Doe", email: "john@example.com", phone: "+123"}`
- Entonces 201 Created y `id` es UUID, `createdAt` es ISO8601

**Escenario: Validación nombre vacío**
- Dado un request con `name: ""`
- Cuando POST /api/v1/clients
- Entonces 400 con `code: "VALIDATION_ERROR"`

---
