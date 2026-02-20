#!/bin/bash
set -e

echo "================================================"
echo "Microsoft.Testing.Platform Demo Validation"
echo "================================================"
echo ""

# Check .NET SDK version
echo "Checking .NET SDK version..."
dotnet --version

echo ""
echo "Building and testing all demo projects..."
echo ""

cd demos

# Test each demo
for demo in */; do
    demo_name="${demo%/}"
    echo "----------------------------------------"
    echo "Testing: $demo_name"
    echo "----------------------------------------"

    cd "$demo_name"

    # Build and run tests with demo-specific handling
    case "$demo_name" in
        01-MTP-Basics)
            # Two sub-projects: Classic and Modern
            for sub in Classic Modern; do
                echo "Building $sub..."
                dotnet build "$sub" --configuration Release
                echo "Running tests ($sub)..."
                dotnet test --project "$sub" --no-build --configuration Release
                echo "✓ $sub passed"
            done
            ;;
        02-Ecosystem-MSTest-xUnit)
            echo "Building..."
            dotnet build --configuration Release
            # Workaround: dotnet test has a known bug with xUnit v3 in .NET 10
            # (KeyNotFoundException: The given key '8' was not present)
            # Use dotnet run instead
            echo "Running tests..."
            dotnet run --configuration Release
            ;;
        05-MTP-Extensions)
            echo "Building..."
            dotnet build --configuration Release
            # This demo has flaky/crash/hang tests for extension demos.
            # Exclude CrashDemo and HangDemo categories (they crash/hang by design).
            echo "Running tests..."
            dotnet test --no-build --configuration Release -- --retry-failed-tests 3 --filter "TestCategory!=CrashDemo&TestCategory!=HangDemo"
            ;;
        06-CI-CD-Retry)
            echo "Building..."
            dotnet build --configuration Release
            # This demo has intentionally flaky tests (40% failure rate).
            # Use --retry-failed-tests to handle them, as designed.
            echo "Running tests..."
            dotnet test --no-build --configuration Release -- --retry-failed-tests 5
            ;;
        *)
            echo "Building..."
            dotnet build --configuration Release
            echo "Running tests..."
            dotnet test --no-build --configuration Release
            ;;
    esac

    echo "✓ $demo_name passed"
    echo ""

    cd ..
done

echo "================================================"
echo "All demos passed successfully! ✓"
echo "================================================"
