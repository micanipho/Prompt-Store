using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services;

/// <summary>
/// Claude API implementation of <see cref="IShoppingAssistant"/> using Anthropic's Messages API.
/// Swap this for any other provider by implementing <see cref="IShoppingAssistant"/> and updating DI registration.
/// </summary>
public sealed class ClaudeShoppingAssistant : IShoppingAssistant, IDisposable
{
    private readonly HttpClient _httpClient;

    private const string ApiUrl = "https://api.anthropic.com/v1/messages";
    private const string Model = "claude-sonnet-4-6";
    private const string AnthropicVersion = "2023-06-01";

    public ClaudeShoppingAssistant(string apiKey)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        _httpClient.DefaultRequestHeaders.Add("anthropic-version", AnthropicVersion);
    }

    public async Task<string> GetResponseAsync(
        string userMessage,
        ShoppingContext context,
        IList<ChatMessage> conversationHistory,
        CancellationToken cancellationToken = default)
    {
        var messages = conversationHistory
            .Select(m => new { role = m.Role, content = m.Content })
            .Append(new { role = "user", content = userMessage })
            .ToArray();

        var requestBody = new
        {
            model = Model,
            max_tokens = 1024,
            system = BuildSystemPrompt(context),
            messages
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(ApiUrl, httpContent, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        return result.GetProperty("content")[0].GetProperty("text").GetString()
               ?? "I'm sorry, I couldn't generate a response.";
    }

    private static string BuildSystemPrompt(ShoppingContext context)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are a friendly and knowledgeable AI shopping assistant for an online store called Prompt Store.");
        sb.AppendLine("Help customers find products, get personalised recommendations, track orders, compare prices, and make decisions.");
        sb.AppendLine("Be concise and helpful. Use the live store data below for accurate answers.");
        sb.AppendLine("Prices are in South African Rand (R). When recommending products, include their ID so the customer can add them to cart.");
        sb.AppendLine();
        sb.AppendLine($"Customer: {context.CustomerName}");
        sb.AppendLine();

        sb.AppendLine("=== AVAILABLE PRODUCTS ===");
        foreach (var p in context.Products)
            sb.AppendLine($"  ID:{p.Id} | {p.Name} | {p.Category} | R{p.Price:F2} | Stock:{p.Stock} | {p.Description}");

        if (context.CartItems.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("=== CUSTOMER'S CART ===");
            foreach (var item in context.CartItems)
                sb.AppendLine($"  {item.Product.Name} x{item.Quantity} @ R{item.Product.Price:F2}");
        }

        if (context.RecentOrders.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("=== RECENT ORDERS (last 5) ===");
            foreach (var order in context.RecentOrders)
                sb.AppendLine($"  Order #{order.Id} | {order.PlacedAt:yyyy-MM-dd} | {order.Status} | R{order.Total:F2} | {order.Items.Count} item(s)");
        }

        return sb.ToString();
    }

    public void Dispose() => _httpClient.Dispose();
}
