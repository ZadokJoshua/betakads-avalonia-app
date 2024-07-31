namespace Betakads.Services.Interfaces;

public interface IAIService
{
    Task<string> ConvertTextToCardsList(PromptPayload payload);
}
