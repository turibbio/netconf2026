using System.Net;
using System.Runtime.CompilerServices;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace TUnitDemo;

public class UrlShortenerTests
{
    private readonly UrlShortener _shortener = new();

    [Test]
    public async Task Encode_ShouldReturn8Chars()
    {
        // Arrange
        var url = "https://www.microsoft.com";

        // Act
        var result = _shortener.Encode(url);

        // Assert - Assertion async fluent di TUnit
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Length).IsEqualTo(8);
    }

    [Test]
    [Arguments("https://github.com")]
    [Arguments("https://google.com")]
    [Arguments("https://microsoft.com")]
    public async Task Encode_ShouldReturnConsistentLength(string url)
    {
        // Act
        var result = _shortener.Encode(url);

        // Assert
        await Assert.That(result.Length).IsEqualTo(8);
        await Assert.That(_shortener.Validate(result)).IsTrue();
    }

    [Test]
    [Arguments("http", "google.com")]
    [Arguments("https", "bing.com")]
    [Arguments("https", "github.com")]
    public async Task Encode_DifferentProtocols_ShouldWork(string protocol, string domain)
    {
        // Arrange
        var url = $"{protocol}://{domain}";

        // Act
        var result = _shortener.Encode(url);

        // Assert
        await Assert.That(result).IsNotNull()
            .And.IsNotEmpty();
        await Assert.That(result.Length).IsEqualTo(8);
    }

    [Test]
    [MatrixDataSource]
    public async Task Encode_Matrix_AllCombinations(
        [Matrix("http", "https")] string protocol,
        [Matrix("google.com", "bing.com", "github.com")] string domain)
    {
        // [MatrixDataSource] genera automaticamente 2 × 3 = 6 test
        // dal prodotto cartesiano dei parametri [Matrix]
        // Niente piu' elenco manuale di [Arguments]!

        // Arrange
        var url = $"{protocol}://{domain}";

        // Act
        var result = _shortener.Encode(url);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Length).IsEqualTo(8);
    }

    [Test]
    public async Task Encode_EmptyString_ShouldReturnEmpty()
    {
        // Act
        var result = _shortener.Encode("");

        // Assert
        await Assert.That(result).IsEmpty();
    }

    [Test]
    public async Task Encode_SameInput_ShouldReturnSameOutput()
    {
        // Arrange
        var url = "https://example.com/test";

        // Act
        var result1 = _shortener.Encode(url);
        var result2 = _shortener.Encode(url);

        // Assert - Hashing deterministico
        await Assert.That(result1).IsEqualTo(result2);
    }

    [Test]
    [DependsOn(nameof(Encode_ShouldReturn8Chars))]
    public async Task NativeAot_PerformanceCheck()
    {
        // Questo test gira SOLO se 'Encode_ShouldReturn8Chars' passa
        // DependsOn e' una feature unica di TUnit per l'ordinamento dei test

        // Arrange
        const int iterations = 1000;
        var start = DateTime.UtcNow;

        // Act - Esegui encoding 1000 volte
        for (int i = 0; i < iterations; i++)
        {
            _shortener.Encode($"https://example.com/{i}");
        }

        var duration = DateTime.UtcNow - start;

        // Assert - Deve completare in meno di 1 secondo (margine generoso per la demo)
        await Assert.That(duration.TotalSeconds).IsLessThan(1);
    }

    [Test]
    [Retry(3)] // Retry built-in di TUnit - riprova fino a 3 volte se il test fallisce
    public async Task Simulate_Flaky_External_Call()
    {
        // Simula jitter di rete casuale (50% probabilita' di fallimento)
        // Retry riesegue automaticamente questo test fino a 3 volte
        var shouldFail = Random.Shared.Next(0, 10) < 5;

        if (shouldFail)
        {
            throw new Exception("Errore di Rete Simulato.");
        }

        await Assert.That(shouldFail).IsFalse();
    }

    [Test]
    [NotInParallel] // Questo test gira in sequenza, non in parallelo con gli altri
    public async Task Encode_ThreadSafety_Check()
    {
        // TUnit esegue i test in parallelo di default
        // Usa [NotInParallel] quando serve esecuzione sequenziale

        var tasks = Enumerable.Range(0, 10)
            .Select(i => Task.Run(() => _shortener.Encode($"https://test.com/{i}")))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        await Assert.That(results.Length).IsEqualTo(10);
        await Assert.That(results.All(r => r.Length == 8)).IsTrue();
    }

    // ========================================================================
    // MethodDataSource async - Generazione asincrona dei dati di test
    // ========================================================================

    public static async IAsyncEnumerable<Func<string>> GetUrlsAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        // Simula caricamento asincrono dei dati (es. da database o API esterna)
        var urls = new[] { "https://microsoft.com", "https://github.com", "https://dotnet.dev" };
        foreach (var url in urls)
        {
            await Task.Delay(10, ct);
            yield return () => url;
        }
    }

    [Test]
    [MethodDataSource(nameof(GetUrlsAsync))]
    public async Task Encode_WithAsyncDataSource(string url)
    {
        // I dati vengono generati in modo asincrono da GetUrlsAsync
        // TUnit unwrappa automaticamente Func<string> -> string
        var result = _shortener.Encode(url);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Length).IsEqualTo(8);
        await Assert.That(_shortener.Validate(result)).IsTrue();
    }

    // ========================================================================
    // NOVITA' TUnit 1.14.0 - HTTP Response Assertions
    // ========================================================================

    [Test]
    public async Task HttpResponse_IsOk_StatusCode()
    {
        // NOVITA' v1.14.0: Assertion built-in per HttpResponseMessage
        // Prima: await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        // Ora: sintassi fluent dedicata

        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"shortUrl\": \"abc12345\"}", System.Text.Encoding.UTF8, "application/json")
        };

        // Assert - Nuove assertion HTTP fluent
        await Assert.That(response).IsOk();
        await Assert.That(response).HasJsonContent();
    }

    [Test]
    [Arguments(HttpStatusCode.OK)]
    [Arguments(HttpStatusCode.Created)]
    [Arguments(HttpStatusCode.NoContent)]
    public async Task HttpResponse_IsSuccessStatusCode(HttpStatusCode statusCode)
    {
        // Arrange
        var response = new HttpResponseMessage(statusCode);

        // Assert - Verifica che sia uno status code di successo (2xx)
        await Assert.That(response).IsSuccessStatusCode();
        await Assert.That(response).HasStatusCode(statusCode);
    }

    [Test]
    public async Task HttpResponse_ErrorCodes_Detection()
    {
        // Arrange
        var notFound = new HttpResponseMessage(HttpStatusCode.NotFound);
        var unauthorized = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        var serverError = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        // Assert - Assertion specifiche per codici di errore
        await Assert.That(notFound).IsNotFound();
        await Assert.That(unauthorized).IsUnauthorized();
        await Assert.That(serverError).IsServerErrorStatusCode();
    }

    [Test]
    public async Task HttpResponse_HasHeader_Check()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Headers.Add("X-Request-Id", "demo-12345");

        // Assert - Verifica la presenza di header specifici
        await Assert.That(response).IsOk();
        await Assert.That(response).HasHeader("X-Request-Id");
    }
}
