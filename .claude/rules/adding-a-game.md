# Adding a New Game

Full checklist. Do these in order.

## 1. Create the Game Repo

```bash
mkdir ~/Desktop/<game-name>
cd ~/Desktop/<game-name>
git init
git remote add origin git@github.com:alon-shviki/<game-name>.git
```

Write a `CLAUDE.md` at the root — see BH's as a template. Must include:
- Stack-specific commands (run, test, build)
- Portal integration notes (auth, scores, leaderboard)
- Hard rules for this game's architecture
- The standard **Issue Triage**, **Agentic Workflow**, and **Auto-Issue Rule** sections (copy from BH's `CLAUDE.md`)

Create `setup.sh` at the repo root — see BH's as a template. It must:
- Symlink shared scripts from portal (`~/Desktop/game/.claude/scripts/`) into `~/.local/bin/`
- Symlink any game-specific scripts from `.claude/scripts/` into `~/.local/bin/`
- Print a helpful error if portal is not cloned

Create `.claude/scripts/` for any game-specific scripts (may be empty at first).

Add the game to `REPOS`/`ROOTS` in `~/Desktop/game/.claude/scripts/start-issue` so `start-issue <N>` auto-detects it.

## 2. Game Repo Gitignore

```
.obsidian/
.worktrees/
.env
.env.*
.claude/settings.local.json
```
Plus whatever the stack generates (`bin/`, `obj/`, `node_modules/`, etc.).

## 3. Init the Game Vault

```bash
mkdir .obsidian
echo '{"userIgnoreFilters":["node_modules","bin","obj"]}' > .obsidian/app.json
```

Create a `Notes/` folder:
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

Create `~/Desktop/game/Games/<Game Name>.md` hub page:
- 1-paragraph description
- Tech stack list
- Current status (done / pending)
- Wiki links into game notes via the symlink

## 5. Update Portal Docs

- `Home.md` — add row to the games table
- Create a GitHub issue on `game-portal` for the new game milestone (don't edit Roadmap)

## 6. Wire Auth

Game server must NOT have its own `/register` or `/login`. It validates JWTs from the portal auth server. Same signing key, set via environment variable. See `architecture.md`.

## 7. GitHub Setup (do this before first push)

**Make the repo public** (required for free-tier branch protection):
```bash
gh repo edit alon-shviki/<game-name> --visibility public --accept-visibility-change-consequences
```

**Issue labels:**
```bash
gh label create "bug"              --color "d73a4a" --description "Something isn't working"    --repo alon-shviki/<game-name> --force
gh label create "enhancement"      --color "a2eeef" --description "New feature or request"     --repo alon-shviki/<game-name> --force
gh label create "question"         --color "d876e3" --description "Problem or decision needed"  --repo alon-shviki/<game-name> --force
gh label create "priority:high"    --color "e11d48" --description "Must fix soon"              --repo alon-shviki/<game-name> --force
gh label create "priority:medium"  --color "f59e0b" --description "Important, not urgent"      --repo alon-shviki/<game-name> --force
gh label create "priority:low"     --color "6b7280" --description "Nice to have"               --repo alon-shviki/<game-name> --force
gh label create "good first issue" --color "7057ff" --description "Good for newcomers"         --repo alon-shviki/<game-name> --force
```

**Issue templates** — copy from portal:
```bash
mkdir -p .github/ISSUE_TEMPLATE
cp ~/Desktop/game/.github/ISSUE_TEMPLATE/*.md .github/ISSUE_TEMPLATE/
```

**CI workflow** — create `.github/workflows/ci.yml` matching the game's stack. At minimum:
- Run tests on every PR targeting `main`
- Push image to GHCR on merge to `main`

**Branch protection** (after first CI run):
```bash
gh api repos/alon-shviki/<game-name>/branches/main/protection --method PUT --input - <<'EOF'
{
  "required_status_checks": { "strict": true, "contexts": ["build"] },
  "enforce_admins": false,
  "required_pull_request_reviews": { "required_approving_review_count": 0 },
  "restrictions": null
}
EOF
```

## 8. Wire Docker Compose

Add the new game's nginx container to `~/Desktop/game/docker-compose.yml`. See `architecture.md` for the proxy pattern.
