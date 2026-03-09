namespace Application.Dtos;

/// <summary>
/// Context passed to the AI assistant on every request.
/// Includes the live <see cref="Customer"/> object so action-capable providers (e.g. the fallback)
/// can call services to mutate cart and order state directly.
/// </summary>
public class ShoppingContext
{
    public Customer Customer { get; init; } = null!;
    public string CustomerName { get; init; } = string.Empty;
    public IReadOnlyList<Product> Products { get; init; } = [];
    public IReadOnlyList<CartItem> CartItems { get; init; } = [];
    public IReadOnlyList<Order> RecentOrders { get; init; } = [];
}
