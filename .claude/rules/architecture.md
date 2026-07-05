# Portal Architecture

## Target Production Layout

```
nginx  (TLS termination, reverse proxy)
  /                     → portal shell (static HTML or Blazor WASM)
  /api/auth/*           → portal auth server  (ASP.NET Core)
  /<game-name>/*        → game client         (static WASM or HTML)
```

All services in one top-level `docker-compose.yml`.

## Shared Auth

One JWT signing key across every server. The portal auth server issues tokens; game clients read the token from `localStorage["jwt"]` and send it as `Authorization: Bearer` on every API call.

```
Player logs in at portal
  → POST /api/auth/login → { token }
  → stored in localStorage["jwt"]

Player opens a game
  → game client reads localStorage["jwt"]
  → proxies API calls through game nginx → portal auth server
  → portal auth server validates JWT, records scores
```

JWT claims: `sub` (user ID), `unique_name` (username), `exp` (30 days — long-lived so a docker restart doesn't force re-login).

Game clients expose **no** `/register` or `/login`. Auth is portal-only.

## Portal Auth Server — Endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/register` | — | Create portal account |
| POST | `/api/login` | — | Login, returns JWT |
| GET | `/api/me` | Bearer | Who am I? |
| POST | `/api/scores/{game}` | Bearer | Submit a score |
| GET | `/api/leaderboard/{game}` | — | Top 10 for a game |
| GET | `/api/leaderboard/{game}/me` | Bearer | User's personal best (top 5) |
| GET | `/health` | — | Liveness |

## Per-Game Stack Contract

Every game client's nginx must proxy these paths to `portal-auth:5001`:

| Game path | Proxies to |
|-----------|-----------|
| `= /api/scores` | `/api/scores/{game-slug}` |
| `= /api/scores/me` | `/api/leaderboard/{game-slug}/me` |
| `/api/leaderboard` | `/api/leaderboard/{game-slug}` |

Games have **no** API server of their own. No game-specific DB.

## Environment Variables

| Variable | Where | Description |
|----------|-------|-------------|
| `JWT_KEY` | portal auth | JWT signing secret |
| `PORTAL_DB` / `ConnectionStrings__Default` | portal auth | Portal DB connection |
| `POSTGRES_PASSWORD` | postgres | DB root password |

All via `.env` (gitignored) — never hardcoded.

## Current State

- Portal shell: built (`shell/index.html`) — auth + leaderboard page
- Portal auth server: built (`portal-auth/`) — auth + scores + leaderboard endpoints
- Docker Compose: `docker-compose.yml` — portal nginx (3000), portal-auth, postgres, bh-client (8080), orbit-break-client (8081)
- Bullet Heaven: game client image on GHCR (`ghcr.io/alon-shviki/bh-client`) — nginx proxies scores/leaderboard to portal-auth; no standalone API server
- Orbit Break: game client image on GHCR (`ghcr.io/alon-shviki/orbit-break-client`) — Blazor WASM, same client-only contract (slug `orbit-break`)
