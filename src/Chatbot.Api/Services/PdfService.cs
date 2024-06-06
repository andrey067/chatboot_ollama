using Chatbot.Shared.Handler;
using LangChain.Databases.Sqlite;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Splitters.Text;

namespace Chatbot.Api.Services;

public class PdfService : IPdfService
{
    private const string DatabaseFile = "vectors.db";
    private const string DatabaseTable = "vectors";
    private readonly OllamaLanguageModelEmbeddings _embeddingModel;
    private readonly OllamaLanguageModelInstruction _chatModel;
    private readonly TextSplitter _textSplitter;
    private readonly SqLiteVectorDatabase _vectorDatabase;

    public PdfService()
    {
        bool isNewDatabase = !File.Exists(DatabaseFile);

        _embeddingModel = new OllamaLanguageModelEmbeddings("nomic-embed-text");
        _chatModel = new OllamaLanguageModelInstruction(
            "llama3",
            options: new OllamaLanguageModelOptions
            {
                Stop = new[] { "\n" },
                Temperature = 0.0f,
            });

        _chatModel.PromptSent += (sender, e) => Console.Write(e);
        _chatModel.PartialResponseGenerated += (sender, e) => Console.Write(e);

        _vectorDatabase = new SqLiteVectorDatabase(DatabaseFile);
        _textSplitter = new RecursiveCharacterTextSplitter(chunkSize: 500, chunkOverlap: 200);

        if (isNewDatabase)
        {
            Console.WriteLine("Indexing document (may take a while on first run)");            
        }
    }

    public async Task<string> GetAnswerFromPdfAsync(IFormFile pdfFile, string question)
    {
        var vectorCollection = await _vectorDatabase.AddDocumentsFromAsync<PdfPigPdfLoader>(
                                                        _embeddingModel,
                                                        dimensions: 384,
                                                        dataSource: DataSource.FromStream(pdfFile.OpenReadStream()),
                                                        collectionName: "document",
                                                        textSplitter: _textSplitter);


        var similarDocuments = await vectorCollection.GetSimilarDocuments(_embeddingModel, question, amount: 5);

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