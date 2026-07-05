---
description: Commit, push, and open a PR from the current worktree
argument-hint: "PR title"
---

From inside the task worktree, run `bash "$(git worktree list | awk 'NR==1{print $1}')/.claude/scripts/auto-pr" $ARGUMENTS`.

The script commits everything, pushes, opens the PR, and removes the worktree. The user merges manually.
