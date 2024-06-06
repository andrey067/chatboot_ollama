using Microsoft.AspNetCore.Http;

namespace Chatbot.Shared.Handler;

public interface IPdfService
{
    Task<string> GetAnswerFromPdfAsync(IFormFile pdfFile, string question);
}
