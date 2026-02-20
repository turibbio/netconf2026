namespace NUnitDemo;

public class TemperatureConverter
{
    public double CelsiusToFahrenheit(double celsius)
    {
        return celsius * 9.0 / 5.0 + 32;
    }

    public double FahrenheitToCelsius(double fahrenheit)
    {
        return (fahrenheit - 32) * 5.0 / 9.0;
    }

    public double CelsiusToKelvin(double celsius)
    {
        if (celsius < -273.15)
            throw new ArgumentOutOfRangeException(nameof(celsius), "Temperature below absolute zero.");

        return celsius + 273.15;
    }

    public double KelvinToCelsius(double kelvin)
    {
        if (kelvin < 0)
            throw new ArgumentOutOfRangeException(nameof(kelvin), "Kelvin cannot be negative.");

        return kelvin - 273.15;
    }

    public string Classify(double celsius) => celsius switch
    {
        < -40 => "Extreme Cold",
        < 0 => "Freezing",
        < 15 => "Cold",
        < 25 => "Comfortable",
        < 35 => "Warm",
        < 45 => "Hot",
        _ => "Extreme Heat"
    };
}
