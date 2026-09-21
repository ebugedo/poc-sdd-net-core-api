# Contratos de Schemas

## Propósito
Este directorio contiene schemas compartidos (JSON Schema) para validación.

## Uso
- Validación de request/response
- Generación de modelos
- Documentación

## Archivos

### `resource.schema.json`
Schema para la entidad Resource.

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "type": "object",
  "properties": {
    "id": { "type": "string" },
    "name": { "type": "string" }
  },
  "required": ["name"]
}
```
