using Betakads.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML.OnnxRuntimeGenAI;

namespace Betakads.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<IAIService, AIService>();
        collection.AddTransient<IPdfService, PdfService>();
        collection.AddTransient<IYoutubeService, YoutubeService>();
        collection.AddTransient<MainViewModel>();
    }

    public static void AddAIService(this IServiceCollection collection)
    {
        string modelPath = AppSettings.MODEL_PATH;

        collection.AddSingleton(new Model(modelPath)).AddSingleton<Tokenizer>();
        collection.AddTransient<IAIService, AIService>();
    }
}
