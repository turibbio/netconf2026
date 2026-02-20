#!/bin/bash
# Demo 4: TUnit e Native AOT
cd demos/04-TUnit-AOT

# Mostra i test (DependsOn, Arguments, Retry, async Assert, HTTP assertions)
cat UrlShortenerTests.cs

# Mostra il .csproj (PublishAot=true, NON aggiungere Microsoft.NET.Test.Sdk)
cat TUnitDemo.csproj

# Build
dotnet build

# Esecuzione (Source Generators, niente reflection)
./bin/Debug/net10.0/TUnitDemo

# Output dettagliato (mostra ogni test + Console.WriteLine)
./bin/Debug/net10.0/TUnitDemo --output detailed

# List tests
./bin/Debug/net10.0/TUnitDemo --list-tests

# Publish Native AOT
dotnet publish -c Release
