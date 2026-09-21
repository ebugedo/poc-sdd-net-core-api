# Guía de Despliegue

## Arquitectura de Despliegue

```
┌─────────────────────────────────────────────────────────────┐
│                        GitHub                               │
│  ┌─────────────┐    ┌─────────────────────────────────────┐ │
│  │   Repo      │    │      GitHub Container Registry      │ │
│  │  (source)   │───▶│         (ghcr.io)                   │ │
│  └─────────────┘    └──────────────┬──────────────────────┘ │
│         │                          │                        │
│         │ GitHub Actions           │ Docker Pull            │
│         ▼                          ▼                        │
└─────────────────────────────────────────────────────────────┘
                                      │
                                      │
┌─────────────────────────────────────┼───────────────────────┐
│                    VPS (Producción) │                        │
│  ┌──────────────────────────────────┼─────────────────────┐ │
│  │                                  ▼                     │ │
│  │  ┌─────────────────────────────────────────────────┐  │ │
│  │  │              Docker Container                   │  │ │
│  │  │  ┌───────────────────────────────────────────┐  │  │ │
│  │  │  │           ASP.NET Core API                │  │  │ │
│  │  │  │              (8080)                       │  │  │ │
│  │  │  └────────────────────┬──────────────────────┘  │  │ │
│  │  │                       │                         │  │ │
│  │  │                       ▼                         │  │ │
│  │  │  ┌───────────────────────────────────────────┐  │  │ │
│  │  │  │      PostgreSQL Container (5432)          │  │  │ │
│  │  │  │              (ya desplegado)              │  │  │ │
│  │  │  └───────────────────────────────────────────┘  │  │ │
│  │  └─────────────────────────────────────────────────┘  │ │
│  └───────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## Flujo CI/CD

```
1. Push a main
   ↓
2. GitHub Actions ejecuta:
   ├── Build & Test (.NET)
   ├── Build & Push Docker Image → GHCR
   └── Deploy via SSH → VPS
   ↓
3. VPS ejecuta:
   ├── docker pull ghcr.io/...
   ├── docker stop旧容器
   └── docker run新容器
```

## Configuración Inicial del VPS

### 1. Instalar Docker
```bash
# Ubuntu/Debian
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh

# Habilitar Docker
sudo systemctl enable docker
sudo systemctl start docker
```

### 2. Configurar GitHub Secrets
En tu repo de GitHub → Settings → Secrets and variables → Actions:

| Secret | Descripción |
|--------|-------------|
| `VPS_HOST` | IP del VPS |
| `VPS_USER` | Usuario SSH (ej: root) |
| `VPS_SSH_KEY` | Clave SSH privada |
| `DB_PASSWORD` | Password de PostgreSQL |

### 3. Verificar PostgreSQL en VPS
```bash
# Verificar que PostgreSQL está corriendo
docker ps | grep postgres

# Si no está corriendo, iniciarlo:
docker run -d \
  --name postgres \
  --restart unless-stopped \
  -p 5432:5432 \
  -e POSTGRES_DB=poc_sdd \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=tu-password \
  -v postgres_data:/var/lib/postgresql/data \
  postgres:16-alpine
```

## Comandos Útiles

### Desarrollo Local
```bash
# Ejecutar con docker-compose (desarrollo)
docker-compose up -d

# Ver logs
docker-compose logs -f api

# Detener
docker-compose down
```

### Producción (VPS)
```bash
# Ver contenedores
docker ps

# Ver logs
docker logs -f poc-sdd-api

# Reiniciar
docker restart poc-sdd-api

# Actualizar manualmente
docker pull ghcr.io/tu-usuario/poc-sdd-net-core-api:latest
docker stop poc-sdd-api
docker rm poc-sdd-api
docker run -d --name poc-sdd-api --restart unless-stopped -p 8080:8080 ghcr.io/tu-usuario/poc-sdd-net-core-api:latest
```

## Variables de Entorno

### Producción
```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=poc_sdd;Username=postgres;Password=xxx
```

## Health Check

La API expone un endpoint de health check en:
```
GET http://localhost:8080/health
```

Respuesta esperada:
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0012345"
}
```

## Troubleshooting

### El contenedor no inicia
```bash
docker logs poc-sdd-api
```

### No conecta a PostgreSQL
```bash
# Verificar que PostgreSQL está corriendo
docker ps | grep postgres

# Probar conexión
docker exec -it poc-sdd-api bash
curl http://localhost:5432
```

### Puertos en conflicto
```bash
# Ver qué está usando el puerto
sudo lsof -i :8080
sudo lsof -i :5432
```
