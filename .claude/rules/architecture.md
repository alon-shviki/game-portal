# Portal Architecture

## Target Production Layout

```
nginx  (TLS termination, reverse proxy)
  /                     → portal shell (static HTML or Blazor WASM)
  /api/auth/*           → portal auth server  (ASP.NET Core)
  /<game-name>/*        → game client         (static WASM or HTML)
  /api/<game-name>/*    → game API server     (ASP.NET Core)
```

All services in one top-level `docker-compose.yml` (not yet created — Bullet Heaven still has its own standalone compose for dev).

## Shared Auth

One JWT signing key across every server. The portal auth server issues tokens; game servers validate them with the same key — no cross-service round-trip needed.

```
Player logs in at portal
  → POST /api/auth/login → { token }
  → stored in localStorage

Player opens a game
  → game client reads token from localStorage
  → game API calls: Authorization: Bearer <token>
  → game server validates using shared JWT_KEY
```

JWT claims: `sub` (user ID), `username`, `iat`, `exp` (24h).

Game servers expose no `/register` or `/login`. They only accept and validate tokens.

## Environment Variables

| Variable | Where | Description |
|----------|-------|-------------|
| `JWT_KEY` | all servers | Shared JWT signing secret |
| `PORTAL_DB` | portal auth | Portal user DB connection string |
| `<GAME>_DB` | game server | Game-specific DB connection string |
| `POSTGRES_PASSWORD` | postgres | DB root password |

All via `.env` (gitignored) or a secrets manager — never hardcoded.

## Per-Game Stack Contract

Every game server must expose:

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/scores` | Bearer | Save a run score |
| GET | `/api/leaderboard` | Public | Top 10 `{ username, score }` |
| GET | `/health` | Public | Liveness check |

The portal aggregates leaderboard data and displays personal bests on game cards.

## Current State

- Portal shell: not built yet
- Portal auth server: not built yet — Bullet Heaven has standalone auth for now
- Portal docker-compose: not built yet
- Bullet Heaven: fully functional standalone at `~/Desktop/Bullet-Heaven`
