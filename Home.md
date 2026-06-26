# Game Portal — Home

Central hub for all games in development. Each game is a standalone project; this portal will eventually host them all under one web frontend.

## Games in Development

| Game | Status | Stack | Path |
|------|--------|-------|------|
| [[Games/Bullet Heaven]] | Active development | Blazor WASM + ASP.NET Core | `~/Desktop/Bullet-Heaven` |

## Portal Vision

A single landing page that links out to each playable game. Games share:
- A common **auth system** (JWT, one user account works across games)
- A common **leaderboard API** per game
- A shared **visual identity** (dark theme, consistent UI components)

## Pending Games (Ideas)

- Platformer — simple 2D side-scroller, possibly Blazor or vanilla JS canvas
- Tower Defense — grid-based, turn-based wave management
- Puzzle / Match-3 — quick session game for the portal homepage

## Design Notes

- [[Design/Vision]] — landing page layout, user journey, key decisions
- [[Design/Shared Auth]] — single JWT across all games, migration plan
- [[Design/UI Identity]] — colour palette, typography, game card design

## Tech Notes

- [[Tech/Infrastructure]] — Docker Compose, nginx routing, environment vars
- [[Tech/Adding a New Game]] — step-by-step checklist for onboarding a new game

## Games

- [[Games/Bullet Heaven]] — full game page
- [[Roadmap]] — status of all tasks + what's next
