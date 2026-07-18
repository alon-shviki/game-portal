# Game Portal

The hub that serves every game. One nginx reverse proxy at `:3000` routes to a static portal shell, a shared auth/scores API, and each game's Blazor WASM client. There's a single login, a single JWT, and a single leaderboard database — games never build their own.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Reverse proxy | nginx (path-based routing, `sub_filter` base-href rewrite) |
| Portal shell | Static HTML/JS (`shell/index.html`) — login, game launcher, leaderboard |
| Auth server | ASP.NET Core Minimal API (`portal-auth/`) |
| Database | PostgreSQL 17 (EF Core, auto-migrate on startup) |
| Auth | JWT Bearer (shared signing key across every service) |
| Tests | xUnit (`PortalAuth.Tests`) |
| Deployment | Docker Compose |

## Getting Started

### Prerequisites

- Docker + Docker Compose
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (for running `portal-auth` or its tests outside Docker)

### Fresh clone

```bash
git clone git@github.com:alon-shviki/game-portal.git game
cd game
cp .env.example .env   # set POSTGRES_PASSWORD and JWT_KEY
```

### Run the full stack

```bash
docker compose up --build
```

| Service | URL |
|---------|-----|
| Portal shell | http://localhost:3000 |
| Bullet Heaven | http://localhost:3000/bh/ (dev-only direct: :8080) |
| Orbit Break | http://localhost:3000/orbit-break/ (dev-only direct: :8081) |

### Run tests

```bash
dotnet test PortalAuth.Tests
```

## Project Structure

```
game/
├── shell/                    # Portal static shell — login, launcher, leaderboard UI
├── portal-auth/              # ASP.NET Core auth + scores + leaderboard API
│   └── Migrations/           # EF Core migrations (auto-applied on startup)
├── PortalAuth.Tests/         # xUnit tests for the auth server
├── nginx.conf                # Portal reverse proxy — path routing to games
├── nginx-tls.conf            # Opt-in TLS variant (see docker-compose.tls.yml)
├── docker-compose.yml
├── docker-compose.tls.yml    # TLS override (mkcert-based local HTTPS)
├── Design/ Tech/ Games/      # Obsidian vault — architecture, roadmap, per-game hub pages
└── .claude/scripts/          # Agentic pipeline: start-issue, start-task, finish-issue, auto-pr
```

## Architecture

```
nginx (:3000)
  /                     → portal shell (static HTML)
  /api/auth/*           → portal-auth (strips /api/auth prefix)
  /bh/*                 → bh-client            (base-href rewritten to /bh/)
  /orbit-break/*        → orbit-break-client   (base-href rewritten to /orbit-break/)
```

**One JWT signing key** across every service. The portal auth server issues tokens on login/register; each game client stores the token in `localStorage["jwt"]` and sends it as `Authorization: Bearer` on every API call. Game clients expose **no** `/register` or `/login` of their own — auth is portal-only, and games have no database.

**sub_filter contract**: every game's `index.html` must contain the literal string `<base href="/"` — the portal nginx rewrites it to `<base href="/<game>/">` on the way through. Exact-string match; don't reformat that tag in game repos.

## Portal Auth Server — Endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/register` | — | Create portal account (rate-limited: 10/min/IP) |
| POST | `/api/login` | — | Login, returns JWT (rate-limited: 10/min/IP) |
| GET | `/api/me` | Bearer | Who am I? |
| POST | `/api/scores/{game}` | Bearer | Submit a score |
| GET | `/api/leaderboard/{game}` | — | Top 10 for a game |
| GET | `/api/leaderboard/{game}/me` | Bearer | User's personal best (top 5) |
| GET | `/health` | — | Liveness |

Passwords hashed with BCrypt (work factor 12). JWT claims: `sub` (user ID), `unique_name` (username), 30-day expiry.

## Games

| Game | Slug | Repo | Status |
|------|------|------|--------|
| Bullet Heaven | `bh` | [Bullet-Heaven](https://github.com/alon-shviki/Bullet-Heaven) | Active |
| Orbit Break | `ob` | [orbit-break](https://github.com/alon-shviki/orbit-break) | Active |

Each game is a self-contained Blazor WASM client with its own nginx image, proxying `/api/scores` and `/api/leaderboard` back to `portal-auth`. No game has its own API server or database — see each repo's README for details.

## Environment Variables

| Variable | Where | Description |
|----------|-------|-------------|
| `JWT_KEY` | portal-auth | JWT signing secret |
| `POSTGRES_PASSWORD` | postgres, portal-auth | DB password |

All set via `.env` (gitignored, copy from `.env.example`).

## Optional: TLS for Local Dev

```bash
docker compose -f docker-compose.yml -f docker-compose.tls.yml up --build
```

Uses `nginx-tls.conf` with mkcert-issued local certs — see `Tech/Infrastructure.md` for setup.

## Contributing / Workflow

This repo drives an agentic pipeline shared across all three repos:

- **Pick work**: `/pick-work` — scores and ranks open issues across all managed repos.
- **Issue work**: `bash .claude/scripts/start-issue <number> <slug>` → work in the worktree → `bash .claude/scripts/finish-issue`.
- **Non-issue work**: `bash .claude/scripts/start-task <name> <slug>` → `bash .claude/scripts/auto-pr "description"`.
- Never commit directly to `main`.

See `Tech/Agentic Pipeline.md` and `Tech/Scripts.md` for the full workflow, and `Tech/Architecture.md` / `Tech/Infrastructure.md` for architecture and deployment details.
