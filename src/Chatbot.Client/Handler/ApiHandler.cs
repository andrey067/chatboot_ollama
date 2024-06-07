using Chatbot.Shared.Data;
using Chatbot.Shared.Handler;
using Chatbot.Shared.Requests;
using System.Net.Http.Json;

namespace Chatbot.Client.Handler;

public class ApiHandler(IHttpClientFactory httpClientFactory) : IApiHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);

    public async Task<BaseResponse<List<string>>> ListModels()
    {
        var response = await _client.GetAsync("/serve/models");
        var content = await response.Content.ReadFromJsonAsync<List<string>>();
        return new BaseResponse<List<string>>(content!);
    }

    public async Task<BaseResponse<List<ConversationDTO>>> ListConversations()
    {
        var response = await _client.GetAsync("/serve/conversations");
        var content = await response.Content.ReadFromJsonAsync<List<ConversationDTO>>();
        return new BaseResponse<List<ConversationDTO>>(content!);
    }

    public async Task<BaseResponse<ConversationDTO>> CreateConversation(string modelName)
    {
        var response = await _client.PostAsJsonAsync("/serve/conversations", new { modelName });
        var content = await response.Content.ReadFromJsonAsync<ConversationDTO>();
        return new BaseResponse<ConversationDTO>(content!);
    }

    public async Task<BaseResponse<MessageDTO>> ProcessMessage(PostMessageDTO message, Guid conversationId)
    {
        var response = await _client.PostAsJsonAsync($"/serve/conversations/{conversationId}/messages", message);
        var content = await response.Content.ReadFromJsonAsync<MessageDTO>();
        return new BaseResponse<MessageDTO>(content!);
    }

    public async Task<BaseResponse<ConversationDTO>> GetConversation(Guid conversationId)
    {
        var response = await _client.GetAsync($"/serve/conversations/{conversationId}");
        var content = await response.Content.ReadFromJsonAsync<ConversationDTO>();
        return new BaseResponse<ConversationDTO>(content!);
    }

    public async Task<BaseResponse<List<MessageDTO>>> ListMessages(Guid conversationId)
    {
        var response = await _client.GetAsync($"/serve/conversations/{conversationId}/messages");
        var content = await response.Content.ReadFromJsonAsync<List<MessageDTO>>();
        return new BaseResponse<List<MessageDTO>>(content!);
    }

    public async Task DeleteConversation(Guid conversationId)
    {
        await _client.DeleteAsync($"/serve/conversations/{conversationId}");
    }
}
