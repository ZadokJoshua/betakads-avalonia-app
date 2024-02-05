namespace Betakads.Services.Interfaces;

interface IOpenAIService
{
    Task<string> ConvertTextToCardsList(PromptPayload payload);
}
