# Demo 2: Ecosistema - xUnit su MTP
Set-Location demos/02-Ecosystem-MSTest-xUnit

# Mostra il .csproj (UseMicrosoftTestingPlatformRunner=true per xUnit v3)
Get-Content Ecosystem.csproj

# Build
dotnet build

# Info: stessa struttura di output della Demo 1 (Piattaforma Unificata)
.\bin\Debug\net10.0\Ecosystem.exe --info

# Esecuzione test (dotnet run perche' dotnet test ha un bug con xUnit v3 in .NET 10)
dotnet run

# List tests
.\bin\Debug\net10.0\Ecosystem.exe --list-tests
