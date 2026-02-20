#!/bin/bash
# Demo 5: Estensioni Microsoft.Testing.Platform
cd demos/05-MTP-Extensions

# Mostra il .csproj (TestingExtensionsProfile=AllMicrosoft + coverlet.MTP)
cat ExtensionsDemo.csproj

# Build
dotnet build

# ── 1. Diagnostics (built-in) ──
# Mostra architettura, estensioni registrate, opzioni disponibili
dotnet run -- --info --filter "TestCategory!=CrashDemo&TestCategory!=HangDemo"

# ── 2. Output (Terminal Test Reporter, built-in) ──
# 2a. Output dettagliato: mostra anche i test passati (default: Normal, solo falliti)
dotnet run -- --output Detailed --filter "TestCategory!=CrashDemo&TestCategory!=HangDemo"

# 2b. Disabilita barra di progresso animata (utile in CI o redirect)
dotnet run -- --no-progress --filter "TestCategory!=CrashDemo&TestCategory!=HangDemo"

# 2c. Disabilita escape codes ANSI (utile per terminali senza supporto ANSI)
dotnet run -- --no-ansi --filter "TestCategory!=CrashDemo&TestCategory!=HangDemo"

# ── 3. Report TRX ──
# Genera un file Visual Studio Test Results (.trx) per CI/CD
dotnet run -- --report-trx --report-trx-filename TestResults.trx --filter "TestCategory!=CrashDemo&TestCategory!=HangDemo"

# ── 4. Azure DevOps Report ──
# In CI AzDo, gli errori appaiono come annotazioni nelle PR GitHub
dotnet run -- --report-azdo --filter "TestCategory!=CrashDemo&TestCategory!=HangDemo"

# ── 5. Code Coverage (Microsoft) ──
# Instrumentazione nativa (funziona su Linux/Windows, non su macOS ARM)
dotnet run -- --coverage --coverage-output-format cobertura --coverage-output coverage.xml --filter "TestCategory!=CrashDemo&TestCategory!=HangDemo"

# ── 6. Code Coverage (Coverlet MTP) ──
# Instrumentazione managed cross-platform (funziona anche su macOS ARM)
dotnet run -- --coverlet --coverlet-include-test-assembly --coverlet-include "[ExtensionsDemo]*" --coverlet-output-format cobertura --filter "TestCategory!=CrashDemo&TestCategory!=HangDemo"

# ── 7. Retry ──
# 7a. Riprova i test falliti fino a 3 volte
dotnet run -- --retry-failed-tests 3 --filter "TestCategory!=CrashDemo&TestCategory!=HangDemo"

# 7b. Soglia massima: non riprovare se piu' del 50% dei test fallisce
dotnet run -- --retry-failed-tests 3 --retry-failed-tests-max-percentage 50 --filter "TestCategory!=CrashDemo&TestCategory!=HangDemo"

# 7c. Soglia massima: non riprovare se piu' di 2 test falliscono
dotnet run -- --retry-failed-tests 3 --retry-failed-tests-max-tests 2 --filter "TestCategory!=CrashDemo&TestCategory!=HangDemo"

# ── 8. Crash Dump ──
# Genera un dump (.dmp) nella cartella TestResults/ quando il processo crasha.
# Il test CrashDemo provoca un crash con Environment.FailFast.
# Tipi: Mini, Full, Triage, Heap
dotnet run -- --crashdump --crashdump-type Heap --filter "TestCategory=CrashDemo" || true
ls -lh bin/Debug/net10.0/TestResults/*crash*.dmp 2>/dev/null
# Analizzare il dump con dotnet-dump (installare con: dotnet tool install -g dotnet-dump)
# dotnet-dump analyze bin/Debug/net10.0/TestResults/*crash*.dmp
# Esempio di comandi dotnet-dump:
# > clrstack

# ── 9. Hang Dump ──
# Genera un dump (.dmp) se i test non terminano entro il timeout.
# Il test HangDemo simula un blocco infinito.
# Tipi: Mini, Full, Triage, Heap
dotnet run -- --hangdump --hangdump-timeout 10s --hangdump-type Mini --filter "TestCategory=HangDemo" || true
ls -lh bin/Debug/net10.0/TestResults/*hang*.dmp 2>/dev/null
# Analizzare il dump con dotnet-dump
# dotnet-dump analyze bin/Debug/net10.0/TestResults/*hang*.dmp

# ── 10. Tutto insieme ──
# TRX + Coverage + Retry in un'unica esecuzione (esclude CrashDemo e HangDemo)
dotnet run -- --report-trx --report-trx-filename TestResults.trx --coverage --coverage-output-format cobertura --coverage-output coverage.xml --retry-failed-tests 3 --filter "TestCategory!=CrashDemo&TestCategory!=HangDemo"

# Nota: Microsoft Fakes (Microsoft.Testing.Extensions.Fakes) richiede VS Enterprise.
# Se disponibile, si abilita con EnableMicrosoftTestingExtensionsFakes nel .csproj.
