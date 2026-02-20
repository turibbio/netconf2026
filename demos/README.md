# Microsoft.Testing.Platform - Demo Projects

Questi progetti dimostrano le funzionalità di **Microsoft.Testing.Platform (MTP)**, la nuova architettura di testing per .NET 10+.

## Prerequisiti

- **.NET 10 SDK** o superiore
- **Visual Studio 2026 v18.3+** oppure **VS Code**
- Familiarità con testing in .NET

## Struttura Demo

| Demo | Descrizione | Tecnologie Chiave |
|------|-------------|-------------------|
| **01-MTP-Basics** | Introduzione a MTP, MSTest.Sdk, le 4 modalità di esecuzione | MSTest.Sdk 4.1.0, .NET 10 |
| **02-Ecosystem-MSTest-xUnit** | Supporto multi-framework, xUnit v3 con MTP | xUnit v3, MSTest, MTP |
| **03-NUnit-MTP** | NUnit 4.3.2 su MTP con EnableNUnitRunner | NUnit 4.3.2, MTP |
| **04-TUnit-AOT** | TUnit framework nativo su MTP, Native AOT support | TUnit, PublishAot |
| **05-MTP-Extensions** | Estensioni MTP: TRX, AzDo, Coverage, Retry, Dump | AllMicrosoft, coverlet.MTP |
| **06-CI-CD-Retry** | Retry automatici, Azure DevOps, GitHub Actions, Code Coverage | Retry, Flaky Tests, Coverage |

## Quick Start

### Validare tutti i progetti

**Bash/macOS/Linux:**
```bash
./test-all.sh
```

**PowerShell/Windows:**
```powershell
.\test-all.ps1
```

### Eseguire un singolo demo

```bash
cd demos/01-MTP-Basics

# Modalità 1: dotnet test (raccomandato)
dotnet test

# Modalità 2: dotnet run
dotnet run --project MTP-Intro.csproj

# Modalità 3: Esecuzione diretta
dotnet build
./bin/Debug/net10.0/MTP-Intro

# Mostrare info sulla piattaforma
dotnet test -- --info

# Listare i test senza eseguirli
dotnet test -- --list-tests
```

## Demo 1: MTP Basics

**Obiettivo:** Mostrare l'esperienza "zero config" di MSTest.Sdk e le opzioni CLI di MTP.

**Highlights:**
- `MSTest.Sdk/4.1.0` - SDK completo senza PackageReference
- MSTest v4 APIs: `Assert.ThrowsExactlyAsync<T>()`
- Primary constructors per dependency injection
- Test async completi con Task-based APIs
- Più di 10 test che coprono validazioni complesse

**Eseguire:**
```bash
cd 01-MTP-Basics
dotnet test -- --output Detailed
dotnet test -- --filter TestCategory=Integration
```

## Demo 2: Ecosystem (MSTest + xUnit)

**Obiettivo:** Dimostrare che MTP funziona con tutti i framework principali, non solo MSTest.

**Highlights:**
- xUnit v3 con `UseMicrosoftTestingPlatformRunner`
- Stesso codice testato con MSTest e xUnit
- `OutputType=Exe` richiesto per xUnit/NUnit

**Eseguire:**

```bash
cd 02-Ecosystem-MSTest-xUnit

# ⚠️ Nota: dotnet test ha un bug con xUnit v3 in .NET 10
# Usare dotnet run invece:
dotnet run --project .

# Alternativa (se dotnet test è stato risolto):
# dotnet test
```

> **Known Issue:** `dotnet test` crash con xUnit v3 in .NET 10 ("The given key '8' was not present"). Usare `dotnet run` come workaround.

## Demo 3: NUnit su MTP

**Obiettivo:** Dimostrare NUnit 4.3.2 integrato con Microsoft Testing Platform tramite `EnableNUnitRunner=true`.

**Highlights:**

- NUnit 4.3.2 con `EnableNUnitRunner=true`
- `[TestCase]` - test parametrizzati
- `Assert.That(x, Is.EqualTo(y))` - constraint-based assertions
- `Assert.Multiple` - asserzioni multiple

**Eseguire:**
```bash
cd 03-NUnit-MTP
dotnet test
```

## Demo 4: TUnit + Native AOT

**Obiettivo:** TUnit è il primo framework costruito da zero su MTP. Mostra Native AOT, assertion fluent async, [Matrix], [DependsOn].

**Highlights:**
- `PublishAot=true` - supporto completo Native AOT
- Assertion async: `await Assert.That(x).IsEqualTo(y)`
- `[MatrixArguments]` - combinazioni automatiche
- `[DependsOn]` - dipendenze tra test
- `[Retry(N)]` - built-in retry

