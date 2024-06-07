using Chatbot.Client;
using Chatbot.Client.Handler;
using Chatbot.Shared;
using Chatbot.Shared.Handler;
using Markdig;
using Markdown.ColorCode;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .UseColorCode()
    .Build();

builder.Services.AddSingleton(pipeline);
builder.Services.AddSingleton<IApiHandler, ApiHandler>();
builder.Services.AddSingleton<IAppStateHandler, AppStateHandler>();

builder.Services
    .AddHttpClient(
        WebConfiguration.HttpClientName,
        opt => opt.BaseAddress = new Uri(Configuration.BackendUrl));

await builder.Build().RunAsync();
