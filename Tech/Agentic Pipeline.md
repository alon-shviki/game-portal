# Agentic Pipeline

How Claude Code works on this project — from picking an issue to merging a fix, without manual branch management or PR wrangling.

## Overview

```
GitHub Issues  →  triage  →  worktree  →  tests  →  CI  →  merge
     ↑                                                        ↓
  auto-filed                                           worktree deleted
```

Claude handles the full cycle. You pick what to work on and approve the merge.

## Issue Lifecycle

### Filing issues
Three templates available on every repo:
- **Bug** — auto-labels `bug, priority:medium`
- **Feature** — auto-labels `enhancement, priority:medium`
- **Problem / Discussion** — auto-labels `question, priority:medium`

Anyone (you, Claude, external contributors) can open an issue. Claude also auto-files issues whenever it spots a problem outside the current task.

### Triage
Say "what should I work on?" — Claude fetches all open issues and scores them:

| Signal | Points |
|--------|--------|
| `bug` label | +3 |
| `priority:high` | +3 |
| `priority:medium` | +2 |
| `priority:low` | +1 |
| Each 👍 reaction | +1 |
| Every 7 days old | +1 |

Returns top 5 ranked with scores and reasoning. You pick a number.

## Working on an Issue

### 1. Start
```bash
start-issue <number>
```
Creates an isolated git worktree from fresh `origin/main`, prints the path. Claude works entirely inside the worktree — main working directory is untouched.

### 2. Work
Claude reads the issue, edits files in the worktree, runs local checks. If it finds an unrelated problem, it opens a new GitHub issue and continues.

### 3. Finish
```bash
finish-issue   # run from inside the worktree
```
Steps:
1. Runs tests (auto-detects `.Tests.csproj`)
2. If tests fail → stops, reports failure
3. If pass → commits, pushes branch, opens PR (links `Closes #N`)
4. Waits for CI via `wait_for_ci` (retries while checks register, then watches)
5. **CI green** → merges PR → removes worktree → pulls `origin/main` → deletes remote branch
6. **CI red** → skips the merge and **keeps the worktree** so you can fix and re-run

### Non-issue work
```bash
start-task <name>
# ... make changes in the worktree ...
auto-pr "description"
```
`auto-pr` commits, pushes, opens/updates the PR, then waits for CI. **Green → removes the worktree** (PR left open for you to merge); **red → keeps the worktree** so you can fix and re-run.

## Scripts

All scripts live **only** in the portal repo at `.claude/scripts/` (versioned); `setup.sh` symlinks the entry points into `~/.local/bin/`. Game repos reference them by absolute path — no per-repo copies. Shared logic lives in `lib.sh`.

| Script | What it does |
|--------|-------------|
| `start-issue <n>` | Creates worktree + branch from `origin/main` |
| `start-task <name>` | Same, for non-issue work (`feat/<name>` branch) |
| `finish-issue` | Tests → PR → wait for CI → **merge if green** → remove worktree (kept if red) |
| `auto-pr "msg"` | Commit → PR → wait for CI → **remove worktree if green** (kept if red); no merge |
| `lib.sh` | Shared `wait_for_ci` + `remove_worktree` helpers, sourced by the two above |

**New machine setup:**
```bash
git clone git@github.com:alon-shviki/game-portal.git ~/Desktop/game
bash ~/Desktop/game/setup.sh
```

## Game Sub-Agent Pipelines

Each game keeps its own QA/test/docs/e2e sub-agents in its own repo (e.g. Bullet-Heaven's `.claude/agents/qa-reviewer.md`, `test-generator.md`, `docs-generator.md`, `playwright-e2e.md`, spawned per `.claude/rules/pipeline.md`). They are **not** copied into the portal — a portal session does those pipeline steps inline instead of spawning sub-agents (see `CLAUDE.md` → Game Context Rule). Portal previously had `bh-`-prefixed copies (PR #17); these were reverted (PR #19) since they weren't used from portal. Verified 2026-07-03: with the portal-side copies gone, Bullet-Heaven's pipeline still spawns correctly stand-alone (agent files present, `pipeline.md` references match their unprefixed names, no leftover `bh-` references anywhere in that repo).

## CI / Branch Protection

`game-portal`, `Bullet-Heaven`, and `orbit-break`:
- Direct pushes to `main` blocked
- PRs require the `build` check to pass
- `build` job: cache → `dotnet format --verify` → build → test
- `push-image` job: runs only on merge to `main`, pushes to GHCR

New games must follow the same pattern — see `.claude/rules/adding-a-game.md`.

## Adding a New Project

See `.claude/rules/adding-a-game.md` — it includes the full GitHub setup (labels, templates, CI, branch protection) so every new game gets this pipeline from day one.
