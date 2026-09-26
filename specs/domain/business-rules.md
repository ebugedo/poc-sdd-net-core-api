# Reglas de Negocio

## Reglas de Validación

### BR-001: Nombre de Cliente Requerido
- **Descripción**: El nombre del cliente es obligatorio para crear o actualizar
- **Condición**: `string.IsNullOrWhiteSpace(name) == true` => error
- **Mensaje de error**: `Name is required`
- **Se aplica a**: `Client.Create()`, `Client.Update()` - `src/Domain/Entities/Client.cs`

### BR-002: Email de Cliente Requerido
- **Descripción**: El email del cliente es obligatorio y debe ser formato válido
- **Condición**: `string.IsNullOrWhiteSpace(email) == true` => error
- **Mensaje de error**: `Email is required`
- **Se aplica a**: `Client.Create()`, `Client.Update()` - `src/Domain/Entities/Client.cs`

### BR-003: Teléfono Opcional
- **Descripción**: El teléfono es opcional, puede ser null
- **Condición**: `phone == null` => permitido
- **Mensaje de error**: N/A
- **Se aplica a**: `Client.Create(phone)`, `Client.Update(phone)`

### BR-007: Logo de Cliente Opcional
- **Descripción**: El logo del cliente es una URL opcional. No se almacenan ficheros binarios en el sistema
- **Condición**: `logo == null` => permitido. Si se informa, debe ser URL absoluta http/https de máx. 2048 caracteres
- **Mensaje de error**: `Logo must be a valid absolute URL` / `Logo must not exceed 2048 characters`
- **Se aplica a**: `Client.Create(logo)`, `Client.Update(logo)` - `src/Domain/Entities/Client.cs`

### BR-008: Título de Proyecto Requerido
- **Descripción**: El título del proyecto es obligatorio
- **Condición**: `string.IsNullOrWhiteSpace(title) == true` => error
- **Mensaje de error**: `Title is required`
- **Se aplica a**: `Project.Create(title)`, `Project.Update(title)`

### BR-009: Descripción de Proyecto Requerida
- **Descripción**: La descripción del proyecto es obligatoria
- **Condición**: `string.IsNullOrWhiteSpace(description) == true` => error
- **Mensaje de error**: `Description is required`
- **Se aplica a**: `Project.Create(description)`, `Project.Update(description)`

### BR-010: Tecnologías Requeridas
- **Descripción**: El texto de tecnologías utilizadas es obligatorio
- **Condición**: `string.IsNullOrWhiteSpace(technologies) == true` => error
- **Mensaje de error**: `Technologies is required`
- **Se aplica a**: `Project.Create(technologies)`, `Project.Update(technologies)`

### BR-011: Duración en Meses Positiva
- **Descripción**: La duración del proyecto es opcional y se expresa en meses enteros
- **Condición**: `durationMonths == null` => permitido. Si se informa, `durationMonths <= 0` => error
- **Mensaje de error**: `DurationMonths must be greater than 0`
- **Se aplica a**: `Project.Create(durationMonths)`, `Project.Update(durationMonths)`

### BR-012: Cliente del Proyecto Debe Existir
- **Descripción**: Un proyecto siempre pertenece a un cliente existente
- **Condición**: `clientId == Guid.Empty` o cliente inexistente => error
- **Mensaje de error**: `ClientId is required` (dominio) / `Client not found` (404 en API)
- **Se aplica a**: `CreateProjectCommandHandler` valida contra `IClientRepository`

### BR-013: No se Puede Eliminar un Cliente con Proyectos
- **Descripción**: Un cliente con proyectos asociados no puede eliminarse
- **Condición**: existen proyectos con `client_id` del cliente => error de BD
- **Mensaje de error**: `Cannot delete a client with projects` (409 futuro)
- **Se aplica a**: FK `projects.client_id` con `ON DELETE RESTRICT`

### BR-014: Sector de Proyecto Obligatorio
- **Descripción**: Todo proyecto debe pertenecer a un sector del catálogo
- **Condición**: `sectorId == Guid.Empty` => error
- **Mensaje de error**: `SectorId is required`
- **Se aplica a**: `Project.Create(sectorId)`, `Project.Update(sectorId)` - `src/Domain/Entities/Project.cs`

### BR-015: Sector Debe Existir
- **Descripción**: El `sectorId` debe corresponder a un sector sembrado
- **Condición**: sector inexistente => error
- **Mensaje de error**: `Sector not found` (404 en API)
- **Se aplica a**: `CreateProjectCommandHandler`, `UpdateProjectCommandHandler`

### BR-016: Catálogo de Sectores Fijo
- **Descripción**: El catálogo de sectores es de solo lectura con 7 valores: Administración pública, Ingeniería, Publicidad, Servicios financieros, Servicios tecnológicos, Transporte y Sector inmobiliario
- **Condición**: no existen endpoints de escritura; los datos se siembran con la migración `AddSectorsAndProjectSector`
- **Mensaje de error**: N/A
- **Se aplica a**: `SectorConfiguration` (`HasData`), `SectorsController` (solo GET)

## Reglas de Negocio

### BR-004: Generación de Id
- **Cuándo**: Al crear un cliente con `Client.Create()`
- **Qué sucede**: Se genera `Guid.NewGuid()` automáticamente
- **Excepciones**: Ninguna

### BR-005: Fecha de Creación Automática
- **Cuándo**: Al crear un cliente
- **Qué sucede**: `CreatedAt = DateTime.UtcNow`
- **Excepciones**: No se puede modificar manualmente

### BR-006: Email Único
- **Cuándo**: Al persistir en PostgreSQL
- **Qué sucede**: Índice único `idx_clients_email` en `clients.email`
- **Excepciones**: Violación genera error de BD (409 Conflict futuro)

## Invariantes

- `Client.Id != Guid.Empty` siempre
- `Client.Name` nunca es null/vacío después de creación
- `Client.Email` nunca es null/vacío después de creación
- `Client.Logo` es null o una URL absoluta válida
- `Client.CreatedAt` es UTC y se establece solo una vez
- `Project.Id != Guid.Empty` siempre
- `Project.Title`, `Project.Description` y `Project.Technologies` nunca son null/vacío
- `Project.ClientId != Guid.Empty` siempre
- `Project.SectorId != Guid.Empty` siempre y apunta a un sector existente
- El catálogo de sectores tiene exactamente 7 entradas con nombre único
- `Project.DurationMonths` es null o > 0
- `Project.CreatedAt` es UTC y se establece solo una vez
