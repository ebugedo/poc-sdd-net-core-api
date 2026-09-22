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
│  │  ┌───────────────────────────────┼──────────────────┐  │ │
│  │  │  Docker: poc-sdd-api (8080)   │                  │  │ │
│  │  │  ┌───────────────────────┐    │                  │  │ │
│  │  │  │  ASP.NET Core API     │────┼──host.docker.internal──┼──┐
│  │  │  │  Program + Startup    │    │  :5432           │  │  │
│  │  │  └───────────────────────┘    │                  │  │  │
│  │  └───────────────────────────────┼──────────────────┘  │ │
│  │                                  │                     │ │
│  │  ┌───────────────────────────────┼──────────────────┐  │ │
│  │  │  Docker: postgres (5432)      │  (ya desplegado) │  │ │
│  │  │  PostgreSQL 16 - externo      ◀──────────────────┘  │ │
│  │  └──────────────────────────────────────────────────┘  │ │
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

### 3. PostgreSQL en VPS (ya desplegado)
> **Nota**: PostgreSQL ya está corriendo en un container independiente en el VPS. No se despliega ni se gestiona desde este repo.

```bash
# Verificar que PostgreSQL está corriendo
docker ps | grep postgres
# Debe mostrar: postgres  0.0.0.0:5432->5432/tcp

# Ver logs si es necesario
docker logs postgres

# La API se conecta vía host.docker.internal:5432 (ver --add-host en deploy)
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

# Actualizar manualmente (PostgreSQL externo)
docker pull ghcr.io/tu-usuario/poc-sdd-net-core-api:latest
docker stop poc-sdd-api
docker rm poc-sdd-api
docker run -d --name poc-sdd-api --restart unless-stopped --add-host=host.docker.internal:host-gateway -p 8080:8080 -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=poc_sdd;Username=postgres;Password=xxx" ghcr.io/tu-usuario/poc-sdd-net-core-api:latest
```

## Variables de Entorno

### Producción (PostgreSQL externo)
```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Host=host.docker.internal;Port=5432;Database=poc_sdd;Username=postgres;Password=xxx
# Requiere --add-host=host.docker.internal:host-gateway en docker run
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

### No conecta a PostgreSQL (container externo)
```bash
# Verificar que PostgreSQL está corriendo
docker ps | grep postgres

# Probar conexión desde la API hacia host.docker.internal
docker exec -it poc-sdd-api bash
apt-get update && apt-get install -y postgresql-client
psql "host=host.docker.internal port=5432 dbname=poc_sdd user=postgres" -c "SELECT 1"

# Verificar que el mapping existe
docker inspect poc-sdd-api | grep host.docker.internal
```

### Puertos en conflicto
```bash
# Ver qué está usando el puerto
sudo lsof -i :8080
sudo lsof -i :5432
```
