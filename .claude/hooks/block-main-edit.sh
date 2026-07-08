#!/usr/bin/env bash
# PreToolUse hook (Edit|Write matcher): enforce "work in a worktree".
# The point isn't the branch — it's that code changes happen in a linked
# worktree (from start-issue/start-task), not the main checkout. Exit 2 = block.
input=$(cat)

file=$(printf '%s' "$input" | jq -r '.tool_input.file_path // empty')
[ -z "$file" ] && exit 0

dir=$(dirname "$file")
top=$(git -C "$dir" rev-parse --show-toplevel 2>/dev/null) || exit 0

# Main worktree is always the first line of `git worktree list` (see finish-issue).
main_wt=$(git -C "$dir" worktree list 2>/dev/null | head -1 | awk '{print $1}')
if [ "$top" = "$main_wt" ]; then
  echo "Blocked: '$file' is in the main checkout, not a worktree (CLAUDE.md hard rule) — run start-issue <N> or start-task <name> first, then edit there." >&2
  exit 2
fi
exit 0
