---
description: Crea un commit y push de cambios existentes creando una rama y un PR
agent: build
---

# Crear Commit y PR

Este comando realiza el siguiente flujo para subir cambios al repositorio:

## Flujo (en este orden):

1. **Verificar estado**: `git status` para ver cambios pendientes
2. **Crear rama**: `git checkout -b feature/descripcion-corta` (solicitando nombre al usuario)
3. **Agregar cambios**: `git add .` o archivos específicos (solicitando confirmación)
4. **Crear commit**: `git commit -m "mensaje descriptivo"` (solicitando mensaje al usuario)
5. **Push rama**: `git push origin feature/descripcion-corta`
6. **Crear PR**: `gh pr create` con título y descripción (solicitando confirmación)

## Restricciones:
- NUNCA hacer push directamente a main
- SIEMPRE crear una rama nueva
- SIEMPRE crear un PR para revisión
- El usuario aprueba y mergea el PR manualmente

## Argumentos:
- `$ARGUMENTS`: Descripción de los cambios a subir

## Ejecución:
Preguntar al usuario:
1. Nombre de la rama (o sugerir uno basado en la descripción)
2. Mensaje del commit
3. Confirmación antes de cada paso de escritura
