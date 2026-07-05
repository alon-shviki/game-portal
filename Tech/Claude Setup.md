# Claude Setup

How the `.claude/` folder is organized and how to extend it. Obsidian can't index hidden folders, so this doc is the visible map.

## Layout

| Path | What |
|------|------|
| `.claude/rules/*.md` | Always-loaded project rules — architecture, obsidian vault, adding a game |
| `.claude/scripts/` | Workflow scripts (`start-issue`, `finish-issue`, `start-task`, `auto-pr`) — see [[Scripts]] |
| `.claude/skills/pick-work/` | "What should I work on" triage skill — scoring rules from [[Agentic Pipeline]] |
| `.claude/commands/*.md` | Slash commands: `/start-issue`, `/finish-issue`, `/start-task`, `/auto-pr` — thin wrappers over the scripts |
| `.claude/hooks/` + `.claude/settings.json` | PreToolUse guard that blocks `git commit`/`git push` while on `main` (enforces the CLAUDE.md hard rule) |
| `.claude/agents/` | Intentionally empty — pipeline steps run inline in the main session (see its README) |
| `.claude/settings.local.json` | Per-machine overrides — gitignored, never commit |

## Extending

- **New skill**: `.claude/skills/<name>/SKILL.md` with `name` + `description` frontmatter; body is the instructions Claude follows when it triggers.
- **New slash command**: `.claude/commands/<name>.md` — frontmatter `description` (+ optional `argument-hint`), body is the prompt; `$ARGUMENTS` expands to what the user typed.
- **New hook**: script in `.claude/hooks/`, register it under `hooks` in `.claude/settings.json`. Exit 2 from a PreToolUse hook blocks the tool call.
- **Agents**: only if a task truly needs isolation — `.claude/agents/<name>.md`. Default is inline.
- **Plugins**: installed per-user via `/plugin` in Claude Code — not versioned in this repo.

## Related

[[Scripts]] · [[Agentic Pipeline]] · [[Architecture]]
