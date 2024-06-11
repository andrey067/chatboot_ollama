using Chatbot.Shared.Handler;
using LangChain.Databases;
using LangChain.Databases.Sqlite;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Providers.OpenAI;
using LangChain.Splitters.Text;

namespace Chatbot.Core.Handler;

public class VectorHandler : IVectorHandler
{
    public OllamaLanguageModelEmbeddings EmbeddingModel { get; }
    public OpenAiEmbeddingModel OpenAi { get; }
    private readonly TextSplitter _textSplitter;
    private readonly SqLiteVectorDatabase _sqLiteVector;
    private IVectorCollection _vectorCollection;

    public VectorHandler()
    {
        //TODO - Passar para um resource
        EmbeddingModel = new OllamaLanguageModelEmbeddings("chroma/all-minilm-l6-v2-f32");
        _textSplitter = new RecursiveCharacterTextSplitter(chunkSize: 1500, chunkOverlap: 150);
        //Todo - Passar para um resource        
        _sqLiteVector = new SqLiteVectorDatabase("vector.db");
        //TODO - Implementar um provider para o OpenAI
        OpenAi = new OpenAiEmbeddingModel("api-key", "text-embedding-ada-002");
    }

    public async Task CreateVectorCollection(string vectorName, string collectionName, List<Stream> streams)
    {
        Console.WriteLine("Indexing document (may take a while on first run)");

        bool isNewDatabase = !File.Exists(vectorName);
        if (isNewDatabase)
        {
            foreach (var stream in streams)
            {
                _vectorCollection = await _sqLiteVector.AddDocumentsFromAsync<PdfPigPdfLoader>(
     EmbeddingModel,
                dimensions: 384,
                dataSource: DataSource.FromStream(stream),
                collectionName: collectionName,
                textSplitter: _textSplitter);
            }
        }

        Console.WriteLine("Finally indexing !!");
    }

    public async Task<IVectorCollection> GetVectorCollection(string collectionName)
    => await _sqLiteVector.GetCollectionAsync(collectionName);


    public async Task LoadDataBase(string path)
    {        
        var pdfFiles = Directory.GetFiles(path, "*.pdf");

        foreach (var pdfFile in pdfFiles)
        {
            using var stream = File.OpenRead(pdfFile);
            await _sqLiteVector.AddDocumentsFromAsync<PdfPigPdfLoader>(
                EmbeddingModel,
                dimensions: 1536,
                dataSource: DataSource.FromStream(stream),
                collectionName: "adacollection",
                textSplitter: null);
        }
    }
}
