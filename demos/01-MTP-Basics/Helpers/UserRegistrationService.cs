namespace MTP_Basics;

public record User(string Id, string Email, string HashedPassword, DateTime RegisteredAt);

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<User> CreateUserAsync(string email, string hashedPassword);
}

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = [];

    public Task<bool> EmailExistsAsync(string email)
    {
        return Task.FromResult(_users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<User> CreateUserAsync(string email, string hashedPassword)
    {
        var user = new User(Guid.NewGuid().ToString(), email, hashedPassword, DateTime.UtcNow);
        _users.Add(user);
        return Task.FromResult(user);
    }
}

public class PasswordHasher
{
    public string Hash(string password) =>
        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password + "_SALT"));

    public bool Verify(string password, string hash) =>
        Hash(password) == hash;
}

public class UserRegistrationService(IUserRepository repository, PasswordHasher hasher)
{
    private static readonly System.Text.RegularExpressions.Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", System.Text.RegularExpressions.RegexOptions.Compiled);

    public async Task<(bool Success, string Message, User? User)> RegisterUserAsync(string email, string password)
    {
        // Email validation
        if (string.IsNullOrWhiteSpace(email))
            return (false, "Email cannot be empty.", null);

        if (!EmailRegex.IsMatch(email))
            return (false, "Invalid email format.", null);

        // Password strength validation
        if (password.Length < 8)
            return (false, "Password must be at least 8 characters long.", null);

        if (!password.Any(char.IsUpper))
            return (false, "Password must contain at least one uppercase letter.", null);

        if (!password.Any(char.IsLower))
            return (false, "Password must contain at least one lowercase letter.", null);

        if (!password.Any(char.IsDigit))
            return (false, "Password must contain at least one digit.", null);

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            return (false, "Password must contain at least one special character.", null);

        // Check for existing email
        if (await repository.EmailExistsAsync(email))
            return (false, "Email already in use.", null);

        // Create user
        var hashedPassword = hasher.Hash(password);
        var user = await repository.CreateUserAsync(email, hashedPassword);

        return (true, "User registered successfully.", user);
    }
}
