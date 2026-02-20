#!/bin/bash
# Demo 1: Essenziali di MTP
cd demos/01-MTP-Basics

# --- Configurazione ---

# Mostra global.json (configura MTP come test runner di default)
cat ../global.json

# Mostra il .csproj classico (Microsoft.NET.Sdk: tutto va configurato a mano)
# NOTA: OutputType=Exe e' obbligatorio per MTP (il test project e' l'eseguibile runner)
cat Classic/Classic.csproj

# Mostra il .csproj moderno (MSTest.Sdk: tutto automatico, incluso OutputType=Exe)
cat Modern/MTP-Intro.csproj

# Mostra il .csproj helper (IsTestApplication=false: genera DLL, non EXE)
cat Helpers/Helpers.csproj

# --- Helpers: build (produce una DLL, non un eseguibile) ---

cd Helpers
dotnet build
cd ..

# --- Classic: build e test ---

cd Classic
dotnet build
dotnet run
cd ..

# --- Modern: build e test ---

cd Modern
dotnet build
dotnet run

# Esecuzione diretta del binario
./bin/Debug/net10.0/MTP-Intro

# Esecuzione tramite dotnet run
dotnet run

# Esecuzione tramite dotnet exec (utile per la DLL)
dotnet exec bin/Debug/net10.0/MTP-Intro.dll

# Help: argomenti CLI in stile Unix
./bin/Debug/net10.0/MTP-Intro --help

# Info: architettura, estensioni, ambiente
./bin/Debug/net10.0/MTP-Intro --info

# List tests: discovery senza esecuzione
./bin/Debug/net10.0/MTP-Intro --list-tests

# .NET 10: dotnet test nella cartella corrente
dotnet test
cd ..

# Esecuzione parallela di piu' moduli di test
dotnet test --max-parallel-test-modules 4
