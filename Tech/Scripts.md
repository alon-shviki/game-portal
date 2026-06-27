# Agentic Scripts

Three scripts live in `.claude/scripts/` (version-controlled) and are symlinked into `~/.local/bin` via `setup.sh`.

## start-issue

Creates an isolated git worktree for working on a GitHub issue.

```bash
start-issue <number> <slug>    # e.g. start-issue 4 bh
start-issue <number>           # auto-detects repo (fails if ambiguous)
```

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

To add a new game: add a row above **and** update `REPOS`/`ROOTS` maps in `.claude/scripts/start-issue`.

## finish-issue

Run from **inside** the worktree when work is complete.

```bash
cd ~/Desktop/Bullet-Heaven/.worktrees/issue-4
finish-issue
```

**Steps:**
1. Auto-detects `.Tests.csproj` and runs `dotnet test` — stops on failure
2. Commits all staged changes with the issue title as the commit message
3. Pushes branch to `origin`
4. Opens a PR with `Closes #<N>` in the body
5. Waits for CI (`gh pr checks --watch`)
6. If CI green: merges PR (squash), deletes remote branch, removes worktree
7. Pulls `origin/main` into the repo root so main is up to date
8. If CI fails: stops, leaves PR open for manual fix

## auto-pr

For non-issue work (docs, refactors, config changes).

```bash
git checkout -b feat/<name>
# make changes
auto-pr "Short description of what this does"
```

**What it does:** stages everything, commits, pushes, opens PR. Does **not** auto-merge — you review and merge manually.

If a PR is already open for the branch, it pushes an update and skips opening a duplicate.

## New Machine Setup

```bash
git clone git@github.com:alon-shviki/game-portal.git ~/Desktop/game
bash ~/Desktop/game/setup.sh
```

`setup.sh` symlinks all scripts from `.claude/scripts/` into `~/.local/bin/`.

## Related

- [[Tech/Agentic Pipeline]] — full workflow including issue triage
- [[Tech/CI and Branch Protection]] — what CI checks the PRs
