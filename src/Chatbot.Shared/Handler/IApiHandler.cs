using Chatbot.Shared.Data;
using Chatbot.Shared.Requests;

namespace Chatbot.Shared.Handler;

public interface IApiHandler
{
    Task<BaseResponse<List<string>>> ListModels();
    Task<BaseResponse<List<ConversationDTO>>> ListConversations();
    Task<BaseResponse<ConversationDTO>> CreateConversation(string modelName);
    Task<BaseResponse<MessageDTO>> ProcessMessage(PostMessageDTO message, Guid conversationId);    
    Task<BaseResponse<ConversationDTO>> GetConversation(Guid conversationId);
    Task<BaseResponse<List<MessageDTO>>> ListMessages(Guid conversationId);
    Task DeleteConversation(Guid conversationId);
}