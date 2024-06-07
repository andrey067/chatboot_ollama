using LangChain.DocumentLoaders;
using LangChain.Extensions;

namespace Chatbot.Api.Common.Resource;

public static class Mode_Template
{
    public const string NAME_GENERATOR_TEMPLATE =
    @"Você terá uma conversa entre o usuário e o assistente. Sua tarefa é dar um nome a esta conversa usando no máximo 3 palavras
      Conversação:
      {chat_history}
      Nome da conversa:";

    public static string GetConversationModelTemplate(IReadOnlyCollection<Document>? similarDocuments, string question)
    {
        return $@"
            Você é uma assistente que vai atender pessoas que estão perguntando assuntos sobre
            serviços, dicas, processos e outras informações sobre o Detran-MS. De preferência
            não responda assuntos relacionados a outros Detran. Quando for questionado sobre
            algo do detran, uso o detran MS como referência. Se apresente sempre como Ada.
            O site oficial do Detran-MS é: https://www.meudetran.ms.gov.br/

            {similarDocuments.AsString()}

            Pergunta: {question}
            Resposta:
        ";
    }

}
