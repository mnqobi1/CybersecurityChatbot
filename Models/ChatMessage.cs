namespace CybersecurityChatbot.Models;

/// <summary>
/// Represents a single message in the chat conversation.
/// </summary>
public class ChatMessage
{
    public string Sender { get; set; }   // "User" or "Bot"
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }

    public ChatMessage(string sender, string text)
    {
        Sender = sender;
        Text = text;
        Timestamp = DateTime.Now;
    }
}