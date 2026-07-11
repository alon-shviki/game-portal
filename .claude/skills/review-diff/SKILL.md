---
name: review-diff
description: Pre-merge checklist review of the current worktree's diff against main. Use before finish-issue or auto-pr, or when the user says "review the diff" / "pre-merge check".
---

# review-diff

Run `git diff main...HEAD` in the current worktree and check the diff against this list. Report findings as a short bullet list — one line per hit, file:line, no essays. If everything passes, say so in one line.

## Checklist

1. **Secrets** — no hardcoded passwords, JWT keys, connection strings, or tokens. Config comes from env vars / `.env` (gitignored).
2. **`MapInboundClaims = false` untouched** — portal-auth endpoints read raw `sub`/`unique_name` claims; removing this silently breaks every authed endpoint.
3. **Migrations not hand-edited** — files under `Migrations/` only change via `dotnet ef migrations add`.
4. **Documentation Rule** — code changed but no `.md` touched? Flag it.
5. **CI workflow safety** — if `.github/workflows/` changed: the `build` job in `dotnet-ci.yml` must keep its name (`ci / build` is pinned by branch protection on all 3 repos AND orbit-break's ruleset). Changes take effect for all repos instantly (they call `@main`).
6. **sub_filter contract** — if a game's `index.html` changed: the literal `<base href="/"` string must survive (portal nginx rewrites it).
7. **Scope creep** — diff contains changes unrelated to the task/issue? Flag for a separate PR.

No subagents — run the checks inline in the current session.
