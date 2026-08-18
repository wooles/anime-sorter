# setup.ps1 — anime-sorter development environment setup
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "    anime-sorter (sort.moe) Setup        " -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

# 1. Check Git
if (Get-Command git -ErrorAction SilentlyContinue) {
    Write-Host "[OK] Git is available: $(git --version)" -ForegroundColor Green
} else {
    Write-Host "[WARN] Git not found in PATH." -ForegroundColor Yellow
}

# 2. Check Python (for server.py)
if (Get-Command python -ErrorAction SilentlyContinue) {
    Write-Host "[OK] Python is available: $(python --version)" -ForegroundColor Green
} else {
    Write-Host "[INFO] Python is not installed. You can still open index.html directly in browser." -ForegroundColor Gray
}

# 3. Check .NET SDK (for mal-proxy)
$dotnetSdks = & dotnet --list-sdks 2>$null
if ($LASTEXITCODE -eq 0 -and $dotnetSdks) {
    Write-Host "[OK] .NET SDK is available" -ForegroundColor Green
    if (Test-Path "mal-proxy/MalProxy.csproj") {
        Write-Host "[*] Restoring .NET dependencies for mal-proxy..." -ForegroundColor Cyan
        dotnet restore mal-proxy/MalProxy.csproj
    }
} else {
    Write-Host "[INFO] .NET SDK not detected (optional, only needed for local mal-proxy microservice)." -ForegroundColor Gray
}

Write-Host "`n[SUCCESS] Environment checked! Ready to work on sort.moe." -ForegroundColor Green
Write-Host "Run 'python server.py' or open 'index.html' in your browser." -ForegroundColor Cyan
