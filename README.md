# ⚡Betakads
**Anki Generator Powered by AI**

Betakads is an Anki cards generator powered by AI, designed to streamline the creation of flashcards. Users begin by selecting a data source, choosing between a PDF file or a YouTube link. The application then extracts text from the selected source, whether video captions from YouTube or textual content from a PDF document. Once the data is extracted, users can specify the number of flashcards they wish to generate. The app intelligently processes the information and creates flashcards accordingly. Finally, users can save these generated flashcards in a .txt format that aligns seamlessly with Anki Import Settings, facilitating easy integration into the Anki platform for further study and review. The intuitive interface and AI-driven functionality make Betakads a valuable tool for enhancing the efficiency of Anki flashcard creation.

To get started, please insert your Azure Open AI URL and Key or Open AI API key into the designated placeholders in the OpenAIService.cs file.
```csharp
private readonly OpenAIClient _openAIClient = useAzureOpenAI ? new OpenAIClient(
    new Uri(AZURE_URI), new AzureKeyCredential(AZURE_KEY))
        : new OpenAIClient(OPEN_AI_KEY);
```
Replace "YOUR_AZURE_OPEN_AI_URL" and "YOUR_AZURE_KEY" with the actual Azure Open AI URL and Key values you intend to use.

***
![betakads window-initial state](/Images/Betakads-Fluent1.png)
![betakads window-result](/Images/Betakads-Fluent2.png)
***
**Prompt Reference:**
- [Casting a spell on ChatGPT: Let it write Anki cards for you — A Prompt Engineering Case - Jarrett Ye](https://medium.com/@JarrettYe/casting-a-spell-on-chatgpt-let-it-write-anki-cards-for-you-a-prompt-engineering-case-fd7d577b9d94)
