namespace Chatbot.Shared.Handler;

public interface IAdaAiHandler
{
    Task<string> GetAnswerFromPdfAsync(string question);
}
