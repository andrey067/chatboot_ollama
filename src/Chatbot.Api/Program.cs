using Chatbot.Api;
using Chatbot.Api.Common.Api;
using Chatbot.Api.Extensions;
using LangChain.Serve;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLangChainServe();
builder.Services.ConfigureNameGenerator();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddConfiguration();
builder.AddCrossOrigin();
builder.AddDocumentation();
builder.AddServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.ConfigureDevEnvironment();

await app.LoadFilesAda();

app.UseLangChainServe((options) =>
{
    options.ConfigureModels(app);    
});

app.UseCors(ApiConfiguration.CorsPolicyName);


app.MapControllers();

app.Run();
