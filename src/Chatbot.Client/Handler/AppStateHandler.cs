using Chatbot.Shared.Data;
using Chatbot.Shared.Handler;
using Chatbot.Shared.Model;

namespace Chatbot.Client.Handler;

public class AppStateHandler : IAppStateHandler
{
    private readonly IApiHandler _api;

    public AppStateHandler(IApiHandler api)
    {
        _api = api;
    }
    public string? SelectedModel { get; set; }

    public List<string> AvailableModels { get; set; } = new List<string>();

    public List<ConversationDTO> History { get; set; } = new List<ConversationDTO>();

    public Guid? ConversationId { get; set; }
    public List<Message> Messages { get; set; } = new();

    public async Task LoadModels()
    {
        var result = await _api.ListModels();

        if (result.IsSuccess)
            AvailableModels = result.Data;
    }

    public async Task ReloadConversations()
    {
        var result = await _api.ListConversations();

        if (result.IsSuccess)
            History = result.Data;

    }

    public async Task<MessageDTO> PostMessage(string message)
    {
        bool historyNeedsRefresh = false;
        if (ConversationId == null)
        {
            historyNeedsRefresh = true;
            var conversation = await _api.CreateConversation(SelectedModel);
            ConversationId = conversation.Data.ConversationId;
        }

        var answer = await _api.ProcessMessage(new PostMessageDTO() { Content = message }, ConversationId.Value);

        if (historyNeedsRefresh)
            await ReloadConversations();

        return answer.Data;
    }

    public async Task ReloadMessages()
    {

        if (ConversationId == null)
        {
            return;
        }
        var messagesDTO = await _api.ListMessages(ConversationId.Value);
        Messages = messagesDTO.Data.Select(x => new Message { DisplayName = x.Author, Content = x.Content }).ToList();

    }
}