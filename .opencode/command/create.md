---
description: Crea un archivo nuevo con confirmación del usuario
agent: build
---

# Crear Archivo Nuevo

Este comando crea un archivo nuevo en el proyecto.

## Flujo:

1. **Verificar ubicación**: Confirmar que la ruta es correcta
2. **Verificar contenido**: Preguntar si quiere contenido inicial o vacío
3. **Solicitar confirmación**: Preguntar al usuario confirma creación
4. **Crear archivo**: Usar herramienta de escritura solo después de confirmación

## Restricciones:
- SIEMPRE mostrar la ruta completa antes de crear
- SIEMPRE pedir confirmación al usuario
- Verificar que no existe un archivo con ese nombre
- Sugerir ubicación correcta según la arquitectura del proyecto

## Argumentos:
- `$ARGUMENTS`: Ruta del archivo a crear
