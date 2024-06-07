using Chatbot.Shared.Data;

namespace Chatbot.Shared.Models;

public class ModelState
{
    public string? SelectedModel { get; set; }
    public List<string> AvailableModels { get; set; } = new List<string>();
    public List<ConversationDTO> History { get; set; } = new List<ConversationDTO>();
}
