namespace Application.Services;

/// <summary>
/// Orchestrates the AI shopping assistant: builds a live shopping context from store data
/// and delegates to the configured <see cref="IShoppingAssistant"/> provider.
/// </summary>
public class ShoppingAssistantService
{
    private readonly IShoppingAssistant _assistant;
    private readonly ProductService _productService;
    private readonly CartService _cartService;
    private readonly OrderService _orderService;

    public ShoppingAssistantService(
        IShoppingAssistant assistant,
        ProductService productService,
        CartService cartService,
        OrderService orderService)
    {
        _assistant = assistant;
        _productService = productService;
        _cartService = cartService;
        _orderService = orderService;
    }

    /// <summary>
    /// Processes a user message, enriches it with live shopping context, and returns the assistant's response.
    /// The conversation history is updated with the new exchange after the response is obtained.
    /// </summary>
    public async Task<string> ChatAsync(
        Customer customer,
        string userMessage,
        IList<ChatMessage> conversationHistory,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.NullOrWhiteSpace(userMessage, message: "Message cannot be empty.");

        var context = BuildContext(customer);
        var response = await _assistant.GetResponseAsync(userMessage, context, conversationHistory, cancellationToken);

        conversationHistory.Add(new ChatMessage { Role = "user", Content = userMessage });
        conversationHistory.Add(new ChatMessage { Role = "assistant", Content = response });

        return response;
    }

    private ShoppingContext BuildContext(Customer customer) =>
        new()
        {
            Customer = customer,
            CustomerName = customer.UserName,
            Products = _productService.GetAllProducts().ToList(),
            CartItems = _cartService.GetCartItems(customer).ToList(),
            RecentOrders = _orderService.GetOrderHistory(customer).Take(5).ToList()
        };
}