**Eseguire:**
```bash
cd 04-TUnit-AOT
dotnet test
dotnet publish -c Release  # Crea eseguibile AOT
```

## Demo 5: Estensioni MTP

**Obiettivo:** Mostrare tutte le estensioni di Microsoft.Testing.Platform disponibili con `TestingExtensionsProfile=AllMicrosoft`.

**Estensioni dimostrate:**

- **Diagnostics** — `--info`, `--diagnostic` (built-in)
- **Output** — `--output Detailed`, `--no-progress`, `--no-ansi` (Terminal Test Reporter, built-in)
- **TRX Report** — `--report-trx` (Visual Studio Test Results)
- **Azure DevOps Report** — `--report-azdo` (annotazioni nelle PR)
- **Code Coverage Microsoft** — `--coverage --coverage-output-format cobertura`
- **Code Coverage Coverlet MTP** — `--coverlet` (cross-platform, anche macOS ARM)
- **Retry** — `--retry-failed-tests N`, `--retry-failed-tests-max-percentage N`, `--retry-failed-tests-max-tests N`
- **Crash Dump** — `--crashdump` (dump se il processo crasha)
- **Hang Dump** — `--hangdump --hangdump-timeout` (dump se i test si bloccano)
- **Microsoft Fakes** — richiede VS Enterprise

**Eseguire:**
```bash
cd 05-MTP-Extensions

# Mostra le estensioni registrate
dotnet test -- --info

# Output dettagliato (mostra anche test passati)
dotnet test -- --output Detailed

# TRX report
dotnet test -- --report-trx

# Code coverage (Coverlet MTP, cross-platform)
dotnet test -- --coverlet --coverlet-include-test-assembly --coverlet-include "[ExtensionsDemo]*"

# Retry test flaky (3 tentativi)
dotnet test -- --retry-failed-tests 3

# Retry con soglia: non riprovare se >50% fallisce
dotnet test -- --retry-failed-tests 3 --retry-failed-tests-max-percentage 50

# Retry con soglia: non riprovare se >2 test falliscono
dotnet test -- --retry-failed-tests 3 --retry-failed-tests-max-tests 2

# Crash dump (test dedicato con filtro)
dotnet test -- --crashdump --crashdump-type Heap --filter "TestCategory=CrashDemo"

# Hang dump (test dedicato con filtro, timeout 10s)
dotnet test -- --hangdump --hangdump-timeout 10s --hangdump-type Mini --filter "TestCategory=HangDemo"

# Tutto insieme
dotnet test -- --report-trx --coverage --coverage-output-format cobertura --retry-failed-tests 3
```

> **Nota:** I test `CrashDemo` e `HangDemo` sono esclusi dall'esecuzione normale (`test-all.sh`) e vanno eseguiti solo con i filtri indicati sopra. I dump vengono generati in `bin/Debug/net10.0/TestResults/`.

## Demo 6: CI/CD — Retry, Azure DevOps e GitHub Actions

**Obiettivo:** Risolvere i test instabili nelle pipeline CI/CD con retry intelligente, report e code coverage.

**Struttura progetto:**

- `FlakyDemo.Lib/` — Libreria con `PaymentGateway` (codice di produzione, separato per il coverage)
- `FlakyDemo.csproj` — Progetto test con `ProjectReference` alla lib
- `PaymentGatewayTests.cs` — 21 test: flaky (retry), validazione, fee, Luhn algorithm

**Highlights:**

- Test deliberatamente flaky (40% failure rate) per dimostrare retry
- `--retry-failed-tests N` — solo i test falliti vengono rieseguiti, non l'intera suite
- Codice di produzione in progetto separato per coverage reale (senza `IncludeTestAssembly`)
- Metodi `RefundPaymentAsync` e `ProcessBatchAsync` intenzionalmente non coperti (coverage < 100%)
- **Azure DevOps:** `AllowPtrToDetectTestRunRetryFiles: true`, `--report-azdo`, PublishTestResults, PublishCodeCoverageResults
- **GitHub Actions:** `upload-artifact`, `GITHUB_STEP_SUMMARY` con riepilogo coverage per classe
- TRX separati per ogni tentativo di retry

**Pipeline incluse:**

| Pipeline | File | Funzionalità |
|----------|------|--------------|
| Azure DevOps | `.azure-pipelines/demo06-tests.yml` | Retry + TRX + AzDo Report + Coverage + PublishTestResults |
| GitHub Actions | `.github/workflows/demo06-tests.yml` | Retry + TRX + Coverage + Artifact + Summary |

**Eseguire:**

```bash
cd 06-CI-CD-Retry

# Senza retry - alcuni test falliranno (~40%)
dotnet run

# Con retry - i test flaky passeranno
dotnet run -- --retry-failed-tests 5

# Retry + TRX report
dotnet run -- --retry-failed-tests 5 --report-trx

# Retry + Coverage
dotnet run -- --retry-failed-tests 5 --coverage --coverage-output-format cobertura
```

