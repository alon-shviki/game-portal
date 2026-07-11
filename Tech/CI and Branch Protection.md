# CI and Branch Protection

## What's Protected

All three repos — `game-portal`, `Bullet-Heaven`, `orbit-break` — have the same rules on `main`:

- Direct pushes blocked — all changes go through a PR
- `build` check must pass before merge
- Force pushes blocked
- Branch deletion blocked

## The `build` gate

Every repo runs the **same gate** on each PR + push, in this order:

1. **Cache NuGet** — `actions/cache` on `~/.nuget/packages`, keyed by `hashFiles('**/*.csproj')`
2. **Format check** — `dotnet format <project>.csproj --verify-no-changes` (per project; targets the `.csproj`, not the `.slnx`)
3. **Build** — `dotnet build -c Release`
4. **Test** — `dotnet test -c Release` (all three repos have a test project)

A red gate blocks merge. `push-image` runs only on merge to `main`.

## Workflow Files

**game-portal** — `.github/workflows/ci.yml`

| Job | Trigger | What it does |
|-----|---------|--------------|
| `build` | every PR + push | cache → format → `dotnet build portal-auth` → `dotnet test PortalAuth.Tests` |
| `push-image` | merge to main only | builds + pushes `ghcr.io/alon-shviki/portal-auth:latest` |

**Bullet-Heaven** — `.github/workflows/docker.yml`

| Job | Trigger | What it does |
|-----|---------|--------------|
| `build` | every PR + push | cache → format → `dotnet build` client → `dotnet test BulletHeaven.Tests` |
| `push-image` | merge to main only | builds + pushes `ghcr.io/alon-shviki/bh-client:latest` |

**orbit-break** — `.github/workflows/docker.yml`

| Job | Trigger | What it does |
|-----|---------|--------------|
| `build` | every PR + push | cache → format → `dotnet build` client → `dotnet test OrbitBreak.Tests` |
| `push-image` | merge to main only | builds + pushes `ghcr.io/alon-shviki/orbit-break-client:latest` |

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
