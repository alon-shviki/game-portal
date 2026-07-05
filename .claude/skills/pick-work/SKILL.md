---
name: pick-work
description: Fetch open issues from all managed repos (portal, Bullet Heaven, Orbit Break), score and rank them, present the top 5. Use when the user asks "what should I work on" or wants to pick the next task.
---

# Pick Work

1. Fetch open issues from every managed repo (repos listed in `CLAUDE.md` → Managed Repos):

```bash
gh issue list --repo alon-shviki/game-portal   --state open --json number,title,labels,reactionGroups,createdAt
gh issue list --repo alon-shviki/Bullet-Heaven --state open --json number,title,labels,reactionGroups,createdAt
gh issue list --repo alon-shviki/orbit-break   --state open --json number,title,labels,reactionGroups,createdAt
```

2. Score each issue (from `Tech/Agentic Pipeline.md`):

| Signal | Points |
|--------|--------|
| `bug` label | +3 |
| `priority:high` | +3 |
| `priority:medium` | +2 |
| `priority:low` | +1 |
| Each 👍 reaction | +1 |
| Every 7 days old | +1 |

3. Present the top 5 as a ranked table — repo, issue #, title, score, one-line reasoning — and ask which one to start.

4. On pick: `bash .claude/scripts/start-issue <number> <slug>`, then read **every** file in the LOAD GAME CONTEXT block before writing any code.
