# Demo 1: Essenziali di MTP
Set-Location demos/01-MTP-Basics

# --- Configurazione ---

# Mostra global.json (configura MTP come test runner di default)
Get-Content ..\global.json

# Mostra il .csproj classico (Microsoft.NET.Sdk: tutto va configurato a mano)
# NOTA: OutputType=Exe e' obbligatorio per MTP (il test project e' l'eseguibile runner)
Get-Content Classic\Classic.csproj

# Mostra il .csproj moderno (MSTest.Sdk: tutto automatico, incluso OutputType=Exe)
Get-Content Modern\MTP-Intro.csproj

# Mostra il .csproj helper (IsTestApplication=false: genera DLL, non EXE)
Get-Content Helpers\Helpers.csproj

# --- Helpers: build (produce una DLL, non un eseguibile) ---

Set-Location Helpers
dotnet build
Set-Location ..

# --- Classic: build e test ---

Set-Location Classic
dotnet build
dotnet run
Set-Location ..

# --- Modern: build e test ---

Set-Location Modern
dotnet build
dotnet run

# Esecuzione diretta del binario (piu' veloce)
.\bin\Debug\net10.0\MTP-Intro.exe

# Esecuzione tramite dotnet run
dotnet run

# Esecuzione tramite dotnet exec (utile per la DLL)
dotnet exec bin\Debug\net10.0\MTP-Intro.dll

# Help: argomenti CLI in stile Unix
.\bin\Debug\net10.0\MTP-Intro.exe --help

# Info: architettura, estensioni, ambiente
.\bin\Debug\net10.0\MTP-Intro.exe --info

# List tests: discovery senza esecuzione
.\bin\Debug\net10.0\MTP-Intro.exe --list-tests

# .NET 10: dotnet test nella cartella corrente
dotnet test
Set-Location ..

# Esecuzione parallela di piu' moduli di test
dotnet test --max-parallel-test-modules 4
