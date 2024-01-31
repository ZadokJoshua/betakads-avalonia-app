namespace Betakads.Models;

// public record Deck(string Name, List<Card> Cards);

public record Card(string Front, string Back);

public record YoutubeMetadata(string Title, string Author, TimeSpan? Duration);
