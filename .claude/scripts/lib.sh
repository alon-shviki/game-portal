#!/usr/bin/env bash
# Shared helpers for the pipeline scripts (auto-pr, finish-issue).
# Single source of truth — game repos reference these via ~/Desktop/game/.claude/scripts,
# they do NOT keep their own copies (that drifts). See CLAUDE.md "Managed Repos".

# wait_for_ci <pr-number>
# Blocks until the PR's checks finish. Returns 0 if all green, 1 if any failed.
# Polls the check STATES rather than racing `gh pr checks --watch`: checks register
# a few seconds after `gh pr create` (slower for cross-repo reusable workflows), and
# --watch exits immediately if it's called in that gap, which used to false-report red.
# Empty state list = not registered yet (wait); any pending = wait; any failed = red.
wait_for_ci() {
  local pr="$1" i states
  for i in $(seq 1 90); do   # ~15 min ceiling at 10s/poll
    states=$(gh pr checks "$pr" --json state --jq '.[].state' 2>/dev/null || true)
    if [ -z "$states" ]; then
      echo "  waiting for checks to register (poll $i)…" >&2
      sleep 10; continue
    fi
    if grep -qiE 'FAIL|ERROR|CANCEL|TIMED_OUT|ACTION_REQUIRED|STARTUP_FAILURE|STALE' <<<"$states"; then
      return 1
    fi
    if grep -qiE 'PENDING|QUEUE|IN_PROGRESS|EXPECTED|REQUEST|WAIT' <<<"$states"; then
      sleep 10; continue
    fi
    return 0   # all terminal, none failed (SUCCESS / SKIPPED / NEUTRAL)
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

# remind_docs
# Documentation Rule nudge: if the STAGED change touches code but no .md doc,
# print a reminder. Non-blocking (always returns 0) and heuristic — it does not
# block trivial changes, it just makes "I forgot the docs" visible before the PR.
# Call after `git add -A`, before commit.
remind_docs() {
  local staged docs code
  staged=$(git diff --cached --name-only)
  [ -z "$staged" ] && return 0
  docs=$(echo "$staged" | grep -E '\.md$' || true)
  code=$(echo "$staged" | grep -vE '\.md$|\.obsidian/' || true)
  if [ -n "$code" ] && [ -z "$docs" ]; then
    echo "" >&2
    echo "  ⚠  Documentation Rule: this change touches code but no .md." >&2
    echo "     If it changes behavior / architecture / pipeline, update the relevant doc" >&2
    echo "     (portal Tech/… or the game's Notes/…) — ideally in this same PR." >&2
    echo "" >&2
  fi
  return 0
}
