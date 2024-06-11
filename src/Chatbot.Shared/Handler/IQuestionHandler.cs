namespace Chatbot.Shared.Handler;

public interface IQuestionHandler
{
    Task<string> ProcessQuestionAsync(string question);
}