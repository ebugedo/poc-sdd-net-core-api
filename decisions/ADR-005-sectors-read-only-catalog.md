# ADR-005: Sectores como Catálogo de Solo Lectura con FK

## Estado
[x] Aceptado

## Fecha
2026-09-25

## Contexto
Cada proyecto debe clasificarse en uno de estos 7 sectores: Administración pública, Ingeniería, Publicidad, Servicios financieros, Servicios tecnológicos, Transporte y Sector inmobiliario.

Faltaba decidir:
1. ¿Cómo se exponen los sectores? (CRUD vs. catálogo fijo)
2. ¿Cómo se relacionan con el proyecto? (FK a tabla, enum o texto)
3. ¿Cómo se añade `sector_id` obligatorio si ya hubiera proyectos?

## Decisión
1. **Catálogo de solo lectura**:
   - Tabla `sectors` con **7 filas sembradas** por la migración (`HasData`) y **GUIDs fijos** (`11111111-...` … `77777777-...`)
   - Solo dos endpoints: `GET /api/v1/sectors` y `GET /api/v1/sectors/{id}`
   - **No hay Commands, ni endpoints de escritura, ni repositorio de escritura**
   - Añadir o renombrar un sector en el futuro requiere una nueva migración

2. **Relación por FK**: `projects.sector_id UUID NOT NULL` → `sectors(id)`, con `ON DELETE RESTRICT` e índice `idx_projects_sector_id`:
   - El dominio valida `sectorId != Guid.Empty` (BR-014)
   - Los handlers de escritura verifican que el sector exista y lanzan `SectorNotFoundException` → 404 (BR-015)

3. **Migración en dos pasos dentro de la misma migración**: crear tabla `sectors` + `HasData`, después añadir `projects.sector_id` como `NOT NULL`. Se verificó que la BD no tenía proyectos, así que no hace falta backfill.

## Consecuencias
### Positivas
- [x] No se puede dejar un proyecto sin sector: la columna es `NOT NULL` y el dominio lo exige
- [x] No se pueden inventar ni borrar sectores: el catálogo es estable y los ids no cambian
- [x] Los ids fijos permiten documentarlos y usarlos en seeds y tests sin hardcodear cadenas
- [x] La integridad referencial impide sectores huérfanos (`RESTRICT`)
- [x] El índice por `sector_id` permite agrupar/contar por sector

### Negativas
- [x] Cambiar el catálogo (añadir un sector) exige desplegar una migración, no un endpoint
- [x] Añadir atributos al sector (descripción, icono) requiere otra migración
- [x] Cada proyecto ahora depende de dos tablas (`clients` + `sectors`): más joins y dos validaciones en los commands
- [x] Un sector nunca se puede eliminar aunque no esté en uso (no hay endpoint de borrado por diseño)

## Alternativas Consideradas
### Alternativa 1: CRUD completo de sectores
**Descripción**: `POST/PUT/DELETE /api/v1/sectors`.
**Pros**: el catálogo se podría gestionar en runtime sin redeploy.
**Contras**: permite borrar o renombrar sectores ya usados, deja el historial inconsistente y convierte un catálogo fijo en un CRUD más que mantener.

### Alternativa 2: Enum en el dominio (`public enum Sector { AdministracionPublica, ... }`)
**Descripción**: sin tabla; el endpoint proyecta el enum y `Project.Sector` es un enum mapeado a `int`/`varchar`.
**Pros**: cero migraciones, exhaustividad garantizada por el compilador, sin tabla extra.
**Contras**: cambiar el catálogo obliga a migrar y recompilar; no hay ids estables ni datos de negocio asociados; consultas por sector menos expresivas.

### Alternativa 3: Texto libre en `projects.sector`
**Descripción**: `projects.sector VARCHAR(50)` con el nombre del sector.
**Pros**: sin tabla ni FK; el nombre se lee directamente.
**Contras]: sin integridad (cualquier variante orthográfica), sin índice fiable, `@Size` fijo y sin forma de renombrar de forma segura.

### Alternativa 4: Migración con backfill
**Descripción**: añadir `sector_id` nullable, rellenar con un sector por defecto y pasar a `NOT NULL`.
**Pros**: segura si hubiera datos.
**Contras**: complejidad innecesaria; se verificó que la tabla `projects` estaba vacía.

## Notas Adicionales
- `Sector` es la única entidad sin Commands: es un catálogo, no un agregado editable.
- Los GUIDs fijos son deliberadamente "bonitos" (`1111...`, `2222...`) para poder reconocerlos en logs, seeds y documentación.
- Si en el futuro el catálogo pasa a ser gestionable, esta decisión se sustituye en un ADR nuevo (la tabla y la FK ya quedan prepared).
