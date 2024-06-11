using Chatbot.Shared.Handler;
using Chatbot.Shared.Resource;
using LangChain.Databases;
using LangChain.Providers;
using static LangChain.Chains.Chain;

namespace Chatbot.Core.Handler;

public class QuestionHandler : IQuestionHandler
{
    private readonly IVectorCollection _vectorCollection;
    private readonly OllamaLanguageModelEmbeddings _embeddingModel;
    private readonly OllamaLanguageModelInstruction _ollamaLanguageModel;

    public QuestionHandler(IVectorCollection vectorCollection,
                           IVectorHandler vectorHandler,
                           string promptText)
    {
        _vectorCollection = vectorCollection;
        _embeddingModel = vectorHandler.EmbeddingModel;
        _ollamaLanguageModel = new OllamaLanguageModelInstruction(
                                "llama3",
                                options: new OllamaLanguageModelOptions
                                {
                                    Stop = ["\n"],
                                    Temperature = 0.0f
                                });
    }

    public async Task<string> ProcessQuestionAsync(string question)
    {
        var chain =
            Set(question, outputKey: "question")       // set the question
            | RetrieveDocuments(
                _vectorCollection,
                _embeddingModel,
                inputKey: "question",
                outputKey: "documents",
                amount: 5)                                                      // take 5 most similar documents
            | StuffDocuments(inputKey: "documents", outputKey: "context")       // combine documents together and put them into context
            | Template(Mode_Template.CONVERTER_TEMPLATE_ADA)                                              // replace context and question in the prompt with their values
            | LLM(_ollamaLanguageModel);                                                   // send the result to the language model

        var result = await chain.RunAsync("text", CancellationToken.None);  // get chain result
        return result!;
    }
}
