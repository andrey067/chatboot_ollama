using LangChain.Databases;
using LangChain.Providers;

namespace Chatbot.Shared.Handler;

public interface IVectorAdaHandler
{
    public OllamaLanguageModelEmbeddings EmbeddingModel { get; }
    Task LoadDataBase();
    Task<IVectorCollection> VectorDataBaseAda();
}
