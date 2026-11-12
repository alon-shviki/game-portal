# Iron Crypt

> First-person raycasting shooter. Descend through procedurally generated crypts, find the keys, clear out the garrison and reach the exit — every level comes from a seed, so any run can be replayed.

**Repo:** [alon-shviki/iron-crypt](https://github.com/alon-shviki/iron-crypt)
**Play:** http://localhost:3000/iron-crypt/ (via `docker compose up`; direct dev port 8082) or `dotnet run --project IronCrypt.Client`

---

## What It Is

- Software raycaster written in C#: textured walls with distance shading, sliding doors, depth-sorted sprites.
- Seeded room-and-corridor levels with red/blue keys and locked doors, always solvable.
- Three weapons (pistol, shotgun, chaingun) with magazines and reloads; three enemy types (grunt, shooter, tank) with line-of-sight, hearing and pathfinding.
- No asset files: textures, sprites, sound effects and music are all generated in code.
- Keyboard + mouse on desktop, touch controls on phones. Score submitted to the portal leaderboard when a run ends.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Game client | Blazor WASM (.NET 10) |
| Rendering | C# raycaster → RGBA framebuffer → `putImageData` (no `Blazor.Extensions.Canvas` — too slow per pixel) |
| Game loop | `requestAnimationFrame` → JS interop → C# tick |
| Audio | WebAudio synthesis in `gameInterop.js` |
| Auth / Scores / Leaderboard | Portal auth server (shared across all games), slug `iron-crypt` |

No standalone Iron Crypt API server or database, matching every other game on the portal.

## Status

| Area | Status |
|------|--------|
| GitHub repo, labels, CI, branch protection | ✅ Done |
| Engine core, levels, combat, enemies | ✅ Done |
| Audio, game flow, polish | ✅ Done |
| Portal wiring (nginx, compose :8082, shell card, leaderboard tab, script slug `ic`) | ✅ Done |
| Tests | ✅ 44 headless tests incl. bot playthroughs; 60 FPS measured on the Release image |
| Playtest on a real phone, audio and balance tuning | ⏳ Pending — needs human hands and ears |

---

## Design Notes

- [[Games/iron-crypt/Design/Core Loop|Core Loop]] — loop, seeds, scoring
- [[Games/iron-crypt/Design/Weapons|Weapons]] — the three guns, magazines, reload rules
- [[Games/iron-crypt/Design/Enemies|Enemies]] — stats and what each asks of the player
- [[Games/iron-crypt/Design/Difficulty|Difficulty]] — per-depth curve and supply rates
- [[Games/iron-crypt/Design/Level Design|Level Design]] — rooms, doors, keys, exit, minimap
- [[Games/iron-crypt/Design/Visual Identity|Visual Identity]] — palette, rendering style
- [[Games/iron-crypt/Design/Controls and UI|Controls and UI]] — keyboard, mouse, touch, screens

## Tech Notes

- [[Games/iron-crypt/Tech/Architecture|Architecture]] — stack, portal integration contract
- [[Games/iron-crypt/Tech/Engine|Engine]] — code layout, frame flow, tuning knobs
- [[Games/iron-crypt/Tech/Renderer|Renderer]] — raycaster, sprites, resolution
- [[Games/iron-crypt/Tech/Level Generator|Level Generator]] — algorithm, locks
- [[Games/iron-crypt/Tech/Enemy AI|Enemy AI]] — state machine, pathfinding
- [[Games/iron-crypt/Tech/Input and Interop|Input and Interop]] — frame contract, touch
- [[Games/iron-crypt/Tech/Audio|Audio]] — synthesized sound and music
- [[Games/iron-crypt/Tech/Testing|Testing]] — coverage and how to write a test
- [[Games/iron-crypt/Tech/Performance|Performance]] — measurements
- [[Games/iron-crypt/Tech/Decisions|Decisions]] — choices and reasons

## Tasks

- [[Games/iron-crypt/Tasks|Tasks]] — build phases and what is next
