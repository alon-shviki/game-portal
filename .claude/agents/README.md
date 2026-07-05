# Agents — intentionally empty

Pipeline steps (QA, tests, docs, e2e) run **inline** in the main session. Spawning per-game subagents from the portal was tried and rejected: each agent starts cold and re-derives context the session already has.

If a task genuinely needs an isolated agent, define it here as `<name>.md` with frontmatter (`name`, `description`, `tools`) — but default to inline.
