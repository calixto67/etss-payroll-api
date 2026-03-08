# Build and run ETSS Payroll API (so Swagger and /api/v1/users are up to date).
# Run from repo root: .\scripts\build-and-run.ps1
# IMPORTANT: Stop any running API process first (Ctrl+C or stop in Visual Studio).

$ErrorActionPreference = "Stop"
$apiProject = "src\PayrollApi.API\PayrollApi.API.csproj"

Write-Host "Building API project..." -ForegroundColor Cyan
dotnet build $apiProject -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed. Stop the running API (e.g. stop debugging) and try again." -ForegroundColor Red
    exit 1
}

Write-Host "Build succeeded. Starting API..." -ForegroundColor Green
Write-Host "Swagger: http://localhost:5000/swagger  (or http://192.168.100.224:5000/swagger if on that host)" -ForegroundColor Yellow
dotnet run --project $apiProject -c Release --launch-profile http
