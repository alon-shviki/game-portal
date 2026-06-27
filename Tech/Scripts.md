# Agentic Scripts

All scripts live in `.claude/scripts/` and are version-controlled in the portal repo. Run them with `bash .claude/scripts/<script>` or by path. No machine-local setup required.

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

To add a new game: add a row above **and** update `REPOS`/`ROOTS` maps in `.claude/scripts/start-issue` and `.claude/scripts/start-task`.

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
finish-issue
```

**Steps:**
1. Auto-detects `.Tests.csproj` and runs `dotnet test` — stops on failure
2. Commits all staged changes with the issue title as the commit message
3. Pushes branch to `origin`
4. Opens a PR with `Closes #<N>` in the body
5. Waits for CI (`gh pr checks --watch`)
6. If CI green: merges PR (squash), deletes remote branch, removes worktree, pulls `origin/main`
7. If CI fails: stops, leaves PR open for manual fix

## auto-pr

Run from **inside** a worktree (created by `start-task`) when non-issue work is done.

```bash
cd ~/Desktop/game/.worktrees/task-update-nginx-config
auto-pr "Update nginx config to add cache headers"
```

**Steps:**
1. Commits all changes
2. Pushes branch to `origin`
3. Opens a PR (does **not** auto-merge — review manually or wait for CI)
4. Removes the worktree and pulls `origin/main`

If a PR is already open for the branch, it pushes an update and skips opening a duplicate.

## Related

- [[Tech/Agentic Pipeline]] — full workflow including issue triage
- [[Tech/CI and Branch Protection]] — what CI checks the PRs
