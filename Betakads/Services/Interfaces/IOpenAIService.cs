namespace Betakads.Services.Interfaces;

public interface IOpenAIService
{
    Task<string> ConvertTextToCardsList(PromptPayload payload);
}
