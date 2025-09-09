# Start MCP GitHub Server
# Pastikan file .env.mcp sudah dikonfigurasi dengan token GitHub yang valid

Write-Host "Starting MCP GitHub Server..." -ForegroundColor Green

# Load environment variables dari .env.mcp
if (Test-Path ".env.mcp") {
    Get-Content ".env.mcp" | ForEach-Object {
        if ($_ -match '^([^#][^=]*)=(.*)$') {
            [System.Environment]::SetEnvironmentVariable($matches[1], $matches[2], "Process")
            Write-Host "Loaded: $($matches[1])" -ForegroundColor Yellow
        }
    }
} else {
    Write-Error ".env.mcp file not found. Please create it with your GitHub token."
    exit 1
}

# Check if token is set
$token = [System.Environment]::GetEnvironmentVariable("GITHUB_PERSONAL_ACCESS_TOKEN", "Process")
if (-not $token -or $token -eq "YOUR_TOKEN_HERE") {
    Write-Error "GitHub Personal Access Token not configured in .env.mcp"
    Write-Host "Please edit .env.mcp and set your GitHub token" -ForegroundColor Yellow
    exit 1
}

Write-Host "Token configured successfully" -ForegroundColor Green
Write-Host "Starting MCP GitHub server..." -ForegroundColor Blue

# Start MCP server
try {
    mcp-server-github
} catch {
    Write-Error "Failed to start MCP GitHub server: $_"
}
