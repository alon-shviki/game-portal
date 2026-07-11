# Portal Roadmap

## Now — Bullet Heaven v1

- [x] Core game loop (RAF → JS interop → C# tick)
- [x] Player movement, shooting, XP, leveling
- [x] Enemy types: Standard, Runner, Tank, Elite, Boss
- [x] Upgrade catalogue (Common / Rare / Epic)
- [x] Object pools (bullets ×500, enemies ×1000)
- [x] Quadtree spatial collision (O(N log N))
- [x] Full backend: auth (JWT + bcrypt), scores, leaderboard (PostgreSQL)
- [ ] Web Worker physics offload (WORK-001, WORK-002) → [[Games/Bullet-Heaven/Notes/Tasks|full spec]]

## Now — Portal Shell + Shared Leaderboard

- [x] Landing page (`shell/index.html`) — see [[Design/Vision]] for layout
- [x] Shared auth server (`portal-auth/`) — see [[Design/Shared Auth]] for design
- [x] Dockerised deployment + nginx (`docker-compose.yml`, `nginx.conf`) → http://localhost:3000
- [x] Portal owns all scores — `POST /api/scores/{game}`, `GET /api/leaderboard/{game}`, personal best
- [x] Leaderboard page in portal shell — per-game tab selector
- [x] BH client proxies score/leaderboard calls to portal via nginx (no BH API server)
- [x] BH reads portal JWT from `localStorage["jwt"]` — one login, works everywhere
- [x] BH images on GHCR (`ghcr.io/alon-shviki/bh-client`) — CI pushes on every merge to main

## Next — Agentic Git Branch Management & Pipelines

- [x] Git branch strategy — feature branches, PR reviews, protected main
- [x] Agentic pipeline — Claude Code runs on branches, auto-opens PRs, CI gates merge

## Next — Game Ideas

- [x] Brainstorm and pick Game 2 (genre, stack, scope) → Orbit Break (physics arena/breakout hybrid, Blazor WASM + Canvas, ambitious scope) — see [[Design/Orbit Break]]
- [ ] Brainstorm and pick Game 3

## Done — Hardening (July 2026)

- [x] Path-based game routing behind portal nginx (`/bh/`, `/orbit-break/`) + security headers + gzip
- [x] portal-auth integration tests (Testcontainers postgres, all endpoints)
- [x] portal-auth hardening — EF Core 10, BCrypt cost 12, explicit JWT validation, rate-limited auth endpoints (refresh tokens deliberately skipped — see [[Design/Shared Auth]])
- [x] CI: vulnerable-package gate + `:sha-*` image tags (single-source `dotnet-ci.yml`)
- [x] Opt-in TLS (`docker-compose.tls.yml` + mkcert) — see [[Tech/Infrastructure]]
- [x] Legacy `BulletHeaven.Server` deleted (portal owns all backend)

## Later — Additional Games

- [x] Game 2 — Orbit Break (shipped — client on GHCR, wired into portal)
- [ ] Game 3 (TBD from ideas above)
