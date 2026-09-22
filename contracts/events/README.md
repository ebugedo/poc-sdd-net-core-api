# Contratos de Eventos

## Propósito
Este directorio contiene los contratos de eventos para comunicación asíncrona entre servicios.

## Archivos

### `events.yaml`
Definición de eventos del sistema:
- Eventos de dominio
- Eventos de integración
- Schema de cada evento

## Formato de Evento
```json
{
  "eventId": "uuid",
  "eventType": "EventType",
  "timestamp": "ISO8601",
  "data": {},
  "metadata": {}
}
```
