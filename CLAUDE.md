# Game Portal

Top-level portal — multiple standalone browser games, one platform.

## Commands
- **Fresh clone**: `git clone <repo> && cd game && cp .env.example .env` (fill in secrets)
- **Everything** (recommended): `cd ~/Desktop/game && docker compose up --build`
  - Portal → http://localhost:3000
  - Bullet Heaven → http://localhost:8080  (images pulled from `ghcr.io/alon-shviki/bh-*`)
  - Port 80 is taken by `idp-control-plane`; portal runs on 3000

## Games

| Game | Repo | Stack | Status |
|------|------|-------|--------|
| Bullet Heaven | `~/Desktop/Bullet-Heaven` | Blazor WASM (client only) | Active |

Games have no standalone API server. Auth, scores, and leaderboard live in `portal-auth`. See `.claude/rules/architecture.md`.

## Detailed Rules — read the matching file before working on:

| Task | Read first |
|------|-----------|
| Obsidian vaults, symlinks, wiki links, gitignore | `.claude/rules/obsidian.md` |
| Adding a new game to the portal | `.claude/rules/adding-a-game.md` |
| Portal architecture, shared auth, nginx, Docker | `.claude/rules/architecture.md` |

## Hard Rules
- `.obsidian/` is gitignored everywhere — never commit it.
- `Games/<game-folder>` entries are symlinks to game repos — never replace with copies.
- Game-specific notes live in the game's own repo under `Notes/` — don't duplicate here.
- Before working on a specific game, go to that repo and read its `CLAUDE.md`.
