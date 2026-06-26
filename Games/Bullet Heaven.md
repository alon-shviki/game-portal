# Bullet Heaven

> Vampire Survivors-style top-down survival shooter. Survive endless enemy waves, collect XP, pick upgrades, and chase the high score.

**Repo:** `~/Desktop/Bullet-Heaven`  
**Run:** `cd BulletHeaven.Client && dotnet run` → http://localhost:5292  
**Full stack:** `docker compose up --build` → http://localhost:8080

---

## What It Is

- Top-down arena survival. Player moves, weapons auto-fire.
- Enemies track the player; killing them drops XP gems.
- Level up → pause → pick 1 of 3 upgrades → resume.
- Score is submitted to a global leaderboard on game over.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Game client | Blazor WASM (.NET 10) |
| Rendering | HTML5 Canvas via `Blazor.Extensions.Canvas` |
| Game loop | `requestAnimationFrame` → JS interop → C# tick |
| Backend | ASP.NET Core Web API |
| Database | PostgreSQL via EF Core |
| Auth | JWT Bearer + bcrypt |
| Deploy | Docker Compose + nginx |

## Status

| Area | Status |
|------|--------|
| Core game loop | ✅ Done |
| 5 enemy types (Standard, Runner, Tank, Elite, Boss) | ✅ Done |
| 3 secondary weapons (Orb, Pulse, Aura) | ✅ Done |
| 20+ upgrades — Common / Rare / Epic | ✅ Done |
| Object pools (500 bullets, 1000 enemies) | ✅ Done |
| Quadtree spatial collision O(N log N) | ✅ Done |
| Auth + leaderboard backend | ✅ Done |
| Web Worker physics offload | ⏳ Pending |

---

## Design Notes

- [[Games/Bullet-Heaven/Design/Core Loop|Core Loop]] — state machine, tick pipeline, XP & leveling
- [[Games/Bullet-Heaven/Design/Entities|Entities]] — Player stats, all 5 enemy types, Projectile, XpGem
- [[Games/Bullet-Heaven/Design/Weapons & Upgrades|Weapons & Upgrades]] — all weapons + full upgrade catalogue
- [[Games/Bullet-Heaven/Design/Difficulty|Difficulty]] — wave scaling, spawn patterns

## Tech Notes

- [[Games/Bullet-Heaven/Tech/Architecture|Architecture]] — project file map, JS↔C# bridge, render pipeline
- [[Games/Bullet-Heaven/Tech/Performance|Performance]] — frame budget rules, pools, quadtree internals
- [[Games/Bullet-Heaven/Tech/Backend|Backend]] — API endpoints, DB schema, auth security rules

## Tasks

- [[Games/Bullet-Heaven/Tasks|Tasks]] — full task list, WORK-001/002 spec
