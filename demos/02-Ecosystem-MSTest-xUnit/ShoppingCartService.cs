namespace EcosystemAdoption;

public class ShoppingCartItem
{
    public string ProductId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class ShoppingCartService
{
    private readonly List<ShoppingCartItem> _items = new();

    public void AddItem(string productId, decimal price, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _items.Add(new ShoppingCartItem { ProductId = productId, Price = price, Quantity = quantity });
        }
    }

    public decimal CalculateTotal(string? discountCode = null)
    {
        var subtotal = _items.Sum(i => i.Price * i.Quantity);

        if (string.IsNullOrEmpty(discountCode))
            return subtotal;

        return discountCode switch
        {
            "SAVE10" => subtotal * 0.90m,
            "BLACKFRIDAY" => subtotal * 0.50m,
            _ => subtotal
        };
    }
}
