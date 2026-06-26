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

## Next — Portal Shell

- [ ] Landing page — see [[Design/Vision]] for layout
- [ ] Shared auth server — see [[Design/Shared Auth]] for design
- [ ] Shared leaderboard UI component reused per game
- [ ] Dockerised deployment — see [[Tech/Infrastructure]] for compose layout

## Later — Additional Games

- [ ] Game 2 TBD
- [ ] Game 3 TBD
