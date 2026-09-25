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
│  │  │  Docker: poc-sdd-api (8080)   │  nginx-net       │  │ │
│  │  │  ┌───────────────────────┐    │                  │  │ │
│  │  │  │  ASP.NET Core API     │────┼──host.docker.internal──┼──┐
│  │  │  │  Program + Startup    │◀───┼──nginx:80/443    │  │  │
│  │  │  └───────────────────────┘    │                  │  │  │
│  │  └───────────────────────────────┼──────────────────┘  │ │
│  │                                  │                     │ │
│  │  ┌───────────────────────────────┼──────────────────┐  │ │
│  │  │  Docker: postgres (5432)      │  (ya desplegado) │  │ │
│  │  │  PostgreSQL 16 - externo      ◀──────────────────┘  │ │
│  │  └──────────────────────────────────────────────────┘  │ │
│  │  nginx (host) ── proxy_pass http://poc-sdd-api:8080 (nginx-net) │ │
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

## Entorno del Servidor

- **OS**: Debian (VPS)
- **Proxy inverso**: nginx (Nginx Proxy Manager) en red `nginx-net`
- **Gestión**: Portainer
- **Runtime**: Docker + Docker Compose instalados, pero los contenedores **no se construyen en el VPS**, se extraen de `ghcr.io` (`docker pull`)

## Configuración Inicial del VPS (Debian)

### 1. Docker + Docker Compose (ya instalados)
```bash
# Debian - ya instalados en este VPS, referencia:
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh
sudo systemctl enable docker && sudo systemctl start docker
docker compose version  # verificado
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
> **Nota**: PostgreSQL ya está corriendo en un container independiente en el VPS (Debian). No se despliega ni se gestiona desde este repo.

- **Database**: `postgresql-db-ia-tests`
- **Usuario**: `timeforsoftware@gmail.com`
- **Password**: secreto `DB_PASSWORD` (GitHub Secrets, inyectado vía `ConnectionStrings__DefaultConnection` en `ci-cd.yml:110`)
- En producción no se usa `docker-compose`, solo `docker pull` de `ghcr.io` y `docker run` con `--network nginx-net`

```bash
# Verificar que PostgreSQL está corriendo
docker ps | grep postgres
# Debe mostrar: postgres  0.0.0.0:5432->5432/tcp

# Ver logs si es necesario
docker logs postgres

# La API se conecta vía host.docker.internal:5432 (ver --add-host en deploy)
```

> **Requisito**: la base `postgresql-db-ia-tests` y el rol `timeforsoftware@gmail.com` deben existir en el PostgreSQL del VPS. Crear como superusuario:
> ```bash
> docker exec -it <postgres-container> psql -U postgres -c "CREATE ROLE \"timeforsoftware@gmail.com\" LOGIN PASSWORD '***';"
> docker exec -it <postgres-container> psql -U postgres -c "CREATE DATABASE \"postgresql-db-ia-tests\" OWNER \"timeforsoftware@gmail.com\";"
> ```

### 4. Migraciones (tablas)
Las tablas se crean automáticamente en la primera ejecución vía `Startup.cs:54` `Database.Migrate()` (`src/Infrastructure/Migrations/20260923102117_InitialCreate.cs` crea `clients`).
- **Local**: `docker-compose up` usa `Host=postgres` con DB `postgresql-db-ia-tests` / password `postgres` (`src/Api/appsettings.json`)
- **Prod**: `Host=host.docker.internal` con DB `postgresql-db-ia-tests` / password `${{ secrets.DB_PASSWORD }}`

No es necesario `dotnet ef database update` manual en VPS.

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

### Producción (VPS) - Red nginx-net (puerto host no estándar)
```bash
# Ver contenedores y red
docker ps
docker network inspect nginx-net
# Verificar puerto 3010 no ocupado
sudo ss -tulpn | grep 3010 || echo "3010 libre"

# Ver logs
docker logs -f poc-sdd-api

# Reiniciar
docker restart poc-sdd-api

# Actualizar manualmente (PostgreSQL externo, red nginx-net, puerto host 3010)
docker pull ghcr.io/tu-usuario/poc-sdd-net-core-api:latest
docker stop poc-sdd-api
docker rm poc-sdd-api
docker network create nginx-net || true
docker run -d --name poc-sdd-api --restart unless-stopped --network nginx-net --add-host=host.docker.internal:host-gateway -p 3010:8080 -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=postgresql-db-ia-tests;Username=timeforsoftware@gmail.com;Password=xxx" ghcr.io/tu-usuario/poc-sdd-net-core-api:latest
# Acceso directo: http://VPS_IP:3010/api/v1/clients  (interno 8080)
# Vía nginx: https://pocsddnetcoreapi.timeforsoftware.com/api/v1/clients -> proxy_pass http://poc-sdd-api:8080
```

### Nginx Reverse Proxy (nginx-net)
```nginx
# /etc/nginx/sites-available/api
server {
    listen 80;
    server_name tu-dominio.com;
    location /api/ {
        proxy_pass http://poc-sdd-api:8080;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
    location /swagger/ { proxy_pass http://poc-sdd-api:8080; }
}
# Requiere: docker network create nginx-net (ya creado por deploy)
# nginx debe estar en nginx-net: docker network connect nginx-net nginx  # si nginx es container
# o si nginx es host, usa proxy_pass http://localhost:8080 y red no necesaria para host
```

## Variables de Entorno

### Producción (PostgreSQL externo)
```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Host=host.docker.internal;Port=5432;Database=postgresql-db-ia-tests;Username=timeforsoftware@gmail.com;Password=xxx
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
psql "host=host.docker.internal port=5432 dbname=postgresql-db-ia-tests user=timeforsoftware@gmail.com" -c "SELECT 1"

# Verificar que el mapping existe
docker inspect poc-sdd-api | grep host.docker.internal
```

### Puertos en conflicto
```bash
# Ver qué está usando el puerto
sudo lsof -i :8080
sudo lsof -i :5432
```
