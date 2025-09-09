# MCP GitHub Server Setup Guide

## 📋 Overview

Model Context Protocol (MCP) GitHub Server memungkinkan AI assistant untuk berinteraksi langsung dengan GitHub repositories, issues, pull requests, dan berbagai operasi GitHub lainnya.

## 🔧 Prerequisites

✅ **Sudah Terinstall:**
- Node.js v22.18.0
- npm v10.9.3
- @modelcontextprotocol/server-github

## 🔑 Step 1: Membuat GitHub Personal Access Token (PAT)

### 1.1 Akses GitHub Settings
1. Login ke [GitHub.com](https://github.com)
2. Klik **avatar Anda** (pojok kanan atas)
3. Pilih **"Settings"**
4. Scroll ke bawah, pilih **"Developer settings"** (sidebar kiri)
5. Klik **"Personal access tokens"**
6. Pilih **"Tokens (classic)"**

### 1.2 Generate New Token
1. Klik **"Generate new token"**
2. Pilih **"Generate new token (classic)"**
3. **Token name**: `MCP GitHub Server - Blazor Arsip`
4. **Expiration**: Pilih `90 days` atau `No expiration` (untuk development)

### 1.3 Select Permissions (Scopes)

**✅ Mandatory Permissions:**
- `repo` - **Full control of private repositories**
  - [x] repo:status
  - [x] repo_deployment
  - [x] public_repo
  - [x] repo:invite
  - [x] security_events
- `user` - **Update ALL user data**
  - [x] read:user
  - [x] user:email
  - [x] user:follow

**🔧 Optional (Recommended for Organizations):**
- `admin:org` - **Full control of orgs and teams**
- `gist` - **Create gists**
- `notifications` - **Access notifications**
- `workflow` - **Update GitHub Action workflows**

### 1.4 Generate Token
1. Scroll ke bawah, klik **"Generate token"**
2. **PENTING**: Copy token immediately dan simpan di tempat aman
3. Token format: `ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx`

⚠️ **WARNING**: Token hanya ditampilkan sekali! Simpan segera.

## ⚙️ Step 2: Konfigurasi Environment

### 2.1 Edit .env.mcp File
```bash
# Buka file .env.mcp yang sudah dibuat
notepad .env.mcp
```

### 2.2 Isi dengan Token GitHub
```env
# GitHub Personal Access Token untuk MCP
GITHUB_PERSONAL_ACCESS_TOKEN=ghp_your_actual_token_here_40_characters

# Contoh:
# GITHUB_PERSONAL_ACCESS_TOKEN=ghp_1234567890abcdef1234567890abcdef12345678
```

**Ganti `ghp_your_actual_token_here_40_characters` dengan token yang baru Anda buat.**

## 🚀 Step 3: Test MCP Server

### 3.1 Test Manual
```powershell
# Load environment dan jalankan server
.\start-mcp-github.ps1
```

### 3.2 Verifikasi Output
Jika berhasil, Anda akan melihat:
```
Starting MCP GitHub Server...
Loaded: GITHUB_PERSONAL_ACCESS_TOKEN
Token configured successfully
Starting MCP GitHub server...
GitHub MCP Server running on stdio
```

### 3.3 Test dengan Environment Variable
```powershell
# Set token untuk sesi saat ini
$env:GITHUB_PERSONAL_ACCESS_TOKEN="ghp_your_token_here"

# Test server
mcp-server-github
```

## 📡 Step 4: Konfigurasi MCP Client

### 4.1 Claude Desktop Configuration

Jika menggunakan Claude Desktop, edit file config:

**Windows**: `%APPDATA%\Claude\claude_desktop_config.json`

```json
{
  "mcpServers": {
    "github": {
      "command": "mcp-server-github",
      "env": {
        "GITHUB_PERSONAL_ACCESS_TOKEN": "ghp_your_token_here"
      }
    }
  }
}
```

### 4.2 Warp AI Terminal Configuration

Jika menggunakan Warp (terminal ini), MCP server perlu dijalankan secara terpisah:

```powershell
# Terminal 1: Start MCP Server
.\start-mcp-github.ps1

# Terminal 2: Connect to server (jika diperlukan)
# MCP client akan connect otomatis melalui stdio
```

## 🧪 Step 5: Test Functionality

### 5.1 Test Commands
```powershell
# Test 1: Cek server berjalan
Get-Process | Where-Object {$_.ProcessName -like "*node*"}

# Test 2: Verifikasi token
$env:GITHUB_PERSONAL_ACCESS_TOKEN
```

### 5.2 Test dengan GitHub API
```powershell
# Test akses ke GitHub API menggunakan token
$headers = @{
    "Authorization" = "token $env:GITHUB_PERSONAL_ACCESS_TOKEN"
    "Accept" = "application/vnd.github.v3+json"
}

# Test get user info
Invoke-RestMethod -Uri "https://api.github.com/user" -Headers $headers
```

## 📋 Step 6: Available MCP GitHub Operations

Setelah setup berhasil, MCP GitHub server mendukung operasi:

### Repository Operations
- List repositories
- Get repository details
- Create/update files
- List branches and tags
- Search code and repositories

### Issue Management
- List issues
- Create new issues
- Update issue status
- Add comments
- Assign issues

### Pull Request Operations
- List pull requests
- Create pull requests
- Review PRs
- Merge PRs
- Get PR diff

### Collaboration Features
- List collaborators
- Manage repository settings
- Webhook management
- GitHub Actions workflow triggers

## 🔒 Security Best Practices

### Token Security
1. **Never commit** token ke repository
2. Use **environment variables** only
3. Set **appropriate expiration** date
4. **Rotate tokens** regularly
5. Use **minimal required permissions**

### File Security
```bash
# Pastikan files ini di .gitignore
.env.mcp
.mcp-config.json
claude_desktop_config.json
```

## 🐛 Troubleshooting

### Common Issues

**1. "Authentication failed"**
```
Solution: Verify token di .env.mcp file
Check: Token tidak expired
Check: Token memiliki permissions yang tepat
```

**2. "Command not found: mcp-server-github"**
```bash
# Reinstall global
npm install -g @modelcontextprotocol/server-github

# Check PATH
npm list -g --depth=0
```

**3. "PowerShell execution policy"**
```powershell
# Allow script execution
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

**4. "Server not responding"**
```bash
# Check if server is running
Get-Process | Where-Object {$_.ProcessName -like "*node*"}

# Kill existing processes
Stop-Process -Name "node" -Force

# Restart server
.\start-mcp-github.ps1
```

### Debug Mode
```powershell
# Enable debug logging
$env:DEBUG="mcp:*"
mcp-server-github
```

## ✅ Verification Checklist

- [ ] GitHub Personal Access Token created
- [ ] Token saved in `.env.mcp` file
- [ ] Environment variables loaded correctly
- [ ] MCP server starts without errors
- [ ] GitHub API access working
- [ ] Security files in `.gitignore`
- [ ] Client configuration complete

## 🎯 Next Steps

After successful setup:
1. **Test repository operations** in AI assistant
2. **Create issues and PRs** through MCP
3. **Automate GitHub workflows**
4. **Integrate with CI/CD pipelines**

---

## 📞 Support

**GitHub API Documentation**: https://docs.github.com/en/rest  
**MCP Protocol**: https://modelcontextprotocol.io/  
**Token Management**: https://github.com/settings/tokens  

**Issues**: Check logs in PowerShell terminal for detailed error messages.
