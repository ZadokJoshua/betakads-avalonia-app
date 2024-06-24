namespace Betakads.Models;

public record Card(int CardId, string Front, string Back);

public record PromptPayload(string ExtractedText, int NumberOfCards);

public record YoutubeMetadata(string Title, string Author, TimeSpan? Duration);