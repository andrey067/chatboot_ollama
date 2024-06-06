using LangChain.Providers;
using LangChain.Serve;
using LangChain.Serve.Abstractions.Repository;
using static LangChain.Chains.Chain;

namespace Chatbot.Api.Extensions;

public static class LangChainConfigExtensions
{
    static IChatModel _model = new OllamaLanguageModelInstruction("mistral:latest", options: new OllamaLanguageModelOptions()
    {
        Temperature = 0,
        Stop = new[] { "User:" },
    }).UseConsoleForDebug();


    private const string NAME_GENERATOR_TEMPLATE =
        @"You will be given conversation between User and Assistant. Your task is to give a name to this conversation using maximum 3 words
Conversation:
{chat_history}
Conversation name: ";

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

    public static void ConfigureModels(this ServeOptions options)
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
    }
}