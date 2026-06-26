# Bullet Heaven

> Vampire Survivors-style top-down survival shooter. Survive endless enemy waves, collect XP, pick upgrades, and chase the high score.

**Repo:** `~/Desktop/Bullet-Heaven`  
**Play:** `cd ~/Desktop/game && docker compose up` → http://localhost:8080  
**Dev only (no auth):** `cd BulletHeaven.Client && dotnet run` → http://localhost:5292

---

## What It Is

- Top-down arena survival. Player moves, weapons auto-fire.
- Enemies track the player; killing them drops XP gems.
- Level up → pause → pick 1 of 3 upgrades → resume.
- Score is submitted to the portal leaderboard on game over.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Game client | Blazor WASM (.NET 10) |
| Rendering | HTML5 Canvas via `Blazor.Extensions.Canvas` |
| Game loop | `requestAnimationFrame` → JS interop → C# tick |
| Auth / Scores / Leaderboard | Portal auth server (shared across all games) |
| Deploy | GHCR image (`ghcr.io/alon-shviki/bh-client`) — CI on push to `main` |

No standalone BH API server or database. See [[Design/Shared Auth]] for how auth and scores work.

## Status

| Area | Status |
|------|--------|
| Core game loop | ✅ Done |
| 5 enemy types (Standard, Runner, Tank, Elite, Boss) | ✅ Done |
| 3 secondary weapons (Orb, Pulse, Aura) | ✅ Done |
| 20+ upgrades — Common / Rare / Epic | ✅ Done |
| Object pools (500 bullets, 1000 enemies) | ✅ Done |
| Quadtree spatial collision O(N log N) | ✅ Done |
| Auth + leaderboard (via portal) | ✅ Done |
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
- [[Games/Bullet-Heaven/Tech/Backend|Backend]] — portal integration, score/leaderboard flow

## Tasks

- [[Games/Bullet-Heaven/Tasks|Tasks]] — full task list, WORK-001/002 spec
