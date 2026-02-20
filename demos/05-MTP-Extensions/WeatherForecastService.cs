namespace ExtensionsDemo;

public class WeatherForecastService
{
    public string GetForecast(string city, int dayOffset)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required", nameof(city));
        if (dayOffset < 0 || dayOffset > 7)
            throw new ArgumentOutOfRangeException(nameof(dayOffset), "Must be 0-7");

        var temperature = city.ToLower() switch
        {
            "roma" => 25 + dayOffset,
            "milano" => 20 + dayOffset,
            "napoli" => 28 + dayOffset,
            "bolzano" => 15 + dayOffset,
            _ => 22 + dayOffset
        };

        return temperature switch
        {
            < 10 => $"{city}: {temperature}°C - Freddo",
            < 20 => $"{city}: {temperature}°C - Fresco",
            < 30 => $"{city}: {temperature}°C - Mite",
            _ => $"{city}: {temperature}°C - Caldo"
        };
    }

    public async Task<string> GetExtendedForecastAsync(string city, CancellationToken ct = default)
    {
        await Task.Delay(500, ct);
        return GetForecast(city, 0);
    }
}
