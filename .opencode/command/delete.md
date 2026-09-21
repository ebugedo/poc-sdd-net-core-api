---
description: Elimina archivos de forma segura con confirmación
agent: build
---

# Eliminar Archivos

Este comando elimina archivos del proyecto de forma segura.

## Flujo:

1. **Verificar estado**: `git status` para ver archivos existentes
2. **Mostrar archivos**: Listar archivos que se van a eliminar
3. **Solicitar confirmación**: Preguntar al usuario confirma eliminación
4. **Eliminar archivo**: `rm <archivo>` solo después de confirmación
5. **Agregar al staging**: `git rm <archivo>` para registrar la eliminación

## Restricciones:
- SIEMPRE mostrar qué se va a eliminar antes de hacerlo
- SIEMPRE pedir confirmación al usuario
- NUNCA eliminar archivos del sistema o configuración
- Recomendar usar `git rm` en lugar de `rm` para mantener historial

## Argumentos:
- `$ARGUMENTS`: Ruta del archivo a eliminar
