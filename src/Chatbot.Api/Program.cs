using Chatbot.Api.Extensions;
using Chatbot.Api.Services;
using Chatbot.Shared;
using Chatbot.Shared.Handler;
using LangChain.Serve;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLangChainServe();
builder.Services.ConfigureNameGenerator();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPdfService, PdfService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseLangChainServe(options => options.ConfigureModels());

app.UseAuthorization();

app.MapControllers();

app.Run();
