# Manejo de Errores

## Formato de Error
```json
{
  "code": "ERROR_CODE",
  "message": "Descripción del error",
  "details": {}
}
```

## Códigos de Error

### Errores de Cliente (4xx)
| Código | HTTP Status | Descripción |
|--------|-------------|-------------|
| VALIDATION_ERROR | 400 | Error de validación |
| NOT_FOUND | 404 | Recurso no encontrado |
| CONFLICT | 409 | Conflicto de estado |

### Errores de Servidor (5xx)
| Código | HTTP Status | Descripción |
|--------|-------------|-------------|
| INTERNAL_ERROR | 500 | Error interno |
| SERVICE_UNAVAILABLE | 503 | Servicio no disponible |
