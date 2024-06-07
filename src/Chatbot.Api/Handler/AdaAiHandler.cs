using Chatbot.Api.Common.Resource;
using Chatbot.Shared.Handler;
using LangChain.Extensions;
using LangChain.Providers;

namespace Chatbot.Api.Handler;

public class AdaAiHandler : IAdaAiHandler
{
    private readonly IChatModel _chatModel;
    private readonly IVectorAdaHandler _vectorAdaHandler;

    public AdaAiHandler(IVectorAdaHandler vectorAdaHandler)
    {
        _vectorAdaHandler = vectorAdaHandler;
        _chatModel = new OllamaLanguageModelInstruction(
            "llama3",
            options: new OllamaLanguageModelOptions
            {
                Stop = new[] { "\n" },
                Temperature = 0.0f,
            });

        _chatModel.PromptSent += (sender, e) => Console.Write(e);
        _chatModel.PartialResponseGenerated += (sender, e) => Console.Write(e);
    }

    public async Task<string> GetAnswerFromPdfAsync(string question)
    {
        var vectorCollection = await _vectorAdaHandler.VectorDataBaseAda();

        var similarDocuments = await vectorCollection.GetSimilarDocuments(_vectorAdaHandler.EmbeddingModel, question, amount: 5);

        ChatResponse? answer = await _chatModel.GenerateAsync(
                  $"""
                   Você é uma assistente que vai atender pessoas que estão perguntando assuntos sobre
                   serviços, dicas, processos e outras informações sobre o Detran-MS. De preferência
                   não responda assuntos relacionados a outros Detran. Quando for questionado sobre
                   algo do detran, uso o detran MS como referência. Se apresente sempre como Ada.
                   O site oficial do Detran-MS é: https://www.meudetran.ms.gov.br/

                   {similarDocuments.AsString()}

                   Pergunta: {question}
                   Resposta:
                   """); 
        
        return answer;
    }
}
