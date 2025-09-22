#!/usr/bin/env bash
set -euo pipefail

# Starts the MCP Filesystem Server pointing to a local folder.
# Usage:
#   ./start-mcp-fs.sh                 # uses MCP_FS_ROOT from .env.mcp or env
#   ./start-mcp-fs.sh "/path/to/folder"
#
# Requirements:
#   - Node.js + npm
#   - npm install -g @modelcontextprotocol/server-filesystem

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Load .env.mcp if present (KEY=VALUE, ignore comments/blank lines)
ENV_FILE="$SCRIPT_DIR/.env.mcp"
if [[ -f "$ENV_FILE" ]]; then
  while IFS= read -r line || [[ -n "$line" ]]; do
    [[ -z "${line// }" || "${line}" =~ ^# ]] && continue
    if [[ "$line" == *"="* ]]; then
      key=${line%%=*}
      val=${line#*=}
      key=$(echo "$key" | xargs)
      val=$(echo "$val" | xargs)
      export "$key"="$val"
      printf 'Loaded: %s\n' "$key" >&2
    fi
  done < "$ENV_FILE"
else
  echo ".env.mcp not found (optional). You can set MCP_FS_ROOT via arg or env." >&2
fi

ROOT="${1:-}" 
if [[ -z "${ROOT}" ]]; then
  ROOT="${MCP_FS_ROOT:-}"
fi

if [[ -z "${ROOT}" ]]; then
  read -r -p "Enter folder path to expose: " ROOT
fi

if [[ ! -d "$ROOT" ]]; then
  echo "Folder not found: $ROOT" >&2
  exit 1
fi

echo "Root folder: $ROOT"
[[ -n "${DEBUG:-}" ]] && echo "DEBUG=$DEBUG"

if command -v mcp-server-filesystem >/dev/null 2>&1; then
  exec mcp-server-filesystem "$ROOT"
fi

if command -v npx >/dev/null 2>&1; then
  echo "Global binary not found. Falling back to: npx @modelcontextprotocol/server-filesystem" >&2
  exec npx @modelcontextprotocol/server-filesystem "$ROOT"
fi

echo "Command not found: mcp-server-filesystem" >&2
echo "Install globally: npm install -g @modelcontextprotocol/server-filesystem" >&2
echo "Or ensure npm global bin is on PATH: export PATH=\"$(npm bin -g):$PATH\"" >&2
exit 1
