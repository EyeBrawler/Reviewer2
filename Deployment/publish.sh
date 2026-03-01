#!/usr/bin/env bash
set -euo pipefail

# ---------------------------------------
# Config
# ---------------------------------------
BRANCH="main"
CONFIGURATION="Release"

# ---------------------------------------
# Resolve paths
# ---------------------------------------
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
# ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
ROOT_DIR="$(git rev-parse --show-toplevel)"

PROJECT="$ROOT_DIR/Reviewer2/Reviewer2.Blazor/Reviewer2.Blazor.csproj"
PUBLISH_DIR="$ROOT_DIR/publish"

# ---------------------------------------
# Preconditions
# ---------------------------------------
cd "$ROOT_DIR"

if [ ! -d ".git" ]; then
  echo "ERROR: Repo root not found (no .git directory)."
  exit 1
fi

if [ ! -f "$PROJECT" ]; then
  echo "ERROR: Project file not found:"
  echo "  $PROJECT"
  exit 1
fi

# ---------------------------------------
# Git safety checks (pull-only)
# ---------------------------------------
echo "Verifying git working tree is clean..."

if ! git diff-index --quiet HEAD --; then
  echo
  echo "❌ Working tree is dirty."
  echo "    This machine should not have local changes."
  echo "    Reset or fix before publishing."
  echo
  exit 1
fi

echo "Fetching latest changes from origin..."
git fetch origin "$BRANCH"

echo "Fast-forwarding '$BRANCH'..."
git checkout "$BRANCH"
git pull --ff-only origin "$BRANCH"

# ---------------------------------------
# Publish
# ---------------------------------------
echo
echo "Publishing from commit:"
git --no-pager log -1 --oneline
echo

if [ -d "$PUBLISH_DIR" ]; then
  echo "Cleaning publish directory..."
  rm -rf "$PUBLISH_DIR"
fi

mkdir -p "$PUBLISH_DIR"

dotnet publish "$PROJECT" \
  -c "$CONFIGURATION" \
  -o "$PUBLISH_DIR"

echo
echo "Publish completed successfully."