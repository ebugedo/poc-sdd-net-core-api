# ADR-002: Despliegue con Docker y CI/CD

## Estado
[x] Aceptado

## Fecha
2024-01-XX

## Contexto
Se necesita definir la estrategia de despliegue para la aplicación considerando:
- Despliegue consistente entre desarrollo y producción
- Automatización del proceso de deploy
- Imágenes versionadas y rastreables
- PostgreSQL ya desplegado como contenedor en el VPS
- VPS como servidor de destino

## Decisión
Se implementa despliegue basado en contenedores Docker con CI/CD en GitHub Actions.

### Arquitectura

| Componente | Tecnología | Ubicación |
|------------|------------|-----------|
| **Container Runtime** | Docker | VPS |
| **Registry** | GitHub Container Registry (ghcr.io) | GitHub |
| **CI/CD** | GitHub Actions | GitHub |
| **Server** | VPS (Ubuntu/Debian) | On-premise |
| **Database** | PostgreSQL 16 Container | VPS (ya existente) |

### Flujo

```
Push to main → GitHub Actions → Build & Push → GHCR → Deploy to VPS
```

### Archivos

| Archivo | Propósito |
|---------|-----------|
| `Dockerfile` | Build multi-stage de la aplicación |
| `.dockerignore` | Archivos excluidos del build |
| `docker-compose.yml` | Desarrollo local con PostgreSQL |
| `.github/workflows/ci-cd.yml` | Pipeline CI/CD |
| `.env.example` | Variables de entorno de ejemplo |

## Consecuencias

### Positivas
- **Consistencia**: Mismo container en dev y prod
- **Versionado**: Cada push genera una imagen versionada
- **Automatización**: Deploy automático en push a main
- **Rollback**: Fácil volver a versión anterior
- **Registry centralizado**: ghcr.io integrado con GitHub

### Negativas
- **Complejidad inicial**: Configurar GitHub Secrets y VPS
- **Docker overhead**: Ligero consumo de recursos
- **Dependencia de GitHub**: Si GitHub cae, no hay deploy

## Configuración Requerida

### GitHub Secrets
```
VPS_HOST=ip-del-vps
VPS_USER=root
VPS_SSH_KEY=clave-ssh-privada
DB_PASSWORD=password-postgres
```

### VPS Requisitos
- Docker instalado
- PostgreSQL corriendo en contenedor
- Puerto 8080 abierto

## Alternativas Consideradas

### Alternativa 1: Kubernetes
**Pros**: Orquestación avanzada, auto-scaling
**Contras**: Complejidad excesiva para un VPS simple

### Alternativa 2: Docker Swarm
**Pros**: Más simple que K8s
**Contras**: Menos funcional, comunidad menor

### Alternativa 3: Deploy manual via SSH
**Pros**: Sin dependencia de GitHub Actions
**Contras**: Error-prone, sin versionado de imágenes

## Notas Adicionales
- PostgreSQL está desplegado por separado (no en docker-compose de la app)
- Se usa `--restart unless-stopped` para auto-reinicio
- Health check configurado en el Dockerfile
