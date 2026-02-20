# Microsoft.Testing.Platform Demo Validation Script (PowerShell)

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "Microsoft.Testing.Platform Demo Validation" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Check .NET SDK version
Write-Host "Checking .NET SDK version..." -ForegroundColor Yellow
dotnet --version

Write-Host ""
Write-Host "Building and testing all demo projects..." -ForegroundColor Yellow
Write-Host ""

Set-Location demos

# Test each demo
$demos = Get-ChildItem -Directory
foreach ($demo in $demos) {
    Write-Host "----------------------------------------" -ForegroundColor Gray
    Write-Host "Testing: $($demo.Name)" -ForegroundColor White
    Write-Host "----------------------------------------" -ForegroundColor Gray

    Set-Location $demo.Name

    # Build and run tests with demo-specific handling
    switch ($demo.Name) {
        "01-MTP-Basics" {
            # Two sub-projects: Classic and Modern
            foreach ($sub in @("Classic", "Modern")) {
                Write-Host "Building $sub..." -ForegroundColor Gray
                dotnet build "$sub" --configuration Release
                if ($LASTEXITCODE -ne 0) { Write-Host "Build failed for $sub" -ForegroundColor Red; exit 1 }
                Write-Host "Running tests ($sub)..." -ForegroundColor Gray
                dotnet test --project "$sub" --no-build --configuration Release
                if ($LASTEXITCODE -ne 0) { Write-Host "Tests failed for $sub" -ForegroundColor Red; exit 1 }
                Write-Host "✓ $sub passed" -ForegroundColor Green
            }
        }
        "02-Ecosystem-MSTest-xUnit" {
            Write-Host "Building..." -ForegroundColor Gray
            dotnet build --configuration Release
            if ($LASTEXITCODE -ne 0) { Write-Host "Build failed for $($demo.Name)" -ForegroundColor Red; exit 1 }
            # Workaround: dotnet test has a known bug with xUnit v3 in .NET 10
            # (KeyNotFoundException: The given key '8' was not present)
            # Use dotnet run instead
            Write-Host "Running tests..." -ForegroundColor Gray
            dotnet run --configuration Release
            if ($LASTEXITCODE -ne 0) { Write-Host "Tests failed for $($demo.Name)" -ForegroundColor Red; exit 1 }
        }
        "05-MTP-Extensions" {
            Write-Host "Building..." -ForegroundColor Gray
            dotnet build --configuration Release
            if ($LASTEXITCODE -ne 0) { Write-Host "Build failed for $($demo.Name)" -ForegroundColor Red; exit 1 }
            # This demo has flaky/crash/hang tests for extension demos.
            # Exclude CrashDemo and HangDemo categories (they crash/hang by design).
            Write-Host "Running tests..." -ForegroundColor Gray
            dotnet test --no-build --configuration Release -- --retry-failed-tests 3 --filter "TestCategory!=CrashDemo&TestCategory!=HangDemo"
            if ($LASTEXITCODE -ne 0) { Write-Host "Tests failed for $($demo.Name)" -ForegroundColor Red; exit 1 }
        }
        "06-CI-CD-Retry" {
            Write-Host "Building..." -ForegroundColor Gray
            dotnet build --configuration Release
            if ($LASTEXITCODE -ne 0) { Write-Host "Build failed for $($demo.Name)" -ForegroundColor Red; exit 1 }
            # This demo has intentionally flaky tests (40% failure rate).
            # Use --retry-failed-tests to handle them, as designed.
            Write-Host "Running tests..." -ForegroundColor Gray
            dotnet test --no-build --configuration Release -- --retry-failed-tests 5
            if ($LASTEXITCODE -ne 0) { Write-Host "Tests failed for $($demo.Name)" -ForegroundColor Red; exit 1 }
        }
        default {
            Write-Host "Building..." -ForegroundColor Gray
            dotnet build --configuration Release
            if ($LASTEXITCODE -ne 0) { Write-Host "Build failed for $($demo.Name)" -ForegroundColor Red; exit 1 }
            Write-Host "Running tests..." -ForegroundColor Gray
            dotnet test --no-build --configuration Release
            if ($LASTEXITCODE -ne 0) { Write-Host "Tests failed for $($demo.Name)" -ForegroundColor Red; exit 1 }
        }
    }

    Write-Host "✓ $($demo.Name) passed" -ForegroundColor Green
    Write-Host ""

    Set-Location ..
}

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "All demos passed successfully! ✓" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan
