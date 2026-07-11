# CI and Branch Protection

## What's Protected

All three repos — `game-portal`, `Bullet-Heaven`, `orbit-break` — have the same rules on `main`:

- Direct pushes blocked — all changes go through a PR
- `ci / build` check must pass before merge
- Force pushes blocked
- Branch deletion blocked

## One reusable workflow, three callers

CI is **single-source**, like the pipeline scripts. The gate lives once in the portal repo and every repo calls it:

- **`game-portal/.github/workflows/dotnet-ci.yml`** — the reusable workflow (`on: workflow_call`). Two jobs:
  - `build` — cache NuGet (`~/.nuget/packages`, keyed by `hashFiles('**/*.csproj')`) → `dotnet format <project> <tests> --verify-no-changes` → `dotnet build -c Release` → vulnerable-package gate (`dotnet list package --vulnerable --include-transitive`, failed by grep) → `dotnet test -c Release`
  - `push-image` — on push to `main` only, builds + pushes the image to GHCR tagged `:latest` **and** `:sha-<git-sha>` (immutable rollback target)
- **Each repo's `ci.yml`** is a thin caller that passes its own paths as inputs (`project`, `tests`, `image`, `context`, `dockerfile`):

| Repo | Caller | Inputs (project / tests / image) |
|------|--------|----------------------------------|
| `game-portal` | `ci.yml` → `./.github/workflows/dotnet-ci.yml` (local ref) | `portal-auth` / `PortalAuth.Tests` / `portal-auth` |
| `Bullet-Heaven` | `ci.yml` → `…/dotnet-ci.yml@main` | `BulletHeaven.Client` / `BulletHeaven.Tests` / `bh-client` |
| `orbit-break` | `ci.yml` → `…/dotnet-ci.yml@main` | `OrbitBreak.Client` / `OrbitBreak.Tests` / `orbit-break-client` |

**Check name:** because a caller job (`ci`) invokes the reusable workflow, the status check is reported as **`ci / build`** (not `build`) — that's the required context in branch protection. **Never rename the `build` job**: `ci / build` is pinned by branch protection on all three repos AND by orbit-break's ruleset — renaming it silently un-gates merges until both layers are updated. Editing `dotnet-ci.yml` changes the gate for all three repos at once. Pin the games to a tag/SHA instead of `@main` if you want changes to roll out deliberately rather than immediately.

## Issue Labels

All three repos share the same label set:

| Label | Colour | Meaning |
|-------|--------|---------|
| `bug` | red | Something is broken |
| `enhancement` | blue | New feature |
| `question` | purple | Problem or decision needed |
| `priority:high` | bright red | Must fix soon |
| `priority:medium` | amber | Important, not urgent |
| `priority:low` | grey | Nice to have |
| `good first issue` | violet | Good for newcomers |

## Adding a New Game

New games must be public (free-tier branch protection requirement) and follow the same setup. See [[Tech/Adding a New Game]] — step 7 is the full GitHub setup script.

## Related

- [[Tech/Agentic Pipeline]] — how PRs are opened and merged automatically
- [[Tech/Adding a New Game]] — full setup checklist including CI and branch protection
