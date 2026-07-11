---
description: Commit, push, and open a PR from the current worktree
argument-hint: "PR title"
---

From inside the task worktree, run:

```bash
bash "$(git worktree list | awk 'NR==1{print $1}')/.claude/scripts/auto-pr" "<title>" "<body>"
```

**Always pass a real body** (second argument): 2–6 lines of markdown covering what changed, why, and how it was verified. Don't rely on the auto-generated fallback when you know the context.

The script commits everything, pushes, opens the PR, and removes the worktree. The user merges manually.
