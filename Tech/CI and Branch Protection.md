# CI and Branch Protection

## What's Protected

Both `game-portal` and `Bullet-Heaven` have the same rules on `main`:

- Direct pushes blocked — all changes go through a PR
- `build` check must pass before merge
- Force pushes blocked
- Branch deletion blocked

## Workflow Files

**game-portal** — `.github/workflows/ci.yml`

| Job | Trigger | What it does |
|-----|---------|--------------|
| `build` | every PR + push | `dotnet build portal-auth/PortalAuth.csproj` |
| `push-image` | merge to main only | builds + pushes `ghcr.io/alon-shviki/portal-auth:latest` |

**Bullet-Heaven** — `.github/workflows/docker.yml`

| Job | Trigger | What it does |
|-----|---------|--------------|
| `build` | every PR + push | `dotnet build` client + `dotnet test BulletHeaven.Tests` |
| `push-image` | merge to main only | builds + pushes `ghcr.io/alon-shviki/bh-client:latest` |

## Issue Labels

Both repos share the same label set:

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
