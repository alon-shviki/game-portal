---
name: obsidian-vault
description: Search, create, and manage notes in the portal Obsidian vault with wikilinks. Use when the user wants to find, create, or organize notes in Obsidian.
---

# Obsidian Vault — Portal

## Vault location

`/home/alon/Desktop/game/`

Structure (see `.claude/rules/obsidian.md` for the full rules):

```
Home.md            ← dashboard
Roadmap.md
Design/            ← portal design notes
Tech/              ← portal tech notes
Games/
  <Game Name>.md   ← hub page per game
  <game-folder>/   ← SYMLINK into ~/Desktop/<game-folder>/Notes
```

## Naming conventions

- **Title Case** for note filenames.
- Notes are organized into `Design/` and `Tech/` folders — not flat.
- `Games/<Game Name>.md` is the hub/index page per game.

## Linking

- Use Obsidian `[[wikilinks]]`: `[[Tech/Architecture]]`.
- Link into game notes via the symlink path (no `/Notes` segment):
  `[[Games/Bullet-Heaven/Design/Core Loop|Core Loop]]`.
- Add related-note links at the bottom of each note.
- Never duplicate a game note here — it lives in the game's own vault.

## Workflows

### Search
```bash
find -L "/home/alon/Desktop/game/" -name "*.md" | grep -i "keyword"   # by filename
grep -Rl "keyword" "/home/alon/Desktop/game/" --include="*.md"        # by content
```
Or use the Grep/Glob tools on the vault path.

### Create a note
1. Title Case filename in the right folder (`Design/` or `Tech/`).
2. Write the content.
3. Add `[[wikilinks]]` to related notes at the bottom.

### Find backlinks
```bash
# matches bare, aliased ([[Note Title|X]]), and path-qualified ([[Folder/Note Title]]) links
grep -Rl "\[\[[^]]*Note Title" "/home/alon/Desktop/game/" --include="*.md"
```

## Rules

- `.obsidian/` is gitignored — never commit it.
- `Games/<game-folder>` is a symlink — never replace with a copy.
