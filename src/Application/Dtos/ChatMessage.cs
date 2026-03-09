namespace Application.Dtos;

/// <summary>Represents a single message in the AI assistant conversation.</summary>
public class ChatMessage
{
    public string Role { get; set; } = string.Empty;    // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
}
