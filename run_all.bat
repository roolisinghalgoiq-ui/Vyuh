@echo off
title VYUH Engine - Platform Launcher
echo ====================================================================
echo             VYUH Options Rebalancing Platform Launcher
echo ====================================================================
echo.
echo Make sure local Redis (port 6379) and PostgreSQL (port 5432) are running.
echo.

echo 1. Launching Ingestion API (Port 5048)...
start "VYUH Ingestion Service" cmd /k "cd /d e:\VYUH\src\Services\Ingestion\VYUH.Ingestion.Api && dotnet run"
ping 127.0.0.1 -n 3 > nul

echo 2. Launching Optimizer API (Port 5206)...
start "VYUH Optimizer Service" cmd /k "cd /d e:\VYUH\src\Services\Optimizer\VYUH.Optimizer.Api && dotnet run"
ping 127.0.0.1 -n 3 > nul

echo 3. Launching Risk API (Port 5084)...
start "VYUH Risk Service" cmd /k "cd /d e:\VYUH\src\Services\Risk\VYUH.Risk.Api && dotnet run"
ping 127.0.0.1 -n 3 > nul

echo 4. Launching Gateway API (Port 5063)...
start "VYUH Gateway Service" cmd /k "cd /d e:\VYUH\src\Services\Gateway\VYUH.Gateway.Api && dotnet run"
ping 127.0.0.1 -n 4 > nul

echo 5. Launching Next.js Frontend Dashboard (Port 3000)...
start "VYUH Web UI Console" cmd /k "cd /d e:\VYUH\vyuh-ui && npm run dev"

echo.
echo ====================================================================
echo   All processes successfully initiated!
echo   - Interactive API Specs: http://localhost:5063/scalar/v1
echo   - Web UI Dashboard:     http://localhost:3000
echo ====================================================================
echo.
