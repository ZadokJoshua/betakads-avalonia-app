# ⚡ Betakads
**AI-Powered Anki Flashcard Generator**

Betakads simplifies the creation of Anki flashcards using AI. Choose a data source—either a PDF document or a YouTube video. The app extracts text from these sources, whether it's video captions or content from a PDF. Specify the number of flashcards you need, and Betakads will generate them, saving the results in a .txt format that is ready for easy import into Anki for seamless study and review.

## Getting Started

1. **Download the Model**  
   Download the `phi3-mini-4k-instruct-onnx` model to your local machine using the command:
   ```bash
   git clone https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx <FOLDER_PATH>
   ```

2. **Configure the Model Path**  
   Insert the absolute path to the folder containing the `.onnx` file into the designated placeholder in the `ServiceCollectionExtensions.cs` file:
   ```csharp
   public static void AddAIService(this IServiceCollection collection)
   {
       string modelPath = "<MODEL_PATH>";
       collection.AddSingleton(new Model(modelPath)).AddSingleton<Tokenizer>();
       collection.AddTransient<IAIService, AIService>();
   }
   ```
   
#
![betakads window-initial state](/Images/Betakads-Fluent1.png)
![betakads window-result](/Images/Betakads-Fluent2.png)
#
**Prompt Reference:**
- [Casting a spell on ChatGPT: Let it write Anki cards for you — A Prompt Engineering Case - Jarrett Ye](https://medium.com/@JarrettYe/casting-a-spell-on-chatgpt-let-it-write-anki-cards-for-you-a-prompt-engineering-case-fd7d577b9d94)

<div align="center">
    <img src="https://github.com/ZadokJoshua/betakads-avalonia-app/assets/65626254/f39824f0-0d70-4dec-ae4d-a2871006219a"  Height=150/>
</div>
