using Chatbot.Shared.Handler;
using LangChain.Databases;
using LangChain.Databases.Sqlite;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Splitters.Text;

namespace Chatbot.Api.Handler;

public class VectorAdaHandler : IVectorAdaHandler
{
    private const string DatabaseFile = "ada.db";
    private const string DatabaseTable = "ada";
    private const string AdaCollection = "adacollection";
    private readonly SqLiteVectorDatabase _vectorDatabase;
    private readonly TextSplitter _textSplitter;
    public OllamaLanguageModelEmbeddings EmbeddingModel { get; }

    public VectorAdaHandler()
    {
        bool isNewDatabase = !File.Exists(DatabaseFile);
        EmbeddingModel = new OllamaLanguageModelEmbeddings("nomic-embed-text");
        _vectorDatabase = new SqLiteVectorDatabase(DatabaseFile);
        _textSplitter = new RecursiveCharacterTextSplitter(chunkSize: 500, chunkOverlap: 200);

        if (isNewDatabase)
        {
            Console.WriteLine("Indexing document (may take a while on first run)");
        }
    }


    public async Task<IVectorCollection> VectorDataBaseAda()
    {
        return await _vectorDatabase.GetCollectionAsync(AdaCollection);
    }

    public async Task LoadDataBase()
    {
        var pdfDirectory = "E:\\Trabalho Inovvati\\ProjetosTests\\Chatbot\\Files";
        var pdfFiles = Directory.GetFiles(pdfDirectory, "*.pdf");

        foreach (var pdfFile in pdfFiles)
        {
            using var stream = File.OpenRead(pdfFile);
            await _vectorDatabase.AddDocumentsFromAsync<PdfPigPdfLoader>(
                EmbeddingModel,
                dimensions: 384,
                dataSource: DataSource.FromStream(stream),
                collectionName: "adacollection",
                textSplitter: _textSplitter);
        }
    }
}
