# Orbit Break

> Game 2 concept — chosen 2026-07-03. Physics arena/breakout hybrid: launch balls that curve around gravity wells to shatter procedurally generated block constellations. Infinite difficulty ramp, pure high-score chase.

## Pitch

A single ball launches from the bottom of the arena. Instead of bouncing off flat walls (classic Breakout), the arena contains **gravity wells** — orbs that pull the ball into curved, slingshot trajectories. Blocks are arranged in procedurally generated "constellations" around and between the wells. Chaining a single launch through multiple wells to clear blocks in one pass is the core skill — each well-assisted hit stacks a combo multiplier.

No lives in the traditional sense: the run ends when the ball can no longer be returned to the launcher (falls off-arena) or a "collapse" hazard reaches the player line. Difficulty escalates by generating denser, faster-moving constellations forever — so score is uncapped and purely a high-score chase, which maps directly onto the portal's leaderboard (best score, no other state needed).

## Core Loop

1. Aim + launch ball (mouse/touch drag, release to fire).
2. Ball travels, curving around any gravity well within its influence radius; breaks blocks on contact.
3. Combo multiplier increases per well-assisted deflection in a single flight; resets when the ball returns to the launcher and stops.
4. Every N launches, the constellation regenerates one tier harder (more wells, tighter block clusters, occasional moving/hazard blocks).
5. Run ends on: ball lost past the arena boundary with no balls remaining, or a hazard block reaches the launcher line.
6. Score submitted to portal leaderboard on run end.

## Systems (matching Bullet Heaven's ambition)

- **Physics**: gravity-well influence (inverse-square pull within radius), ball-block collision, wall bounces outside well influence. Reuses BH's fixed-tick physics step pattern.
- **Procedural constellation generator**: tiered difficulty curve, seeded per run (enables a future "daily seed" mode without new portal infra — pure client-side determinism).
- **Combo/scoring system**: multiplier stacking, well-chain bonuses, block-type value differences (standard / armored / explosive / hazard).
- **Block variety**: standard (1 hit), armored (multi-hit), explosive (area clear + chain reaction), hazard (advances toward launcher if untouched).
- **Ball variants** (unlocked via in-run pickups, not persisted): heavy ball (more damage, less curve), split ball (fires two), phase ball (passes through one block).
- **Juice**: particle bursts on block break, screen shake on big combos, trail rendering on curved flight — same rendering-loop muscle BH already built.

## Scope Estimate

Matches Bullet Heaven's scale:
- Physics/orbit engine + collision: comparable effort to BH's quadtree + projectile system.
- 4+ block types, 3+ ball variants, procedural tiered generator: comparable to BH's enemy roster + upgrade catalogue.
- Juice/particles/combo UI: direct pattern reuse from BH.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Game client | Blazor WASM (.NET), same as Bullet Heaven |
| Rendering | HTML5 Canvas via `Blazor.Extensions.Canvas` |
| Game loop | `requestAnimationFrame` → JS interop → C# tick (reuse BH's loop pattern) |
| Auth / Scores / Leaderboard | Portal auth server — no game-specific backend or DB, per [[Design/Shared Auth]] |

## Fit with Portal Constraints

- No `/register`/`/login` of its own — JWT via portal, same as BH.
- No persistent save state needed: the entire loop is a single-run score chase, so the portal's one-score-per-game + top-10-leaderboard model is a complete fit, not a limitation.
- Any per-run unlocks (ball variants, etc.) are transient within a run, not saved — no `localStorage` workaround required.
- Ships as its own repo + `CLAUDE.md` + Notes vault + `Games/Orbit Break.md` hub page, following [[.claude/rules/adding-a-game|Adding a New Game]], once implementation begins.

## Status

Concept chosen, not yet built. Implementation (new repo, docker-compose wiring, CI) is a separate future task per the standard game-onboarding checklist.
