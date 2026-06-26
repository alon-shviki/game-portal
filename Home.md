# Game Portal — Home

A single platform hosting multiple browser games. One account, shared leaderboard, one `docker compose up`.

## Games

| Game | Status | Stack | Port |
|------|--------|-------|------|
| [[Games/Bullet Heaven]] | ✅ Live | Blazor WASM | :8080 |

## Portal

| Layer | Status |
|-------|--------|
| Shell (`shell/index.html`) | ✅ Live — dark theme, game cards, auth modal, leaderboard |
| Auth server (`portal-auth/`) | ✅ Live — register, login, JWT, scores, leaderboard API |
| Docker Compose | ✅ Live — portal :3000, BH :8080, one command |

## Design Notes

- [[Design/Vision]] — landing page layout, user journey, key decisions
- [[Design/Shared Auth]] — JWT flow, cross-origin token relay, portal leaderboard
- [[Design/UI Identity]] — colour palette, typography, game card design

## Tech Notes

- [[Tech/Infrastructure]] — Docker Compose, nginx routing, CI/CD
- [[Tech/Adding a New Game]] — step-by-step checklist for onboarding a new game

## Links

- [[Games/Bullet Heaven]] — full game page
- [[Roadmap]] — status of all tasks + what's next
