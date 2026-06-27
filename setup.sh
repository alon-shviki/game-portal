#!/usr/bin/env bash
# Run once after cloning on a new machine.
# Symlinks the agentic scripts into ~/.local/bin so they're on PATH.
set -e

SCRIPTS="$(cd "$(dirname "$0")/.claude/scripts" && pwd)"
mkdir -p ~/.local/bin

for script in "$SCRIPTS"/*; do
  name=$(basename "$script")
  ln -sf "$script" ~/.local/bin/"$name"
  echo "linked: $name"
done

echo "Done. Make sure ~/.local/bin is in your PATH."
