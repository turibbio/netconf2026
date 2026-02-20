# Demo 3: NUnit su MTP
Set-Location demos/03-NUnit-MTP

# Mostra il .csproj (EnableNUnitRunner=true, OutputType=Exe)
Get-Content NUnitDemo.csproj

# Mostra i test (TestCase, Assert.That con constraint model, Assert.Multiple)
Get-Content TemperatureConverterTests.cs

# Build
dotnet build

# Esecuzione
dotnet run

# Info: stessa struttura della Demo 1 e 2 (Piattaforma Unificata)
.\bin\Debug\net10.0\NUnitDemo.exe --info

# List tests
.\bin\Debug\net10.0\NUnitDemo.exe --list-tests
