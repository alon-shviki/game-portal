# Orbit Break

> Physics arena/breakout hybrid. Launch balls that curve around gravity wells to shatter procedurally generated block constellations, chaining combos for an ever-escalating high score.

**Repo:** `~/Desktop/orbit-break` (docs scaffold only — no GitHub repo or code yet)
**Play:** not yet buildable

---

## What It Is

- Physics-driven breakout: balls curve around gravity wells instead of bouncing off flat walls.
- Chaining a launch through multiple wells stacks a combo multiplier.
- Difficulty escalates forever via denser, harder constellations — pure high-score chase.
- Score will be submitted to the portal leaderboard on run end, same as Bullet Heaven.

## Tech Stack (planned)

| Layer | Technology |
|-------|-----------|
| Game client | Blazor WASM (.NET) |
| Rendering | HTML5 Canvas via `Blazor.Extensions.Canvas` |
| Game loop | `requestAnimationFrame` → JS interop → C# tick |
| Auth / Scores / Leaderboard | Portal auth server (shared across all games) |

No standalone Orbit Break API server or database, matching every other game on the portal.

## Status

| Area | Status |
|------|--------|
| Concept brainstorm | ✅ Done — see [[Design/Orbit Break]] |
| Docs scaffold (CLAUDE.md, Obsidian vault, Notes/) | ✅ Done |
| GitHub repo, CI, branch protection | ⏳ Not started |
| Blazor WASM project scaffold | ⏳ Not started |
| Core game loop | ⏳ Not started |
| docker-compose wiring | ⏳ Not started |

---

## Design Notes

- [[Games/orbit-break/Design/Core Loop|Core Loop]] — launch/orbit/combo loop, systems, block & ball variety, scope

## Tech Notes

- [[Games/orbit-break/Tech/Architecture|Architecture]] — planned stack, portal integration contract

## Tasks

- [[Games/orbit-break/Tasks|Tasks]] — full path from concept to playable
