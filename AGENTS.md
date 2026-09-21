# AGENTS.md - Reglas y Restricciones del Proyecto

## Restricciones Generales

### 1. Confirmación Obligatoria para Escritura
**REGLA ABSOLUTA**: Cualquier operación de escritura REQUIERE confirmación explícita del usuario antes de ejecutarse.

Esto incluye:
- **Crear archivos**: Solicitar confirmación con ruta completa
- **Modificar archivos**: Mostrar diff antes de aplicar cambios
- **Eliminar archivos**: Listar archivos a eliminar y confirmar
- **Ejecutar comandos git**: Confirmar cada operación (add, commit, push, etc.)

### 2. Flujo de Git - NUNCA escribir directo a main

```
❌ PROHIBIDO:  git push origin main
✅ CORRECTO:   git checkout -b feature/descripcion
               git add .
               git commit -m "mensaje"
               git push origin feature/descripcion
               gh pr create
```

**Flujo obligatorio**:
1. Crear rama nueva (naming: `feature/`, `fix/`, `chore/`)
2. Agregar cambios (con confirmación)
3. Crear commit (con mensaje descriptivo)
4. Push a la rama
5. Crear Pull Request
6. Usuario aprueba y mergea manualmente

### 3. Permisos por Herramienta

| Herramienta | Permiso | Acción |
|-------------|---------|--------|
| `read` | ✅ Allow | Lectura libre |
| `glob` | ✅ Allow | Búsqueda libre |
| `grep` | ✅ Allow | Búsqueda libre |
| `list` | ✅ Allow | Listado libre |
| `edit` | ⚠️ Ask | Confirmar cada edición |
| `bash` | ⚠️ Ask | Confirmar cada comando |
| `write` | ⚠️ Ask | Confirmar cada escritura |

### 4. Comandos Disponibles

Usar estos comandos para operaciones controladas:

| Comando | Uso |
|---------|-----|
| `/commit-pr` | Crear commit y PR (flujo completo) |
| `/add` | Agregar archivos al staging |
| `/create` | Crear archivo nuevo |
| `/delete` | Eliminar archivo con confirmación |

### 5. Reglas Específicas por Tipo de Operación

#### Crear Archivo
```
1. Verificar que no existe
2. Mostrar ruta completa
3. Preguntar contenido (vacío/inicial)
4. Confirmar creación
5. Crear archivo
```

#### Modificar Archivo
```
1. Leer archivo actual
2. Mostrar diff propuesto
3. Confirmar cambios
4. Aplicar edición
```

#### Eliminar Archivo
```
1. Listar archivos a eliminar
2. Verificar que no son críticos
3. Confirmar eliminación
4. Ejecutar rm + git rm
```

#### Comandos Git
```
1. git status (siempre primero)
2. Mostrar qué se va a hacer
3. Confirmar operación
4. Ejecutar comando
```

### 6. Archivos Sensibles - NUNCA tocar

- `.env` o `.env.*`
- `*password*`, `*secret*`, `*key*`
- `appsettings.Development.json` (solo mostrar, no editar)
- Credenciales de任何 tipo

### 7. Naming de Ramas

```
feature/descripcionbreve    # Nueva funcionalidad
fix/descripcionbreve        # Corrección de bug
chore/descripcionbreve      # Mantenimiento
docs/descripcionbreve       # Documentación
refactor/descripcionbreve   # Refactorización
```

### 8. Mensajes de Commit

Formato:
```
tipo: descripción corta

tipo(icono): descripción

Tipos: feat, fix, chore, docs, refactor, test, style
```

Ejemplos:
```
feat: agregar endpoint de recursos
fix: corregir validación de nombre
chore: actualizar dependencias
docs: agregar guía de despliegue
```

### 9. Pull Requests

Template mínimo:
```markdown
## Descripción
[Qué hace este PR]

## Cambios
- [Lista de cambios]

## Testing
- [ ] Pruebas unitarias pasan
- [ ] No hay errores de compilación

## Notas
- [Cualquier nota relevante]
```

## Flujo de Desarrollo Recomendado

```
1. Especificar (specs/)     → Definir QUÉ
2. Diseñar (design/)        → Definir CÓMO
3. Implementar (src/)       → Escribir código
4. Testear (tests/)         → Verificar
5. Subir (/commit-pr)       → Crear PR
6. Revisar                  → Aprobar y mergear
```

## Recordatorios

- **SIEMPRE** preguntar antes de escribir
- **NUNCA** asumir que está bien modificar
- **MOSTRAR** qué se va a hacer antes de hacerlo
- **CONFIRMAR** cada operación de escritura
- **CREAR PR** para todo cambio que suba al repo
