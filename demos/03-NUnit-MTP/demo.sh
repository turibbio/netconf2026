#!/bin/bash
# Demo 3: NUnit su MTP
cd demos/03-NUnit-MTP

# Mostra il .csproj (EnableNUnitRunner=true, OutputType=Exe)
cat NUnitDemo.csproj

# Mostra i test (TestCase, Assert.That con constraint model, Assert.Multiple)
cat TemperatureConverterTests.cs

# Build
dotnet build

# Esecuzione
dotnet run

# Info: stessa struttura della Demo 1 e 2 (Piattaforma Unificata)
./bin/Debug/net10.0/NUnitDemo --info

# List tests
./bin/Debug/net10.0/NUnitDemo --list-tests
