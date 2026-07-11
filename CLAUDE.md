# Game Portal — Claude Instructions

## Commands
- **Fresh clone**: `git clone <repo> && cd game && cp .env.example .env`
- **Full stack**: `cd ~/Desktop/game && docker compose up --build`
  - Portal → http://localhost:3000 · Bullet Heaven → http://localhost:3000/bh/ · Orbit Break → http://localhost:3000/orbit-break/ (direct dev ports: 8080 / 8081)

## Games

| Game | Slug | Root | Status |
|------|------|------|--------|
| Bullet Heaven | `bh` | `~/Desktop/Bullet-Heaven` | Active |
| Orbit Break | `ob` | `~/Desktop/orbit-break` | Active |

## Read Before Working

| Task | Doc |
|------|-----|
| Portal architecture, auth, nginx | `Tech/Architecture.md` |
| Docker Compose, local dev, CI | `Tech/Infrastructure.md` |
| Adding a new game | `Tech/Adding a New Game.md` |
| Agentic scripts (start-issue, finish-issue, auto-pr) | `Tech/Scripts.md` |
| Full agentic workflow | `Tech/Agentic Pipeline.md` |
| `.claude/` layout — skills, commands, hooks | `Tech/Claude Setup.md` |

## Documentation Rule

**After completing any task or finding any problem: write or update a `.md` file.**

- Done with a feature → update or create the relevant doc in `Tech/` or `Design/`
- Found a bug or concern outside current task → open a GitHub issue AND note it in the relevant doc
- Use `[[Wiki Links]]` to connect related docs
- Visible folders only (`Tech/`, `Design/`, `Games/`) — Obsidian cannot read hidden directories like `.claude/`

## Workflow

**Pick work**: say "what should I work on" (or `/pick-work`) → Claude fetches issues from all managed repos, scores and ranks them, you pick one. See `Tech/Agentic Pipeline.md` for scoring rules.

**Do work**: `start-issue <number> <slug>` → **read every file in the LOAD GAME CONTEXT block** → work in the worktree → `finish-issue`. See `Tech/Scripts.md`.

**Non-issue work**: `start-task <name> <slug>` → work in the worktree → `auto-pr "description"`.

Never commit directly to `main`.

## Game Context Rule

When `start-issue` prints a `LOAD GAME CONTEXT` block, Claude **must** read every file listed in it before writing a single line of code. This loads the game's rules, pipeline, hard constraints, and architecture into the current session — replacing the need to open a separate Claude Code window for that game.

## Auto-Issue Rule

Spot a problem outside the current task → create a GitHub issue immediately, then continue.

```bash
gh issue create --repo <repo> --title "..." --label "bug,priority:medium" \
  --body $'**What\'s the problem:**\n\n**Why it matters:**\n\n**Possible approaches:**'
```

Use `bug` for breakage · `question` for decisions · `enhancement` for improvements.

## Managed Repos

| Slug | Repo | Root |
|------|------|------|
| `portal` | `alon-shviki/game-portal` | `~/Desktop/game` |
| `bh` | `alon-shviki/Bullet-Heaven` | `~/Desktop/Bullet-Heaven` |
| `ob` | `alon-shviki/orbit-break` | `~/Desktop/orbit-break` |

Adding a new game: add a row here + update `REPOS`/`ROOTS` in `.claude/scripts/start-issue` and `.claude/scripts/start-task`.

## Hard Rules
- `.obsidian/` is gitignored everywhere — never commit it
- `Games/<game-folder>` entries are symlinks — never replace with copies
- Game-specific notes live in the game's own `Notes/` — don't duplicate here
- Never add auth endpoints or a scores DB to a game — portal owns that
