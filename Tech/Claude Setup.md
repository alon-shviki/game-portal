# Claude Setup

How the `.claude/` folder is organized and how to extend it. Obsidian can't index hidden folders, so this doc is the visible map.

## Layout

| Path | What |
|------|------|
| `.claude/rules/*.md` | Always-loaded project rules — architecture, obsidian vault, adding a game |
| `.claude/scripts/` | Workflow scripts (`start-issue`, `start-task`, `finish-issue`, `auto-pr`) + shared `lib.sh` — the **single source** for every repo; see [[Scripts]] |
| `.claude/skills/*/SKILL.md` | `pick-work` (issue triage — [[Agentic Pipeline]]), `ci-cd` (pipeline reference), `obsidian-vault` (note management), `review-diff` (pre-merge checklist on the worktree diff — run before `finish-issue`/`auto-pr`) |
| `.claude/commands/*.md` | Slash commands: `/start-issue`, `/finish-issue`, `/start-task`, `/auto-pr` — thin wrappers over the scripts |
| `.claude/hooks/` + `.claude/settings.json` | Two PreToolUse guards: `block-main-commit.sh` blocks `git commit`/`git push` on `main`, and `block-main-edit.sh` blocks Edit/Write to files in the main checkout — both enforce the worktree-only hard rule. Plus a PostToolUse→Stop **build gate**: any `.cs` edit flags `/tmp/portal-build-pending`; on Stop, `PortalAuth.Tests` is compiled (portal-auth builds transitively) in the worktree that was edited — a broken build blocks the session from finishing. `settings.json` also carries `permissions`: read-deny on `.env`/usersecrets, allow-list for the boring read-only + dotnet commands |
| `.claude/agents/` | No agent definitions (just a `README` explaining why) — pipeline steps run inline in the main session |
| `.claude/settings.local.json` | Per-machine overrides — gitignored, never commit |

## Extending

- **New skill**: `.claude/skills/<name>/SKILL.md` with `name` + `description` frontmatter; body is the instructions Claude follows when it triggers.
- **New slash command**: `.claude/commands/<name>.md` — frontmatter `description` (+ optional `argument-hint`), body is the prompt; `$ARGUMENTS` expands to what the user typed.
- **New hook**: script in `.claude/hooks/`, register it under `hooks` in `.claude/settings.json`. Exit 2 from a PreToolUse hook blocks the tool call.
- **Agents**: only if a task truly needs isolation — `.claude/agents/<name>.md`. Default is inline.
- **Plugins**: installed per-user via `/plugin` in Claude Code — not versioned in this repo.

## Related

[[Scripts]] · [[Agentic Pipeline]] · [[Architecture]]
