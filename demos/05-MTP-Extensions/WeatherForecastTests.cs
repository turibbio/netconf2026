using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExtensionsDemo;

[TestClass]
public class WeatherForecastTests
{
    private WeatherForecastService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _service = new WeatherForecastService();
    }

    // ── Validazione input ──

    [TestMethod]
    public void GetForecast_NullCity_ThrowsArgumentException()
    {
        Assert.ThrowsExactly<ArgumentException>(() => _service.GetForecast(null!, 0));
    }

    [TestMethod]
    public void GetForecast_InvalidDayOffset_ThrowsArgumentOutOfRange()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _service.GetForecast("Roma", 10));
    }

    // ── Branch per città (coverage) ──

    [TestMethod]
    [DataRow("Roma", 0, "Roma: 25°C - Mite")]
    [DataRow("Milano", 0, "Milano: 20°C - Mite")]
    [DataRow("Napoli", 0, "Napoli: 28°C - Mite")]
    [DataRow("Bolzano", 0, "Bolzano: 15°C - Fresco")]
    [DataRow("Firenze", 0, "Firenze: 22°C - Mite")]
    public void GetForecast_KnownCities_ReturnsExpected(string city, int day, string expected)
    {
        var result = _service.GetForecast(city, day);
        Assert.AreEqual(expected, result);
    }

    // ── Branch per temperatura (coverage) ──

    [TestMethod]
    public void GetForecast_Bolzano_Day0_Fresco()
    {
        var result = _service.GetForecast("Bolzano", 0);
        StringAssert.Contains(result, "Fresco");
    }

    [TestMethod]
    public void GetForecast_Napoli_Day7_Caldo()
    {
        // 28 + 7 = 35 >= 30 -> Caldo
        var result = _service.GetForecast("Napoli", 7);
        StringAssert.Contains(result, "Caldo");
    }

    [TestMethod]
    public void GetForecast_Bolzano_Day7_Mite()
    {
        // 15 + 7 = 22 -> Mite
        var result = _service.GetForecast("Bolzano", 7);
        StringAssert.Contains(result, "Mite");
    }

    // ── Metodo async (utile per hang dump demo) ──

    [TestMethod]
    public async Task GetExtendedForecast_ReturnsValidResult()
    {
        var result = await _service.GetExtendedForecastAsync("Roma");
        StringAssert.StartsWith(result, "Roma:");
    }

    // ── Test crash (utile per crashdump demo) ──

    [TestMethod]
    [TestCategory("CrashDemo")]
    public void CrashTest_SimulateProcessCrash()
    {
        // Provoca un crash del processo per generare il crash dump.
        // Eseguire SOLO con: --filter "TestCategory=CrashDemo" --crashdump
        Environment.FailFast("Simulated crash for dump demo");
    }

    // ── Test hang (utile per hangdump demo) ──

    [TestMethod]
    [TestCategory("HangDemo")]
    public async Task HangTest_SimulateTimeout()
    {
        // Simula un test che non termina mai per generare l'hang dump.
        // Eseguire SOLO con: --filter "TestCategory=HangDemo" --hangdump --hangdump-timeout 10s
        await Task.Delay(Timeout.Infinite);
    }

    // ── Test flaky (utile per retry demo) ──

    [TestMethod]
    [TestCategory("Flaky")]
    public void GetForecast_RandomCity_HasTemperature()
    {
        // Simula un test flaky: fallisce ~30% delle volte
        var cities = new[] { "Roma", "Milano", "Napoli", "Bolzano", "Firenze" };
        var city = cities[Random.Shared.Next(cities.Length)];
        var result = _service.GetForecast(city, Random.Shared.Next(0, 8));

        // Fallisce casualmente per dimostrare il retry
        if (Random.Shared.NextDouble() < 0.3)
            Assert.Fail("Simulated transient failure (flaky test for retry demo)");

        Assert.IsTrue(result.Contains("°C"));
    }
}
