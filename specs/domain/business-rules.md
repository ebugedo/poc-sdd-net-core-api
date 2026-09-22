# Reglas de Negocio

## Reglas de Validación

### BR-001: Nombre de Cliente Requerido
- **Descripción**: El nombre del cliente es obligatorio para crear o actualizar
- **Condición**: `string.IsNullOrWhiteSpace(name) == true` => error
- **Mensaje de error**: `Name is required`
- **Se aplica a**: `Client.Create()`, `Client.Update()` - `src/domain/Entities/Client.cs:15`

### BR-002: Email de Cliente Requerido
- **Descripción**: El email del cliente es obligatorio y debe ser formato válido
- **Condición**: `string.IsNullOrWhiteSpace(email) == true` => error
- **Mensaje de error**: `Email is required`
- **Se aplica a**: `Client.Create()`, `Client.Update()` - `src/domain/Entities/Client.cs:18`

### BR-003: Teléfono Opcional
- **Descripción**: El teléfono es opcional, puede ser null
- **Condición**: `phone == null` => permitido
- **Mensaje de error**: N/A
- **Se aplica a**: `Client.Create(phone)`, `Client.Update(phone)`

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
- `Client.CreatedAt` es UTC y se establece solo una vez
