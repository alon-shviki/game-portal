---
description: Create a worktree for a GitHub issue and load game context
argument-hint: <number> [slug]
---

Run `bash .claude/scripts/start-issue $ARGUMENTS` from the repo root.

If it prints a `LOAD GAME CONTEXT` block, read every file listed there before touching any code. Then work inside the printed worktree; when done, run `/finish-issue` from inside it.
