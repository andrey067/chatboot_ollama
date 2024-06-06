using LangChain.Memory;
using LangChain.Providers;
using LangChain.Serve.Abstractions.Repository;

namespace Chatbot.Api.Extensions;

public static class ConversationBufferExtensions
{
    public static async Task<ConversationBufferMemory> ConvertToConversationBuffer(this IReadOnlyCollection<StoredMessage> list)
    {
        var conversationBufferMemory = new ConversationBufferMemory();
        conversationBufferMemory.Formatter.HumanPrefix = "User";
        conversationBufferMemory.Formatter.AiPrefix = "Assistant";
        List<Message> converted = list
            .Select(x => new Message(x.Content, x.Author == MessageAuthor.User ? MessageRole.Human : MessageRole.Ai)).ToList();
        await conversationBufferMemory.ChatHistory.AddMessages(converted);
        return conversationBufferMemory;
    }
}