using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace TUnitDemo;

/// <summary>
/// Test con Dependency Injection nativa tramite IServiceProvider.
/// TUnit inietta IUrlShortener nel costruttore usando il DI container.
/// </summary>
[ClassConstructor<DependencyInjectionClassConstructor>]
public class UrlShortenerDITests(IUrlShortener shortener)
{
    // Setup globale: eseguito una volta prima di tutti i test della classe
    [Before(Class)]
    public static void SetupClass(ClassHookContext context)
    {
        Console.WriteLine($"[DI] Setup classe: {context.ClassType.Name} - ServiceProvider configurato");
    }

    // Setup per test: eseguito prima di ogni singolo test
    [Before(Test)]
    public void SetupTest(TestContext context)
    {
        Console.WriteLine($"[DI] Inizio test: {context.Metadata.TestName} - IUrlShortener iniettato: {shortener.GetType().Name}");
    }

    [Test]
    public async Task Encode_WithDI_ShouldReturn8Chars()
    {
        // IUrlShortener e' stato iniettato dal ServiceProvider
        var result = shortener.Encode("https://www.microsoft.com");

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Length).IsEqualTo(8);
    }

    [Test]
    [Arguments("https://github.com")]
    [Arguments("https://google.com")]
    public async Task Encode_WithDI_ShouldReturnConsistentLength(string url)
    {
        var result = shortener.Encode(url);

        await Assert.That(result.Length).IsEqualTo(8);
        await Assert.That(shortener.Validate(result)).IsTrue();
    }

    [Test]
    public async Task Validate_WithDI_ShouldRejectInvalidCodes()
    {
        var isValid = shortener.Validate("abc");

        await Assert.That(isValid).IsFalse();
    }

}
