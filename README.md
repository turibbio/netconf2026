# Microsoft Testing Platform: La Nuova Era dei Test in .NET

## Panoramica della Sessione
**Conferenza:** .NET Conf 2026    
**Titolo:** Microsoft Testing Platform: La Nuova Era dei Test in .NET    

Questa sessione esplora i cambiamenti radicali nel testing e nell'automazione della compilazione in .NET 10. Ci focalizziamo su **Microsoft.Testing.Platform (MTP)**, il nuovo runner moderno, e le ottimizzazioni per le moderne pipeline CI/CD.

## Concetti Chiave
1.  **MTP è il Futuro**: VSTest è deprecato; MTP è lo standard per .NET 10+.
2.  **Architettura Rivoluzionaria**: Determinismo, zero dipendenze, supporto Native AOT, ogni progetto di test è un eseguibile autonomo.
3.  **Percorso di Migrazione**: Un percorso concreto per passare da VSTest a MTP senza tempi di inattività.
4.  **CLI Potente**: `--info`, `--list-tests`, `--timeout`, `--maximum-failed-tests`, `--diagnostic`, file di risposta (`@args.rsp`).
5.  **CI/CD Moderno**: Retry automatici, crash dump, rilevamento di blocchi e reporting intelligente in Azure DevOps.
6.  **Unità dell'Ecosistema**: MSTest, xUnit, NUnit e TUnit sono eseguiti sulla stessa piattaforma robusta.

## Struttura del Repository

- `demos/`: Contiene i progetti demo per la sessione.
    - `01-MTP-Basics`: Introduzione al runner, opzioni CLI e modalità di esecuzione (`dotnet test`, `dotnet run`, exe diretto).
    - `02-Ecosystem-MSTest-xUnit`: Mostra come xUnit v3 e MSTest v4 utilizzano entrambi MTP.
    - `03-NUnit-MTP`: Esempio di **NUnit 5** con supporto nativo a Microsoft Testing Platform.
    - `04-TUnit-AOT`: Esempio di **TUnit**, un nuovo framework di test costruito interamente su MTP con supporto NativeAOT.
    - `05-MTP-Extensions`: Tutte le **estensioni MTP** (TRX, AzDo Report, Code Coverage, Coverlet, Retry, CrashDump, HangDump, Fakes).
    - `06-CI-CD-Retry`: Configurazione per l'integrazione avanzata di Azure DevOps con retry automatici e reporting intelligente.

## Prerequisiti
- .NET 10 SDK (o latest .NET 9 preview se 10 non è ancora GA).
- Visual Studio 2026 o VS Code.
