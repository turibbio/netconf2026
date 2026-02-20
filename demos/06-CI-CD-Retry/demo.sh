#!/bin/bash
# Demo 6: CI/CD — Retry, Pipeline Azure DevOps e GitHub Actions
cd demos/06-CI-CD-Retry

# ── 1. Struttura: codice di produzione separato dai test ──
# PaymentGateway è in FlakyDemo.Lib/ (progetto separato per il coverage)
cat FlakyDemo.Lib/PaymentGateway.cs

# 21 test: flaky (retry), validazione, fee, Luhn, status
cat PaymentGatewayTests.cs

# .csproj: TestingExtensionsProfile=AllMicrosoft + ProjectReference alla lib
cat FlakyDemo.csproj

# Build
dotnet build

# ── 2. Esecuzione SENZA retry ──
# Alcuni test falliranno (~40% failure rate). E' il comportamento voluto.
dotnet run || true

# ── 3. Esecuzione CON retry ──
# MTP riprova SOLO i test falliti (non l'intera suite!)
dotnet run -- --retry-failed-tests 5

# ── 4. Retry + TRX Report ──
# Genera un file .trx per ogni tentativo di retry
dotnet run -- --retry-failed-tests 5 --report-trx
ls -la bin/Debug/net10.0/TestResults/*.trx 2>/dev/null || true

# ── 5. Retry + Coverage ──
# Il coverage mostra dati reali: FlakyDemo.Lib è un assembly separato.
# RefundPaymentAsync e ProcessBatchAsync sono intenzionalmente non coperti.
dotnet run -- --retry-failed-tests 5 --coverage --coverage-output-format cobertura --coverage-output coverage.xml
ls -la bin/Debug/net10.0/TestResults/coverage.xml 2>/dev/null || true

# ── 6. Pipeline Azure DevOps ──
# Punti chiave:
# - AllowPtrToDetectTestRunRetryFiles: true → unisce i .trx dei retry
# - --report-azdo → annotazioni errori direttamente nelle PR
# - PublishTestResults → tab "Tests" in Azure DevOps
# - PublishCodeCoverageResults → tab "Code Coverage"
echo ""
echo "=== .azure-pipelines/demo06-tests.yml ==="
cat ../../.azure-pipelines/demo06-tests.yml

# ── 7. Pipeline GitHub Actions ──
# Punti chiave:
# - --retry-failed-tests 5 → retry intelligente
# - --report-trx → risultati Visual Studio
# - --coverage → code coverage Cobertura
# - upload-artifact → TRX e coverage scaricabili
# - GITHUB_STEP_SUMMARY → riepilogo con coverage per classe
echo ""
echo "=== .github/workflows/demo06-tests.yml ==="
cat ../../.github/workflows/demo06-tests.yml
