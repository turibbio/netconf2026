# Demo 4: TUnit e Native AOT
Set-Location demos/04-TUnit-AOT

# Mostra i test (DependsOn, Arguments, Retry, async Assert, HTTP assertions)
Get-Content UrlShortenerTests.cs

# Mostra il .csproj (PublishAot=true, NON aggiungere Microsoft.NET.Test.Sdk)
Get-Content TUnitDemo.csproj

# Build
dotnet build

# Esecuzione (Source Generators, niente reflection)
.\bin\Debug\net10.0\TUnitDemo.exe

# List tests
.\bin\Debug\net10.0\TUnitDemo.exe --list-tests

# Publish Native AOT
dotnet publish -c Release
