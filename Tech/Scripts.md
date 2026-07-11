# Agentic Scripts

All pipeline scripts live in **one place** — the portal repo at `.claude/scripts/`. There are **no per-repo copies**: the game repos (Bullet Heaven, Orbit Break) reference them by absolute path (`bash ~/Desktop/game/.claude/scripts/<script>`), and the scripts auto-detect which repo you're in. Shared logic lives in `lib.sh`, sourced by `auto-pr` and `finish-issue`. No machine-local setup required.

## start-issue

Creates an isolated git worktree for working on a GitHub issue.

```bash
start-issue <number> <slug>    # explicit — e.g. start-issue 4 bh
start-issue <number>           # no slug: auto-detects from git remote, then searches all repos
```

**Auto-detect order:** if the current directory is inside a managed repo (matched by `git remote get-url origin`), that repo is used with no API call. Useful when Claude Code is opened directly in a game directory.

**What it does:**
1. Fetches `origin/main` of the target repo
2. Creates `.worktrees/issue-<N>/` inside that repo's directory
3. Checks out a new branch `feat/issue-<N>-<title-slug>`
4. Prints issue details + worktree path

**Managed repos / slugs:**

| Slug | Repo | Root |
|------|------|------|
| `portal` | `alon-shviki/game-portal` | `~/Desktop/game` |
| `bh` | `alon-shviki/Bullet-Heaven` | `~/Desktop/Bullet-Heaven` |
| `ob` | `alon-shviki/orbit-break` | `~/Desktop/orbit-break` |

To add a new game: add a row above **and** update `REPOS`/`ROOTS` maps in `.claude/scripts/start-issue` and `.claude/scripts/start-task`. Do **not** copy the scripts into the new repo — reference the portal's by absolute path.

## start-task

Creates a worktree for non-issue work (docs, config, small refactors) that doesn't have a GitHub issue.

```bash
start-task <branch-name> <slug>    # e.g. start-task update-nginx-config portal
start-task <branch-name>           # auto-detects repo from git remote
```

**What it does:** same as `start-issue` but without an issue number. Creates `.worktrees/task-<name>/` on a `feat/<name>` branch.

## finish-issue

Run from **inside** the worktree when issue work is complete.

```bash
cd ~/Desktop/Bullet-Heaven/.worktrees/issue-4
finish-issue "Fixed the enemy spawn overlap by clamping spawn rings to the arena bounds. Verified with 3 new EnemySpawner tests."
```

**Steps:**
1. Auto-detects `.Tests.csproj` and runs `dotnet test` — stops on failure
2. Commits all staged changes with the issue title as the commit message
3. Pushes branch to `origin`
4. Opens a PR: `Closes #<N>` + the body you pass as `$1` — **always pass a real what/why/how-verified body**; without one a fallback (commit list + diffstat) is generated (reuses an already-open PR)
5. Waits for CI via `wait_for_ci` (retries while checks are still registering, then watches)
6. **CI green** → merges PR (squash), removes the worktree completely, pulls `origin/main`, deletes the remote branch
7. **CI red** → skips the merge and **keeps the worktree** so you can fix and re-run

## auto-pr

Run from **inside** a worktree (created by `start-task`) when non-issue work is done.

```bash
cd ~/Desktop/game/.worktrees/task-update-nginx-config
auto-pr "Update nginx config to add cache headers" "Adds Cache-Control for static assets so the shell stops re-downloading fonts on every load. Verified with curl -I against the running compose stack."
```

**Steps:**
1. Commits all changes
2. Pushes branch to `origin`
3. Opens a PR (or updates an already-open one) — title = `$1`, body = `$2`. **Always pass a real what/why/how-verified body**; without one a fallback (commit list + diffstat) is generated. Does **not** merge — that's left to you.
4. Waits for CI via `wait_for_ci`
5. **CI green** → removes the worktree; the PR is left open, green, and ready to merge
6. **CI red** → **keeps the worktree** at its path; fix the failure and re-run `auto-pr` to update the same PR

Because it now blocks on CI, `auto-pr` takes as long as the pipeline (~1 min here) instead of returning instantly.

## lib.sh

Shared helpers sourced by `auto-pr` and `finish-issue` — the single source that keeps the two scripts in sync:

- `wait_for_ci <pr>` — blocks until the PR's checks finish; returns `0` if all green, `1` if any failed. Polls check **states** (not a raw `gh pr checks --watch`) so it waits correctly while checks are still registering *or* pending — cross-repo reusable-workflow checks register a beat slower, and the old `--watch` race would false-report red in that gap.
- `remove_worktree <path> <main>` — force-removes the worktree, then `rm -rf`s the directory (git leaves ignored `bin/`/`obj/` behind, which would otherwise strand it), retries once if a concurrent build (e.g. a Stop-hook build gate) recreated `bin/obj` mid-delete, and runs `git worktree prune`.
- `pr_body_fallback` — auto-generated PR body (commit subjects + diffstat) used only when the caller didn't pass one.
- `remind_docs` — **Documentation Rule nudge.** Called right after `git add -A` in both scripts: if the staged change touches code but no `.md`, it prints a reminder to update the relevant doc (portal `Tech/…` or the game's `Notes/…`). Non-blocking and heuristic — it never blocks a trivial change, it just makes "I forgot the docs" visible before the PR is opened.

**Worktree lifecycle:** both scripts remove the worktree **only when CI is green**. A red build keeps it in place so the fix happens on the same branch.

## Related

- [[Tech/Agentic Pipeline]] — full workflow including issue triage
- [[Tech/CI and Branch Protection]] — what CI checks the PRs