**Configurazione Azure DevOps — Chiave:**

```yaml
variables:
  # Unisce i .trx dei retry in un'unica esecuzione logica
  AllowPtrToDetectTestRunRetryFiles: true
```

**GitHub Actions — Summary:**

La pipeline genera automaticamente un riepilogo nel tab "Summary" del workflow con:
- Configurazione retry utilizzata
- Tabella code coverage (line + branch) con dettaglio per classe
- Link agli artifact scaricabili (TRX + coverage)

## Opzioni CLI MTP (Comuni a Tutti i Demo)

### Informazioni e Discovery

```bash
# Info sulla piattaforma e estensioni
dotnet test -- --info

# Listare tutti i test
dotnet test -- --list-tests

# Help completo
dotnet test -- --help
```

### Filtering

```bash
# Per nome
dotnet test -- --filter "FullyQualifiedName~Registration"

# Per categoria
dotnet test -- --filter "TestCategory=Integration"
```

### Diagnostica

```bash
# Logging diagnostico dettagliato
dotnet test -- --diagnostic --diagnostic-verbosity Trace

# Timeout globale
dotnet test -- --timeout 30s

# Fermare dopo N fallimenti
dotnet test -- --maximum-failed-tests 5
```

### Target Selection (.NET 10)

```bash
# Progetto singolo
dotnet test --project ./MTP-Intro.csproj

# Solution
dotnet test --solution ../NetConf2026.sln

# Assembly pre-compilate (globbing)
dotnet test --test-modules "**/bin/**/Debug/net10.0/*.dll"

# Parallelismo tra moduli
dotnet test --max-parallel-test-modules 4
```

## global.json

Il file `global.json` nella root di `demos/` configura MTP come runner di default:

```json
{
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestMinor"
  },
  "test": {
    "runner": "Microsoft.Testing.Platform"
  }
}
```

Questo abilita `dotnet test` a usare MTP invece di VSTest in tutti i progetti della solution.

## Architettura MTP - Pilastri

1. **Determinismo** - Niente reflection runtime per coordinare i test
2. **Trasparenza runtime** - No AppDomain, no AssemblyLoadContext custom
3. **Registrazione a compile-time** - Estensioni registrate in compilazione
4. **Zero dipendenze** - Core = singolo assembly senza dipendenze
5. **Hostable** - Gira in qualsiasi app .NET (console, device, browser)
6. **Native AOT support** - Tutti i form factor .NET
7. **Single module deploy** - Unico modulo per estensioni in-process/out-of-process

## Troubleshooting

### dotnet test crash con xUnit v3 (.NET 10)

**Errore:**

```text
System.Collections.Generic.KeyNotFoundException: The given key '8' was not present in the dictionary.
```

**Causa:** Bug noto in .NET 10 SDK quando `dotnet test` comunica con xUnit v3 (incompatibilità protocol version 8).

**Workaround:**

```bash
# ✅ Soluzione: usare dotnet run invece di dotnet test
dotnet run --project ./02-Ecosystem-MSTest-xUnit

# Oppure eseguire direttamente il binario
cd 02-Ecosystem-MSTest-xUnit
dotnet build
./bin/Debug/net10.0/Ecosystem
```

**Nota:** Questo bug affetta SOLO xUnit v3. MSTest e TUnit funzionano correttamente con `dotnet test`.

### "Cannot find .NET 10 SDK"

Verifica la versione installata:
```bash
dotnet --list-sdks
```

Installa .NET 10 SDK: https://dotnet.microsoft.com/download/dotnet/10.0

### "Project does not contain any tests"

Assicurati che il `global.json` sia nella cartella `demos/` e contenga:
```json
{
  "test": {
    "runner": "Microsoft.Testing.Platform"
  }
}
```

### Exit codes MTP

| Code | Significato |
|------|-------------|
| 0 | Successo |
| 2 | Test falliti |
| 8 | Zero test eseguiti |
| 13 | Maximum failed tests raggiunto |

## Risorse

- [Microsoft.Testing.Platform Docs](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-intro)
- [MSTest v4 Migration](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-mstest-migration-v3-v4)
- [TUnit](https://tunit.dev/)
- [MTP Source Code](https://github.com/microsoft/testfx/tree/main/src/Platform/Microsoft.Testing.Platform)
- [Blog: Enhance CLI testing with dotnet test](https://devblogs.microsoft.com/dotnet/dotnet-test-with-mtp/)
- [Blog: MTP adoption by frameworks](https://devblogs.microsoft.com/dotnet/mtp-adoption-frameworks/)

## License

MIT
