# Game Portal

Top-level portal — multiple standalone browser games, one platform.

## Commands
- Bullet Heaven (dev): `cd ~/Desktop/Bullet-Heaven/BulletHeaven.Client && dotnet run` → http://localhost:5292
- Bullet Heaven (full stack): `cd ~/Desktop/Bullet-Heaven && docker compose up --build` → http://localhost:8080

## Games

| Game | Repo | Stack | Status |
|------|------|-------|--------|
| Bullet Heaven | `~/Desktop/Bullet-Heaven` | Blazor WASM + ASP.NET Core + PostgreSQL | Active |

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
