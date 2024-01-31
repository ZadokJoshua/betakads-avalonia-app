namespace Betakads.Services.Interfaces;

interface IOpenAIService
{
    Task<string> ConvertTextToCardsList(string promptPayload, int numberOfCards);
}
