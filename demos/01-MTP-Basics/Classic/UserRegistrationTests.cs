using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MTP_Basics;

[TestClass]
public class UserRegistrationTests
{
    private UserRegistrationService _service = null!;
    private InMemoryUserRepository _repository = null!;
    private PasswordHasher _hasher = null!;

    [TestInitialize]
    public void Setup()
    {
        _repository = new InMemoryUserRepository();
        _hasher = new PasswordHasher();
        _service = new UserRegistrationService(_repository, _hasher);
    }

    [TestMethod]
    public async Task RegisterUser_ShouldSucceed_WhenCredentialsAreValid()
    {
        // Arrange
        var email = "newuser@example.com";
        var password = "SecurePass123!";

        // Act
        var result = await _service.RegisterUserAsync(email, password);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual("User registered successfully.", result.Message);
        Assert.IsNotNull(result.User);
        Assert.AreEqual(email, result.User.Email);
    }

    [TestMethod]
    [DataRow("", "Password123!", "Email cannot be empty.")]
    [DataRow("   ", "Password123!", "Email cannot be empty.")]
    [DataRow("bad-email", "Password123!", "Invalid email format.")]
    [DataRow("valid@email.com", "short", "Password must be at least 8 characters long.")]
    [DataRow("valid@email.com", "alllowercase", "Password must contain at least one uppercase letter.")]
    [DataRow("valid@email.com", "ALLUPPERCASE", "Password must contain at least one lowercase letter.")]
    [DataRow("valid@email.com", "NoDigits!", "Password must contain at least one digit.")]
    [DataRow("valid@email.com", "NoSpecial123", "Password must contain at least one special character.")]
    public async Task RegisterUser_ShouldFail_WhenValidationRulesAreBreached(string email, string password, string expectedError)
    {
        // Act
        var result = await _service.RegisterUserAsync(email, password);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Message.Contains(expectedError),
            $"Expected message to contain '{expectedError}' but got '{result.Message}'");
        Assert.IsNull(result.User);
    }

    [TestMethod]
    public async Task RegisterUser_ShouldFail_WhenEmailAlreadyExists()
    {
        // Arrange
        var email = "existing@example.com";
        await _service.RegisterUserAsync(email, "FirstPass123!");

        // Act
        var result = await _service.RegisterUserAsync(email, "SecondPass456!");

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual("Email already in use.", result.Message);
        Assert.IsNull(result.User);
    }

    [TestMethod]
    public async Task RegisterUser_ShouldHashPassword()
    {
        // Arrange
        var email = "test@example.com";
        var password = "MyPassword123!";

        // Act
        var result = await _service.RegisterUserAsync(email, password);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.User);
        Assert.AreNotEqual(password, result.User.HashedPassword, "Password should be hashed");
        Assert.IsTrue(_hasher.Verify(password, result.User.HashedPassword), "Hash verification should succeed");
    }

    [TestMethod]
    public async Task RegisterUser_ShouldThrowException_WhenRepositoryIsNull()
    {
        // Arrange
        UserRegistrationService invalidService = null!;

        // Act & Assert - MSTest v4 new API
        await Assert.ThrowsExactlyAsync<NullReferenceException>(async () =>
        {
            await invalidService.RegisterUserAsync("test@example.com", "Pass123!");
        });
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task RegisterUser_Integration_MultipleUsers()
    {
        // Arrange & Act
        var user1 = await _service.RegisterUserAsync("user1@example.com", "Pass123!ABC");
        var user2 = await _service.RegisterUserAsync("user2@example.com", "Pass456!DEF");
        var user3 = await _service.RegisterUserAsync("user3@example.com", "Pass789!GHI");

        // Assert
        Assert.IsTrue(user1.Success);
        Assert.IsTrue(user2.Success);
        Assert.IsTrue(user3.Success);

        // Verify all users have unique IDs
        var ids = new[] { user1.User!.Id, user2.User!.Id, user3.User!.Id };
        Assert.AreEqual(3, ids.Distinct().Count());
    }
}
