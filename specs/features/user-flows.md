# Flujos de Usuario

## Flujo: [Nombre del Flujo]

### Pasos
1. [Paso 1]
2. [Paso 2]
3. [Paso 3]

### Decisiones
- Si [condición] → [acción]
- Si [condición] → [acción]

### Resultados Exitosos
- [Resultado 1]
- [Resultado 2]

### Errores Posibles
- [Error 1]: [Descripción]
- [Error 2]: [Descripción]

---

## Flujo: Registrar Proyecto de un Cliente

### Pasos
1. El usuario obtiene o crea el cliente (`GET /api/v1/clients`)
2. Envía `POST /api/v1/projects` con `clientId`, `title`, `description`, `technologies`, `startDate` y `durationMonths` (opcional)
3. El API valida el cuerpo y envía el `CreateProjectCommand` al Mediator
4. El handler valida que el cliente exista, persiste el proyecto y devuelve `ProjectResponse`
5. El API responde 201 Created

### Decisiones
- Si el cliente no existe → `ClientNotFoundException` → 404
- Si `title`, `description` o `technologies` vienen vacíos → 400 `VALIDATION_ERROR`
- Si `durationMonths` viene informado y es <= 0 → 400 `VALIDATION_ERROR`
- Si `durationMonths` no viene informado → el proyecto se considera en curso (duración abierta)

### Resultados Exitosos
- Proyecto creado con `id` UUID y `createdAt` ISO8601
- El proyecto queda listado en `GET /api/v1/projects?clientId={clientId}`

### Errores Posibles
- 400: cuerpo inválido (campos requeridos vacíos o duración <= 0)
- 404: el `clientId` no corresponde a ningún cliente
- 500: error inesperado de persistencia

---

## Flujo: Consultar Proyectos de un Cliente

### Pasos
1. El usuario solicita `GET /api/v1/projects?clientId={id}` (o sin filtro para ver todos)
2. El API envía el `GetAllProjectsQuery` con el filtro opcional
3. El handler consulta el repositorio (con `Where` si hay `clientId`) y mapea a `ProjectResponse`
4. El API responde 200 con la lista

### Decisiones
- Si no hay `clientId` → se devuelven todos los proyectos ordenados por fecha de inicio
- Si el `clientId` no existe → se devuelve lista vacía con 200 (el filtro no valida existencia)

### Resultados Exitosos
- Lista de proyectos con `durationMonths: null` cuando el proyecto sigue abierto

### Errores Posibles
- 500: error inesperado al consultar

---

## Flujo: Eliminar Cliente con Proyectos

### Pasos
1. El usuario solicita `DELETE /api/v1/clients/{id}`
2. El API envía el `DeleteClientCommand` al Mediator
3. El handler elimina el cliente

### Decisiones
- Si existen proyectos con ese `client_id` → la BD rechaza el borrado por `ON DELETE RESTRICT`

### Resultados Exitosos
- 204 No Content cuando el cliente no tiene proyectos asociados

### Errores Posibles
- 404: el cliente no existe
- Conflicto de integridad referencial: el cliente tiene proyectos (ver BR-013; manejo 409 futuro)

---
