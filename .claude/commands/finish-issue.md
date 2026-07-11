---
description: Tests → push → PR → wait for CI → merge (run from inside an issue worktree)
---

Recommended: run the `review-diff` skill first (pre-merge checklist on the worktree's diff).

From inside the issue worktree, run `bash "$(git worktree list | awk 'NR==1{print $1}')/.claude/scripts/finish-issue"`.

If tests or CI fail, stop and report the failure — do not force anything. On success the script merges the PR and removes the worktree.
