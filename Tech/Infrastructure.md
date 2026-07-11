# Infrastructure & Deployment

## Local Dev

Everything runs from the portal compose. One command starts the whole stack:

```bash
cd ~/Desktop/game && docker compose up
# Portal shell  → http://localhost:3000
# Bullet Heaven → http://localhost:8080
# Orbit Break   → http://localhost:8081
```

Game client only (no auth/scores — for pure game dev):
```bash
cd ~/Desktop/Bullet-Heaven/BulletHeaven.Client && dotnet run   # → http://localhost:5292
cd ~/Desktop/orbit-break/OrbitBreak.Client && dotnet run        # Orbit Break
```

---

## Current Architecture

```
docker-compose.yml  (~/Desktop/game/)
  ├── nginx                :3000  → portal shell (static) + /api/auth/ → portal-auth
  ├── portal-auth          :5001  → ASP.NET Core — auth, scores, leaderboard
  ├── postgres             :5432  → portal DB (users + scores)
  ├── bh-client            :8080  → GHCR image (Blazor WASM + nginx)
  │                                  nginx proxies /api/scores, /api/leaderboard → portal-auth
  └── orbit-break-client   :8081  → GHCR image (Blazor WASM + nginx), same proxy contract
```

Games have **no standalone API server**. Auth, scores and leaderboard live entirely in `portal-auth`.

---

## Docker Compose

```yaml
# ~/Desktop/game/docker-compose.yml
services:
  nginx:                # portal — port 3000
  portal-auth:          # ASP.NET Core minimal API — port 5001 (built locally via `build: ./portal-auth`)
  postgres:             # portal DB
  bh-client:            # image: ghcr.io/alon-shviki/bh-client:latest — port 8080
  orbit-break-client:   # image: ghcr.io/alon-shviki/orbit-break-client:latest — port 8081
```

See the actual file for full env var wiring. Port 80 is taken by `idp-control-plane` on the dev machine, so portal runs on 3000.

---

## nginx Routing (Portal, port 3000)

```nginx
location /api/auth/   → portal-auth:5001/api/     (auth, scores, leaderboard)
location /            → /usr/share/nginx/html       (portal shell static files)
```

BH's internal nginx (port 8080):
```nginx
location = /api/scores        → portal-auth:5001/api/scores/bullet-heaven
location = /api/scores/me     → portal-auth:5001/api/leaderboard/bullet-heaven/me
location /api/leaderboard     → portal-auth:5001/api/leaderboard/bullet-heaven
location /                    → Blazor WASM static files
```

---

## Environment Variables

| Variable | Where Used | Description |
|----------|-----------|-------------|
| `JWT_KEY` | portal-auth | JWT signing secret (shared with BH via same server) |
| `POSTGRES_PASSWORD` | postgres, portal-auth | DB password |

All secrets in `.env` (gitignored). Copy `.env.example` on a fresh clone.

---

## CI / Docker Images

| Image | Built & pushed by | Consumed by compose |
|-------|-------------------|---------------------|
| `ghcr.io/alon-shviki/bh-client` | BH `docker.yml`, merge to main | pulled |
| `ghcr.io/alon-shviki/orbit-break-client` | OB `docker.yml`, merge to main | pulled |
| `ghcr.io/alon-shviki/portal-auth` | portal `ci.yml`, merge to main | **built locally** (`build: ./portal-auth`) — the pushed image isn't pulled by compose today |

Every workflow's `build` gate runs cache → format → build → test before the image is pushed — see [[Tech/CI and Branch Protection]].

To deploy a game change: push to `main` → CI builds + pushes the new image → `docker compose pull <service> && docker compose up -d`.
