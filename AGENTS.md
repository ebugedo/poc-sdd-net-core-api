# AGENTS.md - Reglas y Restricciones del Proyecto

## Restricciones Generales

### 1. Confirmación Obligatoria para Escritura
**REGLA ABSOLUTA**: Cualquier operación de escritura REQUIERE confirmación explícita del usuario antes de ejecutarse.

Esto incluye:
- **Crear archivos**: Solicitar confirmación con ruta completa
- **Modificar archivos**: Mostrar diff antes de aplicar cambios
- **Eliminar archivos**: Listar archivos a eliminar y confirmar
- **Ejecutar comandos git**: Confirmar cada operación (add, commit, push, etc.)

### 2. Control Total del Repo - Sin Acciones sin Autorización
**REGLA ABSOLUTA**: NINGUNA acción que modifique el repo se ejecuta sin autorización explícita del usuario.

**Prohibido sin confirmación explícita**:
- ❌ `git add` (agregar archivos al staging)
- ❌ `git commit` (crear commits)
- ❌ `git push` (subir cambios al remoto)
- ❌ `gh pr create` (crear pull requests)
- ❌ `gh pr merge` (mergear pull requests)
- ❌ `git checkout -b` (crear ramas)
- ❌ Cualquier comando git que modifique el estado del repo

**Flujo obligatorio**:
```
1. Agentepropone acción → Muestra qué va a hacer
2. Usuario autoriza → "sí", "ok", "confirmo", etc.
3. Agente ejecuta → Solo después de la autorización
4. Agente reporta → Qué se hizo exitosamente
```

**Ejemplo de interacción**:
```
Agente: "Voy a crear un commit con los siguientes cambios:
         - Archivo: src/Api/Controllers/ResourceController.cs
         - Mensaje: feat: add resource controller
         ¿Autorizas el commit?"

Usuario: "sí"

Agente: Ejecuta git add + git commit
```

### 3. Flujo de Git - NUNCA escribir directo a main

```
❌ PROHIBIDO:  git push origin main
✅ CORRECTO:   git checkout -b feature/descripcion
               git add .
               git commit -m "mensaje"
               git push origin feature/descripcion
               gh pr create
```

**Flujo obligatorio** (cada paso requiere autorización):
1. **Crear rama**: Preguntar nombre de rama → Esperar confirmación → `git checkout -b`
2. **Agregar cambios**: Mostrar archivos → Esperar confirmación → `git add`
3. **Crear commit**: Mostrar mensaje → Esperar confirmación → `git commit`
4. **Push**: Mostrar rama destino → Esperar confirmación → `git push`
5. **Crear PR**: Mostrar título/descripción → Esperar confirmación → `gh pr create`
6. **Merge**: Usuario aprueba y mergea manualmente en GitHub

### 3.1 Inmutabilidad de PR - PROHIBIDO modificar PR existente
**REGLA ABSOLUTA**: Una vez creada una PR, es inmutable. NUNCA se hace push adicional a esa rama ni se modifica su código.

**Prohibido sin excepción**:
- ❌ `git push origin fix/rama-existente` cuando ya existe PR para esa rama
- ❌ `git commit --amend` + push a rama con PR
- ❌ `gh pr edit` para modificar código (solo título/descripción permitido, nunca código)
- ❌ Cualquier push que altere una PR abierta o cerrada

**Flujo obligatorio para cualquier corrección post-PR**:
```
1. Dejar PR anterior intacta (no tocar)
2. git checkout main && git pull
3. git checkout -b fix/descripcion-v2  # NUEVA rama siempre
4. Re-aplicar cambios + correcciones
5. git add / git commit / git push (con autorización paso a paso)
6. gh pr create (NUEVA PR, referencia a PR anterior en descripción)
```

**Verificación antes de push**:
```bash
gh pr list --head <nombre-rama>  # Si devuelve PR, NO hacer push, crear nueva rama
```

### 4. Permisos por Herramienta

| Herramienta | Permiso | Acción |
|-------------|---------|--------|
| `read` | ✅ Allow | Lectura libre |
| `glob` | ✅ Allow | Búsqueda libre |
| `grep` | ✅ Allow | Búsqueda libre |
| `list` | ✅ Allow | Listado libre |
| `edit` | ⚠️ Ask | Confirmar cada edición |
| `bash` | ⚠️ Ask | Confirmar cada comando |
| `write` | ⚠️ Ask | Confirmar cada escritura |
| `git operations` | 🔒 Deny by default | Requiere autorización explícita |

### 5. Comandos Disponibles

Usar estos comandos para operaciones controladas:

| Comando | Uso |
|---------|-----|
| `/commit-pr` | Crear commit y PR (flujo completo) |
| `/add` | Agregar archivos al staging |
| `/create` | Crear archivo nuevo |
| `/delete` | Eliminar archivo con confirmación |

### 6. Reglas Específicas por Tipo de Operación

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

### 7. Archivos Sensibles - NUNCA tocar

- `.env` o `.env.*`
- `*password*`, `*secret*`, `*key*`
- `appsettings.Development.json` (solo mostrar, no editar)
- Credenciales de任何 tipo

### 8. Naming de Ramas

```
feature/descripcionbreve    # Nueva funcionalidad
fix/descripcionbreve        # Corrección de bug
chore/descripcionbreve      # Mantenimiento
docs/descripcionbreve       # Documentación
refactor/descripcionbreve   # Refactorización
```

### 9. Mensajes de Commit

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

### 10. Pull Requests

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

## Regla: Actualizar Documentación al Generar Código

**REGLA OBLIGATORIA**: Siempre que se genere o modifique código, se debe actualizar la documentación de SDD correspondiente.

### Qué documentar al escribir código

| Tipo de Código | Documentación a Actualizar |
|----------------|---------------------------|
| **Nueva entidad** | `specs/domain/domain-model.md`, `design/data-model/data-model.md` |
| **Nuevo endpoint** | `specs/api/api-spec.md`, `contracts/api/openapi.yaml` |
| **Nueva regla de negocio** | `specs/domain/business-rules.md` |
| **Nuevo flujo** | `specs/features/user-flows.md`, `design/flows/system-flows.md` |
| **Nueva feature** | `specs/features/user-stories.md` |
| **Cambio de arquitectura** | `design/architecture/architecture-overview.md`, `decisions/` |
| **Nuevo comando bash** | `AGENTS.md` sección de comandos |

### Flujo obligatorio al implementar

```
1. Actualizar specs/     → Reflejar QUÉ se implementa
2. Actualizar design/    → Reflejar CÓMO se implementa
3. Actualizar contracts/ → Reflejar contratos formales
4. Actualizar src/       → Escribir código
5. Actualizar tests/     → Escribir pruebas
6. Confirmar documentación antes de commitear
```

### Checklist antes de cada commit

- [ ] ¿Se creó/modificó una entidad? → Actualizar `specs/domain/domain-model.md`
- [ ] ¿Se agregó un endpoint? → Actualizar `specs/api/api-spec.md`
- [ ] ¿Se cambió el modelo de datos? → Actualizar `design/data-model/data-model.md`
- [ ] ¿Se tomó una decisión técnica? → Crear ADR en `decisions/`
- [ ] ¿Se modificó la arquitectura? → Actualizar `design/architecture/`

## Recordatorios

- **SIEMPRE** preguntar antes de escribir
- **NUNCA** asumir que está bien modificar
- **MOSTRAR** qué se va a hacer antes de hacerlo
- **CONFIRMAR** cada operación de escritura
- **CREAR PR** para todo cambio que suba al repo
