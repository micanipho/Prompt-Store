namespace Application.Interfaces;

/// <summary>
/// Strategy interface for AI shopping assistant providers.
/// Implementations can swap Claude for any other AI provider without changing the application layer.
/// </summary>
public interface IShoppingAssistant
{
    /// <summary>Sends a user message with the current shopping context and conversation history, returning the assistant's response.</summary>
    Task<string> GetResponseAsync(
        string userMessage,
        ShoppingContext context,
        IList<ChatMessage> conversationHistory,
        CancellationToken cancellationToken = default);
}
