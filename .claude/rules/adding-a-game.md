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

**Do NOT copy the pipeline scripts** (`start-issue`, `start-task`, `auto-pr`, `finish-issue`, `lib.sh`) into the game repo — they live only in the portal and are repo-agnostic (they auto-detect the repo from cwd). Reference them by absolute path from the game's `CLAUDE.md`, exactly like Bullet Heaven does (`bash ~/Desktop/game/.claude/scripts/start-issue <n>`). Add a `.claude/scripts/` folder only for genuinely game-specific scripts.

Add the game to `REPOS`/`ROOTS` in both `.claude/scripts/start-issue` and `.claude/scripts/start-task` in the portal repo so `start-issue <N>` and `start-task <name>` auto-detect it.

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

**CI workflow** — create a thin `.github/workflows/ci.yml` that **calls the portal's reusable workflow** (don't hand-write the build steps — they're single-source in `game-portal/.github/workflows/dotnet-ci.yml`):
```yaml
name: CI
on:
  pull_request: { branches: [main] }
  push:         { branches: [main] }
permissions:
  contents: read
  packages: write
jobs:
  ci:
    uses: alon-shviki/game-portal/.github/workflows/dotnet-ci.yml@main
    with:
      project: <Client>/<Client>.csproj
      tests: <Tests>/<Tests>.csproj
      image: <game>-client
      context: .
      dockerfile: <Client>/Dockerfile
```

**Branch protection** (after first CI run) — the required check is **`ci / build`** (the reusable workflow's `build` job under the caller job `ci`):
```bash
gh api repos/alon-shviki/<game-name>/branches/main/protection --method PUT --input - <<'EOF'
{
  "required_status_checks": { "strict": true, "contexts": ["ci / build"] },
  "enforce_admins": false,
  "required_pull_request_reviews": { "required_approving_review_count": 0 },
  "restrictions": null
}
EOF
```

## 8. Wire Docker Compose

Add the new game's nginx container to `~/Desktop/game/docker-compose.yml`. See `architecture.md` for the proxy pattern.
