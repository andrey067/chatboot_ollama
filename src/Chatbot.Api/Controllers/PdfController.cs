using Chatbot.Shared.Handler;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PdfController : ControllerBase
{
    private readonly IPdfService _pdfService;

    public PdfController(IPdfService pdfService)
    {
        _pdfService = pdfService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadPdf(IFormFile file, [FromQuery] string question)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("A valid PDF file is required.");
        }

        if (string.IsNullOrEmpty(question))
        {
            return BadRequest("Question is required.");
        }

        var answer = await _pdfService.GetAnswerFromPdfAsync(file, question);
        return Ok(answer);
    }
}
