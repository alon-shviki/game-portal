#!/usr/bin/env bash
# Shared helpers for the pipeline scripts (auto-pr, finish-issue).
# Single source of truth — game repos reference these via ~/Desktop/game/.claude/scripts,
# they do NOT keep their own copies (that drifts). See CLAUDE.md "Managed Repos".

# wait_for_ci <pr-number>
# Blocks until the PR's checks finish. Returns 0 if all green, 1 if any failed.
# Checks register a few seconds after `gh pr create`, so retry while none exist yet;
# a genuine failure returns immediately.
wait_for_ci() {
  local pr="$1" attempt
  for attempt in $(seq 1 6); do
    if gh pr checks "$pr" --watch; then
      return 0
    fi
    if gh pr checks "$pr" 2>&1 | grep -qi "no checks reported"; then
      echo "  checks not registered yet (attempt $attempt/6) — retrying in 15s…" >&2
      sleep 15
      continue
    fi
    return 1   # checks ran and at least one failed
  done
  return 1
}

# remove_worktree <worktree-path> <main-repo-path>
# Fully deletes a worktree. `git worktree remove` leaves ignored files (bin/, obj/)
# behind, stranding the directory — so rm -rf the path and prune afterwards.
remove_worktree() {
  local wt="$1" main="$2"
  cd "$main"
  git worktree remove "$wt" --force 2>/dev/null || true
  rm -rf "$wt"
  git worktree prune
}
