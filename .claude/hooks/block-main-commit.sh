#!/usr/bin/env bash
# PreToolUse hook (Bash matcher): enforce the "never commit directly to main"
# hard rule from CLAUDE.md. Exit 2 = block the tool call.
input=$(cat)

# Only care about direct git commit/push commands.
printf '%s' "$input" | grep -qE 'git (commit|push)' || exit 0

# ponytail: `git -C <path>` targets another checkout — let it through,
# the guard is for the common mistake of plain `git commit` on main.
printf '%s' "$input" | grep -q 'git -C' && exit 0

branch=$(git branch --show-current 2>/dev/null)
if [ "$branch" = "main" ]; then
  echo "Blocked: you are on main — never commit/push directly to main (CLAUDE.md hard rule). Use start-issue or start-task to get a worktree." >&2
  exit 2
fi
exit 0
