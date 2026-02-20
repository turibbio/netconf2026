using Xunit;

namespace EcosystemAdoption;

public class ShoppingCartTests
{
    [Fact]
    public void AddItem_ShouldUpdateQuantity_WhenItemExists()
    {
        // Arrange
        var cart = new ShoppingCartService();
        cart.AddItem("P1", 100m, 1);

        // Act
        cart.AddItem("P1", 100m, 2);

        // Assert
        // In a real scenario we'd expose Validate or use generic list inspection
        Assert.Equal(300m, cart.CalculateTotal()); 
    }

    [Theory]
    [InlineData("SAVE10", 90)]
    [InlineData("BLACKFRIDAY", 50)]
    [InlineData("INVALID", 100)]
    [InlineData(null, 100)]
    public void CalculateTotal_ShouldApplyDiscountsCorrectly(string code, decimal expectedTotal)
    {
        // Arrange
        var cart = new ShoppingCartService();
        cart.AddItem("P1", 100m, 1);

        // Act
        var total = cart.CalculateTotal(code);

        // Assert
        Assert.Equal(expectedTotal, total);
    }
}
