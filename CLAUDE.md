# Game Portal

Top-level portal — multiple standalone browser games, one platform.

## Commands
- **Fresh clone**: `git clone <repo> && cd game && cp .env.example .env && bash setup.sh` (fill in secrets, setup.sh symlinks agentic scripts)
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

## Managed Repos

This portal is the central hub — all work across every game is initiated from here.

| Slug | Repo | Root |
|------|------|------|
| `portal` | `alon-shviki/game-portal` | `~/Desktop/game` |
| `bh` | `alon-shviki/Bullet-Heaven` | `~/Desktop/Bullet-Heaven` |

When adding a new game, add a row here **and** update the `REPOS`/`ROOTS` maps in `.claude/scripts/start-issue`.

## Issue Triage

When the user says "what should I work on", "pick an issue", or similar:

1. Fetch from **all managed repos**:
   ```bash
   gh issue list --repo alon-shviki/game-portal --json number,title,labels,reactionGroups,createdAt --limit 50
   gh issue list --repo alon-shviki/Bullet-Heaven --json number,title,labels,reactionGroups,createdAt --limit 50
   ```
2. Score: `bug` +3, `priority:high` +3, `priority:medium` +2, `priority:low` +1, each 👍 +1, every 7 days old +1
3. Present top 5 with repo slug prefix, score, labels, one-line reason:
   ```
   1. [bh #4]     WORK-001: Web Worker setup — score 9  (bug, priority:high)
   2. [bh #5]     WORK-002: Delegate physics — score 6  (enhancement, priority:high)
   3. [portal #7] Pick Game 2              — score 3  (enhancement, priority:medium)
   ```
4. User picks → `start-issue <number> <slug>` (e.g. `start-issue 4 bh`)

If no issues exist across any repo, say so and ask what to create first.

Whenever you would add a task or idea to `Roadmap.md`, create a GitHub issue instead:
```bash
gh issue create --repo <repo> --title "..." --body "..." --label "enhancement,priority:medium"
```
`Roadmap.md` stays as high-level overview only — individual tasks live as issues.

## Agentic Workflow

**For issue-based work** (the default — works from portal for any game):
1. `start-issue <number> <slug>` — creates worktree in the game's own repo, shows issue details
2. Do all work inside the worktree path it prints
3. `finish-issue` (run from inside the worktree) — tests → PR → CI → merge → delete worktree

**For non-issue work:**
- `git checkout -b feat/<name>` then `auto-pr "description"` when done

Never commit directly to `main`. The worktree is automatically cleaned up after merge.

Scripts live in `.claude/scripts/` (version controlled). `setup.sh` symlinks them into `~/.local/bin` on each machine.

## Auto-Issue Rule

When you spot a bug, problem, performance issue, or architectural concern **outside the current task** — create a GitHub issue immediately, then continue with the task. Do not fix it inline.

```bash
gh issue create --repo <repo> --title "..." \
  --label "bug,priority:medium" \
  --body $'**What\'s the problem:**\n\n**Why it matters:**\n\n**Possible approaches:**'
```

Use `bug` for clear breakage, `question` for decisions/discussion, `enhancement` for improvements. Always set a priority label.

## Hard Rules
- `.obsidian/` is gitignored everywhere — never commit it.
- `Games/<game-folder>` entries are symlinks to game repos — never replace with copies.
- Game-specific notes live in the game's own repo under `Notes/` — don't duplicate here.
- Before working on a specific game, go to that repo and read its `CLAUDE.md`.
