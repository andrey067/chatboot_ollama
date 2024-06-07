using Chatbot.Shared.Handler;
using LangChain.Serve;
using LangChain.Serve.Abstractions;
using LangChain.Serve.Abstractions.Repository;
using LangChain.Serve.Classes.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot.Api.Extensions;

public static class AdaAiConfigExtensions
{
    public static WebApplication UseAdaServeAi(this WebApplication app, ServeOptions options)
    {
        app = app ?? throw new ArgumentNullException(nameof(app));
        options = options ?? throw new ArgumentNullException(nameof(options));

        var repository = app.Services.GetRequiredService<IConversationRepository>();
        var conversationNameProvider = app.Services.GetRequiredService<IConversationNameProvider>();
        var controller = new ServeController(options, repository, conversationNameProvider);

        app.MapPost("/ada/conversations/{conversationId:guid}/messages", async ([FromServices] IAdaAiHandler _adaAiHandler,
                                                                                              PostMessageDto message,
                                                                                              Guid conversationId) =>
        {
            message = message ?? throw new ArgumentNullException(nameof(message));

            var conversation = await repository.GetConversation(conversationId).ConfigureAwait(false);
            if (conversation == null)
            {
                return null;
            }

            var convertedMessage = message.ToStoredMessage(conversationId);
            await repository.AddMessage(convertedMessage).ConfigureAwait(false);

            var allMessages = await repository.ListMessages(conversation.ConversationId).ConfigureAwait(false);


            var answer = await _adaAiHandler.GetAnswerFromPdfAsync(message.Content);
            var response = new StoredMessage()
            {
                Author = MessageAuthor.Ai,
                Content = answer,
                ConversationId = conversationId,
                MessageId = Guid.NewGuid()
            };

            await repository.AddMessage(response).ConfigureAwait(false);

            if (string.IsNullOrEmpty(conversation.ConversationName))
            {
                var withResponse = allMessages.Concat(new[] { response }).ToList();
                var name = await conversationNameProvider.GetConversationName(withResponse).ConfigureAwait(false);
                await repository.UpdateConversationName(conversation.ConversationId, name).ConfigureAwait(false);
            }

            var convertedResponse = MessageDto.FromStoredMessage(response, conversation.ModelName);
            return convertedResponse;
        });

        return app;
    }

    public static async Task LoadFilesAda(this WebApplication app)
    {
        var vectorAdaHandler = app.Services.GetService<IVectorAdaHandler>();
        await vectorAdaHandler.LoadDataBase();
    }
}
