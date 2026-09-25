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

### US-002: Logo del Cliente (Opcional)
**Como** usuario del sistema
**Quiero** asociar una URL de logo a un cliente
**Para** identificar visualmente al cliente

**Criterios de Aceptación:**
- [ ] El campo `logo` es opcional en request y response
- [ ] Si se informa, debe ser una URL absoluta http/https de máx. 2048 caracteres
- [ ] Un `logo` inválido devuelve 400 `VALIDATION_ERROR`
- [ ] Un `logo` ausente se persiste como `null` y se devuelve como `null`

**Escenario: Crear cliente con logo**
- Dado un request con `logo: "https://cdn.timeforsoftware.com/logos/acme.png"`
- Cuando POST /api/v1/clients
- Entonces 201 Created y el response incluye ese `logo`

**Escenario: Logo inválido**
- Dado un request con `logo: "no-es-una-url"`
- Cuando POST /api/v1/clients
- Entonces 400 con `code: "VALIDATION_ERROR"`

---

### US-003: Gestión de Proyectos (CRUD)
**Como** usuario del sistema
**Quiero** crear, consultar, actualizar y eliminar proyectos de un cliente
**Para** registrar los proyectos que se realizan para cada cliente

**Criterios de Aceptación:**
- [ ] Puedo listar todos los proyectos `GET /api/v1/projects` -> 200
- [ ] Puedo filtrar por cliente `GET /api/v1/projects?clientId={id}` -> 200
- [ ] Puedo obtener un proyecto por Id `GET /api/v1/projects/{id}` -> 200 / 404
- [ ] Puedo crear un proyecto con `clientId`, `title`, `description`, `technologies` y `startDate` requeridos `POST /api/v1/projects` -> 201 / 400
- [ ] Si el `clientId` no existe, `POST /api/v1/projects` -> 404
- [ ] Puedo actualizar un proyecto `PUT /api/v1/projects/{id}` -> 200 / 404 / 400
- [ ] Puedo eliminar un proyecto `DELETE /api/v1/projects/{id}` -> 204 / 404
- [ ] `durationMonths` es opcional y, si se informa, debe ser > 0
- [ ] No se puede eliminar un cliente que tiene proyectos asociados

**Escenario: Crear proyecto válido**
- Dado un cliente existente con id `c1`
- Cuando POST /api/v1/projects con `{clientId: "c1", title: "Portal", description: "Intranet", technologies: ".NET 8, PostgreSQL", startDate: "2026-01-15", durationMonths: 6}`
- Entonces 201 Created y el response incluye `clientId: "c1"` y `durationMonths: 6`

**Escenario: Cliente inexistente**
- Dado un `clientId` que no existe
- Cuando POST /api/v1/projects
- Entonces 404 con `code: "NOT_FOUND"`

**Escenario: Duración inválida**
- Dado un request con `durationMonths: 0`
- Cuando POST /api/v1/projects
- Entonces 400 con `code: "VALIDATION_ERROR"`

**Escenario: Filtrar por cliente**
- Dado dos clientes con proyectos
- Cuando GET /api/v1/projects?clientId={id del cliente A}
- Entonces 200 con solo los proyectos del cliente A

---
