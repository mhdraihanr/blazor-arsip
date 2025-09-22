<#
    Starts the MCP Filesystem Server pointing to a local folder.

    Usage examples (PowerShell):
      # Option A: Set via .env.mcp (MCP_FS_ROOT=<full_path>) then run:
      .\start-mcp-fs.ps1

      # Option B: Pass path as first argument:
      .\start-mcp-fs.ps1 "D:\\Projects\\MyFolder"

    Requirements:
      - Node.js + npm installed
      - Global package: @modelcontextprotocol/server-filesystem
        Install: npm install -g @modelcontextprotocol/server-filesystem

    Notes:
      - On first run, you may need: Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
#>

[CmdletBinding()]
param(
    [Parameter(Position=0)]
    [string]$Root
)

Write-Host "Starting MCP Filesystem Server..." -ForegroundColor Cyan

# Load environment variables from .env.mcp if present
if (Test-Path ".env.mcp") {
    Get-Content ".env.mcp" | ForEach-Object {
        $line = $_.Trim()
        if (-not [string]::IsNullOrWhiteSpace($line) -and -not $line.StartsWith('#') -and $line.Contains('=')) {
            $kv = $line.Split('=',2)
            if ($kv.Count -eq 2) {
                $name = $kv[0].Trim()
                $value = $kv[1].Trim()
                [System.Environment]::SetEnvironmentVariable($name, $value, "Process")
                Write-Host "Loaded: $name" -ForegroundColor DarkGray
            }
        }
    }
} else {
    Write-Host ".env.mcp not found (optional). You can set MCP_FS_ROOT via param or env." -ForegroundColor Yellow
}

# Resolve root folder priority: param > env > prompt
if (-not $Root -or [string]::IsNullOrWhiteSpace($Root)) {
    if ($env:MCP_FS_ROOT) {
        $Root = $env:MCP_FS_ROOT
    }
}

if (-not $Root -or [string]::IsNullOrWhiteSpace($Root)) {
    $Root = Read-Host "Enter folder path to expose (e.g. D:\\Projects\\MyFolder)"
}

if (-not (Test-Path -Path $Root -PathType Container)) {
    Write-Error "Folder not found: $Root"
    exit 1
}

Write-Host "Root folder: $Root" -ForegroundColor Green

# Ensure the server binary exists
$serverCmd = "mcp-server-filesystem"
$exists = $false
try {
    $null = Get-Command $serverCmd -ErrorAction Stop
    $exists = $true
} catch {
    $exists = $false
}

# Optional: enable debug by setting $env:DEBUG = "mcp:*"
if ($env:DEBUG) { Write-Host "DEBUG=$($env:DEBUG)" -ForegroundColor DarkGray }

Write-Host "Starting filesystem server..." -ForegroundColor Cyan
if ($exists) {
    & $serverCmd "$Root"
    exit $LASTEXITCODE
}

# Fallback to npx if global binary not found
$npxCmd = $null
try { $npxCmd = Get-Command npx -ErrorAction Stop } catch { $npxCmd = $null }
if ($npxCmd -ne $null) {
    Write-Host "Global binary not found. Falling back to: npx @modelcontextprotocol/server-filesystem" -ForegroundColor Yellow
    & npx "@modelcontextprotocol/server-filesystem" "$Root"
    exit $LASTEXITCODE
}

Write-Error "Command not found: mcp-server-filesystem"
Write-Host "Install globally: npm install -g @modelcontextprotocol/server-filesystem" -ForegroundColor Yellow
Write-Host "Or ensure npm global bin is on PATH: `$env:Path += ';' + (npm bin -g)" -ForegroundColor Yellow
exit 1
