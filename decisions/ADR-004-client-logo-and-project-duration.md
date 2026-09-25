# ADR-004: Logo de Cliente como URL y Duración de Proyecto en Meses

## Estado
[x] Aceptado

## Fecha
2026-09-25

## Contexto
El dominio crece con dos requisitos nuevos:
1. Cada cliente puede tener un **logo** (recurso gráfico).
2. Nueva entidad **Project** con: cliente, título, descripción, tecnologías (texto), fecha de inicio y duración (opcional).

Para el logo había que decidir **cómo se almacena**: fichero binario en el servidor vs. referencia externa.
Para la duración había que decidir **unidad y tipo**: texto libre, días o meses.

## Decisión
1. **Logo como `string?` (URL)**, no como fichero:
   - `clients.logo VARCHAR(2048) NULL`
   - Si se informa, debe ser una URL absoluta `http`/`https` (`Uri.TryCreate` con `UriKind.Absolute`)
   - Sin endpoint de upload, sin almacenamiento de ficheros, sin nuevo servicio

2. **Duración en meses como `int?`** (`projects.duration_months`):
   - `null` = proyecto en curso (duración abierta)
   - Si se informa, debe ser `> 0` (validado en el dominio)

3. La relación `Client 1:N Project` se modela con **FK `projects.client_id` + `ON DELETE RESTRICT`**: un cliente con proyectos no se puede eliminar (BR-013).

## Consecuencias
### Positivas
- [x] Cero infraestructura nueva para el logo (sin volumen, sin CDN, sin límites de tamaño)
- [x] El cliente puede usar cualquier hosting existente (S3, CDN, GitHub raw)
- [x] `int?` en meses es simple de consultar y comparar; permite calcular fecha fin si en el futuro se necesita
- [x] `null` en duración modela de forma natural el proyecto en curso
- [x] `RESTRICT` protege la integridad: no se pierde historial de proyectos

### Negativas
- [x] El logo no se valida como imagen: se puede guardar cualquier URL válida
- [x] Si la URL muere, el cliente queda sin logo (no hay copia local)
- [x] La granularidad en meses no cubre proyectos de días (se redondea a 1 mes)
- [x] `RESTRICT` produce un error 500 en vez de un 409 limpio al borrar un cliente con proyectos (manejo 409 pendiente)

## Alternativas Consideradas
### Alternativa 1: Upload de imagen (`IFormFile`)
**Descripción**: `POST /api/v1/clients/{id}/logo` que guarda el binario en disco y lo sirve como estático.
**Pros**: el sistema es dueño del recurso; validación de tipo y tamaño; no depende de terceros.
**Contras**: volumen Docker, limpieza, antivirus, límites de tamaño, URLs públicas, cambios en el contrato (multipart) y despliegue (nginx debe servir estáticos).

### Alternativa 2: Duración en texto libre (`string?`)
**Descripción**: `"3 meses"`, `"6 semanas"`.
**Pros**: máxima flexibilidad de expression.
**Contras**: no se puede validar, ordenar ni calcular; requiere parseo en cada lectura.

### Alternativa 3: Duración en días (`int?`)
**Descripción**: la misma semántica pero en días.
**Pros**: más granular; cálculo de fecha fin más preciso.
**Contras**: para proyectos de consultoría (meses) obliga a convertir mentalmente 180 días.

### Alternativa 4: `ON DELETE CASCADE` en `projects.client_id`
**Descripción**: al eliminar un cliente, sus proyectos se borran.
**Pros**: el `DELETE` de clientes nunca falla.
**Contras**: pérdida destructiva e involuntaria de historial; no se cumple la expectativa de "no se puede eliminar un cliente con proyectos".

## Notas Adicionales
- El filtro `GET /api/v1/projects?clientId=` se apoya en `idx_projects_client_id`.
- El `clientId` inexistente en `POST`/`PUT` de proyecto devuelve 404 (`ClientNotFoundException` reutilizado de ADR-003).
- Si más adelante se requieren varios logos, tipos de fichero o tamaño máximo, esta decisión se revisa en un ADR nuevo (el campo `string?` sigue siendo compatible como URL de fallback).
