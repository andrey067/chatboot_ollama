namespace Chatbot.Shared;

public static class Configuration
{
    public const int DefaultStatusCode = 200;
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 25;

    public static string BackendUrl { get; set; } = "http://localhost:5100";
    public static string FrontendUrl { get; set; } = "http://localhost:5165";
}
