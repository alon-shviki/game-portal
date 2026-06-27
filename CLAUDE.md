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

## Issue Triage

When the user says "what should I work on", "pick an issue", or similar:

1. Run `gh issue list --repo <repo> --json number,title,labels,reactions,createdAt --limit 50`
2. Score each issue:
   - `bug` label → +3
   - `priority:high` → +3, `priority:medium` → +2, `priority:low` → +1
   - Each 👍 reaction → +1
   - Every 7 days old → +1 (older = more urgent)
3. Present the top 5 as a numbered list with score, labels, and one-line reason for ranking
4. Wait for the user to pick a number, then implement it and run `auto-pr`

If no issues exist, say so and ask what to add as an issue first.

Whenever you would add a task or idea to `Roadmap.md`, create a GitHub issue instead:
```
gh issue create --repo <repo> --title "..." --body "..." --label "enhancement,priority:medium"
```
`Roadmap.md` stays as a high-level overview only — individual tasks live as issues.

## Agentic Workflow

**For issue-based work** (the default):
1. `start-issue <number>` — creates an isolated worktree from fresh `main`, shows issue details
2. Do all work inside the worktree path it prints
3. `finish-issue` (run from inside the worktree) — runs tests, if pass: pushes PR, waits for CI, merges, deletes worktree

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
