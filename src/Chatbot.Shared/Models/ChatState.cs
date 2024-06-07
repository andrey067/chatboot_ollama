using Chatbot.Shared.Data;

namespace Chatbot.Shared.Model;

public class ChatState
{
    public string? SelectedModel { get; set; }

    public Guid? ConversationId { get; set; }

    public List<Message> Messages = new List<Message>();
}