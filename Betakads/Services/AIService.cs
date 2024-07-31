using Microsoft.ML.OnnxRuntimeGenAI;

namespace Betakads.Services;

public class AIService(Model model, Tokenizer tokenizer) : IAIService
{
    public async Task<string> ConvertTextToCardsList(PromptPayload payload)
    {
        var systemPrompt = "You are a helpful assistant designed to output JSON.";
        var userMessage = """
                A Card has three properties: CardId (integer), Front(string) and Back(string). Starting from 1, the CardId increments for every card object.
                You wiil create an array of 9 cards only from random concepts in this text.
                
                The characteristics of the Dead Sea: Salt lake located on the border between Israel and Jordan. Its shoreline is the lowest point on the Earth's surface, averaging 396 m below sea level. It is 74 km long. It is seven times as salty (30% by volume) as the ocean. Its density keeps swimmers afloat. Only simple organisms can live in its saline waters
                """;
        var assistantMessage = """
                [
                            {
                                "CardId": 1,
                                "front": "Where is the Dead Sea located?",
                                "back": "on the border between Israel and Jordan"
                            },
                            {
                                "CardId": 2,
                                "front": "What is the lowest point on the Earth's surface?",
                                "back": "The Dead Sea shoreline"
                            },
                            {
                                "CardId": 3,
                                "front": "What is the average level on which the Dead Sea is located?",
                                "back": "396 meters (below sea level)"
                            },
                            {
                                "CardId": 4,
                                "front": "How long is the Dead Sea?",
                                "back": "74 km"
                            },
                            {
                                "CardId": 5,
                                "front": "How much saltier is the Dead Sea as compared with the oceans?",
                                "back": "7 times"
                            },
                            {
                                "CardId": 6,
                                "front": "What is the volume content of salt in the Dead Sea?",
                                "back": "30%"
                            },
                            {
                                "CardId": 7,
                                "front": "Why can the Dead Sea keep swimmers afloat?",
                                "back": "due to high salt content"
                            },
                            {
                                "CardId": 8,
                                "front": "Why is the Dead Sea called Dead?",
                                "back": "because only simple organisms can live in it"
                            },
                            {
                                "CardId": 9,
                                "front": "Why only simple organisms can live in the Dead Sea?",
                                "back": "because of high salt content"
                            }
                        ]
                """;

        var userPrompt = $"Create an array of {payload.NumberOfCards} cards only from this text: {payload.ExtractedText}";

        var fullPrompt = $"<|system|>{systemPrompt}<|end|><|user|>{userMessage}<|end|><|assistant|>{assistantMessage}<|end|><|user|>{userPrompt}<|end|><|assistant|>";

        var tokens = tokenizer.Encode(fullPrompt);

        using var generatorParams = new GeneratorParams(model);
        generatorParams.SetSearchOption("max_length", 2048);
        generatorParams.SetInputSequences(tokens);

        var result = new StringBuilder();
        using var generator = new Generator(model, generatorParams);

        while (!generator.IsDone())
        {
            generator.ComputeLogits();
            generator.GenerateNextToken();
            string output = tokenizer.Decode(generator.GetSequence(0)[^1..]);
            result.Append(output);
        }

        return result.ToString();
    }
}