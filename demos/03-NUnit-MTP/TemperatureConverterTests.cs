using NUnit.Framework;

namespace NUnitDemo;

[TestFixture]
public class TemperatureConverterTests
{
    private TemperatureConverter _converter = null!;

    [SetUp]
    public void Setup()
    {
        _converter = new TemperatureConverter();
    }

    [Test]
    public void CelsiusToFahrenheit_BoilingPoint()
    {
        var result = _converter.CelsiusToFahrenheit(100);

        Assert.That(result, Is.EqualTo(212.0));
    }

    [Test]
    public void CelsiusToFahrenheit_FreezingPoint()
    {
        var result = _converter.CelsiusToFahrenheit(0);

        Assert.That(result, Is.EqualTo(32.0));
    }

    // TestCase: test parametrizzati (equivalente di xUnit [InlineData])
    [TestCase(0, 32)]
    [TestCase(100, 212)]
    [TestCase(-40, -40)]
    [TestCase(37, 98.6)]
    public void CelsiusToFahrenheit_KnownValues(double celsius, double expectedFahrenheit)
    {
        var result = _converter.CelsiusToFahrenheit(celsius);

        Assert.That(result, Is.EqualTo(expectedFahrenheit).Within(0.1));
    }

    [TestCase(32, 0)]
    [TestCase(212, 100)]
    [TestCase(-40, -40)]
    [TestCase(98.6, 37)]
    public void FahrenheitToCelsius_KnownValues(double fahrenheit, double expectedCelsius)
    {
        var result = _converter.FahrenheitToCelsius(fahrenheit);

        Assert.That(result, Is.EqualTo(expectedCelsius).Within(0.1));
    }

    [TestCase(0, 273.15)]
    [TestCase(100, 373.15)]
    [TestCase(-273.15, 0)]
    public void CelsiusToKelvin_KnownValues(double celsius, double expectedKelvin)
    {
        var result = _converter.CelsiusToKelvin(celsius);

        Assert.That(result, Is.EqualTo(expectedKelvin).Within(0.01));
    }

    // Assert.Multiple: piu' asserzioni in un blocco (riporta tutti i fallimenti)
    [Test]
    public void RoundTrip_CelsiusToFahrenheitAndBack()
    {
        Assert.Multiple(() =>
        {
            foreach (var temp in new[] { -40.0, 0.0, 20.0, 37.0, 100.0 })
            {
                var fahrenheit = _converter.CelsiusToFahrenheit(temp);
                var backToCelsius = _converter.FahrenheitToCelsius(fahrenheit);
                Assert.That(backToCelsius, Is.EqualTo(temp).Within(0.01),
                    $"Round-trip failed for {temp}°C");
            }
        });
    }

    // Test eccezioni con Assert.Throws
    [Test]
    public void CelsiusToKelvin_BelowAbsoluteZero_ThrowsException()
    {
        Assert.That(() => _converter.CelsiusToKelvin(-300),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void KelvinToCelsius_NegativeKelvin_ThrowsException()
    {
        Assert.That(() => _converter.KelvinToCelsius(-1),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    // TestCase con classificazione
    [TestCase(-50, "Extreme Cold")]
    [TestCase(-10, "Freezing")]
    [TestCase(10, "Cold")]
    [TestCase(22, "Comfortable")]
    [TestCase(30, "Warm")]
    [TestCase(40, "Hot")]
    [TestCase(50, "Extreme Heat")]
    public void Classify_ReturnsCorrectCategory(double celsius, string expectedCategory)
    {
        var result = _converter.Classify(celsius);

        Assert.That(result, Is.EqualTo(expectedCategory));
    }
}
