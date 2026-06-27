# Portal Architecture

## System Layout

```
nginx  (TLS termination, reverse proxy)
  /                  → portal shell  (static HTML)
  /api/auth/*        → portal-auth   (ASP.NET Core)
  /bh/*              → bh-client     (Blazor WASM + nginx)
  /<game-name>/*     → future games
```

All services in one `docker-compose.yml`. See [[Tech/Infrastructure]] for the compose setup.

## Auth Flow

One JWT signing key shared across every service. The portal issues tokens; games validate them. See [[Design/Shared Auth]] for the full design.

```
Player logs in at portal
  → POST /api/auth/login → { token }
  → stored in localStorage["jwt"]

Player opens a game
  → game reads localStorage["jwt"]
  → sends Authorization: Bearer <token> on API calls
  → game nginx proxies to portal-auth:5001
  → portal-auth validates + records score
```

JWT claims: `sub` (user ID), `unique_name` (username), `exp` (24h).

## Portal Auth Endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/register` | — | Create account |
| POST | `/api/login` | — | Returns JWT |
| GET | `/api/me` | Bearer | Current user |
| POST | `/api/scores/{game}` | Bearer | Submit score |
| GET | `/api/leaderboard/{game}` | — | Top 10 |
| GET | `/api/leaderboard/{game}/me` | Bearer | Personal best (top 5) |
| GET | `/health` | — | Liveness |

## Per-Game nginx Contract

Every game nginx must proxy these paths to `portal-auth:5001`:

| Game path | Proxies to |
|-----------|-----------|
| `= /api/scores` | `/api/scores/{game-slug}` |
| `= /api/scores/me` | `/api/leaderboard/{game-slug}/me` |
| `/api/leaderboard` | `/api/leaderboard/{game-slug}` |

Games have **no** API server, no auth endpoints, no DB of their own.

## Environment Variables

| Variable | Where | Description |
|----------|-------|-------------|
| `JWT_KEY` | portal-auth | JWT signing secret |
| `PORTAL_DB` | portal-auth | DB connection string |
| `POSTGRES_PASSWORD` | postgres | DB password |

All via `.env` (gitignored). Copy `.env.example` on fresh clone.

## Related

- [[Tech/Infrastructure]] — Docker Compose, nginx config, local dev
- [[Design/Shared Auth]] — auth design decisions
- [[Tech/Adding a New Game]] — how to wire a new game into this system
