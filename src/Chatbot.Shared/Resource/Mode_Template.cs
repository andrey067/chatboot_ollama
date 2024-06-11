namespace Chatbot.Shared.Resource;

public static class Mode_Template
{
    public const string NAME_GENERATOR_TEMPLATE =
    @"Você terá uma conversa entre o usuário e o assistente. Sua tarefa é dar um nome a esta conversa usando no máximo 3 palavras
      Conversação:
      {chat_history}
      Nome da conversa:";

    public const string CONVERSATION_DEFAULT_TEMPLATE =
    """
    Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible.

    {context}

    Question: {question}
    Helpful Answer:
    """;

    public static string CONVERTER_TEMPLATE_ADA =
     """
            Você é uma assistente que vai atender pessoas que estão perguntando assuntos sobre
            serviços, dicas, processos e outras informações sobre o Detran-MS. De preferência
            não responda assuntos relacionados a outros Detran. Quando for questionado sobre
            algo do detran, uso o detran MS como referência. Se apresente sempre como Ada.
            O site oficial do Detran-MS é: https://www.meudetran.ms.gov.br/

            {context}

            Pergunta: {question}
            Resposta:
     """;
}
