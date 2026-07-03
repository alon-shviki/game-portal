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

## Later — Additional Games

- [ ] Game 2 — Orbit Break (build not started)
- [ ] Game 3 (TBD from ideas above)
