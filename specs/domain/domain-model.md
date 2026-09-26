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

### Sector
- **Descripción**: Catálogo cerrado de sectores de actividad. Es de **solo lectura**: se siembra con 7 valores fijos y no se crea ni modifica por API
- **Propiedades**:
  - `Id` (Guid): Identificador único (valor fijo por sector)
  - `Name` (string): Nombre del sector
- **Valores del catálogo** (BR-015):
  1. Administración pública
  2. Ingeniería
  3. Publicidad
  4. Servicios financieros
  5. Servicios tecnológicos
  6. Transporte
  7. Sector inmobiliario
- **Reglas de negocio**:
  - El nombre es obligatorio y único
  - El catálogo no se modifica en runtime: no hay endpoints de escritura
- **Relaciones**:
  - `1:N` → `Project` (un sector agrupa muchos proyectos)

### Project
- **Descripción**: Representa un proyecto经济区 por un cliente
- **Propiedades**:
  - `Id` (Guid): Identificador único
  - `ClientId` (Guid): Cliente propietario (FK → `clients.id`)
  - `SectorId` (Guid): Sector del proyecto, **obligatorio** (FK → `sectors.id`)
  - `Title` (string): Título del proyecto
  - `Description` (string): Descripción del proyecto
  - `Technologies` (string): Tecnologías utilizadas, texto libre
  - `StartDate` (DateTime): Fecha de inicio del proyecto
  - `DurationMonths` (int?): Duración en meses, opcional
  - `CreatedAt` (DateTime): Fecha y hora de creación
- **Reglas de negocio**:
  - `ClientId` es obligatorio y debe apuntar a un cliente existente
  - `SectorId` es obligatorio y debe apuntar a un sector del catálogo
  - Título, descripción y tecnologías son obligatorios y no pueden estar vacíos
  - `StartDate` es obligatoria
  - `DurationMonths` es opcional, pero si se informa debe ser > 0
  - El Id se genera automáticamente y `CreatedAt` se establece al crear
- **Relaciones**:
  - `N:1` → `Client` (obligatoria)
  - `N:1` → `Sector` (obligatoria)

## Value Objects

### Email (futuro)
- Validación de formato de email
- Inmutabilidad

### PhoneNumber (futuro)
- Normalización de números de teléfono
- Validación de formato

### SectorCatalog
- Los 7 sectores son un catálogo de solo lectura sembrado en la migración
- No hay comandos ni endpoints de escritura para sectores
- Añadir un sector en el futuro requiere una nueva migración con `HasData`

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
