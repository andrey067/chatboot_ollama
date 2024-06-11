using LangChain.Databases;
using LangChain.Providers;
using LangChain.Providers.OpenAI;

namespace Chatbot.Shared.Handler;

public interface IVectorHandler
{
    public OllamaLanguageModelEmbeddings EmbeddingModel { get; }
    public OpenAiEmbeddingModel OpenAi { get; }
    Task CreateVectorCollection(string vectorName, string collectionName, List<Stream> streams);
    Task<IVectorCollection> GetVectorCollection(string collectionName);
}
