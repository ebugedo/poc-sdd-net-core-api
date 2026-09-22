---
description: Agrega un archivo o conjunto de archivos al staging con confirmación
agent: build
---

# Agregar Archivos al Staging

Este comando agrega archivos al área de staging de git de forma controlada.

## Flujo:

1. **Verificar estado**: `git status` para ver qué archivos están modificados
2. **Mostrar cambios**: `git diff --name-only` o `git status --short`
3. **Solicitar confirmación**: Preguntar al usuario qué archivos agregar
4. **Ejecutar add**: `git add <archivos>` solo después de confirmación

## Restricciones:
- SIEMPRE mostrar qué se va a agregar antes de hacerlo
- SIEMPRE pedir confirmación al usuario
- NUNCA agregar archivos sensibles (.env, secrets, etc.)

## Argumentos:
- `$ARGUMENTS`: Patrón de archivos o "all" para todos
