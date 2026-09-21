# Modelo de Dominio

## Entidades Principales

### Client
- **Descripción**: Representa un cliente del sistema
- **Propiedades**:
  - `Id` (Guid): Identificador único
  - `Name` (string): Nombre completo del cliente
  - `Email` (string): Correo electrónico
  - `Phone` (string): Número de teléfono
  - `CreatedAt` (DateTime): Fecha y hora de creación
- **Reglas de negocio**:
  - El nombre es obligatorio y no puede estar vacío
  - El email es obligatorio y debe tener formato válido
  - El teléfono es opcional
  - El Id se genera automáticamente
  - CreatedAt se establece al momento de crear
- **Relaciones**: Ninguna por ahora

## Value Objects

### Email (futuro)
- Validación de formato de email
- Inmutabilidad

### PhoneNumber (futuro)
- Normalización de números de teléfono
- Validación de formato

## Aggregate Roots

### ClientAggregate (futuro)
- Raíz del aggregate de cliente
- Puede incluir entidades hijas como ClientAddress
