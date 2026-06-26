# Obsidian Vault Rules

## Vault Structure

```
~/Desktop/game/          ← portal vault root
  Home.md                ← dashboard
  Roadmap.md
  Design/                ← portal design notes
  Tech/                  ← portal tech notes
  Games/
    <Game Name>.md       ← hub page per game (links into game notes)
    <game-folder>/       ← SYMLINK → ~/Desktop/<game-folder>
```

Each game repo has its own standalone vault at `~/Desktop/<game-folder>/` with `Notes/` inside it. The portal vault reaches those notes via a symlink — edit from either vault, one source of truth.

## Symlink Rule

`Games/<game-folder>` is a symlink to the game's `Notes/` folder only — not the full repo. This keeps code files out of the portal vault index.

Create the symlink when adding a new game:
```bash
ln -s ~/Desktop/<game-folder>/Notes ~/Desktop/game/Games/<game-folder>
```

## app.json — Always Set This

Every vault (portal and each game) must have `.obsidian/app.json` with:
```json
{
  "userIgnoreFilters": ["node_modules", "bin", "obj"]
}
```
Prevents build artifacts from polluting the file index. Add more folders if the game stack generates other output dirs.

## Gitignore Rule

`.obsidian/` is gitignored in this repo and in every game repo. Obsidian writes personal workspace state (open tabs, cache, window layout) into it — that's per-machine, never shared. The `app.json` above is recreated when needed.

## Wiki Link Paths

From the portal vault, link into game notes using the symlink path:
```
[[Games/Bullet-Heaven/Design/Core Loop|Core Loop]]
```

The symlink points directly at the game's `Notes/` folder, so there's no `/Notes` segment in the path.

From a game vault, links are local:
```
[[Design/Core Loop]]
```

Never duplicate notes across both vaults. Game-specific content lives in the game vault; portal-level content lives here.
