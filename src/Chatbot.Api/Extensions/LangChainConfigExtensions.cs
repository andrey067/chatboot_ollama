using Chatbot.Shared.Handler;
using LangChain.Databases;
using LangChain.Databases.Sqlite;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Memory;
using LangChain.Providers;
using LangChain.Serve;
using LangChain.Serve.Abstractions.Repository;
using LangChain.Splitters.Text;
using OpenAI.Constants;
using static LangChain.Chains.Chain;

namespace Chatbot.Api.Extensions;

public static class LangChainConfigExtensions
{
    private const string DatabaseFile = "ada.db";
    private const string DatabaseTable = "ada";
    private const string AdaCollection = "adacollection";

    static IChatModel _model = new OllamaLanguageModelInstruction("mistral:latest", options: new OllamaLanguageModelOptions()
    {
        Temperature = 0,
        Stop = new[] { "User:" },
    });
    static TextSplitter TextSplitter = new RecursiveCharacterTextSplitter(chunkSize: 500, chunkOverlap: 200);


    private const string NAME_GENERATOR_TEMPLATE =
        @"Você terá uma conversa entre o usuário e o assistente. Sua tarefa é dar um nome a esta conversa usando no máximo 3 palavras
          Conversação:
          {chat_history}
          Nome da conversa:";

    private const string CONVERSATION_MODEL_TEMPLATE =
        @"You are helpful assistant. Continue conversation with user. Keep your answers short.
          {chat_history}
          Assistant:";

    public static void ConfigureNameGenerator(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddCustomNameGenerator(async messages =>
         {
             var template = NAME_GENERATOR_TEMPLATE;
             var conversationBufferMemory = await messages.ConvertToConversationBuffer();

             var chain = LoadMemory(conversationBufferMemory, "chat_history")
                         | Template(template)
                         | LLM(_model);

             return await chain.RunAsync("text", CancellationToken.None);
         });
    }

    public static void ConfigureModels(this ServeOptions options, WebApplication app)
    {
        options.RegisterModel("Test model", async (messages) =>
        {
            var template = CONVERSATION_MODEL_TEMPLATE;
            var conversationBufferMemory = await messages.ConvertToConversationBuffer();

            var chain = LoadMemory(conversationBufferMemory, "chat_history")
                        | Template(template)
                        | LLM(_model);

            var response = await chain.RunAsync("text", CancellationToken.None);
            return new StoredMessage()
            {
                Author = MessageAuthor.Ai,
                Content = response
            };
        });

        options.RegisterModel("Ada", async (messages) =>
        {
            string question = messages.LastOrDefault().Content;
            OllamaLanguageModelEmbeddings embeddingModel = new OllamaLanguageModelEmbeddings("nomic-embed-text");
            var conversationBufferMemory = await messages.ConvertToConversationBuffer();

            using var vectorDatabase = new SqLiteVectorDatabase(DatabaseFile);
            var vectorCollection = await vectorDatabase.AddDocumentsFromAsync<PdfPigPdfLoader>(
                embeddingModel,
                dimensions: 1536,
                dataSource: DataSource.FromPath("E:\\Trabalho Inovvati\\ProjetosTests\\Chatbot\\Files\\Unificado\\Emitir certificado de registro e licenciamento do veículo eletrônico (CRLV-e).pdf"),
                collectionName: AdaCollection,
                textSplitter: TextSplitter);

            var similarDocuments = await vectorCollection.GetSimilarDocuments(embeddingModel, question, amount: 5);

            var promptTemplate =
                                @"Você é uma assistente que vai atender pessoas que estão perguntando assuntos sobre
                                  serviços, dicas, processos e outras informações sobre o Detran-MS. De preferência
                                  não responda assuntos relacionados a outros Detran. Quando for questionado sobre
                                  algo do detran, uso o detran MS como referência. Se apresente sempre como Ada.
                                  O site oficial do Detran-MS é: https://www.meudetran.ms.gov.br/
                                  {chat_history}                                  
                                  Pergunta: {text}
                                  Resposta útil:";

            var chain = Set(question)
                                   | LoadMemory(conversationBufferMemory, "chat_history")
                                   | RetrieveSimilarDocuments(vectorCollection, embeddingModel, amount: 5)
                                   | Template(promptTemplate)
                                   | LLM(_model);

            var chainAnswer = await chain.RunAsync("text", CancellationToken.None);
            return new StoredMessage()
            {
                Author = MessageAuthor.Ai,
                Content = chainAnswer
            };
        });

        options.RegisterModel("Ada2", async (messages) =>
        {
            string question = messages.LastOrDefault().Content;
            var conversationBufferMemory = await messages.ConvertToConversationBuffer();
            var vectorAdaHandler = app.Services.GetService<IVectorAdaHandler>();
            var embeddingModel = vectorAdaHandler.EmbeddingModel;
            var vectorCollection = await vectorAdaHandler.VectorDataBaseAda();
            var similarDocuments = await vectorCollection.GetSimilarDocuments(embeddingModel, question, amount: 5);

            var promptTemplate =
                                @"Você é uma assistente que vai atender pessoas que estão perguntando assuntos sobre
                                  serviços, dicas, processos e outras informações sobre o Detran-MS. De preferência
                                  não responda assuntos relacionados a outros Detran. Quando for questionado sobre
                                  algo do detran, uso o detran MS como referência. Se apresente sempre como Ada.
                                  O site oficial do Detran-MS é: https://www.meudetran.ms.gov.br/
                                  {chat_history}                                  
                                  Pergunta: {text}
                                  Resposta útil:";

            var chain = Set(question)
                                   | LoadMemory(conversationBufferMemory, "chat_history")
                                   | RetrieveSimilarDocuments(vectorCollection, embeddingModel, amount: 5)
                                   | Template(promptTemplate)
                                   | LLM(_model);

            var chainAnswer = await chain.RunAsync("text", CancellationToken.None);
            return new StoredMessage()
            {
                Author = MessageAuthor.Ai,
                Content = chainAnswer
            };
        });


        options.RegisterModel("Harry Potter", async (messages) =>
        {
            string question = messages.LastOrDefault().Content;
            OllamaLanguageModelEmbeddings embeddingModel = new OllamaLanguageModelEmbeddings("nomic-embed-text");

            using var vectorDatabase = new SqLiteVectorDatabase(dataSource: "vectors.db");
            var vectorCollection = await vectorDatabase.AddDocumentsFromAsync<PdfPigPdfLoader>(
                embeddingModel,
                dimensions: 1536,
                dataSource: DataSource.FromUrl("https://canonburyprimaryschool.co.uk/wp-content/uploads/2016/01/Joanne-K.-Rowling-Harry-Potter-Book-1-Harry-Potter-and-the-Philosophers-Stone-EnglishOnlineClub.com_.pdf"),
                collectionName: "harrypotter",
                textSplitter: TextSplitter);

            var similarDocuments = await vectorCollection.GetSimilarDocuments(embeddingModel, question, amount: 5);

            var promptTemplate =
                                @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible. Always quote the context in your answer.
                                  {context}
                                  Question: {text}
                                  Helpful Answer:";

            var chain = Set(question)
                                 | RetrieveSimilarDocuments(vectorCollection, embeddingModel, amount: 5)
                                 | CombineDocuments(outputKey: "context")
                                 | Template(promptTemplate)
                                 | LLM(_model.UseConsoleForDebug());

            var chainAnswer = await chain.RunAsync("text", CancellationToken.None);
            return new StoredMessage()
            {
                Author = MessageAuthor.Ai,
                Content = chainAnswer
            };
        });
    }
}