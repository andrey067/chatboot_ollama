using Chatbot.Shared.Data;
using Chatbot.Shared.Model;

namespace Chatbot.Shared.Handler;

public interface IAppStateHandler
{
    string? SelectedModel { get; set; }
    List<string> AvailableModels { get; set; }
    List<ConversationDTO> History { get; set; }
    Guid? ConversationId { get; set; }
    List<Message> Messages { get; set; }

    Task LoadModels();
    Task ReloadConversations();
    Task<MessageDTO> PostMessage(string message);
    Task ReloadMessages();
}
