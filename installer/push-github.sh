#!/usr/bin/env bash
# Публикация на GitHub через Git Bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

GITHUB_USER="${GITHUB_USER:-SharikBot}"
REPO_NAME="${REPO_NAME:-DnD_Hub}"
REMOTE_URL="https://github.com/${GITHUB_USER}/${REPO_NAME}.git"

echo "==> DnD Character Manager — push to GitHub"
echo "    Remote: $REMOTE_URL"
echo ""
echo "Если репозитория ещё нет, создайте пустой:"
echo "  https://github.com/new?name=${REPO_NAME}"
echo "  (без README, .gitignore и license)"
echo ""
if ! git rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  echo "Git repository not found. Run from project root."
  exit 1
fi

if ! git remote get-url origin >/dev/null 2>&1; then
  git remote add origin "$REMOTE_URL"
  echo "Added remote origin"
else
  git remote set-url origin "$REMOTE_URL"
  echo "Updated remote origin to: $REMOTE_URL"
fi

echo "==> Ensure branch main"
git branch -M main

echo "==> Push"
git push -u origin main

echo ""
echo "Done: https://github.com/${GITHUB_USER}/${REPO_NAME}"
