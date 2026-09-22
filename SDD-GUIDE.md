# Guía de Spec-Driven Development (SDD)

## ¿Qué es SDD?
Spec-Driven Development (Desarrollo Guiado por Especificaciones) es una metodología donde las **especificaciones** guían todo el proceso de desarrollo. Primero defines **qué** debe hacer el sistema, luego **cómo** se diseña, y finalmente **cómo** se implementa.

## Flujo de Trabajo SDD

```
1. Especificar (specs/)
   ↓
2. Diseñar (design/)
   ↓
3. Definir Contratos (contracts/)
   ↓
4. Implementar (src/)
   ↓
5. Verificar (tests/)
```

## Flujo Detallado

### 1. Especificar (`specs/`)
**¿Qué hacemos?** Definimos QUÉ debe hacer el sistema.

- **Dominio**: Entidades, reglas de negocio, invariantes
- **API**: Endpoints, request/response, errores
- **Features**: Historias de usuario, criterios de aceptación
- **Glossary**: Términos compartidos

**Archivos clave**:
- `specs/domain/domain-model.md` - Modelo de dominio
- `specs/domain/business-rules.md` - Reglas de negocio
- `specs/api/api-spec.md` - Especificación de API
- `specs/features/user-stories.md` - Historias de usuario

### 2. Diseñar (`design/`)
**¿Qué hacemos?** Definimos CÓMO se estructura el sistema.

- **Arquitectura**: Capas, patrones, componentes
- **Flujos**: Secuencias, diagramas
- **Modelo de datos**: Tablas, relaciones, índices

**Archivos clave**:
- `design/architecture/architecture-overview.md` - Visión general
- `design/flows/system-flows.md` - Flujos del sistema
- `design/data-model/data-model.md` - Modelo de datos

### 3. Definir Contratos (`contracts/`)
**¿Qué hacemos?** Creamos contratos formales para comunicación.

- **API**: OpenAPI/Swagger
- **Schemas**: JSON Schema
- **Eventos**: Contratos de eventos

**Archivos clave**:
- `contracts/api/openapi.yaml` - Especificación OpenAPI
- `contracts/schemas/` - Schemas JSON

### 4. Implementar (`src/`)
**¿Qué hacemos?** Escribimos código que cumple las specs.

- **API**: Controllers, middleware
- **Domain**: Entidades, servicios
- **Infrastructure**: Repositorios, acceso a datos

### 5. Verificar (`tests/`)
**¿Qué hacemos?** Verificamos que el código cumple las specs.

- **Unit**: Pruebas aisladas
- **Integration**: Pruebas de integración
- **Acceptance**: Pruebas de aceptación

## Registro de Decisiones (ADR)

Los `decisions/` contienen las decisiones de arquitectura importantes. Cada ADR documenta:
- El contexto
- La decisión tomada
- Las consecuencias
- Las alternativas consideradas

## Plantillas

### Historia de Usuario
```markdown
### [ID] Título
**Como** [rol]
**Quiero** [objetivo]
**Para** [beneficio]

**Criterios de Aceptación:**
- [ ] Criterio 1
- [ ] Criterio 2
```

### ADR
```markdown
# ADR-NNN: Título

## Estado
Aceptado

## Contexto
[Problema]

## Decisión
[Decisión]

## Consecuencias
[Consecuencias]
```

## Regla de Sincronización: Código y Documentación

**REGLA ABSOLUTA**: Toda implementación de código DEBE ir acompañada de actualización de documentación.

### Flujo Obligatorio

```
Código nuevo/modificado → Documentación actualizada → Commit
```

### Mapeo de Documentación por Tipo de Cambio

| Cuando implementas... | Actualiza... |
|----------------------|--------------|
| Una entidad del dominio | `specs/domain/domain-model.md` + `design/data-model/data-model.md` |
| Un endpoint de API | `specs/api/api-spec.md` + `contracts/api/openapi.yaml` |
| Una regla de negocio | `specs/domain/business-rules.md` |
| Un flujo de usuario | `specs/features/user-flows.md` + `design/flows/system-flows.md` |
| Una nueva feature | `specs/features/user-stories.md` |
| Un cambio arquitectónico | `design/architecture/architecture-overview.md` + `decisions/` |
| Una migración de BD | `design/data-model/data-model.md` |

### Checklist de Sincronización

Antes de cada commit, verificar:

```
□ ¿Creé una entidad?
  → Actualizar specs/domain/domain-model.md
  → Actualizar design/data-model/data-model.md

□ ¿Agregué un endpoint?
  → Actualizar specs/api/api-spec.md
  → Actualizar contracts/api/openapi.yaml

□ ¿Agregué una regla de negocio?
  → Actualizar specs/domain/business-rules.md

□ ¿Implementé un flujo?
  → Actualizar specs/features/user-flows.md
  → Actualizar design/flows/system-flows.md

□ ¿Tomé una decisión técnica?
  → Crear ADR en decisions/

□ ¿Cambié la arquitectura?
  → Actualizar design/architecture/architecture-overview.md
```

### Ejemplo

Si creas una entidad `Product`:

1. **Antes del código**: Actualizar `specs/domain/domain-model.md` con la definición
2. **Crear código**: Crear `Product.cs` en Domain/Entities
3. **Actualizar diseño**: Actualizar `design/data-model/data-model.md` con la tabla
4. **Actualizar API**: Si tiene endpoint, actualizar `specs/api/api-spec.md`
5. **Commit**: Incluir código Y documentación juntos

## Consejos

1. **Empieza por las specs**: Antes de escribir código, define qué debe hacer
2. **Sé específico**: Las specs deben ser claras y medibles
3. **Documenta decisiones**: Los ADRs ayudan a entender el "por qué"
4. **Mantén actualizado**: Las specs deben reflejar el estado actual
5. **Usa lenguaje natural**: Las specs deben ser entendibles por todos
6. **Sincroniza siempre**: Código y documentación deben ir de la mano
