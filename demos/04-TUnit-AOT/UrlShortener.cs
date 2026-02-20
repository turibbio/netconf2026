using System.Security.Cryptography;
using System.Text;

namespace TUnitDemo;

public interface IUrlShortener
{
    string Encode(string input);
    bool Validate(string shortCode);
}

public class UrlShortener : IUrlShortener
{
    public string Encode(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes).Substring(0, 8)
                     .Replace("/", "_").Replace("+", "-");
    }

    public bool Validate(string shortCode)
    {
        // Simple mock validation logic
        return !string.IsNullOrEmpty(shortCode) && shortCode.Length == 8;
    }
}
