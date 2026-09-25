# Modelo de Dominio

## Entidades Principales

### Client
- **Descripción**: Representa un cliente del sistema
- **Propiedades**:
  - `Id` (Guid): Identificador único
  - `Name` (string): Nombre completo del cliente
  - `Email` (string): Correo electrónico
  - `Phone` (string?): Número de teléfono
  - `Logo` (string?): URL del logo del cliente
  - `CreatedAt` (DateTime): Fecha y hora de creación
- **Reglas de negocio**:
  - El nombre es obligatorio y no puede estar vacío
  - El email es obligatorio y debe tener formato válido
  - El teléfono es opcional
  - El logo es opcional y, si se informa, debe ser una URL absoluta válida
  - El Id se genera automáticamente
  - CreatedAt se establece al momento de crear
- **Relaciones**:
  - `1:N` → `Project` (un cliente puede tener muchos proyectos)

### Project
- **Descripción**: Representa un proyecto经济区 por un cliente
- **Propiedades**:
  - `Id` (Guid): Identificador único
  - `ClientId` (Guid): Cliente propietario (FK → `clients.id`)
  - `Title` (string): Título del proyecto
  - `Description` (string): Descripción del proyecto
  - `Technologies` (string): Tecnologías utilizadas, texto libre
  - `StartDate` (DateTime): Fecha de inicio del proyecto
  - `DurationMonths` (int?): Duración en meses, opcional
  - `CreatedAt` (DateTime): Fecha y hora de creación
- **Reglas de negocio**:
  - `ClientId` es obligatorio y debe apuntar a un cliente existente
  - Título, descripción y tecnologías son obligatorios y no pueden estar vacíos
  - `StartDate` es obligatoria
  - `DurationMonths` es opcional, pero si se informa debe ser > 0
  - El Id se genera automáticamente y `CreatedAt` se establece al crear
- **Relaciones**:
  - `N:1` → `Client` (obligatoria)

## Value Objects

### Email (futuro)
- Validación de formato de email
- Inmutabilidad

### PhoneNumber (futuro)
- Normalización de números de teléfono
- Validación de formato

### ProjectDuration (futuro)
- Normalización de "N meses" y cálculo de fecha fin
- En esta versión se modela como `int?` simple (BR-011)

## Aggregate Roots

### ClientAggregate (futuro)
- Raíz del aggregate de cliente
- Puede incluir entidades hijas como ClientAddress

### ProjectAggregate
- Raíz del aggregate de proyecto
- Un proyecto pertenece a exactamente un cliente
- No tiene entidades hijas en esta versión
