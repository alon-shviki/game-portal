# Adding a New Game

Full checklist. Do these in order.

## 1. Create the Game Repo

```bash
mkdir ~/Desktop/<game-name>
cd ~/Desktop/<game-name>
git init
```

Write a `CLAUDE.md` at the root — see BH's as a template.

## 2. Game Repo Gitignore

Must include at minimum:
```
.obsidian/
.env
.env.*
.claude/settings.local.json
```
Plus whatever the stack generates (bin/, obj/, node_modules/, etc.).

## 3. Init the Game Vault

```bash
mkdir .obsidian
echo '{"userIgnoreFilters":["node_modules","bin","obj"]}' > .obsidian/app.json
```

Create a `Notes/` folder with this structure:
```
Notes/
  Home.md        ← quick start, run commands, link map
  Tasks.md       ← task list
  Design/        ← game design notes
  Tech/          ← architecture, stack, backend, performance
```

## 4. Wire Into the Portal Vault

```bash
ln -s ~/Desktop/<game-name>/Notes ~/Desktop/game/Games/<game-name>
```

Create `~/Desktop/game/Games/<Game Name>.md` hub page. It must:
- Describe what the game is (1 paragraph)
- List the tech stack
- Show current status (done / pending)
- Link into the game's Notes via wiki links through the symlink

## 5. Update Portal Docs

- `Home.md` — add row to the games table
- `Roadmap.md` — add a "Now — <Game> vX" section
- `Tech/Adding a New Game.md` — no changes needed (it's the checklist)

## 6. Wire Auth

Game server must NOT have its own `/register` or `/login`. It validates JWTs issued by the portal auth server. Same signing key, set via environment variable. See `architecture.md`.
