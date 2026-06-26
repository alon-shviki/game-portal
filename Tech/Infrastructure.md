# Infrastructure & Deployment

## Local Dev

Each game runs independently. No portal shell yet — just run the game directly:

```bash
# Bullet Heaven
cd ~/Desktop/Bullet-Heaven/BulletHeaven.Client && dotnet run
# → http://localhost:5292

# Full stack (game + postgres)
cd ~/Desktop/Bullet-Heaven && docker compose up --build
# → http://localhost:8080
```

---

## Production Architecture (Target)

```
Internet
    ↓
  nginx  (TLS termination, reverse proxy)
    ├── /                    → Portal shell (static HTML or Blazor WASM)
    ├── /api/auth/*          → Portal Auth Server  (ASP.NET Core)
    ├── /bullet-heaven/*     → BulletHeaven.Client (WASM static files)
    └── /api/bullet-heaven/* → BulletHeaven.Server (ASP.NET Core API)
```

All services run as Docker containers, orchestrated by a single `docker-compose.yml` at the portal level.

---

## Docker Compose (Portal Level)

```yaml
# ~/Desktop/game/docker-compose.yml  (planned)
services:
  nginx:
    image: nginx:alpine
    ports: ["80:80", "443:443"]
    volumes: [./nginx.conf:/etc/nginx/nginx.conf]

  portal-auth:
    build: ./portal-auth
    environment:
      - JWT_KEY=${JWT_KEY}
      - DB_CONNECTION=${PORTAL_DB}

  bullet-heaven-server:
    build: ../Bullet-Heaven/BulletHeaven.Server
    environment:
      - JWT_KEY=${JWT_KEY}          # same key as portal-auth
      - DB_CONNECTION=${BH_DB}

  postgres:
    image: postgres:16
    volumes: [pgdata:/var/lib/postgresql/data]
    environment:
      - POSTGRES_PASSWORD=${POSTGRES_PASSWORD}

volumes:
  pgdata:
```

Key point: `JWT_KEY` is the **same value** in both `portal-auth` and `bullet-heaven-server`, so tokens issued by the portal are accepted by the game server without a round-trip.

---

## nginx Routing

```nginx
# Route game API calls
location /api/bullet-heaven/ {
    proxy_pass http://bullet-heaven-server:5000/api/;
}

# Route auth calls
location /api/auth/ {
    proxy_pass http://portal-auth:5001/api/;
}

# Serve portal static files
location / {
    root /usr/share/nginx/html;
    try_files $uri $uri/ /index.html;
}
```

---

## Environment Variables

| Variable | Where Used | Description |
|----------|-----------|-------------|
| `JWT_KEY` | All servers | Shared JWT signing secret |
| `PORTAL_DB` | portal-auth | Portal user DB connection string |
| `BH_DB` | bullet-heaven-server | Bullet Heaven game DB connection string |
| `POSTGRES_PASSWORD` | postgres | DB root password |

All secrets via `.env` file (gitignored) or a secrets manager — never hardcoded.

---

## Current State

- Bullet Heaven has its **own** `docker-compose.yml` in `~/Desktop/Bullet-Heaven/`
- Portal-level compose doesn't exist yet
- When portal auth is built, Bullet Heaven's auth will be migrated out of its own server
