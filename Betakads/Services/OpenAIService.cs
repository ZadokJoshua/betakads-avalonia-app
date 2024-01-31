using Azure.AI.OpenAI;
using Azure;

namespace Betakads.Services;

public class OpenAIService(bool useAzureOpenAI = false) : IOpenAIService
{
    private readonly OpenAIClient _openAIClient = useAzureOpenAI ? new OpenAIClient(
        new Uri(ApiSettings.AZURE_URI), new AzureKeyCredential(ApiSettings.AZURE_KEY))
            : new OpenAIClient(ApiSettings.OPEN_AI_KEY);

    public async Task<string> ConvertTextToCardsList(string promptPayload, int numberOfCards)
    {
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            Messages =
            {
                new ChatMessage(ChatRole.System, "You are a helpful assistant designed to output JSON."),

                new ChatMessage(ChatRole.User, """
                You wiil create an array of 9 cards from random concepts in this text. A Card has two properties: Front(string) and Back(string).
                
                The characteristics of the Dead Sea: Salt lake located on the border between Israel and Jordan. Its shoreline is the lowest point on the Earth's surface, averaging 396 m below sea level. It is 74 km long. It is seven times as salty (30% by volume) as the ocean. Its density keeps swimmers afloat. Only simple organisms can live in its saline waters
                """),

                new ChatMessage(ChatRole.Assistant, """
                [
                            {
                                "front": "Where is the Dead Sea located?",
                                "back": "on the border between Israel and Jordan"
                            },
                            {
                                "front": "What is the lowest point on the Earth's surface?",
                                "back": "The Dead Sea shoreline"
                            },
                            {
                                "front": "What is the average level on which the Dead Sea is located?",
                                "back": "396 meters (below sea level)"
                            },
                            {
                                "front": "How long is the Dead Sea?",
                                "back": "74 km"
                            },
                            {
                                "front": "How much saltier is the Dead Sea as compared with the oceans?",
                                "back": "7 times"
                            },
                            {
                                "front": "What is the volume content of salt in the Dead Sea?",
                                "back": "30%"
                            },
                            {
                                "front": "Why can the Dead Sea keep swimmers afloat?",
                                "back": "due to high salt content"
                            },
                            {
                                "front": "Why is the Dead Sea called Dead?",
                                "back": "because only simple organisms can live in it"
                            },
                            {
                                "front": "Why only simple organisms can live in the Dead Sea?",
                                "back": "because of high salt content"
                            }
                        ]
                """),
                new ChatMessage(ChatRole.User, $"Create an array of {numberOfCards} cards from this text: {promptPayload}"),
            },

            // DeploymentName = "gpt-35-turbo-1106", // For Azure Open AI
            DeploymentName = "gpt-3.5-turbo", // For Non Azure Open AI
            MaxTokens = 2048,
            Temperature = 0.7f,
            NucleusSamplingFactor = 0.95f
        };

        var completionResult = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);

        return completionResult.Value.Choices[0].Message.Content;
    }
}
